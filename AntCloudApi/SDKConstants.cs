using System.Text;

namespace AntCloudApi
{
    public static class SDKConstants
    {
        public const string BASE_SDK_VERSION_VALUE = "3.4.0";
        public const string DEFAULT_CHARSET = "UTF-8";
        public static readonly Encoding SIGN_CHARSET = Encoding.UTF8;
        public const string DEFAULT_SIGN_TYPE = "HmacSHA1";
        public const string SIGN_TYPE_SHA256 = "HmacSHA256";
        public const string BASE64URL = "antcloud-base64://";

        public static class ParamKeys
        {
            public const string RESPONSE = "response";
            public const string REQ_MSG_ID = "req_msg_id";
            public const string RESULT_CODE = "result_code";
            public const string RESULT_MSG = "result_msg";
            public const string RESULT_MSG_PLACEHOLDER = "result_msg_placeholder";
            public const string RESULT_MSG_ARGS = "result_msg_args";
            public const string SIGN_TYPE = "sign_type";
            public const string SIGN = "sign";
            public const string REQ_TIME = "req_time";
            public const string BASE_SDK_VERSION = "base_sdk_version";
            public const string METHOD = "method";
            public const string VERSION = "version";
            public const string ACCESS_KEY = "access_key";
            public const string SECURITY_TOKEN = "security_token";
            public const string PRODUCT_INSTANCE_ID = "product_instance_id";
            public const string REGION_NAME = "region_name";
            public const string INVOKER_USER = "invoker.user";
            public const string INTERNAL_API = "internal_api";
        }

        public static class ResultCodes
        {
            public const string OK = "OK";
            public const string MISSING_PARAMETER = "MISSING_PARAMETER";
            public const string INVALID_PARAMETER = "INVALID_PARAMETER";
            public const string TRANSPORT_ERROR = "TRANSPORT_ERROR";
            public const string PARASE_URL_ERROR = "PARASE_URL_ERROR";
            public const string RESPONSE_FORMAT_ERROR = "RESPONSE_FORMAT_ERROR";
            public const string BAD_SIGNATURE = "INVALID_RESPONSE_SIGNATURE";
            public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";
            public const string ACCESS_DENIED = "ACCESS_DENIED";
            public const string METHOD_NOT_FOUND = "METHOD_NOT_FOUND";
        }

        public static class ResultMsgPlaceholders
        {
            // The system providing the API behaves unexpectedly
            public const string PROVIDER_UNKNOWN_ERROR = "PROVIDER_UNKNOWN_ERROR";
        }
    }
}
