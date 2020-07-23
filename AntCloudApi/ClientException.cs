using System;

namespace AntCloudApi
{
    public class ClientException : Exception
    {
        public ClientException(string resultCode, string resultMsg)
            : base(resultMsg)
        {
            ResultCode = resultCode;
        }

        public string ResultCode { get; }
    }
}
