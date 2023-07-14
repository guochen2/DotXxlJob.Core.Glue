using DotXxlJob.Core.Glue.TaskExecutors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        public static IServiceCollection AddXxlJobGlue(this IServiceCollection services)
        {
            services.AddSingleton<ITaskExecutor, ShellTaskExecutor>();
            services.AddSingleton<ITaskExecutor, PythonTaskExecutor>();
            services.AddSingleton<ITaskExecutor, PHPTaskExecutor>();
            services.AddSingleton<ITaskExecutor, NodeJSTaskExecutor>();
            services.AddSingleton<ITaskExecutor, PowerShellTaskExecutor>();
            return services;
        }
    }
}
