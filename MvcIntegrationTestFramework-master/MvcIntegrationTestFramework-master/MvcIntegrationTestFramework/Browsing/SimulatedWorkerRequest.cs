using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Linq;

namespace MvcIntegrationTestFramework.Browsing
{
    /// <summary>
    /// A proxy object used by the ASP.Net MVC infrastructure to send and receive request and response values
    /// </summary>
    internal class SimulatedWorkerRequest : SimpleWorkerRequest
    {
        private readonly HttpCookieCollection cookies;
        private readonly string httpVerbName;
        private readonly byte[] bodyData;
        private readonly NameValueCollection headers;

        public int LastStatusCode;
        public string LastStatusDescription;

        public SimulatedWorkerRequest(string page, string query, TextWriter output, HttpCookieCollection cookies,
            string httpVerbName, byte[] bodyData, NameValueCollection headers)
            : base(page, query, output)
        {
            this.cookies = cookies;
            this.httpVerbName = httpVerbName;
            this.bodyData = bodyData;
            this.headers = headers;
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            LastStatusCode = statusCode;
            LastStatusDescription = statusDescription;
            base.SendStatus(statusCode, statusDescription);
        }

        public override string GetHttpVerbName()
        {
            return httpVerbName;
        }

        public override string GetKnownRequestHeader(int index)
        {
            // Override "Content-Type" header for POST requests, otherwise ASP.NET won't read the Form collection
            if (index == 12)
                if (string.Equals(httpVerbName, "post", StringComparison.OrdinalIgnoreCase))
                    return "application/x-www-form-urlencoded";

            switch (index) {
                case 0x19:
                    return MakeCookieHeader();
                default:
                    if (headers == null)
                        return null;
                    return headers[GetKnownRequestHeaderName(index)];
            }
        }

        public override string GetUnknownRequestHeader(string name)
        {
            return headers == null ? null : headers[name];
        }

        public override string[][] GetUnknownRequestHeaders()
        {
            if (headers == null)
                return null;
            var unknownHeaders = from key in headers.Keys.Cast<string>()
                                 let knownRequestHeaderIndex = GetKnownRequestHeaderIndex(key)
                                 where knownRequestHeaderIndex < 0
                                 select new[] { key, headers[key] };
            return unknownHeaders.ToArray();
        }

        public override byte[] GetPreloadedEntityBody()
        {
            return bodyData;
        }

        private string MakeCookieHeader()
        {
            if(cookies == null || cookies.Count == 0)
                return null;
            var sb = new StringBuilder();
            foreach (string cookieName in cookies)
            {
                var httpCookie = cookies[cookieName];
                if (httpCookie != null) sb.AppendFormat("{0}={1};", cookieName, httpCookie.Value);
            }
            return sb.ToString();
        }
    }
}