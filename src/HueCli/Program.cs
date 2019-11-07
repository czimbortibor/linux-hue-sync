using System;
using Python.Runtime;

namespace HueCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BridgeHandler bridgeHandler = new BridgeHandler();
            bridgeHandler.EstablishConnection();

            // bridgeHandler.GetLights();
            // bridgeHandler.TurnOn(lightNumber: 1);

            for (;;)
            {
                dynamic xyzColorTuple;
                using (Py.GIL())
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append("/home/tiborczimbor/linux-hue-sync/src/ColorProcesser/");
                    
                    dynamic colorProcesser = Py.Import("get_dominant_color");

                    xyzColorTuple = colorProcesser.dominant_color_in_xyz();
                    Console.WriteLine(xyzColorTuple);
                }

                float x = (float)xyzColorTuple[0];
                float y = (float)xyzColorTuple[1];

                State newState = new State
                {
                    Xy = new double[2] { x, y },
                    Sat = 254,
                    On = true,
                    Bri = 200,
                    Reachable = true
                };

                // bridgeHandler.SetState(1, newState);
                bridgeHandler.SetState(2, newState);

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
