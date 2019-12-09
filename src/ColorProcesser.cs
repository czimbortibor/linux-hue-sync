using System;
using System.Threading.Tasks;
using Python.Runtime;

namespace HueCli
{
    public class ColorProcesser
    {
        public async Task<(float, float)> CalculateDominantColorInXY()
        {
            (float x, float y, float z) = await Task.Run(() => GetColorFromScreen());
            
            x = x / (x + y + z);
            y = y / (x + y + z);

            return (x, y);
        }

        private (float, float, float) GetColorFromScreen()
        {
            dynamic xyzColorTuple;
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                // TODO remove this dev path
                sys.path.append("/home/tiborczimbor/linux-hue-sync/src/ColorProcesser/");

                dynamic colorProcesser = Py.Import("get_dominant_color");

                xyzColorTuple = colorProcesser.dominant_color_in_xyz();
                Console.WriteLine(xyzColorTuple);
            }

            float x = (float)xyzColorTuple[0];
            float y = (float)xyzColorTuple[1];
            float z = (float)xyzColorTuple[2];

            return (x, y, z);
        }
    }
}
