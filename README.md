# AisHubToComsosDBFunction
C# NET Core - Azure Function which connects to the AisHub API and stores the vessel (ship) position data received in Azure CosmosDB

![AIS Ships](https://github.com/euangordon/AisHubToComsosDBFunction/blob/master/AIS_Ships.JPG)

Prerequisites: 
1. An account with [AisHub](http://www.aishub.net/) to receive data from their API. You will need to contribute data to them, see the [RPIAIS](http://www.aishub.net/rpiais) seciotn for how to do this with a Raspberry Pi.
2. CosmosDB setup, with a new Container with the Partition ID set to /MMSI
3. Download source and enter your API Keys

# Recording Ship Locations in CosmosDB

At any time there are over 50,000 ships moving around the world, transmitting their location using an [AIS (Automatic Identification System)](https://en.wikipedia.org/wiki/Automatic_identification_system) transmitter on 161.975 MHz and 162.025 MHz. AIS was designed to allow ships to share their location with other nearby ships. Land based receivers have been set up by a number of companies and enthusiasts to collect AIS signals, and over recent years satellites have been used to get worldwide coverage.

If you are interested in receiving this data yourself, [AisHub.net](http://www.aishub.net/) allows access to their API if you contribute data yourself. This dataset offers limited coverage, but it provides enough data to get started. You can set up a small receiver using a [Raspberry Pi and an RTL-SDR USB dongle](http://www.aishub.net/rpiais). My small receiver gets some ships in the [Firth of Forth](http://www.aishub.net/stations/2993).

## Getting Started: 
See [Azure Cosmos DB documentation](https://docs.microsoft.com/en-us/azure/cosmos-db/)

## Structuring Data in Azure CosmosDB:
See [Partitioning in Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/partitioning-overview)

Selecting good a partition keys and ids for your data is one of the early decisions you need to make. In this case, each ship could be uniquely identified using the ship's MMSI (Maritime Mobile Service Identity) number so this was chosen for the partition key. Each ships locations was unique based on the DateTime it was received, so it is suitable id. Using the DateTime as the id helped avoid any duplicate data.

![Ais Raw Data in CosmosDB](https://github.com/euangordon/AisHubToComsosDBFunction/blob/master/AIS_RawData.JPG)

## Adding Data to Container
Having created a AzureSQL container, either using the Azure Portal or via code, data can now be added to the container. 

Using this Azure Function data is downloaded from AisHub, parsed, and the recorded into Azure CosmosDB.








