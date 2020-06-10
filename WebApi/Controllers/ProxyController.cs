using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using GrafanaProxy.WebApi.Configuration;
using GrafanaProxy.WebApi.Services;

namespace GrafanaProxy.WebApi.Controllers
{
    public class ProxyController : Controller
    {
        private readonly string _grafanaBaseUrl;
        private readonly bool _injectCustomCss;
        
        private readonly GrafanaProxyConfiguration _config;

        public ProxyController(IOptions<GrafanaProxyConfiguration> optionsAccessor, IConfigEnvironmentProvider configEnvProvider) 
        {            
            var configFromEnv = configEnvProvider.GetConfig();
            this._config = configFromEnv;

            this._grafanaBaseUrl = !string.IsNullOrEmpty(configFromEnv.BaseUrl) ? configFromEnv.BaseUrl : optionsAccessor.Value.BaseUrl;
            
            this._injectCustomCss = configFromEnv.InjectCustomCss.HasValue ? 
                configFromEnv.InjectCustomCss.Value : 
                optionsAccessor.Value.InjectCustomCss.HasValue ? optionsAccessor.Value.InjectCustomCss.Value : false;
        }

        /// <summary>
        /// This request if for html page (initial request)
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [Route("d/{**rest}")]
        public async Task<IActionResult> ProxyRoot(string rest)
        {
            // If you don't need the query string, then you can remove this.
            var queryString = this.Request.QueryString.Value;
            var grafanaTargetUrl = $"{_grafanaBaseUrl}/d/{rest}{queryString}";
            var httpResp = await grafanaTargetUrl
                .WithHeader("Authorization", AuthHelper.GetAuthHeaderValue(this.Request, this._config))
                .GetAsync();            
            var responseContent = await httpResp.Content.ReadAsStringAsync();
            var res = new ContentResult
            {

                ContentType = httpResp.Content.Headers.GetValues("Content-Type").FirstOrDefault(),
                Content = responseContent
            };
            return res;
        }

        /// <summary>
        /// Request for data API
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [Route("api/{**rest}")]
        public async Task<IActionResult> ProxyApi(string rest)
        {
            var queryString = this.Request.QueryString.Value;
            var grafanaTargetUrl = $"{_grafanaBaseUrl}/api/{rest}{queryString}";
            System.Net.Http.HttpResponseMessage httpResp = null;
            var httpReq = grafanaTargetUrl
                .WithHeader("Authorization", AuthHelper.GetAuthHeaderValue(this.Request, this._config));
            switch (Request.Method.ToUpper())
            {
                case "GET":
                    httpResp = await httpReq.GetAsync();
                    break;
                case "POST":
                    var bodyStr = "";
                    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        bodyStr = await reader.ReadToEndAsync();
                    }
                    var postedData = Newtonsoft.Json.JsonConvert.DeserializeObject(bodyStr);
                    httpResp = await httpReq.PostJsonAsync(postedData);
                    break;
                // Suppose, that we won't use any other HTTP verbs
            }

            var responseContent = await httpResp.Content.ReadAsStringAsync();
            var res = new ContentResult
            {
                ContentType = httpResp.Content.Headers.GetValues("Content-Type").FirstOrDefault(),
                Content = responseContent
            };
            return res;
        }

        /// <summary>
        /// Request for static assets
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [Route("public/{**rest}")]
        public async Task<IActionResult> ProxyPublic(string rest)
        {
            var queryString = this.Request.QueryString.Value;
            var grafanaTargetUrl = $"{_grafanaBaseUrl}/public/{rest}{queryString}";
            var httpResp = await grafanaTargetUrl.GetAsync();
            var responseContent = await httpResp.Content.ReadAsStringAsync();

            if (this._injectCustomCss && Request.Path.Value.Contains(".css"))
            {
                // Injected Style to hide header and side menu
                // TO DO - output this snippet to the separate file
                var injectedCss = @"
.submenu-controls {
    visibility: hidden;
    height: 0;
}
.grafana-app {
    margin-top: -50px !important;
    margin-left: -60px !important;
}
.dashboard-container .react-grid-layout layout {
    width: 102%;
    margin-left: 20px;
}";
                responseContent += System.Environment.NewLine + injectedCss;
            }
            
            var res = new ContentResult
            {
                
                ContentType = httpResp.Content.Headers.GetValues("Content-Type").FirstOrDefault(),
                Content = responseContent
            };
            return res;            
        }
    }    
}
