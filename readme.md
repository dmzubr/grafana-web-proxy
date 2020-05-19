# Purpose
This project is aimed to get an out-of-box solution to embed Grafana dashboards to the third party web application. When say "Grafana" we mean not Enterprise edition.

# Problem
If you want to embed Granafa dashboards to your web application - then typical approach will be to use iframe.
Then we'll face some problems:
- Grafana does not support CORS (Cross-Origin Resource Sharing)
- On some requests we need to provide additional Http headers to fit authorization rules.

Additional info can be found here:
- https://community.grafana.com/t/how-to-allow-access-control-allow-origin-for-all-the-apis/2045/9
- https://community.grafana.com/t/grafana-plugin-cors-issue-with-rest-api/4291/7

# Solution
This project catches all requests that was initially targeted to Grafana. Then make an appropriate calls to Grafana service itself (over Http). And returns results back to the requesting web client. No headache with CORS and interceptors to append custom Authorization headers.

# Installation
1) [Create an API key for Grafana organization](https://grafana.com/docs/grafana/latest/http_api/auth/#create-api-token)

2) Run docker container
```sh
$ docker run -itd --name grafana-proxy --rm -p 80:80 \
  --env GRAFANA_PROXY_USE_HTTPS=false \
  --env GRAFANA_BASE_URL=http://your_grafana_host:your_grafana_port \
  --env GRAFANA_API_KEY=configured_api_key_here \
  --env GRAFANA_INJECT_CUSTOM_CSS=true \
cashee/grafana-proxy:latest
```
3) Check service availability in browser.
    - Get some Grafana dashoboard URL (i.e. "http://your_grafana_host:32000/d/kZdoIYxik/k8-deployment?orgId=1")
    - Change Grafana host to host of Grafana Proxy service (i.e. "http://localhost:80/d/kZdoIYxik/k8-deployment?orgId=1")
    - Start this URL in web browser. After loading you should see content of Grafana dashboard without header and sidebar

# Service options
Grafana proxy can be defined by appsettings.json file in service's container or with Environment variables.
Example of appsettings file is in file *WebApi/appsettings_example.json*
Environment variables overrides values from appsettings.json file.
At the moment available options are:
- GRAFANA_PROXY_USE_HTTPS - defines whether or not support Https by proxy service;
- GRAFANA_BASE_URL - URL for root of target (proxied) Grafana service;
- GRAFANA_API_KEY - API Key to perform authorized requests to target Grafana service
- GRAFANA_INJECT_CUSTOM_CSS - Flag to use injection of CSS to hide Grafana's view header, paramaters block and sidebar. Suppose that in the environment of your web app you should restrict user to perform any actions with Grafana except of watching dashboards :)

# Using on client's side
