using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AntCloudApi
{
    public class AntFinTechApiClient
    {
        private static readonly HttpClient _uploadHttpClient = new HttpClient();
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _accessSecret;
        private readonly bool _checkSign;
        private readonly bool _enableAutoRetry;
        private readonly int _autoRetryLimit;
        private readonly HttpClient _httpClient;
        private readonly string _securityToken;


        public AntFinTechApiClient(AntFinTechProfile profile)
        {
            if (profile.HttpClient == null)
            {
                if (profile.HttpConfig == null)
                    profile.HttpConfig = new HttpConfig();
                profile.HttpClient = profile.HttpConfig.BuildHttpClient();
            }
            _httpClient = profile.HttpClient;

            _endpoint = profile.EndPoint;
            _accessKey = profile.AccessKey;
            _accessSecret = profile.AccessSecret;
            _checkSign = profile.CheckSign;
            _enableAutoRetry = profile.EnableAutoRetry;
            _autoRetryLimit = profile.AutoRetryLimit;
            _securityToken = profile.SecurityToken;
        }


        public string EndPoint => _endpoint;

        public string AccessKey => _accessKey;

        public string AccessSecret => _accessSecret;

        public bool CheckSign => _checkSign;


        public event EventHandler<IReadOnlyDictionary<string, string>> RequestSending;

        public event EventHandler<string> ResponseRead;

                                                        
        public Task<AntCloudClientResponse> Execute(AntCloudClientRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Method == null)
                throw new ArgumentException("method can't be null");

            if (request.Version == null)
                throw new ArgumentException("version can't be null");

            PrepareParameters(request);

            return SendRequest(request);
        }

        /// <summary>
        /// 文件直接上传。不触发事件
        /// </summary>
        /// <param name="uploadUrl">调用相关接口获得的上传网址</param>
        /// <param name="contentType">MIME 类型</param>
        /// <param name="fileData">文件数据</param>
        /// <returns></returns>
        public static async Task UploadFile(string uploadUrl, string contentType, byte[] fileData)
        {
            var md5hash = Convert.ToBase64String(MD5.Create().ComputeHash(fileData));

            var content = new ByteArrayContent(fileData);
            content.Headers.Add("Content-Type", contentType);
            content.Headers.ContentLength = fileData.Length;
            content.Headers.Add("Content-MD5", md5hash);

            var message = new HttpRequestMessage(HttpMethod.Put, uploadUrl)
            {
                Content = content,
            };
            var response = await _uploadHttpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }


        private HttpRequestMessage BuildRequest(string endpoint, IReadOnlyDictionary<string, string> request)
        {
            return new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(request),
            };
        }

        private async Task<AntCloudClientResponse> SendRequest(AntCloudClientRequest request)
        {
            var retried = 0;

            var parameters = request.GetParameters();
            var message = BuildRequest(_endpoint, parameters);

            while (true)
            {
                RequestSending?.Invoke(this, parameters);

                var response = await _httpClient.SendAsync(message);
                var responseString = await response.Content.ReadAsStringAsync();

                ResponseRead?.Invoke(this, responseString);

                JObject wholeJson;
                try
                {
                    wholeJson = JObject.Parse(responseString);
                }
                catch (JsonReaderException e)
                {
                    if (_enableAutoRetry && retried < _autoRetryLimit)
                    {
                        retried += 1;
                        continue;
                    }
                    throw new ClientException(SDKConstants.ResultCodes.RESPONSE_FORMAT_ERROR, e.Message);
                }

                if (wholeJson == null)
                    throw new ClientException(SDKConstants.ResultCodes.TRANSPORT_ERROR, "Unexpected gateway response: " + responseString);

                var responseNode = wholeJson["response"] as JObject;
                if (responseNode == null)
                    throw new ClientException(SDKConstants.ResultCodes.TRANSPORT_ERROR, "Unexpected gateway response: " + responseString);

                var result = AntCloudClientResponse.Create(responseNode);
                if (result.IsSuccess && _checkSign)
                {
                    var sign = wholeJson.Value<string>(SDKConstants.ParamKeys.SIGN);
                    var target = ExtractStringToSign(responseString);
                    var calculatedSign = ComputeSign(target, request.GetParameter(SDKConstants.ParamKeys.SIGN_TYPE), _accessSecret, SDKConstants.SIGN_CHARSET);

                    if (!calculatedSign.Equals(sign))
                        throw new ClientException(SDKConstants.ResultCodes.BAD_SIGNATURE, "Invalid signature in response");
                }

                return result;
            }
        }

        private string ExtractStringToSign(string response)
        {
            var responseNodeKey = "\"response\"";
            var signNodeKey = "\"sign\"";

            var indexOfResponseNode = response.IndexOf(responseNodeKey);
            var indexOfSignNode = response.LastIndexOf(signNodeKey);

            if (indexOfResponseNode < 0)
                return null;

            if (indexOfSignNode < 0 || indexOfSignNode < indexOfResponseNode)
            {
                indexOfSignNode = response.LastIndexOf('}') - 1;
            }

            var startIndex = response.IndexOf('{',
                indexOfResponseNode + responseNodeKey.Length);
            var endIndex = response.LastIndexOf("}", indexOfSignNode);

            return response.Substring(startIndex, endIndex + 1);
        }

        private void PrepareParameters(AntCloudClientRequest request)
        {
            request.PutParameter(SDKConstants.ParamKeys.ACCESS_KEY, _accessKey);

            request.PutParameterIfAbsent(SDKConstants.ParamKeys.SIGN_TYPE, SDKConstants.DEFAULT_SIGN_TYPE);
            request.PutParameterIfAbsent(SDKConstants.ParamKeys.REQ_MSG_ID, SdkUtils.GenerateReqMsgId());
            request.PutParameterIfAbsent(SDKConstants.ParamKeys.REQ_TIME, SdkUtils.FormatDate(DateTime.UtcNow));

            var signType = request.GetParameter(SDKConstants.ParamKeys.SIGN_TYPE);
            if (!signType.Equals(SDKConstants.DEFAULT_SIGN_TYPE, StringComparison.OrdinalIgnoreCase) &&
                !signType.Equals(SDKConstants.SIGN_TYPE_SHA256))
                throw new ArgumentException("wrong sign type");

            if (!string.IsNullOrWhiteSpace(_securityToken))
                request.PutParameterIfAbsent(SDKConstants.ParamKeys.SECURITY_TOKEN, _securityToken);

            request.PutParameterIfAbsent(SDKConstants.ParamKeys.BASE_SDK_VERSION, SDKConstants.BASE_SDK_VERSION_VALUE);

            var sign = ComputeSign(request.GetParameters(), signType, _accessSecret, SDKConstants.SIGN_CHARSET);
            request.PutParameter(SDKConstants.ParamKeys.SIGN, sign);
        }

        private string ComputeSign(IEnumerable<KeyValuePair<string, string>> parameters, string signType, string accessSecret, Encoding encoding)
        {
            return ComputeSign(GetSignTarget(parameters), signType, accessSecret, encoding);
        }

        private string ComputeSign(string target, string signType, string accessSecret, Encoding encoding)
        {
            var mac = HMAC.Create(signType);
            mac.Key = encoding.GetBytes(accessSecret);

            var signData = mac.ComputeHash(encoding.GetBytes(target));

            return Convert.ToBase64String(signData);
        }

        private string GetSignTarget(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var sb = new StringBuilder();

            var items = parameters.Where(x => x.Value == null ||
               !x.Value.StartsWith(SDKConstants.BASE64URL))
                .OrderBy(x => x.Key);

            var needsJoin = false;
            foreach (var item in items)
            {
                if (needsJoin)
                    sb.Append('&');
                sb.Append(EscapeParamter(item.Key));
                sb.Append('=');
                sb.Append(EscapeParamter(item.Value));
                needsJoin = true;
            }
            return sb.ToString();
        }

        private string EscapeParamter(string p)
        {
            return Uri.EscapeDataString(p);
        }
    }
}
