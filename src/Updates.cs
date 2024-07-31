namespace NitroType3
{
    class Updates
    {
        public static string VersionCode = "4.6";

        public static async Task<Boolean> ShouldUpdate()
        {
            HttpClient client = new();

            HttpRequestMessage req = new()
            {
                RequestUri = BuildEnvironment.UpdateCheckerEndpoint,
                Method = HttpMethod.Get,
            };

            req.Headers.UserAgent.Add(
                new System.Net.Http.Headers.ProductInfoHeaderValue(
                    "nth-version-checker",
                    VersionCode
                )
            );

            var res = await client.SendAsync(req);

            res.EnsureSuccessStatusCode();

            string LiveVersionCode = await res.Content.ReadAsStringAsync();
            LiveVersionCode = LiveVersionCode
                .ToLower()
                .Replace("\n", "");

            Logger.Log("Live Version Code:" + LiveVersionCode);
            Logger.Log("Current Version Code:" + VersionCode);

            return LiveVersionCode != VersionCode;
        }
    }
}
