using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace HueCli 
{
    public class BridgeHandler 
    {
        private HttpClient _httpClient;
        private const string _bridgeDiscoveryEndpoint = "https://www.meethue.com/api/nupnp";
        private const string _tempUserName = "anything";

        private const string _bridgeAddressCacheFile = "cache.json";
        private AuthorizedBridgeModel _bridgeModel;

        public BridgeHandler()
        {
            _httpClient = new HttpClient();
        }

        public void EstablishConnection() 
        {
            _bridgeModel = TryReadFromCache();

            if (_bridgeModel == null) 
            {
                Console.WriteLine("Lets try to find your Bridge...");

                HttpResponseMessage response = _httpClient.GetAsync(_bridgeDiscoveryEndpoint).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                IEnumerable<FoundBridgeModel> bridges = 
                    JsonConvert.DeserializeObject<IEnumerable<FoundBridgeModel>>(responseContent);
                
                AuthorizedBridgeModel authorizedBridgeModel = null;
                foreach (FoundBridgeModel bridge in bridges)
                {
                    if (bridge == null) 
                    {
                        continue;
                    }

                    string bridgeEndpoint = $"http://{bridge.IpAddress}/api";
                    try 
                    {
                        SuccesfulBridgeConnectionModel succesfulBridgeConnectionModel = TryConnect(bridgeEndpoint, _tempUserName);

                        authorizedBridgeModel = new AuthorizedBridgeModel
                        {
                            Id = bridge.Id,
                            IpAddress = bridge.IpAddress,
                            UserName = succesfulBridgeConnectionModel.Success.UserName
                        };

                        break;
                    }
                    catch (Exception) 
                    {
                        // ignored
                    }
                }

                if (authorizedBridgeModel == null)
                {
                    throw new Exception("Could not find the Bridge. Try again.");
                }

                _bridgeModel = authorizedBridgeModel;
                Cache(_bridgeModel);
            }
        }

        private SuccesfulBridgeConnectionModel TryConnect(string endpoint, string userName) 
        {
            StringContent content = new StringContent("{\"devicetype\": \"huecli#"+userName+"\"}", Encoding.UTF8, "application/json");
            while (true) 
            {
                HttpResponseMessage response = _httpClient.PostAsync(endpoint, content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                string responseMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (responseMessage.Contains("button not pressed") == false) 
                {
                    SuccesfulBridgeConnectionModel succesfulBridgeConnectionModel = 
                        JsonConvert.DeserializeObject<IEnumerable<SuccesfulBridgeConnectionModel>>(responseMessage)
                                    .FirstOrDefault();

                    Console.WriteLine($"Succesful connection. New user: {succesfulBridgeConnectionModel.Success.UserName}");
                    return succesfulBridgeConnectionModel;
                }
                else 
                {
                    Console.WriteLine("Press the button on the Bridge.");
                }

                Thread.Sleep(3000);
            }
        }

        public void GetLights()
        {
            HttpResponseMessage response = _httpClient.GetAsync($"{_bridgeModel.RootEndpoint}/lights").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        }

        public void TurnOn(int lightNumber)
        {
            string turnOnState = "{\"on\": true}";
            var content = new StringContent(turnOnState, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.PutAsync($"{_bridgeModel.RootEndpoint}/lights/{lightNumber}/state", content).GetAwaiter().GetResult();
        }

        public void SetState(int lightNumber, State newState)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string newStateStr = JsonConvert.SerializeObject(newState, serializerSettings);
            var content = new StringContent(newStateStr, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.PutAsync($"{_bridgeModel.RootEndpoint}/lights/{lightNumber}/state", content).GetAwaiter().GetResult();
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
    }
}