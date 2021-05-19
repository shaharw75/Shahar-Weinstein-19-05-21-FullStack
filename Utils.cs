using System;
using Microsoft.Extensions.Configuration;

namespace RealCommerce
{
    public static class Utils
    {
        public static IConfigurationRoot GetAppSettings()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration;
        }
        
        public static T GetConfigByName<T>(string configKey)
        {
            return GetAppSettings().GetValue<T>($"Configurations:{configKey}");
        }
    }
}