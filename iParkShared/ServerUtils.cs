using Stripe;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iParkShared
{
    public class ServerUtils
    {
        public static string GetExMessage(Exception ex)
        {
            string str = String.Empty;

            while (ex != null)
            {
                if (str != String.Empty)
                    str += "\r\n";

                str += ex.Message;
                ex = ex.InnerException;
            }

            return str;
        }

        public static HttpResponseException BuildException(Exception ex)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(GetExMessage(ex)),
                ReasonPhrase = "Server Error"
            };

            return new HttpResponseException(resp);
        }
    }
}