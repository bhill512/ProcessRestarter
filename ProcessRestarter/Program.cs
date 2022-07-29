using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ProcessRestarter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .File(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Process_Restarter_Logs.txt"))
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
            var processesOptions = processes.Get<List<Processes>>();


            var waitTimeBetweenChecksInMinutes = config.GetValue<int>("TimeBetweenChecksInMinutes");

            var processStatus = new ProcessStatus(Log.Logger, config);

            while (true)
            {
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