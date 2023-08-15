using DotXxlJob.Core.Glue.Models;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;
using Natasha.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DotXxlJob.Core.Glue.TaskExecutors
{
    public class NatashaTaskExecutor : ITaskExecutor
    {
        IJobLogger _jobLogger;
        private readonly ILogger<NatashaTaskExecutor> _logger;

        public NatashaTaskExecutor(IJobLogger jobLogger, ILogger<NatashaTaskExecutor> logger)
        {
            _jobLogger = jobLogger;
            _logger = logger;
        }

        public string GlueType { get; } = "GLUE_GROOVY";
        internal static Dictionary<string, NatashaExecModel> glDic = new Dictionary<string, NatashaExecModel>();
        internal static object nlock = new object();
        public async Task<ReturnT> Execute(TriggerParam triggerParam, CancellationToken cancellationToken)
        {
            try
            {
                string cm = ComputeMD5(triggerParam.GlueSource);

                lock (nlock)
                {
                    if (!glDic.ContainsKey(cm))
                    {
                        var action = (Action<IJobLogger, string>)FastMethodOperator.RandomDomain()
                      .Param<IJobLogger>("_jobLogger")
                      .Param<string>("_params")
                      .Body(triggerParam.GlueSource)
                      .Compile();
                        glDic.Add(cm, new NatashaExecModel
                        {
                            action = action,
                        });
                        _logger.LogDebug($"natasha add {cm}");
                    }
                    else glDic[cm].LastExecDate = DateTime.Now;
                }
                glDic[cm].action.Invoke(_jobLogger, triggerParam.ExecutorParams);

                return ReturnT.SUCCESS;
            }
            catch (Exception ex)
            {
                _jobLogger.LogError(ex);
                _logger.LogError(ex, ex.Message);
                return ReturnT.FAIL;
            }
        }
        private static string ComputeMD5(string text)  // 计算字符串的 MD5
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string hc = BitConverter.ToString(md.ComputeHash(Encoding.Default.GetBytes(text)));
            md.Dispose();
            return (hc);
        }
    }
}
