using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HueCli 
{
    public partial class LightModel
    {
        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("swupdate")]
        public Swupdate Swupdate { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modelid")]
        public string Modelid { get; set; }

        [JsonProperty("manufacturername")]
        public string Manufacturername { get; set; }

        [JsonProperty("productname")]
        public string Productname { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }

        [JsonProperty("config")]
        public Config Config { get; set; }

        [JsonProperty("uniqueid")]
        public string Uniqueid { get; set; }

        [JsonProperty("swversion")]
        public string Swversion { get; set; }

        [JsonProperty("swconfigid")]
        public string Swconfigid { get; set; }

        [JsonProperty("productid")]
        public string Productid { get; set; }
    }

    public partial class Capabilities
    {
        [JsonProperty("certified")]
        public bool Certified { get; set; }

        [JsonProperty("control")]
        public Control Control { get; set; }

        [JsonProperty("streaming")]
        public Streaming Streaming { get; set; }
    }

    public partial class Control
    {
        [JsonProperty("mindimlevel")]
        public long Mindimlevel { get; set; }

        [JsonProperty("maxlumen")]
        public long Maxlumen { get; set; }

        [JsonProperty("colorgamuttype")]
        public string Colorgamuttype { get; set; }

        [JsonProperty("colorgamut")]
        public double[][] Colorgamut { get; set; }

        [JsonProperty("ct")]
        public Ct Ct { get; set; }
    }

    public partial class Ct
    {
        [JsonProperty("min")]
        public long Min { get; set; }

        [JsonProperty("max")]
        public long Max { get; set; }
    }

    public partial class Streaming
    {
        [JsonProperty("renderer")]
        public bool Renderer { get; set; }

        [JsonProperty("proxy")]
        public bool Proxy { get; set; }
    }

    public partial class Config
    {
        [JsonProperty("archetype")]
        public string Archetype { get; set; }

        [JsonProperty("function")]
        public string Function { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("startup")]
        public Startup Startup { get; set; }
    }

    public partial class Startup
    {
        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("configured")]
        public bool Configured { get; set; }
    }

    public partial class State
    {
        [JsonProperty("on")]
        public bool On { get; set; }

        [JsonProperty("bri")]
        public long Bri { get; set; }

        [JsonProperty("hue")]
        public long? Hue { get; set; }

        [JsonProperty("sat")]
        public long? Sat { get; set; }

        [JsonProperty("effect")]
        public string Effect { get; set; }

        [JsonProperty("xy")]
        public double[] Xy { get; set; }

        [JsonProperty("ct")]
        public long? Ct { get; set; }

        [JsonProperty("alert")]
        public string Alert { get; set; }

        [JsonProperty("colormode")]
        public string Colormode { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("reachable")]
        public bool Reachable { get; set; }
    }

    public partial class Swupdate
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("lastinstall")]
        public DateTimeOffset Lastinstall { get; set; }
    }

    // internal static class Converter
    // {
    //     public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //     {
    //         NullValueHandling = NullValueHandling.Ignore,
    //         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //         DateParseHandling = DateParseHandling.None,
    //         Converters = 
    //         {
    //             new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //         },
    //     };
    // }
}