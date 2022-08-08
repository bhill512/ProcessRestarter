using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace ProcessRestarter
{
    public class ProcessStatus
    {
        ILogger _logger;
        IConfiguration _config;
        Client _client;

        public ProcessStatus(ILogger logger, IConfiguration config, Client client)
        {
            _logger = logger;
            _config = config;
            _client = client;
        }

        public void StartProcessIfNotRunning(string processName, string processLoc)
        {
            _logger.Information($"Checking the status of {processName}");
            if (System.Diagnostics.Process.GetProcessesByName($"{processName}").Length == 0)
            {
                _logger.Information($"{processName} is not running\n Starting {processName} at location {processLoc}");
                if (File.Exists($"{processLoc}")){
                    System.Diagnostics.Process.Start($"{processLoc}");
                    if (System.Diagnostics.Process.GetProcessesByName($"{processName}").Length == 0)
                    {
                        _logger.Information($"{processName} could not be started\n Will try again later");
                    }
                    else
                    {
                        var processesWithPids = System.Diagnostics.Process.GetProcessesByName($"{processName}");
                        foreach (var p in processesWithPids)
                        {
                            _logger.Information($"{processName} successfully started with PID {p.Id}");
                        }
                    }
                }
                else
                {
                    _logger.Information($"{processLoc} does not exist. Exiting");
                    return;
                }
            }
            _logger.Information($"{processName} is running");
        }

        public void KillProcess(string processName, int retries)
        {
            _logger.Information($"Attemping to Kill process {processName}");
            var process = System.Diagnostics.Process.GetProcessesByName($"{processName}")[0];
            if (process.Responding && retries != 0)
            {
                process.Kill();
                process.WaitForExit(5000);  
                if (process.HasExited)
                {
                    _logger.Information($"Successfully killed {process.ProcessName}");
                }
                else
                {
                    //recursion :-O
                    _logger.Information($"Gonna try to kill it again. Attempt number {retries}");
                    process.Refresh();
                    KillProcess(processName, retries--);
                }
            }
            else
            {
                _logger.Information($"Could not kill process by the name of: {processName}");  
            }
        }

        public async Task AnalyzeProcesses(List<Processes> processOptions)
        {
            foreach(var process in processOptions)
            {
                StartProcessIfNotRunning(process.Name, process.Location);
                if (process.Name == "Plex Media Server" && File.Exists($"{process.Location}"))
                {
                    var plexlibraries = await _client.GetPlexLibraries($"{_config["Plex:ServerUrl"]}", $"{_config["Plex:XPlexToken"]}").ConfigureAwait(false);

                    //either no response from the server, OR there's a response but the process doesn't see the libraries - restart the process
                    if (plexlibraries == null || String.IsNullOrEmpty(plexlibraries?.MediaContainer.Directory[0].Title))
                    {
                        KillProcess(process.Name, 3);

                        StartProcessIfNotRunning(process.Name, process.Location);
                    }
                }
            }
        }
    }
}
