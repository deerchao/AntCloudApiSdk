using System;
using System.Net;
using System.Net.Http;

namespace AntCloudApi
{
    public class HttpConfig
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

        public int MaxConnectionPerHost { get; set; } = 100;

        public string ProxyHost { get; set; }

        public int ProxyPort { get; set; }

        public string ProxyUsername { get; set; }

        public string ProxyPassword { get; set; }


        public HttpClient BuildHttpClient()
        {
            var handler = new HttpClientHandler
            {
                MaxConnectionsPerServer = MaxConnectionPerHost
            };

            if (!string.IsNullOrWhiteSpace(ProxyHost))
            {
                var proxy = new WebProxy
                {
                    Address = new Uri($"http://{ProxyHost}:{ProxyPort}"),
                    BypassProxyOnLocal = true,
                    UseDefaultCredentials = false,
                };

                if (!string.IsNullOrWhiteSpace(ProxyUsername))
                {
                    proxy.Credentials = new NetworkCredential
                    {
                        UserName = ProxyUsername,
                        Password = ProxyPassword,
                    };
                }

                handler.Proxy = proxy;
            }

            return new HttpClient(handler)
            {
                Timeout = Timeout,
            };
        }
    }
}
