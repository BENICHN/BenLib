using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BenLib.Standard
{
    public static partial class Extensions
    {
        public static HttpWebResponse GetHttpResponse(this HttpWebRequest request) => (HttpWebResponse)request.GetResponse();

        public static HttpWebResponse GetHttpResponseNoException(this HttpWebRequest request)
        {
            try { return (HttpWebResponse)request.GetResponse(); }
            catch (WebException ex) { return ex.Response as HttpWebResponse ?? throw ex; }
        }

        public static WebResponse GetResponseNoException(this HttpWebRequest request)
        {
            try { return request.GetResponse(); }
            catch (WebException ex) { return ex.Response ?? throw ex; }
        }

        public static async Task<HttpWebResponse> GetHttpResponseAsync(this HttpWebRequest request) => (HttpWebResponse)await request.GetResponseAsync();

        public static async Task<HttpWebResponse> GetHttpResponseNoExceptionAsync(this HttpWebRequest request)
        {
            try { return (HttpWebResponse)await request.GetResponseAsync(); }
            catch (WebException ex) { return ex.Response as HttpWebResponse ?? throw ex; }
        }

        public static async Task<WebResponse> GetResponseNoExceptionAsync(this HttpWebRequest request)
        {
            try { return await request.GetResponseAsync(); }
            catch (WebException ex) { return ex.Response ?? throw ex; }
        }

        public static void BasicAuthentication(this HttpWebRequest request, string username, string password)
        {
            string login = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            if (request.Headers[HttpRequestHeader.Authorization] != null) request.Headers[HttpRequestHeader.Authorization] = "Basic " + login;
            else request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + login);
        }
    }
}
