using System;

using Xunit;

using GrafanaProxy.WebApi.Configuration;

namespace GrafanaProxy.WebApi.Tests
{
    public class GrafanaProxyConfigurationTests
    {
        [Fact]
        public void CreateConfigInstance_NotThrowsException()
        {
            var config = new GrafanaProxyConfiguration();
            Assert.True(true);
        }

        [Fact]
        public void GetUsersKeysDict_ReturnsValidDictObject()
        {
            var key1 = "odasokpadspoaoksdpokdask";
            var key2 = "2832498432749382743798ds";
            var config = new GrafanaProxyConfiguration
            {
                UsersKeysStr = $"user1:{key1},user2:{key2}"
            };

            Assert.True(config.UsersKeys.ContainsKey("user1"));
            Assert.True(config.UsersKeys.ContainsKey("user2"));
            Assert.True(config.UsersKeys["user1"] == key1);
            Assert.True(config.UsersKeys["user2"] == key2);
        }
    }
}
