using System;

using GrafanaProxy.WebApi.Configuration;

namespace GrafanaProxy.WebApi.Services
{
    /// <summary>
    /// This provide service configuration details from Environment
    /// Env configuration overrides config from appsettings.json
    /// Suppose that this way is an easiest to configure service when run in Docker
    /// </summary>
    public interface IConfigEnvironmentProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        GrafanaProxyConfiguration GetConfig();
    }

    public class ConfigEnvProvider : IConfigEnvironmentProvider
    {
        public GrafanaProxyConfiguration GetConfig()        
        {
            var res = new GrafanaProxyConfiguration
            {
                BaseUrl = Environment.GetEnvironmentVariable("GRAFANA_BASE_URL"),
                ApiKey = Environment.GetEnvironmentVariable("GRAFANA_API_KEY"),
                InjectCustomCss = null
            };

            var injectCustomCssStr = Environment.GetEnvironmentVariable("GRAFANA_INJECT_CUSTOM_CSS");
            bool injectCustomCss = false;
            if (bool.TryParse(injectCustomCssStr, out injectCustomCss))
                res.InjectCustomCss = injectCustomCss;
            
            return res;
        }        
    }
}
