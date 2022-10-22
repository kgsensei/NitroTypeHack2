using System;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;

namespace NitroType2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public static bool isCheatRunning = false;
        public async static void simulateTypingText(string Text, int typingDelay, int accuracy)
        {
            isCheatRunning = true;
            InputSimulator sim = new InputSimulator();
            char[] letters = Text.Replace("\u00A0", " ").ToCharArray();
            int toMiss = (int)Math.Floor(letters.Length * ((decimal)(100 - accuracy) / 100));
            int maxIndex = (int)Math.Floor(letters.Length / (decimal)(toMiss + 1));
            int index = 0;
            foreach (char letter in letters)
            {
                sim.Keyboard.TextEntry(letter);
                if (index == maxIndex)
                {
                    sim.Keyboard.TextEntry('+');
                    index = 0;
                }
                else
                {
                    index = index + 1;
                }
                await Task.Delay(typingDelay);
            }
            isCheatRunning = false;
        }
    }
}
