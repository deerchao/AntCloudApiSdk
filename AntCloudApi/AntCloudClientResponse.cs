﻿using Newtonsoft.Json.Linq;

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

        public T GetData<T>()
        {
            return _data.ToObject<T>();
        }


        public static AntCloudClientResponse Populate(JObject o)
        {
            return new AntCloudClientResponse
            {
                _data = o
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