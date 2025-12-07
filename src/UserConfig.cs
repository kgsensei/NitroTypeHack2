namespace NitroType3 {
    class UserConfig {
        // Gets a configuration option
        public static dynamic Get(string key) {
            try {
                return Properties.Settings.Default[key];
            } catch (Exception) {
                Reset();
                return 0;
            }
        }

        // Sets a configuration option
        public static void Set(string key, dynamic value) {
            Properties.Settings.Default[key] = value;
        }

        // Save the current user settings
        public static void Save() {
            Logger.Log("Saving User Configuration");

            Properties.Settings.Default["UsrCnf_AutoStart"] = Config.AutoStart;
            Properties.Settings.Default["UsrCnf_AutoGame"] = Config.AutoGame;
            Properties.Settings.Default["UsrCnf_UseNitros"] = Config.UseNitros;

            Properties.Settings.Default["UsrCnf_Accuracy"] = Config.Accuracy;
            Properties.Settings.Default["UsrCnf_AccuracyV"] = Config.AccuracyVariancy;

            Properties.Settings.Default["UsrCnf_TypingRate_Real"] = Config.TypingRate;
            Properties.Settings.Default["UsrCnf_TypingRateV"] = Config.TypingRateVariancy;

            // If it fails to save then just hard reset to prevent problems later on
            try {
                Properties.Settings.Default.Save();
            } catch (Exception) {
                Reset();
            }
        }

        // Attempts to hard reset user settings, only used in case of failure
        public static void Reset() {
            try {
                Properties.Settings.Default.Reset();
            } catch (Exception e) {
                MessageBox.Show(
                    "Could not load configuration files:\n" + e,
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
