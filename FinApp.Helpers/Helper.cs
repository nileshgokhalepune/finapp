using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public static T DeserializeJson<T>(JObject jsonData)
        {
            var serializer = JsonSerializer.Create();
            return serializer.Deserialize<T>(new JsonTextReader(new StringReader(jsonData.ToString())));
        }

        public static T DeserializeJson<T>(JArray jsonArray)
        {
            var serializer = JsonSerializer.Create();
            return serializer.Deserialize<T>(new JsonTextReader(new StringReader(jsonArray.ToString())));
        }

    }
}
