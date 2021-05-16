using MemeSearch.Logic.Consts;
using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using MemeSearch.Logic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json;
using Polly;
using Serilog;
using System;
using System.IO;
using System.Threading;

namespace MemeSearch.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void SetHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IMemeSearchHttpService, MemeSearchHttpService>()
                .AddTransientHttpErrorPolicy(
                    p => p.WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    }));
        }

        public static void SetSearchEngine(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionSettings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"]))
                .DefaultIndex(configuration["Elasticsearch:Index"]);

            var client = new ElasticClient(connectionSettings);

            if (configuration["Elasticsearch:Url"].Contains("elastic"))
            {
                var attempts = 0;

                while (!client.Ping().IsValid && attempts++ < 10)
                {
                    Thread.Sleep(15000);
                }
            }

            if (!client.Ping().IsValid)
            {
                Log.Error("Can't connect to Elasticsearch engine on " + configuration["Elasticsearch:Url"]);
                // if the engine is not running the application must be stopped
                Environment.Exit(0);
            }

            services.AddSingleton<IElasticClient>(client);

            if (!client.Indices.Exists(configuration["Elasticsearch:Index"]).Exists)
            {
                Log.Information($"Index {configuration["Elasticsearch:Index"]} not found. Creating...");
                client.Indices.Create(configuration["Elasticsearch:Index"], c =>
                c.Settings(s => s
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Custom("keywords", ca => ca
                                .Tokenizer("keyword")
                                .Filters("lowercase")
                            )
                        )
                    ))
                .Map<Meme>(m => m.AutoMap()));

                if (File.Exists(IndexConsts.MemeDocumentsFile))
                {
                    Log.Information("Reading meme documents to be indexed...");
                    try
                    {
                        var lines = File.ReadAllLines(IndexConsts.MemeDocumentsFile);
                        Log.Information($"Found {lines.Length} meme documents to be indexed. Indexing...");
                        foreach (var line in lines)
                        {
                            var meme = JsonConvert.DeserializeObject<Meme>(line);
                            client.IndexDocument(meme);
                        }
                        Log.Information($"Indexing finished!");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while indexing meme documents. Reason: " + ex.Message);
                    }
                }
                else
                {
                    Log.Warning("Meme documents not present to be indexed. Index will be empty!");
                }
            };
        }

        public static void SetImageDetector(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeepDetect>(configuration.GetSection("DeepDetect"));
            services.AddSingleton<IImageDetectService, ImageDetectService>();
        }
    }
}
