using DotXxlJob.Core.Glue.Options;
using DotXxlJob.Core.Glue.TaskExecutors;
using DotXxlJob.Core.Glue.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Glue.Hosted
{
    public class NatashaJobsExecuteHostedService : BackgroundService
    {
        private readonly ILogger<NatashaJobsExecuteHostedService> _logger;

        public NatashaJobsExecuteHostedService(ILogger<NatashaJobsExecuteHostedService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(">>>>>>>> start exec natasha dispose task <<<<<<<<");
            while (!stoppingToken.IsCancellationRequested)
            {
                lock (NatashaTaskExecutor.nlock)
                {
                    List<string> removekey = null; 
                    foreach (var item in NatashaTaskExecutor.glDic)
                    {
                        if (item.Value.LastExecDate.AddMinutes(3) < DateTime.Now)
                        {
                            item.Value.action.DisposeDomain();
                            item.Value.action.GetDomain().Dispose();
                            if (removekey == null) removekey = new List<string>();
                            removekey.Add(item.Key);
                        }
                    }
                    if (removekey != null)
                    {
                        foreach (var item in removekey)
                        {
                            _logger.LogDebug($"natasha remove {item}");
                            NatashaTaskExecutor.glDic.Remove(item);
                        }
                    }
                }

                //暂停3min
                await Task.Delay(180000, stoppingToken);
            }
            _logger.LogInformation(">>>>>>>> end natasha dispose task <<<<<<<<");
        }

    }
}
