using System.IO;
using System.Net;

namespace FinApp.Helpers
{
    public static class Helper
    {
        public static string GetResponseText(Stream stream)
        {
            return new StreamReader(stream).ReadToEnd();
        }

        public static string GetResponseText(WebResponse response)
        {
            return GetResponseText(response.GetResponseStream());
        }
    }
}
