using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;

namespace DotXxlJob.Core.Glue.TaskExecutors
{
    public class BaseGlueTaskExecutor
    {
        internal string ExecName = null;
        internal string Suffix = null;
        internal string ScriptPath = null;
        private readonly XxlJobExecutorOptions _options;
        private readonly IJobLogger _logger;
        private TriggerParam trigger = null;
        public BaseGlueTaskExecutor(
            string execName, string suffix,
            XxlJobExecutorOptions options, IJobLogger logger)
        {
            ExecName = execName;
            Suffix = suffix;
            _options = options;
            _logger = logger;
        }
        public async Task Init(TriggerParam triggerParam)
        {
            string _ScriptPath = Path.Combine(_options.LogPath, "Script");
            if (!Directory.Exists(_ScriptPath))
            {
                Directory.CreateDirectory(_ScriptPath);
            }
            trigger = triggerParam;
            ScriptPath = Path.Combine(_ScriptPath, $"{triggerParam.LogId}{Suffix}").Replace("\\", "/");
            await File.WriteAllTextAsync(ScriptPath, triggerParam.GlueSource, encoding: Encoding.UTF8);
            _logger.Log($"Create Script To {ScriptPath}");
        }
        public async Task<ReturnT> Execute(CancellationToken cancellationToken)
        {
            string rValue = "", err = "";
            Process p = new Process();
            p.StartInfo.FileName = ExecName;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.StartInfo.Arguments = $"{ScriptPath} {(string.IsNullOrWhiteSpace(trigger.ExecutorParams) ? "-" : trigger.ExecutorParams)} {trigger.BroadcastIndex} {trigger.BroadcastTotal}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.EnableRaisingEvents = true;
            p.Start();
            if (trigger.ExecutorTimeout > 0)
            {
                await p.WaitForExitAsync(cancellationToken).WaitAsync(TimeSpan.FromSeconds(trigger.ExecutorTimeout));
            }
            else
            {
                await p.WaitForExitAsync(cancellationToken);
            }
            rValue = await p.StandardOutput.ReadToEndAsync();
            err = await p.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(rValue))
            {
                _logger.Log($"{rValue}");
            }
            if (!string.IsNullOrEmpty(err))
            {
                _logger.LogError(new Exception(err));
                return ReturnT.FAIL;
            }
            return ReturnT.SUCCESS;
        }
    }
}
