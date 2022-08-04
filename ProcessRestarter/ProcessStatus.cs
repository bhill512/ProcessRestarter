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
    }
}
