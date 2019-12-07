using Newtonsoft.Json;

namespace HueCli
{
    public class FoundBridgeModel 
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("internalipaddress")]
        public string IpAddress { get; set; }
    }

    public class SuccesfulBridgeConnectionModel 
    {
        [JsonProperty("success")]
        public SuccessModel Success { get; set; } 
    }

    public class SuccessModel 
    {
        [JsonProperty("username")]
        public string UserName { get; set; }
    }


    public class AuthorizedBridgeModel 
    {
        public string Id { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }

        public string RootEndpoint => $"http://{IpAddress}/api/{UserName}";
    }
}
