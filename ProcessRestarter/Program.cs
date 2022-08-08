using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Linq;

namespace ProcessRestarter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection().AddHttpClient();
            var builder = new ContainerBuilder();
            builder.Populate(services);
            var container = builder.Build();
            var factory = container.Resolve<IHttpClientFactory>();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .Console()
                .CreateLogger();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"),
                    optional: false);

            if (args?.Length > 0 && args[0] == "dev")
            {
                configBuilder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "appsettings.dev.json"), optional: true);
            }

            Log.Logger.Information("ProcessRestarter is Running");
            while (true)
            {
                // this way you can update "on the fly" because it'll read the config at each iteration
                IConfiguration config = configBuilder.Build();

                var client = new Client(Log.Logger, factory);
                var processStatus = new ProcessStatus(Log.Logger, config, client);

                var processes = config.GetSection("Processes");
                var waitTimeBetweenChecksInMinutes = config.GetValue<int>("TimeBetweenChecksInMinutes");
                var processesOptions = processes.Get<List<Processes>>();
                processStatus?.AnalyzeProcesses(processesOptions);

                Log.Logger.Information($"Sleeping for {waitTimeBetweenChecksInMinutes} minutes");
                Thread.Sleep(waitTimeBetweenChecksInMinutes * 60000); //minutes to milliseconds
            }
        }
    }
}