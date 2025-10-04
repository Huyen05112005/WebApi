
using WebApi.Models.Domain;

namespace WebApi.Repositories
{
    public interface IImageRepository
    {
        Image Upload(Image image);
        public List<Image> GetAllInfoImages();
        public (byte[], string, string) DownloadFile(int Id);
    }

}
