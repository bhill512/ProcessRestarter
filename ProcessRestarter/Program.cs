using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;

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

            IConfiguration config = configBuilder.Build();

            var processes = config.GetSection("Processes");

            var waitTimeBetweenChecksInMinutes = config.GetValue<int>("TimeBetweenChecksInMinutes");

            var client = new Client(Log.Logger, factory);
            var processStatus = new ProcessStatus(Log.Logger, config);

            Log.Logger.Information("ProcessRestarter is Running");
            while (true)
            {
                var plexlibraries = await client.GetPlexLibraries($"{config["Plex:ServerUrl"]}", $"{config["Plex:XPlexToken"]}").ConfigureAwait(false);
                Log.Logger.Information($"Here's some Title Library Info: {plexlibraries.MediaContainer.Directory[0].Title}");

                var processesOptions = processes.Get<List<Processes>>();
                foreach (var p in processesOptions)
                {
                    processStatus.StartProcessIfNotRunning(p.Name, p.Location);
                }
                Log.Logger.Information($"Sleeping for {waitTimeBetweenChecksInMinutes} minutes");
                Thread.Sleep(waitTimeBetweenChecksInMinutes * 60000); //minutes to milliseconds
            }
        }
    }
}