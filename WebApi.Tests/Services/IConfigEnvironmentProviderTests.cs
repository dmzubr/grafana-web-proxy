using System;

using Xunit;

using GrafanaProxy.WebApi.Configuration;
using GrafanaProxy.WebApi.Services;

namespace GrafanaProxy.WebApi.Tests
{
    public class IConfigEnvironmentProviderTests
    {
        [Fact]
        public void GetConfig_NotThrowsException()
        {
            var configProvider = new ConfigEnvProvider();
            var config  = configProvider.GetConfig();
            Assert.True(true);
        }

        [Fact]
        public void GetConfig_ReturnsObjectOfRelevantType()
        {
            var configProvider = new ConfigEnvProvider();
            var config = configProvider.GetConfig();
            Assert.IsType<GrafanaProxyConfiguration>(config);
        }

        [Fact]
        public void GetConfig_ReturnsRealValueFromEnv()
        {
            // Set ENV variable value first
            var varVal = "some_val";
            Environment.SetEnvironmentVariable("GRAFANA_BASE_URL", varVal);
            var configProvider = new ConfigEnvProvider();
            var config = configProvider.GetConfig();
            Assert.Equal(varVal, config.BaseUrl);            
        }
    }
}
