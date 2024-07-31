namespace NitroType3
{
    class AdBlocker
    {
        private static string[] BlockedDomains = {
            "btloader.com",
            "bugsnag.com",
            "cloudfront.net",
            "cloudflareinsights.com",
            "doubleclick.net",
            "facebook.net",
            "facebook.com",
            "googletagmanager.com",
            "intergient.com",
            "proper.io",
            "pub.network",
            "qualaroo.com",
            "vuukle.com",
        };

        public static bool IsBlocked(string Uri)
        {
            for (int i = 0; i < BlockedDomains.Length; i++)
            {
                if (Uri.Contains(BlockedDomains[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
