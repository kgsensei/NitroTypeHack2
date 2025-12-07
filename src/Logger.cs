namespace NitroType3 {
    class Logger {
        public enum Level {
            Debug,
            Error
        };

        readonly static string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"NTH5\Logs\Log-" + System.DateTime.Today.ToString(
                "dd-MM-yyyy"
            ) + "." + "log"
        );

        public static void Log(string message, Level level = Level.Debug) {
            FileInfo LogFileInfo = new(LogFilePath);
            if (LogFileInfo.DirectoryName != null) {
                DirectoryInfo LogDirInfo = new(LogFileInfo.DirectoryName);
                if (!LogDirInfo.Exists) LogDirInfo.Create();
                try {
                    using FileStream fileStream = new(LogFilePath, FileMode.Append); {
                        using StreamWriter Log = new(fileStream); {
                            Log.WriteLine("[" + level + "] " + message);
                        }
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
