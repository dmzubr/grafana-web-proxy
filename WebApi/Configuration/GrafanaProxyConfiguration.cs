using System.Collections.Generic;

namespace GrafanaProxy.WebApi.Configuration
{
    public class GrafanaProxyConfiguration
    {
        /// <summary>
        /// Example is "http://grafana.bar.com:30000"
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Need to get it in Grafana admin UI
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Use custom CSS. Right now only for hiding sidebar and header with params
        /// </summary>
        public bool? InjectCustomCss { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UsersKeysStr { get; set; }

        /// <summary>
        /// This is used to request for target Grafana instance with different API keys
        /// Item key is a username, Key is an Grafana API key related to this user
        /// Use this to avoid direct passing of API keys in query strings or by other way
        /// Suppose that this approach is a little more secure
        /// </summary>
        public IDictionary<string, string> UsersKeys
        {
            get
            {
                if (string.IsNullOrEmpty(this.UsersKeysStr))
                    return null;

                var res = new Dictionary<string, string>();
                var strSpl = this.UsersKeysStr.Split(new char[] { ',' });
                foreach(var userApiKeyPairStr in strSpl)
                {
                    var itemSpl = userApiKeyPairStr.Split(new char[] { ':' });
                    if (itemSpl.Length > 1)
                    {
                        if (!res.ContainsKey(itemSpl[0]))
                            res.Add(itemSpl[0], itemSpl[1]);
                    }
                }
                return res;
            }            
        }
    }
}