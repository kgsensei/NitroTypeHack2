using System.Text;

namespace NitroType3
{
    class Connections
    {
        public static void UsageReport()
        {
            HttpClient client = new();

            HttpRequestMessage req = new()
            {
                RequestUri = new Uri("https://analytics.kgsensei.dev/api/software"),
                Method = HttpMethod.Post,
                Content = new StringContent("{\"project\":\"NitroTypeHack2\"}", Encoding.UTF8, "application/json"),
            };

            req.Headers.Add("origin", "https://kgsensei.dev");

            //client.Send(req);
        }

        public static void ErrorReport(string? errorMessage = "Unknown", string? stackTrace = "Unknown")
        {
            HttpClient client = new();

            string safeStackTrace = stackTrace.Replace("\r", "").Replace("\n", "").Replace("\"", "\'").Replace("\\", "\\\\");

            HttpRequestMessage req = new()
            {
                RequestUri = new Uri("https://analytics.kgsensei.dev/error_reporting"),
                Method = HttpMethod.Post,
                Content = new StringContent("{\"a\":\"" + errorMessage + "\",\"b\":\"None\",\"c\":0,\"d\":0,\"e\":\"" + safeStackTrace + "\",\"f\":\"NitroType Cheat\"}", Encoding.UTF8, "application/json"),
            };

            client.Send(req);
        }
    }
}
