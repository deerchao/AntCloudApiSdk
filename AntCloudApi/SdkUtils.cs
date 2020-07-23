using System;
using System.Collections.Generic;
using System.Text;

namespace AntCloudApi
{
    public static class SdkUtils
    {
        public static string GenerateReqMsgId()
        {
            return Guid.NewGuid().ToString("n");
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
    }
}
