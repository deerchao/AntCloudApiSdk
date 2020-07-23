using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace AntCloudApi
{
    public class AntCloudClientRequest
    {
        private readonly Dictionary<string, string> _parameters;

        public AntCloudClientRequest()
        {
            _parameters = new Dictionary<string, string>();
        }

        public void PutParameter(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _parameters[key] = value;
        }

        public void PutParameterIfAbsent(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!_parameters.ContainsKey(key))
                _parameters.Add(key, value);
        }

        public void RemoveParameter(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _parameters.Remove(key);
        }

        public string GetParameter(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _parameters.TryGetValue(key, out var value)
                            ? value
                            : null;
        }

        public void PutParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            foreach (var p in parameters)
                if (p.Key == null)
                    throw new ArgumentException("Null key is not allowed");

            foreach (var p in parameters)
                _parameters[p.Key] = p.Value;
        }

        public void PutParametersFromObject<T>(T o)
        {
            var j = JToken.FromObject(o);
            PutJsonToken("", j);
        }

        private void PutJsonToken(string path, JToken j)
        {
            if (j is JObject map)
            {
                foreach (var property in map.Properties())
                {
                    PutJsonToken(path + "." + property.Name, property.Value);
                }
            }
            else if (j is JArray array)
            {
                for (var i = 0; i < array.Count; i++)
                {
                    PutJsonToken(path + "." + (i + 1), array[i]);
                }
            }
            else if (j != null)
            {
                PutParameter(path.Substring(1), j.ToString());
            }
        }

        public IReadOnlyDictionary<string, string> GetParameters()
        {
            return new ReadOnlyDictionary<string, string>(_parameters);
        }

        public string Method
        {
            get => GetParameter(SDKConstants.ParamKeys.METHOD);
            set => PutParameter(SDKConstants.ParamKeys.METHOD, value);
        }

        public string Version
        {
            get => GetParameter(SDKConstants.ParamKeys.VERSION);
            set => PutParameter(SDKConstants.ParamKeys.VERSION, value);
        }

        public string ReqMsgId
        {
            get => GetParameter(SDKConstants.ParamKeys.REQ_MSG_ID);
            set => PutParameter(SDKConstants.ParamKeys.REQ_MSG_ID, value);
        }
    }
}
