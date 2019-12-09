using System;
using System.Threading;
using System.Threading.Tasks;
using Python.Runtime;

namespace HueCli
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            InitPythonRuntime();


            BridgeHandler bridgeHandler = new BridgeHandler();
            await bridgeHandler.EstablishConnection();

            // await bridgeHandler.GetLights();
            // await bridgeHandler.TurnOn(lightNumber: 1);

            var colorProcesser = new ColorProcesser();
            for (;;)
            {
                (float x, float y) = await colorProcesser.CalculateDominantColorInXY();

                State newState = new State
                {
                    Xy = new double[2] { x, y },
                    Sat = 254,
                    On = true,
                    Bri = 200,
                    Reachable = true
                };

                // bridgeHandler.SetState(1, newState);
                await bridgeHandler.SetState(lightNumber: 2, newState);

                Thread.Sleep(100);
            }
        }

        public static void InitPythonRuntime()
        {
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
        }
    }
}
