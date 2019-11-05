namespace HueCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BridgeHandler bridgeHandler = new BridgeHandler();
            bridgeHandler.EstablishConnection();

            bridgeHandler.GetLights();
            bridgeHandler.TurnOn(lightNumber: 1);
        }
    }
}
