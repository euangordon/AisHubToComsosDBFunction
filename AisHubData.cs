using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AisHub
{
    public partial class ShipPosition
    {
        [JsonProperty("MMSI")]
        public long Mmsi { get; set; }

        [JsonProperty("TIME")]
        public string Time { get; set; }

        [JsonProperty("LONGITUDE")]
        public double Longitude { get; set; }

        [JsonProperty("LATITUDE")]
        public double Latitude { get; set; }

        [JsonProperty("COG")]
        public double Cog { get; set; }

        [JsonProperty("SOG")]
        public double Sog { get; set; }

        [JsonProperty("HEADING")]
        public long Heading { get; set; }

        [JsonProperty("ROT")]
        public long Rot { get; set; }

        [JsonProperty("NAVSTAT")]
        public long Navstat { get; set; }

        [JsonProperty("IMO")]
        public long Imo { get; set; }

        [JsonProperty("NAME")]
        public string Name { get; set; }

        [JsonProperty("CALLSIGN")]
        public string Callsign { get; set; }

        [JsonProperty("TYPE")]
        public long Type { get; set; }

        [JsonProperty("A")]
        public long A { get; set; }

        [JsonProperty("B")]
        public long B { get; set; }

        [JsonProperty("C")]
        public long C { get; set; }

        [JsonProperty("D")]
        public long D { get; set; }

        [JsonProperty("DRAUGHT")]
        public double Draught { get; set; }

        [JsonProperty("DEST")]
        public string Dest { get; set; }

        [JsonProperty("ETA")]
        public string Eta { get; set; }
    }

    public partial class Summary
    {
        [JsonProperty("ERROR")]
        public bool Error { get; set; }

        [JsonProperty("USERNAME")]
        public string Username { get; set; }

        [JsonProperty("FORMAT")]
        public string Format { get; set; }

        [JsonProperty("RECORDS")]
        public long Records { get; set; }
    }

    public partial struct ShipPositionUnion
    {
        public Summary Summary;
        public List<ShipPosition> ShipPositions;

        public static implicit operator ShipPositionUnion(Summary Summary) => new ShipPositionUnion { Summary = Summary };
        public static implicit operator ShipPositionUnion(List<ShipPosition> ShipPositions) => new ShipPositionUnion { ShipPositions = ShipPositions };
    }

    public class AisHubData
    {
        public static List<ShipPositionUnion> FromJson(string json) => JsonConvert.DeserializeObject<List<ShipPositionUnion>>(json, AisHub.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<ShipPositionUnion> self) => JsonConvert.SerializeObject(self, AisHub.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ShipPositionUnionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ShipPositionUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ShipPositionUnion) || t == typeof(ShipPositionUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Summary>(reader);
                    return new ShipPositionUnion { Summary = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ShipPosition>>(reader);
                    return new ShipPositionUnion { ShipPositions = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ShipPositionUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ShipPositionUnion)untypedValue;
            if (value.ShipPositions != null)
            {
                serializer.Serialize(writer, value.ShipPositions);
                return;
            }
            if (value.Summary != null)
            {
                serializer.Serialize(writer, value.Summary);
                return;
            }
            throw new Exception("Cannot marshal type ShipPositionUnion");
        }

        public static readonly ShipPositionUnionConverter Singleton = new ShipPositionUnionConverter();
    }
}
