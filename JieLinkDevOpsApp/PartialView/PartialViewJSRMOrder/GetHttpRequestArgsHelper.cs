using PartialViewInterface.Utils;
using PartialViewJSRMOrder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PartialViewInterface.Utils.HttpHelper;

namespace PartialViewJSRMOrder
{
    public class GetHttpRequestArgsHelper
    {

        public static HttpRequestArgs GetHttpRequestArgs(string userName, string url, string token, string userId)
        {
            QueryOrder queryOrder = new QueryOrder()
            {
                pageIndex = 1,
                pageSize = 40,
                sidx = "",
                sord = "desc",
                currentNode = "RD",
                currentUserCode = userName
            };

            HttpRequestArgs httpRequestArgs = GetHttpRequestArgs(url, queryOrder);
            httpRequestArgs.Heads = new System.Collections.Specialized.NameValueCollection();
            httpRequestArgs.Heads.Add("userId", token);
            httpRequestArgs.Heads.Add("X-Token", userId);
            return httpRequestArgs;
        }


        public static HttpRequestArgs GetHttpRequestArgs(string url, object data)
        {
            var keyValuePairs = JsonHelper.DeserializeObject<Dictionary<string, string>>(JsonHelper.SerializeObject(data));
            string paramUrlCode = "";

            foreach (var key in keyValuePairs.Keys)
            {
                paramUrlCode += $"{key}={System.Web.HttpUtility.UrlEncode(keyValuePairs[key])}&";
            }

            paramUrlCode = paramUrlCode.TrimEnd('&');

            HttpRequestArgs httpRequestArgs = new HttpRequestArgs()
            {
                Timeout = 5000,
                Url = url,
                Content = paramUrlCode,
                ContentType = "application/x-www-form-urlencoded"
            };
            return httpRequestArgs;
        }
    }
}
