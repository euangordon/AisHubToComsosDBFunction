# AisHubToComsosDBFunction - Recording Ship Locations in CosmosDB
C# NET Core - Azure Function which connects to the AisHub API and stores the vessel (ship) position data received in Azure CosmosDB
![AIS Ships](https://github.com/euangordon/AisHubToComsosDBFunction/blob/master/AIS_Ships.JPG)

### Using this solution: 
To use this solution, you will need a few things:
1. An account with [AisHub](http://www.aishub.net/) to receive data from their API. You will need to contribute data to them, see the [RPIAIS](http://www.aishub.net/rpiais) section for how to do this with a Raspberry Pi.
2. Azure CosmosDB setup, with a new Container with the Partition ID set to /MMSI
3. Download source and enter your API Keys

## Structuring Azure CosmosDB data to support ship positions.
This blog is part 1 of a series of blogs where we will use Azure CosmosDB to store ship locations, query the data, and visualise the data.
Firstly, why did we choose Azure CosmosDB? In most cases we were using Azure SQL to store our data, but we could see that if we were receiving ship positions regularly that the data set was going to become big very quickly, so it suited a noSQL database. Azure CosmosDB allowed us to store and query this scale of data very quickly.

## AIS (Automatic Identification System) Data:
At any time there are over 50,000 ships moving around the world, transmitting their location using an [AIS (Automatic Identification System)](https://en.wikipedia.org/wiki/Automatic_identification_system) transmitter on 161.975 MHz and 162.025 MHz. AIS was designed to allow ships to share their location with other nearby ships. Land based receivers have been set up by a number of companies and enthusiasts to collect AIS signals, and over recent years satellites have been used to get worldwide coverage.
If you are interested in receiving this data yourself, [AisHub.net](http://www.aishub.net/) allows access to their API if you contribute data yourself. This dataset offers limited coverage, but it provides enough data to get started. You can set up a small receiver using a [Raspberry Pi and an RTL-SDR USB dongle](http://www.aishub.net/rpiais). My small receiver gets some ships in the [Firth of Forth](http://www.aishub.net/stations/2993).

## Reference Materials:

### Getting Started with Azure CosmosDB: 
See [Azure Cosmos DB documentation](https://docs.microsoft.com/en-us/azure/cosmos-db/)

### Structuring Data in Azure CosmosDB:
See [Partitioning in Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/partitioning-overview)

## Using an Azure Function to download new positions and update CosmosDB
We chose to use an Azure Function (v3 .net core) Timer Function to connect to the AisHub API every 5 minutes, to download the latest positions and store this in CosmosDB. 
![Ais Data Flow](https://github.com/euangordon/AisHubToComsosDBFunction/blob/master/AIS_DataFlow.JPG.png)

## Structuring the Data:
Selecting a good partition key and ids for your data was one of the early decisions that we needed to make. In this case, each ship could be uniquely identified using the ship's MMSI (Maritime Mobile Service Identity) number so this was chosen for the partition key. 
For the id, the only unique field in the AIS dataset was the date time that the position was received for each ship. So we converted the date time to ticks (string) and used this for the id. By using the date time as the id we could also avoid storing any duplicate position data.
![Ais Raw Data in CosmosDB](https://github.com/euangordon/AisHubToComsosDBFunction/blob/master/AIS_RawData.JPG)

## Deploying the Azure Function
The function has been deployed into Azure on a consumption plan. I did worry that it may take over 5 minutes to complete (the timeout limit for consumption functions), but it is completing within that time. 

## Next Time
We will setup a Web App to connect to the Azure CosmosDB and query it.

Euan Gordon
