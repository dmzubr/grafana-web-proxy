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
    }
}