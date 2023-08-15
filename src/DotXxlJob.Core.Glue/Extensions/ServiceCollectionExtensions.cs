using DotXxlJob.Core.Glue.Hosted;
using DotXxlJob.Core.Glue.Options;
using DotXxlJob.Core.Glue.TaskExecutors;
using DotXxlJob.Core.Glue.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Natasha.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Glue.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加glue支持
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddXxlJobGlue(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<XxlJobGlueExecutorOptions>(configuration.GetSection("xxlJob"));
            services.AddSingleton<ITaskExecutor, ShellTaskExecutor>();
            services.AddSingleton<ITaskExecutor, PythonTaskExecutor>();
            services.AddSingleton<ITaskExecutor, PHPTaskExecutor>();
            services.AddSingleton<ITaskExecutor, NodeJSTaskExecutor>();
            services.AddSingleton<ITaskExecutor, PowerShellTaskExecutor>();
            services.AddSingleton<BaseGlueTaskExecutor>();
            services.AddSingleton<ScriptClearTask>();
            services.AddSingleton<IHostedService, GlueJobsExecuteHostedService>();
            return services;
        }
        /// <summary>
        /// 添加源生代码支持 在xxljob里选择glue(java) 脚本需要填写c#脚本，通过natasha进行解析执行
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddXxlJobGlueGROOVYUseNatasha(this IServiceCollection services)
        {
            services.AddSingleton<ITaskExecutor, NatashaTaskExecutor>();
            //清理natasha domain 没有明显的释放内存
            //services.AddSingleton<IHostedService, NatashaJobsExecuteHostedService>();
            return services;
        }
        public static IApplicationBuilder UseXxlJobGlueGROOVYUseNatasha(this IApplicationBuilder app,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                var logger = app.ApplicationServices.GetService<ILogger<NatashaJobsExecuteHostedService>>();
                logger.LogInformation("start init NatashaInitializer");
                NatashaInitializer.Preheating();
                logger.LogInformation("finish init NatashaInitializer");
            });
            return app;
        }
    }
}
