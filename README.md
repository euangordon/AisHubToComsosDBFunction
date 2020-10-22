# AisHubToComsosDBFunction
C# NET Core - Azure Function which connects to the AisHub API and stores the vessel (ship) position data received in Azure CosmosDB

Prerequisites: 
1. An account with [AisHub](http://www.aishub.net/) to receive data from their API. You will need to contribute data to them, see the [RPIAIS](http://www.aishub.net/rpiais) seciotn for how to do this with a Raspberry Pi.
2. CosmosDB setup, with a new Container with the Partition ID set to /MMSI
