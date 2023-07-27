using DotXxlJob.Core.Glue.Options;
using DotXxlJob.Core.Glue.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Glue.Hosted
{
    public class GlueJobsExecuteHostedService : BackgroundService
    {
        private readonly IExecutorRegistry _registry;
        private readonly ScriptClearTask scriptClearTask;
        private readonly IOptions<XxlJobGlueExecutorOptions> _options;


        public GlueJobsExecuteHostedService(IExecutorRegistry registry, ScriptClearTask scriptClearTask, IOptions<XxlJobGlueExecutorOptions> options)
        {
            this._registry = registry;
            this.scriptClearTask = scriptClearTask;
            _options = options;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.Value.AutoClearScript)
            {
                return scriptClearTask.ExecAsync(stoppingToken);
            }
            return Task.CompletedTask;
        }

    }
}
