using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace TcpChecker
{
    public static class RemoteCaller
    {
        static RemoteCaller()
        {
            DefaultTimeout = 20 * 1000; // 20 seconds
            HttpConnectionLimit = Environment.ProcessorCount * 8;
        }

        public static int DefaultTimeout { get; set; }

        public static int HttpConnectionLimit
        {
            get { return ServicePointManager.DefaultConnectionLimit; }
            set { ServicePointManager.DefaultConnectionLimit = value; }
        }

        public static async Task<object> Request(string url, object param, int timeout = 0)
        {
            return await Request(url, param, null, timeout);
        }

        public static async Task<T> Request<T>(string url, object param, int timeout = 0) where T : class
        {
            return await Request<T>(url, param, null, timeout);
        }

        public static async Task<object> Request(string url, object param, IDictionary<string, string> headers, int timeout = 0)
        {
            return JsonHelper.ToJsonObject(await Request<string>(url, param, headers, timeout));
        }

        public static async Task<T> Request<T>(string url, object param, IDictionary<string, string> headers, int timeout = 0) where T : class
        {
            T result = null;
            string input = null;

            if (param != null)
                input = param is string ? param.ToString() : JsonHelper.ToJsonString(param);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Accept = "*/*";
            httpWebRequest.UserAgent = "curl/7.50.0";
            httpWebRequest.ContentType = "text/plain";
            httpWebRequest.Method = param == null ? "GET" : "POST";

            if (headers != null)
            {
                var reqHeaders = new Dictionary<string, string>(headers);
                if (reqHeaders.ContainsKey("Content-Type"))
                {
                    httpWebRequest.ContentType = reqHeaders["Content-Type"];
                    reqHeaders.Remove("Content-Type");
                }
                foreach (var item in reqHeaders) httpWebRequest.Headers.Add(item.Key, item.Value);
            }

            httpWebRequest.Timeout = timeout > 0 ? timeout : DefaultTimeout;

            if (input != null)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(input);
                    await streamWriter.FlushAsync();
                    streamWriter.Close();
                }
            }

            using (var response = await TryToGetResponse(httpWebRequest))
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string responseString = await streamReader.ReadToEndAsync();
                    var expectedType = typeof(T);
                    if (expectedType == typeof(string)) result = responseString as T;
                    else if (expectedType == typeof(IDictionary<string, object>)) result = JsonHelper.ToDictionary(responseString) as T;
                    else if (expectedType == typeof(Tuple<int, string>))
                    {
                        var ret = new Tuple<int, string>((int)(((HttpWebResponse)response).StatusCode), responseString);
                        result = ret as T;
                    }
                    else result = JsonHelper.ToJsonObject<T>(responseString);
                    streamReader.Close();
                }
            }

            return result;
        }

        public static async Task<object> CustomRequest(string url, object param, Action<HttpWebRequest> updateRequestParamFunc = null)
        {
            return JsonHelper.ToJsonObject(await CustomRequest<string>(url, param, updateRequestParamFunc));
        }

        public static async Task<T> CustomRequest<T>(string url, object param, Action<HttpWebRequest> updateRequestParamFunc = null) where T : class
        {
            T result = null;
            string input = null;

            if (param != null)
                input = param is string ? param.ToString() : JsonHelper.ToJsonString(param);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Accept = "*/*";
            httpWebRequest.UserAgent = "curl/7.50.0";
            httpWebRequest.ContentType = "text/plain";
            httpWebRequest.Method = param == null ? "GET" : "POST";
            httpWebRequest.Timeout = DefaultTimeout;

            updateRequestParamFunc?.Invoke(httpWebRequest);

            if (input != null)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(input);
                    await streamWriter.FlushAsync();
                    streamWriter.Close();
                }
            }

            using (var response = await TryToGetResponse(httpWebRequest))
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string responseString = await streamReader.ReadToEndAsync();
                    var expectedType = typeof(T);
                    if (expectedType == typeof(string)) result = responseString as T;
                    else if (expectedType == typeof(IDictionary<string, object>)) result = JsonHelper.ToDictionary(responseString) as T;
                    else if (expectedType == typeof(Tuple<int, string>))
                    {
                        var ret = new Tuple<int, string>((int)(((HttpWebResponse)response).StatusCode), responseString);
                        result = ret as T;
                    }
                    else result = JsonHelper.ToJsonObject<T>(responseString);
                    streamReader.Close();
                }
            }

            return result;
        }

        public static async Task<WebResponse> TryToGetResponse(HttpWebRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("HttpWebRequest");
            }

            WebResponse response = null;

            try
            {
                response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                response = ex.Response;
                if (response == null) throw;
            }

            return response;
        }
    }
}
