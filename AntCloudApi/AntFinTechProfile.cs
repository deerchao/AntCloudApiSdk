using System;
using System.Net.Http;

namespace AntCloudApi
{
    public class AntFinTechProfile
    {
        private string _baseUrl;


        public string EndPoint { get; set; }

        public HttpConfig HttpConfig { get; set; }

        public HttpClient HttpClient { get; set; }

        public string SecurityToken { get; set; }

        public bool EnableAutoRetry { get; set; }

        public int AutoRetryLimit { get; set; }

        public string AccessKey { get; set; }

        public string AccessSecret { get; set; }

        public TimeSpan Timeout { get; set; }

        public bool CheckSign { get; set; }

        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                if (value != null && !value.EndsWith("/"))
                    _baseUrl = value + '/';
                else
                    _baseUrl = value;
            }
        }


        public static AntFinTechProfile GetProfile(string accessKey, string accessSecret)
        {
            return new AntFinTechProfile
            {
                AccessKey = accessKey,
                AccessSecret = accessSecret,
                BaseUrl = DefaultValues.DEFAULT_BASE_URL,
                Timeout = DefaultValues.DEFAULT_TIMEOUT
            };
        }

        public static AntFinTechProfile getProfile(string baseUrl, string accessKey, string accessSecret)
        {
            return new AntFinTechProfile
            {
                AccessKey = accessKey,
                AccessSecret = accessSecret,
                BaseUrl = baseUrl,
                Timeout = DefaultValues.DEFAULT_TIMEOUT
            };
        }
    }
}
