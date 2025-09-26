using System.Text.RegularExpressions;

namespace WebApi.Models.Validation
{
    public static class Validators
    {
        private static readonly Regex _titleRegex =
            new Regex(@"^[\p{L}\p{N}\s\-_.,'()]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsValidTitle(string? title)
            => !string.IsNullOrWhiteSpace(title) && _titleRegex.IsMatch(title);

        public static bool MinLength(string? s, int len)
            => !string.IsNullOrWhiteSpace(s) && s.Trim().Length >= len;
    }
}
