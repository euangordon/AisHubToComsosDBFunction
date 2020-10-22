using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AisHub
{
    public static class AisHubToComsosDBFunction
    {
        private static readonly string AisHubUserName = Environment.GetEnvironmentVariable("AisHubUserName");
        private static readonly string CosmosEndpointUri = Environment.GetEnvironmentVariable("CosmosEndpointUri");
        private static readonly string CosmosPrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey");
        private static readonly string CosmosDatabaseID = Environment.GetEnvironmentVariable("CosmosDatabaseID");
        private static readonly string CosmosContainerID = Environment.GetEnvironmentVariable("CosmosContainerID");

        [FunctionName("AisHubToComsosDBFunction")]
        public static void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"AisHubToComsosDBFunction function executed at: {DateTime.Now}");

            //Check configuration has been set
            if (AisHubUserName == "" || CosmosEndpointUri == "" || CosmosPrimaryKey == "" || CosmosDatabaseID == "" || CosmosContainerID == "")
            {
                log.LogError($"Please Set All Required Variables : Currently Set As: AisHubUserName - {AisHubUserName}, CosmosEndpointUri - {CosmosEndpointUri}, CosmosPrimaryKey - {CosmosPrimaryKey}, CosmosDatabaseID - {CosmosDatabaseID}, CosmosContainerID - {CosmosContainerID}");
                return;
            }

            //Setup Connection to Azure CosmosDB
            var cosmos = new Cosmos(CosmosEndpointUri, CosmosPrimaryKey, CosmosDatabaseID, CosmosContainerID);

            try
            {
                //Get Data from AisHub API
                var aisHubRawData = GetAisHubData();

                //Convert AisHub Data to our Cosmos Data Format
                var cosmosData = CosmosAisData.ConvertToCosmosFormat(aisHubRawData);

                //Insert Positions to CosmosDB
                foreach (var data in cosmosData)
                {
                    log.LogInformation($"Inserted Location {data.ToString()}");
                    _ = cosmos.AddAisHubDataAsync(data);
                    log.LogInformation($"AIS DataLake Created - {data.MMSI}");
                }

                log.LogInformation($"AisHubToComsosDBFunction function completed at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                log.LogError($"{ex}");
            }

        }

        private static List<ShipPosition> GetAisHubData()
        {
            var aisHubUrl = $"http://data.aishub.net/ws.php?username={AisHubUserName}&format=1&output=json&compress=0";

            var cookieContainer = new CookieContainer();
            using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            using var client = new HttpClient(handler);

            var aisHubResult = client.GetAsync(aisHubUrl);
            aisHubResult.Result.EnsureSuccessStatusCode();

            var jsonString = aisHubResult.Result.Content.ReadAsStringAsync().Result;

            if (jsonString.Contains("error"))
            {
                throw new Exception($"Error getting data from AisHub - {jsonString}");
            }

            var aisHubDataSet = AisHubData.FromJson(jsonString);
            if (aisHubDataSet.Count < 2)
            {
                throw new Exception(aisHubDataSet[0].Summary.Format);
            }
            var shipPositions = aisHubDataSet[1].ShipPositions;

            return shipPositions;
        }


    }
}
