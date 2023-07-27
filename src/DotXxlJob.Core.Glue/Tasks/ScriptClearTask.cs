using DotXxlJob.Core.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Glue.Tasks
{
    /// <summary>
    /// 自动清理1小时之前的脚本
    /// </summary>
    public class ScriptClearTask
    {
        private readonly ILogger<ScriptClearTask> _logger;
        private readonly XxlJobExecutorOptions _options;

        public ScriptClearTask(ILogger<ScriptClearTask> logger, IOptions<XxlJobExecutorOptions> optionsAccessor)
        {
            _logger = logger;
            _options = optionsAccessor.Value;
        }

        public async Task ExecAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(">>>>>>>> start exec script clear task <<<<<<<<");
            string _ScriptPath = Path.Combine(_options.LogPath, "Script");

            while (!cancellationToken.IsCancellationRequested)
            {
                if (Directory.Exists(_ScriptPath))
                {
                    foreach (var file in new DirectoryInfo(_ScriptPath).GetFiles())
                    {
                        if (DateTime.Now.Subtract(file.CreationTime).TotalMinutes > 60)
                        {
                            file.Delete();
                        }
                    }
                }
                //暂停1min
                await Task.Delay(60000, cancellationToken);
            }

            _logger.LogInformation(">>>>>>>> end script clear task <<<<<<<<");
        }
    }
}
