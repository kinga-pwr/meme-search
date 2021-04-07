using MemeSearch.Logic.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MemeSearch.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void SetSearchEngine(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionSettings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"]))
                .DefaultIndex(configuration["Elasticsearch:Index"]);

            var client = new ElasticClient(connectionSettings);

            if (!client.Ping().IsValid)
            {
                // if the engine is not running the application must be stopped
                Environment.Exit(0);
            }

            services.AddSingleton<IElasticClient>(client);

            if (!client.Indices.Exists(configuration["Elasticsearch:Index"]).Exists
                && File.Exists("memes.json"))
            {
                client.Indices.Create(configuration["Elasticsearch:Index"], c => c.Map<Meme>(m => m.AutoMap()));
                var lines = File.ReadAllLines("memes.json");
                foreach (var line in lines)
                {
                    var meme = JsonConvert.DeserializeObject<Meme>(line);
                    client.IndexDocument(meme);
                }
            };
        }
    }
}
