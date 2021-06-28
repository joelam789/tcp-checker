using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TcpChecker
{
    public static class JsonHelper
    {
        static ExpandoObjectConverter m_MapConverter = new ExpandoObjectConverter();

        public static string ToJsonString(object obj)
        {
            string str = "";
            try
            {
                if (obj != null) str = JsonConvert.SerializeObject(obj);
            }
            catch { }
            return str;
        }

        public static object ToJsonObject(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return JsonConvert.DeserializeObject(str);
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public static IDictionary<string, object> ToDictionary(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return JsonConvert.DeserializeObject<ExpandoObject>(str, m_MapConverter);
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public static T ToJsonObject<T>(string str) where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    //if (typeof(T) == typeof(ExpandoObject)) return JsonConvert.DeserializeObject<ExpandoObject>(str, m_MapConverter) as T;
                    //else return JsonConvert.DeserializeObject<T>(str);
                    return JsonConvert.DeserializeObject<T>(str);
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
