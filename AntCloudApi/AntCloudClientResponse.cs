using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AntCloudApi
{
    public class AntCloudClientResponse
    {
        private JObject _data;

        private AntCloudClientResponse()
        {
        }


        public bool IsSuccess
        {
            get => SDKConstants.ResultCodes.OK.Equals(ResultCode);
        }

        public string ReqMsgId
        {
            get => _data.Value<string>(SDKConstants.ParamKeys.REQ_MSG_ID);
            set => _data[SDKConstants.ParamKeys.REQ_MSG_ID] = value;
        }

        public string ResultCode
        {
            get => _data.Value<string>(SDKConstants.ParamKeys.RESULT_CODE);
            private set => _data[SDKConstants.ParamKeys.RESULT_CODE] = value;
        }

        public string ResultMsg
        {
            get => _data.Value<string>(SDKConstants.ParamKeys.RESULT_MSG);
            private set => _data[SDKConstants.ParamKeys.RESULT_MSG] = value;
        }

        public string DataAsString
        {
            get => _data.ToString();
            set => _data = JObject.Parse(value);
        }

        public JObject DataAsJson
        {
            get => _data;
            set => _data = value;
        }


        public T GetData<T>(object key = null)
        {
            var o = key == null
                ? _data
                : _data[key];

            return ToObject<T>(o);
        }


        public static T ToObject<T>(JToken o)
        {
            return o.ToObject<T>(new JsonSerializer
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
        }

        public static AntCloudClientResponse Create(JObject responseNode)
        {
            return new AntCloudClientResponse
            {
                _data = responseNode
            };
        }

        public static AntCloudClientResponse Create(string responseNodeJson)
        {
            return new AntCloudClientResponse
            {
                DataAsString = responseNodeJson,
            };
        }

        public static AntCloudClientResponse Success(AntCloudClientRequest request, object o = null)
        {
            if (o == null)
                o = new object();

            return new AntCloudClientResponse
            {
                _data = JObject.FromObject(o),
                ReqMsgId = request.ReqMsgId,
                ResultCode = SDKConstants.ResultCodes.OK,
            };
        }

        public static AntCloudClientResponse Error(AntCloudClientRequest request, string resultCode, string resultMsg)
        {
            return new AntCloudClientResponse
            {
                _data = new JObject(),
                ReqMsgId = request.ReqMsgId,
                ResultCode = resultCode,
                ResultMsg = resultMsg,
            };
        }
    }
}
