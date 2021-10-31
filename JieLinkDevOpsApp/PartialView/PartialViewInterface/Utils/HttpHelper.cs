using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class HttpHelper
    {

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static T Get<T>(string url, int timeout = 20000)
        {
            string json = Get(url, timeout);
            if (json == null)
            {
                return default(T);
            }
            try
            {
                return JsonHelper.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //可能返回的contxt是404之类的
                return default(T);
            }
        }
        public static string Get(string url, int timeout = 20000)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.CheckValidationResult);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Timeout = timeout;
            HttpWebResponse httpWebResponse = null;
            StreamReader streamReader = null;
            string result;
            try
            {
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                result = streamReader.ReadToEnd();
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                    streamReader = null;
                }
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                    httpWebResponse = null;
                }
            }
            return result;
        }
        public static async Task<T> GetAsync<T>(string url, int timeout = 20000)
        {
            string json = await GetAsync(url, timeout);
            if (json == null)
            {
                return default(T);
            }
            try
            {
                return JsonHelper.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //可能返回的contxt是404之类的
                return default(T);
            }
        }

        public static async Task<string> GetAsync(string url, int timeout = 20000)
        {
            HttpHelper.HttpRequestArgs obj = new HttpHelper.HttpRequestArgs();
            obj.Url = url;
            obj.Timeout = timeout;
            return await TaskHelper.Start<object, string>(delegate (object o)
            {
                HttpHelper.HttpRequestArgs httpRequestArgs = o as HttpHelper.HttpRequestArgs;
                return HttpHelper.Get(httpRequestArgs.Url, httpRequestArgs.Timeout);
            }, obj);
        }

        public static async Task<T> PostAsync<T>(string url, object content, int timeout = 20000, string contentType = "application/json")
        {
            string json = await PostAsync(url, JsonHelper.SerializeObject(content), timeout, contentType);
            if (json == null)
            {
                return default(T);
            }
            try
            {
                return JsonHelper.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //可能返回的contxt是404之类的
                return default(T);
            }
        }

        public static async Task<string> PostAsync(string url, string content, int timeout = 20000, string contentType = "application/json")
        {
            HttpHelper.HttpRequestArgs obj = new HttpHelper.HttpRequestArgs();
            obj.Url = url;
            obj.Content = content;
            obj.Timeout = timeout;
            obj.ContentType = contentType;
            return await TaskHelper.Start<object, string>(delegate (object o)
            {
                HttpHelper.HttpRequestArgs httpRequestArgs = o as HttpHelper.HttpRequestArgs;
                return HttpHelper.Post(httpRequestArgs.Url, httpRequestArgs.Content, httpRequestArgs.Timeout, httpRequestArgs.ContentType);
            }, obj);
        }

        public static T Post<T>(string url, object content, int timeout = 20000, string contentType = "application/json")
        {
            string json = Post(url, JsonHelper.SerializeObject(content), timeout, contentType);
            if (json == null)
            {
                return default(T);
            }
            try
            {
                return JsonHelper.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //可能返回的contxt是404之类的
                return default(T);
            }
        }

        public static T Post<T>(HttpRequestArgs httpRequestArgs)
        {
            string json = Post(httpRequestArgs);
            if (json == null)
            {
                return default(T);
            }
            try
            {
                return JsonHelper.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //可能返回的contxt是404之类的
                return default(T);
            }
        }

        public static async Task<T> PostAsync<T>(HttpRequestArgs httpRequestArgs)
        {
            string json = await TaskHelper.Start<object, string>((object o) =>
            {
                return Post(httpRequestArgs);

            }, httpRequestArgs);

            if (json == null)
            {
                return default(T);
            }
            try
            {
                return JsonHelper.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //可能返回的contxt是404之类的
                return default(T);
            }
        }

        public static string Post(string url, string content, int timeout = 20000, string contentType = "application/json")
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.CheckValidationResult);
            ServicePointManager.DefaultConnectionLimit = 300;
            CookieContainer cookieContainer = new CookieContainer();
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            Stream stream = null;
            StreamReader streamReader = null;
            string result;
            try
            {
                httpWebRequest = (WebRequest.Create(url) as HttpWebRequest);
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ServicePoint.ConnectionLimit = 300;
                httpWebRequest.AllowAutoRedirect = true;
                httpWebRequest.Timeout = timeout;
                httpWebRequest.ReadWriteTimeout = timeout;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Accept = "application/json";
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                httpWebRequest.Proxy = null;
                httpWebRequest.CookieContainer = cookieContainer;
                Stream requestStream;
                stream = (requestStream = httpWebRequest.GetRequestStream());
                try
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                finally
                {
                    if (requestStream != null)
                    {
                        ((IDisposable)requestStream).Dispose();
                    }
                }
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    if (httpWebResponse != null)
                    {
                        httpWebResponse.Close();
                        httpWebResponse = null;
                    }
                    if (httpWebRequest != null)
                    {
                        httpWebRequest.Abort();
                    }
                    result = null;
                }
                else
                {
                    streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                    result = streamReader.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                    streamReader = null;
                }
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                    httpWebResponse = null;
                }
                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                    httpWebRequest = null;
                }
            }
            return result;
        }

        public static string Post(HttpRequestArgs httpRequestArgs)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.CheckValidationResult);
            ServicePointManager.DefaultConnectionLimit = 300;
            CookieContainer cookieContainer = new CookieContainer();
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            Stream stream = null;
            StreamReader streamReader = null;
            string result;
            try
            {
                httpWebRequest = (WebRequest.Create(httpRequestArgs.Url) as HttpWebRequest);
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ServicePoint.ConnectionLimit = 300;
                httpWebRequest.AllowAutoRedirect = true;
                httpWebRequest.Timeout = httpRequestArgs.Timeout;
                httpWebRequest.ReadWriteTimeout = httpRequestArgs.Timeout;
                httpWebRequest.ContentType = httpRequestArgs.ContentType;
                httpWebRequest.Accept = "application/json";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
                byte[] bytes = Encoding.UTF8.GetBytes(httpRequestArgs.Content);
                httpWebRequest.Proxy = null;
                httpWebRequest.CookieContainer = cookieContainer;
                if (httpRequestArgs.Heads != null)
                {
                    httpWebRequest.Headers.Add(httpRequestArgs.Heads);
                }
                Stream requestStream;
                stream = (requestStream = httpWebRequest.GetRequestStream());
                try
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                finally
                {
                    if (requestStream != null)
                    {
                        ((IDisposable)requestStream).Dispose();
                    }
                }
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    if (httpWebResponse != null)
                    {
                        httpWebResponse.Close();
                        httpWebResponse = null;
                    }
                    if (httpWebRequest != null)
                    {
                        httpWebRequest.Abort();
                    }
                    result = null;
                }
                else
                {
                    streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                    result = streamReader.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                    streamReader = null;
                }
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                    httpWebResponse = null;
                }
                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                    httpWebRequest = null;
                }
            }
            return result;
        }

        public class HttpRequestArgs
        {
            public string Url;

            public string Content = "";

            public int Timeout = 20000;

            public string ContentType = "application/json";

            public NameValueCollection Heads { get; set; }
        }
    }
}