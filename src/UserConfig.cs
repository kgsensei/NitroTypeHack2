namespace NitroType3
{
    class UserConfig
    {
        public static dynamic Get(string key)
        {
            return Properties.Settings.Default[key];
        }

        public static void Set(string key, dynamic value)
        {
            Properties.Settings.Default[key] = value;
        }

        public static void Save()
        {
            Logger.Log("Saving User Configuration");

            Properties.Settings.Default["UsrCnf_AutoStart"] = Config.AutoStart;
            Properties.Settings.Default["UsrCnf_AutoGame"] = Config.AutoGame;
            Properties.Settings.Default["UsrCnf_UseNitros"] = Config.UseNitros;

            Properties.Settings.Default["UsrCnf_Accuracy"] = Config.Accuracy;
            Properties.Settings.Default["UsrCnf_AccuracyV"] = Config.AccuracyVariancy;

            Properties.Settings.Default["UsrCnf_TypingRate_Real"] = Config.TypingRate;
            Properties.Settings.Default["UsrCnf_TypingRateV"] = Config.TypingRateVariancy;

            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                Reset();
            }
        }

        public static void Reset()
        {
            try
            {
                Properties.Settings.Default.Reset();
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
                MessageBox.Show(
                    "Could not load configuration files. Already being used by another process, are you running multiple versions of this program?",
                    "Critical Error",
                    MessageBoxButtons.Ok,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
