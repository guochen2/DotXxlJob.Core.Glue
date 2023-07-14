using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core.Glue.TaskExecutors
{
    public class ShellTaskExecutor : BaseGlueTaskExecutor, ITaskExecutor
    {
        IJobLogger _jobLogger;

        public ShellTaskExecutor(IJobLogger jobLogger, IOptions<XxlJobExecutorOptions> _options) :
            base("bash", ".sh", _options.Value, jobLogger)
        {
            _jobLogger = jobLogger;
        }

        public string GlueType { get; } = "GLUE_SHELL";

        public async Task<ReturnT> Execute(TriggerParam triggerParam, CancellationToken cancellationToken)
        {
            try
            {
                await Init(triggerParam);
                return await Execute(cancellationToken);
            }
            catch (Exception ex)
            {
                _jobLogger.LogError(ex);
                return ReturnT.FAIL;
            }
        }
    }
}