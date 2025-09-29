using System.Text.Json;

namespace WebApi.Middleware
{
    public class RequiredJsonFieldsMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly HashSet<string> _bookPostPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/add-book", "/api/books" 
        };

        public RequiredJsonFieldsMiddleware(RequestDelegate next) { _next = next; }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method is "POST" or "PUT" &&
                context.Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(body);
                        var root = doc.RootElement;

                        if (_bookPostPaths.Contains(context.Request.Path.Value ?? ""))
                        {
                            var missing = new List<string>();
                            if (!root.TryGetProperty("title", out _)) missing.Add("Title");
                            if (!root.TryGetProperty("authorIds", out _)) missing.Add("AuthorIds");
                            if (!root.TryGetProperty("publisherID", out _)) missing.Add("PublisherID");

                            if (missing.Count > 0)
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsJsonAsync(new
                                {
                                    message = "Missing required fields",
                                    fields = missing
                                });
                                return;
                            }
                        }

                    }
                    catch (JsonException)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new { message = "Invalid JSON" });
                        return;
                    }
                }
            }
            await _next(context);
        }
    }

    public static class RequiredJsonFieldsExtensions
    {
        public static IApplicationBuilder UseRequiredJsonFields(this IApplicationBuilder app)
            => app.UseMiddleware<RequiredJsonFieldsMiddleware>();
    }
}
