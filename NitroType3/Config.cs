namespace NitroType3
{
    class Config
    {
        // Booleans
        public static bool AutoStart { get; set; } = true;
        public static bool AutoGame { get; set; } = false;
        public static bool UseNitros { get; set; } = false;
        public static bool GodMode { get; set; } = false;

        // Base Ints
        public static int TypingRate { get; set; } = 100;
        public static int Accuracy { get; set; } = 100;

        // Modifier Ints
        public static int TypingRateVariancy { get; set; } = 0;
        public static int AccuracyVariancy { get; set; } = 0;

        // Internal Runtime Settings
        public static bool CheatRunning { get; set; } = false;
    }
}
