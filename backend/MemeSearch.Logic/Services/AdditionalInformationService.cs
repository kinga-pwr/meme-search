using MemeSearch.Logic.Consts;
using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemeSearch.Logic.Services
{
    public class AdditionalInformationService : IAdditionalInformationService
    {
        public HashSet<string> Statuses { get; } = new HashSet<string>();
        public Dictionary<string, int> Details { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> Categories { get; } = new Dictionary<string, int>();

        public AdditionalInformationService()
        {
            if (File.Exists(IndexConsts.MemeDocumentsFile))
            {
                Log.Information("Meme documents found to load metadata");
                
                try
                {
                    var lines = File.ReadAllLines(IndexConsts.MemeDocumentsFile);
                    Log.Information($"Found {lines.Length} meme documents. Loading metadata...");
                    foreach (var line in lines)
                    {
                        var meme = JsonConvert.DeserializeObject<Meme>(line);

                        Statuses.Add(meme.Status);

                        var details = meme.Details.Split(", ");
                        var categories = meme.Category.Split(", ");
                        
                        foreach (var detail in details)
                        {
                            if (Details.ContainsKey(detail))
                            {
                                Details[detail]++;
                            }
                            else
                            {
                                Details.Add(detail, 1);
                            }
                        }

                        foreach (var category in categories)
                        {
                            if (Categories.ContainsKey(category))
                            {
                                Categories[category]++;
                            }
                            else
                            {
                                Categories.Add(category, 1);
                            }
                        }
                    }
                    Log.Information($"Loading metadata finished!");
                }
                catch (Exception ex)
                {
                    Log.Error("Error while loading metadata. Reason: " + ex.Message);
                }
            }
            else
            {
                Log.Warning("Can't find meme documents file to load metadata");
            }
        }
    }
}
