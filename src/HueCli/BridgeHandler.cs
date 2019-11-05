using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HueCli 
{
    public class BridgeHandler 
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
            public string BridgeId { get; set; }
            public string BridgeIpAddress { get; set; }
            public string UserName { get; set; }
        }

       
        private HttpClient _httpClient;
        private const string _bridgeDiscoveryEndpoint = "https://www.meethue.com/api/nupnp";
        private string _bridgeRootEndpoint => $"http://{_bridgeModel.BridgeIpAddress}/api";
        private string _authorizedBridgeRootEndpoint => $"http://{_bridgeModel.BridgeIpAddress}/api/{_bridgeModel.UserName}";

        private const string _bridgeAddressCacheFile = "cache.json";
        private AuthorizedBridgeModel _bridgeModel;

        public BridgeHandler()
        {
            _httpClient = new HttpClient();
        }

        public void EstablishConnection() 
        {
            _bridgeModel = 
                TryReadFromCache();

            if (_bridgeModel == null) 
            {
                Console.WriteLine("Lets try to find your Bridge...");

                HttpResponseMessage response = _httpClient.GetAsync(_bridgeDiscoveryEndpoint).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode) 
                {
                    string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    IEnumerable<FoundBridgeModel> bridges = JsonConvert.DeserializeObject<IEnumerable<FoundBridgeModel>>(responseContent);
                    FoundBridgeModel firstBridge = bridges.FirstOrDefault();

                    if (firstBridge == null) 
                    {
                        throw new Exception("Could not find the Bridge. Try again.");
                    }

                    _bridgeModel = new AuthorizedBridgeModel 
                    {
                        BridgeId = firstBridge.Id,
                        BridgeIpAddress = firstBridge.IpAddress,
                        UserName = null
                    };

                    Connect(_bridgeRootEndpoint);
                    Cache(_bridgeModel);
                }
                else 
                {
                    throw new Exception("Could not find the Bridge. Try again.");
                }
            }
        }

        public void GetLights()
        {
            HttpResponseMessage response = _httpClient.GetAsync($"{_authorizedBridgeRootEndpoint}/lights").GetAwaiter().GetResult();
            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            
        }

        public void TurnOn(int lightNumber)
        {
            string turnOnState = "{\"on\": true}";
            var content = new StringContent(turnOnState, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.PutAsync($"{_authorizedBridgeRootEndpoint}/lights/1/state", content).GetAwaiter().GetResult();
        }

        private void Cache(AuthorizedBridgeModel bridgeModel) 
        {
            using (var writer = new StreamWriter(_bridgeAddressCacheFile, append: false))
            {
                string serializedBrigeModel = JsonConvert.SerializeObject(bridgeModel);
                writer.Write(serializedBrigeModel);
            }
        }

        private AuthorizedBridgeModel TryReadFromCache()
        {
            if (File.Exists(_bridgeAddressCacheFile))
            {
                using (var reader = new StreamReader(_bridgeAddressCacheFile))
                {
                    string serializedBrigeModel = reader.ReadToEnd();
                    var bridgeModel = JsonConvert.DeserializeObject<AuthorizedBridgeModel>(serializedBrigeModel);
                    return bridgeModel;
                }
            }
            else 
            {
                return null;
            }
        }

        private void Connect(string endpoint) 
        {
            string userName = "tibi";
            StringContent content = new StringContent("{\"devicetype\": \"huecli#"+userName+"\"}", Encoding.UTF8, "application/json");
            while (true) 
            {
                HttpResponseMessage response = _httpClient.PostAsync(endpoint, content).GetAwaiter().GetResult();
                string responseMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (responseMessage.Contains("button not pressed") == false) 
                {
                    var succesfulBridgeConnectionModel = JsonConvert.DeserializeObject<IEnumerable<SuccesfulBridgeConnectionModel>>(responseMessage).FirstOrDefault();

                    Console.WriteLine($"Succesful connection. New user: {succesfulBridgeConnectionModel.Success.UserName}");
                    _bridgeModel.UserName = succesfulBridgeConnectionModel.Success.UserName;
                    break;
                } 
                else 
                {
                    Console.WriteLine("Press the button on the Bridge.");
                }

                System.Threading.Thread.Sleep(3000);
            }
        }
    }
}