namespace NitroType3
{
    class Logger
    {
        public enum Level
        {
            Debug,
            Error
        };

        readonly static string LogFilePath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData
            ),
            @"NTH3\Logs\Log-" + System.DateTime.Today.ToString(
                "dd-MM-yyyy"
            ) + "." + "txt"
        );

        public static void Log(string message, Level level = Level.Debug)
        {
            FileInfo LogFileInfo = new(LogFilePath);

            DirectoryInfo LogDirInfo = new(LogFileInfo.DirectoryName);

            if (!LogDirInfo.Exists) LogDirInfo.Create();

            using FileStream fileStream = new(LogFilePath, FileMode.Append);
            {
                using StreamWriter Log = new(fileStream);
                {
                    Log.WriteLine("[" + level + "] " + message);
                }
            }
        }
    }
}
