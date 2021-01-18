using System.Linq;

using GrafanaProxy.WebApi.Configuration;
using Microsoft.AspNetCore.Http;

namespace GrafanaProxy.WebApi.Services
{
    public static class AuthHelper
    {
        /// <summary>
        /// Get auth header for target Grafana instance
        /// Extract userId from request query string
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetAuthHeaderValue(string queryString, GrafanaProxyConfiguration config)
        {
            var res = "";

            var userParamMarker = "proxy_userId=";
            var userParStartIndex = queryString.IndexOf(userParamMarker);
            if (userParStartIndex > -1)
            {
                var userParEndIndex = queryString.IndexOf("&", userParStartIndex+1);
                if (userParEndIndex == -1)
                    userParEndIndex = queryString.Length;
                userParStartIndex = userParStartIndex + userParamMarker.Length;
                var userParVal = queryString.Substring(userParStartIndex, userParEndIndex - userParStartIndex);
                if (config.UsersKeys.ContainsKey(userParVal))
                    res = config.UsersKeys[userParVal];
            }
            else
            {
                // No user found in query string - try to value for the first user
                if (config.UsersKeys != null && config.UsersKeys.Keys.Count > 0)
                    res = config.UsersKeys.FirstOrDefault().Value;
                else
                    // No user keys is defined. So - try to get key value from config's ApiKey field
                    res = config.ApiKey;
            }

            return $"Bearer {res}";
        }

        /// <summary>
        /// Get auth header for target Grafana instance
        /// Extract userId from request HttpRequest object
        /// This methods is for future needs when userId can be passed not in query string only, but in Auth header for example.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetAuthHeaderValue(HttpRequest req, GrafanaProxyConfiguration config)
        {
            var queryString = req.QueryString.Value;
            return GetAuthHeaderValue(queryString, config);
        }
    }
}
