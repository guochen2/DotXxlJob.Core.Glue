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
using DotXxlJob.Core.Glue.Options;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core.Glue.TaskExecutors
{
    public class BaseGlueTaskExecutor
    {
        private readonly AsyncLocal<string> ExecName = new AsyncLocal<string>();
        private readonly AsyncLocal<string> Suffix = new AsyncLocal<string>();
        private readonly AsyncLocal<string> ScriptPath = new AsyncLocal<string>();
        private readonly AsyncLocal<TriggerParam> trigger = new AsyncLocal<TriggerParam>();
        private readonly XxlJobExecutorOptions _options;
        private readonly XxlJobGlueExecutorOptions _glueoptions;
        private readonly IJobLogger _logger;
        public BaseGlueTaskExecutor(IOptions<XxlJobExecutorOptions> options, IJobLogger logger, IOptions<XxlJobGlueExecutorOptions> glueoptions)
        {
            _options = options.Value;
            _logger = logger;
            _glueoptions = glueoptions.Value;
        }
        public void Init(string execName, string suffix, TriggerParam triggerParam)
        {
            ExecName.Value = execName;
            Suffix.Value = suffix;
            string _ScriptPath = Path.Combine(_options.LogPath, "Script");
            if (!Directory.Exists(_ScriptPath))
            {
                Directory.CreateDirectory(_ScriptPath);
            }
            trigger.Value = triggerParam;
            ScriptPath.Value = Path.Combine(_ScriptPath, $"{triggerParam.LogId}{Suffix.Value}").Replace("\\", "/");
            File.WriteAllText(ScriptPath.Value, triggerParam.GlueSource, encoding: new UTF8Encoding(false));
            _logger.Log($"Create Script To {ScriptPath.Value}");
        }
        public async Task<ReturnT> Execute(CancellationToken cancellationToken)
        {
            string rValue = "", err = "";
            Process p = new Process();
            p.StartInfo.FileName = ExecName.Value;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.StartInfo.Arguments = $"{ScriptPath.Value} {(string.IsNullOrWhiteSpace(trigger.Value.ExecutorParams) ? "-" : trigger.Value.ExecutorParams)} {trigger.Value.BroadcastIndex} {trigger.Value.BroadcastTotal}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.EnableRaisingEvents = true;
            p.Start();
            if (trigger.Value.ExecutorTimeout > 0)
            {
                await p.WaitForExitAsync(cancellationToken).WaitAsync(TimeSpan.FromSeconds(trigger.Value.ExecutorTimeout));
            }
            else
            {
                await p.WaitForExitAsync(cancellationToken);
            }
            rValue = await p.StandardOutput.ReadToEndAsync();
            err = await p.StandardError.ReadToEndAsync();
            if (_glueoptions.AutoClearScript)
            {
                new FileInfo(ScriptPath.Value).Delete();
            }
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
