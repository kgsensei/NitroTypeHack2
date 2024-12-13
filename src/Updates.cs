namespace NitroType3
{
    class Updates
    {
        public static string VersionCode = "4.6.2";

        public static async Task<Boolean> ShouldUpdate()
        {
            HttpClient client = new();

            HttpRequestMessage req = new()
            {
                RequestUri = BuildEnvironment.UpdateCheckerEndpoint,
                Method = HttpMethod.Get,
                Headers =
                {
                    CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                    {
                        NoCache = true
                    }
                }
            };

            req.Headers.UserAgent.Add(
                new System.Net.Http.Headers.ProductInfoHeaderValue(
                    "nth-version-checker",
                    VersionCode
                )
            );

            HttpResponseMessage res;
            try
            {
                res = await client.SendAsync(req);
                res.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Unable to connect to update server. A new version might be available. Cannot verify version.",
                    "Internal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }

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
