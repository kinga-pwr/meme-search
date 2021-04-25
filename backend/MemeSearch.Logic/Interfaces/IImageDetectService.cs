using System.Threading.Tasks;

namespace MemeSearch.Logic.Interfaces
{
    public interface IImageDetectService
    {
        bool IsAvailable { get; }

        Task<string> GetImageTags(string imageUrl);
    }
}
