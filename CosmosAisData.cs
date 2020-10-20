using System;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Spatial;
using System.Collections.Generic;
using System.Globalization;

namespace AisHub
{
    public class CosmosAisData
    {
        public string id { get; set; }
        public long MMSI { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Point Location { get; set; }
        public double CourseOverGround { get; set; }
        public double SpeedOverGroud { get; set; }
        public long Heading { get; set; }
        public long RateOfTurn { get; set; }
        public long Navstat { get; set; }
        public long Imo { get; set; }
        public string Callsign { get; set; }
        public long VesselType { get; set; }
        public long DimensionToBow { get; set; }
        public long DimensionToStern { get; set; }
        public long DimensionToPort { get; set; }
        public long DimensionToStarboard { get; set; }
        public double Draught { get; set; }
        public string Destination { get; set; }
        public string ETA { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public CosmosAisData() { }

        public static List<CosmosAisData> ConvertToCosmosFormat(List<ShipPosition> aisHubRawData)
        {
            var cosmosAisData = new List<CosmosAisData>();

            foreach (var a in aisHubRawData)
            {
                var date = DateTime.ParseExact(a.Time, "yyyy-MM-dd HH:mm:ss GMT", CultureInfo.InvariantCulture);
                var id = date.Ticks.ToString();

                var cosmosAisPosition = new CosmosAisData
                {
                    id = id,
                    MMSI = a.Mmsi,
                    Name = a.Name,
                    Date = date,
                    Location = new Point(a.Longitude, a.Latitude),
                    CourseOverGround = a.Cog,
                    SpeedOverGroud = a.Sog,
                    Heading = a.Heading,
                    RateOfTurn = a.Rot,
                    Navstat = a.Navstat,
                    Imo = a.Imo,
                    Callsign = a.Callsign,
                    VesselType = a.Type,
                    DimensionToBow = a.A,
                    DimensionToStern = a.B,
                    DimensionToPort = a.C,
                    DimensionToStarboard = a.D,
                    Draught = a.Draught,
                    Destination = a.Dest,
                    ETA = a.Eta
                };
                cosmosAisData.Add(cosmosAisPosition);
            }

            return cosmosAisData;
        }
    }


}



