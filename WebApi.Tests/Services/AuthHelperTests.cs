using System;

using Xunit;

using GrafanaProxy.WebApi.Configuration;
using GrafanaProxy.WebApi.Services;

namespace GrafanaProxy.WebApi.Tests
{
    public class AuthHelperTests
    {
        [Fact]
        public void EmptyQueryStringWithOneUserInConfig_ReturnsKeyForFirstUser()
        {            
            var queryString = "";
            var key = "1111111111111111";
            var config = new GrafanaProxyConfiguration 
            {
                UsersKeysStr = $"user1:{key},user2:2222222222222",                
            };
            var authHeadreVal = AuthHelper.GetAuthHeaderValue(queryString, config);
            Assert.Equal($"Bearer {key}", authHeadreVal);
        }

        [Fact]
        public void QueryStringWithUserId_ReturnsKeyForThisUser()
        {            
            var key1 = "1111111111111111";
            var key2 = "2222222222222222";
            var config = new GrafanaProxyConfiguration
            {
                UsersKeysStr = $"user1:{key1},user2:{key2}",
            };

            var queryString = "someParamAheadOfUserIdParam=someValue&proxy_userId=user2";
            var authHeadreVal = AuthHelper.GetAuthHeaderValue(queryString, config);
            Assert.Equal($"Bearer {key2}", authHeadreVal);

            queryString = "proxy_userId=user1&someParamBehindOfUserIdParam=someVal";
            authHeadreVal = AuthHelper.GetAuthHeaderValue(queryString, config);
            Assert.Equal($"Bearer {key1}", authHeadreVal);
        }
    }
}

