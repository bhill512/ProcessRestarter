using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRestarter
{
    public class ProcessStatus
    {
        ILogger _logger;
        IConfiguration _config;

        public ProcessStatus(ILogger logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public void StartProcessIfNotRunning(string processName, string processLoc)
        {
            Log.Logger.Information($"Checking the status of {processName}");
            if (System.Diagnostics.Process.GetProcessesByName($"{processName}").Length == 0)
            {
                Log.Logger.Information($"{processName} is not running\n Starting {processName} at location {processLoc}");
                if (File.Exists($"{processLoc}")){
                    System.Diagnostics.Process.Start($"{processLoc}");
                    if (System.Diagnostics.Process.GetProcessesByName($"{processName}").Length == 0)
                    {
                        Log.Logger.Information($"{processName} could not be started\n Will try again later");
                        return;
                    }
                    else
                    {
                        var processesWithPids = System.Diagnostics.Process.GetProcessesByName($"{processName}");
                        foreach (var p in processesWithPids)
                        {
                            Log.Logger.Information($"{processName} successfully started with PID {p.Id}");
                        }
                    }
                }
                else
                {
                    Log.Logger.Information($"{processLoc} does not exist. Exiting");
                    return;
                }
            }
        }
    }
}
