using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Python.Runtime;

namespace HueCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitPythonRuntime();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x => {
                    x.AddDebug();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<BridgeHandler>();
                    services.AddHttpClient<BridgeHandler>();
                });

        public static void InitPythonRuntime()
        {
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
        }
    }
}
