using System.Net.Http;

namespace MemeSearch.Logic.Interfaces
{
    public interface IMemeSearchHttpService
    {
        HttpClient HttpClient { get; }
    }
}
