using MemeSearch.Logic.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MemeSearch.Logic.Services
{
    public class ImageDetectService : IImageDetectService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public bool IsAvailable { get; private set; }

        private int errorCount = 0;

        private string Url { get; }
        private string TaggerName { get; }

        public ImageDetectService(string url, string taggerName)
        {
            Url = url;
            TaggerName = taggerName;

            var existingTaggerResponse = CheckIfTaggerExists().Result;
            if (existingTaggerResponse.HasValue && !existingTaggerResponse.Value)
            {
                CreateTagger().Wait();
            }
        }

        private async Task<bool?> CheckIfTaggerExists()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Url}/services/{TaggerName}");

                if (response.IsSuccessStatusCode)
                {
                    IsAvailable = true;
                    return true;
                }
                if (response.StatusCode == HttpStatusCode.NotFound) return false;
                Log.Error("DeepDetect error - check if tagger exists. Status: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Log.Error("DeepDetect error - check if tagger exists. Reason: " + ex.Message);
            }
            
            return null;
        }

        private async Task<bool> CreateTagger()
        {
            var createTaggerConfig = string.Empty;

            try
            {
                createTaggerConfig = File.ReadAllText("DeepDetect/CreateTagger.json");
            }
            catch (Exception ex)
            {
                Log.Error("Config file read error - create tagger. Reason: " + ex.Message);
            }

            try
            {
                var response = await _httpClient.PutAsync($"{Url}/services/{TaggerName}", new StringContent(createTaggerConfig));

                if (response.IsSuccessStatusCode)
                {
                    IsAvailable = true;
                    return true;
                }

                Log.Error("DeepDetect error - create tagger. Status: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Log.Error("DeepDetect error - create tagger. Reason: " + ex.Message);
            }

            return false;
        }

        public async Task<string> GetImageTags(string imageUrl)
        {
            if (!IsAvailable) return null;

            var requestBody = @"
                {
                    ""service"":""" + TaggerName + @""",
                    ""parameters"": 
                    {
                        ""output"": 
                        {
                            ""confidence_threshold"": 0.3,
                            ""bbox"": true
                        },
                        ""mllib"": 
                        {
                            ""gpu"": true
                        }
                    },
                    ""data"":[""" + imageUrl + @"""]
                }";

            try
            {
                var response = await _httpClient.PostAsync($"{Url}/predict", new StringContent(requestBody));

                if (response.IsSuccessStatusCode)
                {
                    dynamic respBody = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    string imageTags = JsonConvert.SerializeObject(respBody.body.predictions[0].classes);
                    var tags = Regex.Matches(imageTags, "\"cat\":\"(.*?)\"").Select(m => m.Groups[1]);
                    return string.Join(" AND ", tags);
                }

                Log.Error("DeepDetect error - get image tags. Status: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Log.Error("DeepDetect error - get image tags. Reason: " + ex.Message);
            }

            errorCount++;

            // if service is constantly failing make it unavailable
            if (errorCount == 5)
            {
                errorCount = 0;
                IsAvailable = false;
            }
            return null;
        }
    }
}
