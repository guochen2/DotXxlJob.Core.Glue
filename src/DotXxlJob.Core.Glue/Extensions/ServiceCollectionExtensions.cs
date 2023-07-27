using DotXxlJob.Core.Glue.Hosted;
using DotXxlJob.Core.Glue.Options;
using DotXxlJob.Core.Glue.TaskExecutors;
using DotXxlJob.Core.Glue.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
    }
}
