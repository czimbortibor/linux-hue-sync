using System;
using Python.Runtime;

namespace HueCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // using (Py.GIL()) 
            // {
            //     dynamic sys = Py.Import("sys");
            //     sys.path.append("/home/tiborczimbor/linux-hue-sync/src/ColorProcesser/");
                
            //     dynamic colorProcesser = Py.Import("get_dominant_color");
            //     Console.WriteLine(colorProcesser.dominant_hexcolor());
            // }

            BridgeHandler bridgeHandler = new BridgeHandler();
            bridgeHandler.EstablishConnection();

            bridgeHandler.GetLights();
            bridgeHandler.TurnOn(lightNumber: 1);
        }
    }
}
