using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Glue.Options;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core.Glue.TaskExecutors
{
    public class PHPTaskExecutor : ITaskExecutor
    {
        IJobLogger _jobLogger;
        BaseGlueTaskExecutor _baseGlueTaskExecutor;

        public PHPTaskExecutor(IJobLogger jobLogger, BaseGlueTaskExecutor baseGlueTaskExecutor)
        {
            _jobLogger = jobLogger;
            _baseGlueTaskExecutor = baseGlueTaskExecutor;
        }

        public string GlueType { get; } = "GLUE_PHP";

        public async Task<ReturnT> Execute(TriggerParam triggerParam, CancellationToken cancellationToken)
        {
            try
            {
                _baseGlueTaskExecutor.Init("php", ".php", triggerParam);
                return await _baseGlueTaskExecutor.Execute(cancellationToken);
            }
            catch (Exception ex)
            {
                _jobLogger.LogError(ex);
                return ReturnT.FAIL;
            }
        }
    }
}