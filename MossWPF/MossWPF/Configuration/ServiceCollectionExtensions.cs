﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MossWPF.Domain.Services;
using MossWPF.Services;

namespace MossWPF.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                var logger = provider.GetService<ILogger<WritableOptions<T>>>();
                return new WritableOptions<T>(options, configuration, section.Key, file, logger);
            });
        }
    }
}
