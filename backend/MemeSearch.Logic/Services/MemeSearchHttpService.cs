using MemeSearch.Logic.Interfaces;
using System;
using System.Net.Http;

namespace MemeSearch.Logic.Services
{
    public class MemeSearchHttpService : IMemeSearchHttpService
    {
        public HttpClient HttpClient { get; }

        public MemeSearchHttpService(HttpClient httpClient)
        {
            HttpClient = httpClient;
            HttpClient.Timeout = TimeSpan.FromMinutes(10);
        }
    }
}
