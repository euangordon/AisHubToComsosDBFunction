using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AisHub
{
    public class Cosmos
    {
        // The Cosmos client instance
        private CosmosClient cosmosClient;
        // The database we will create
        private Database database;
        // The container we will create.
        private Container container;

        private string DatabaseID;
        private string ContainerID;

        public Cosmos(string endpointUri, string primaryKey, string databaseID, string containerID)
        {
            DatabaseID = databaseID;
            ContainerID = containerID;
            cosmosClient = new CosmosClient(endpointUri, primaryKey);
            database = cosmosClient.GetDatabase(DatabaseID);
            container = database.GetContainer(ContainerID);
        }

        public async Task AddAisHubDataAsync(CosmosAisData aisData)
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<CosmosAisData> itemResponse = await container.ReadItemAsync<CosmosAisData>(aisData.id, new PartitionKey(aisData.MMSI));
                Console.WriteLine($"MMSI {itemResponse.Resource.MMSI} - Item in database with LastUpdatedTicks: {itemResponse.Resource.id} already exists");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<CosmosAisData> itemResponse = await container.CreateItemAsync(aisData, new PartitionKey(aisData.MMSI));
                Console.WriteLine($"MMSI {itemResponse.Resource.MMSI} - Item added to database with id: {itemResponse.Resource.id}");
            }
        }

        public async Task<List<AisSummaryCount>> QueryAisSummaryCountAsync()
        {
            var sqlQueryText = @"SELECT c.MMSI, MIN(c.id) as Ticks, COUNT(1) as PositionCount FROM c GROUP BY c.MMSI";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<AisSummaryCount> queryResultSetIterator = container.GetItemQueryIterator<AisSummaryCount>(queryDefinition);

            List<AisSummaryCount> aisDataSet = new List<AisSummaryCount>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<AisSummaryCount> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (AisSummaryCount aisData in currentResultSet)
                {
                    aisDataSet.Add(aisData);
                }
            }

            return aisDataSet;
        }

        public class AisSummaryCount
        {
            public int MMSI { get; set; }
            public string Ticks { get; set; }
            public int PositionCount { get; set; }

            public AisSummaryCount() { }
        }

    }
}
