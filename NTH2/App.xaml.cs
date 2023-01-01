using System;
using WindowsInput;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;

namespace NitroType2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public static bool isCheatRunning = false;
        public async static void simulateTypingText(string Text, int typingDelay, int accuracy, WebView2 webView2)
        {
            isCheatRunning = true;
            InputSimulator sim = new InputSimulator();
            char[] letters = Text.Replace("\u00A0", " ").ToCharArray();
            if (thingsorwhatever.randomize)
            {
                var gen = new Random();
                accuracy -= (int)gen.NextDouble() * 10;
                typingDelay -= (int)gen.NextDouble() * 10;
            }
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
            if (thingsorwhatever.autoGame)
            {
                await Task.Delay(5000);
                webView2.Reload();
                isCheatRunning = false;
            } else
            {
                isCheatRunning = false;
            }
        }
    }
}
