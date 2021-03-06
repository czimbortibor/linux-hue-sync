using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HueCli 
{
    public class BridgeHandler : BackgroundService
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BridgeHandler> _logger;

        private const string _bridgeDiscoveryEndpoint = "https://www.meethue.com/api/nupnp";
        private const string _tempUserName = "anything";

        private const string _bridgeAddressCacheFile = "cache.json";
        private AuthorizedBridgeModel _bridgeModel;


        public BridgeHandler(IHttpClientFactory httpClientFactory, ILogger<BridgeHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                await DoWork();
                
                await Task.Delay(100);
            }
        }

        private async Task DoWork()
        {
            var colorProcesser = new ColorProcesser();

            (float x, float y) = await colorProcesser.CalculateDominantColorInXY();

            State newState = new State
            {
                Xy = new double[2] { x, y },
                Sat = 254,
                On = true,
                Bri = 200,
                Reachable = true
            };

            await SetState(lightNumber: 2, newState);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await EstablishConnection();

            await base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public async Task EstablishConnection() 
        {
            var httpClient = _httpClientFactory.CreateClient();

            _bridgeModel = await TryReadFromCache();

            if (_bridgeModel == null) 
            {
                Console.WriteLine("Lets try to find your Bridge...");

                HttpResponseMessage response = await httpClient.GetAsync(_bridgeDiscoveryEndpoint);

                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();

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
                        SuccesfulBridgeConnectionModel succesfulBridgeConnectionModel = await TryConnect(bridgeEndpoint, _tempUserName);

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
                await Cache(_bridgeModel);
            }
        }

        private async Task<SuccesfulBridgeConnectionModel> TryConnect(string endpoint, string userName) 
        {
            var httpClient = _httpClientFactory.CreateClient();

            StringContent content = new StringContent("{\"devicetype\": \"huecli#"+userName+"\"}", Encoding.UTF8, "application/json");
            while (true) 
            {
                HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                string responseMessage = await response.Content.ReadAsStringAsync();

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

        public async Task GetLights()
        {
            var httpClient = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync($"{_bridgeModel.RootEndpoint}/lights");
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

        }

        public async Task TurnOn(int lightNumber)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string turnOnState = "{\"on\": true}";
            var content = new StringContent(turnOnState, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync($"{_bridgeModel.RootEndpoint}/lights/{lightNumber}/state", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetState(int lightNumber, State newState)
        {
            var httpClient = _httpClientFactory.CreateClient();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string newStateStr = JsonConvert.SerializeObject(newState, serializerSettings);
            var content = new StringContent(newStateStr, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync($"{_bridgeModel.RootEndpoint}/lights/{lightNumber}/state", content);
            response.EnsureSuccessStatusCode();
        }

        private async Task Cache(AuthorizedBridgeModel bridgeModel) 
        {
            using (var writer = new StreamWriter(_bridgeAddressCacheFile, append: false))
            {
                string serializedBrigeModel = JsonConvert.SerializeObject(bridgeModel);
                await writer.WriteAsync(serializedBrigeModel);
            }
        }

        private async Task<AuthorizedBridgeModel> TryReadFromCache()
        {
            if (File.Exists(_bridgeAddressCacheFile))
            {
                using (var reader = new StreamReader(_bridgeAddressCacheFile))
                {
                    string serializedBrigeModel = await reader.ReadToEndAsync();
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