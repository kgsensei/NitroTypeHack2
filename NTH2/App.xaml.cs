using System;
using System.Linq;
using WindowsInput;
using System.Windows;
using System.Threading.Tasks;

using CefSharp;

namespace NitroType2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public static bool isCheatRunning = false;

        public async static void simulateTypingText(string Text, int typingDelay, int accuracy, CefSharp.Wpf.ChromiumWebBrowser CefEmbed)
        {
            isCheatRunning = true;
            InputSimulator sim = new InputSimulator();

            // If use nitros is set to true try and find the longest word
            // Note to self: This is currently broken for whatever reason, FIX.
            if (thingsorwhatever.useNitros)
            {
                var words = Text.Split('\u00A0');
                int longest = words.Max(w => w.Length);
                words[longest] = "\n";
                Text = string.Join(" ", words);
            }

            // Convert the letters to a character array
            char[] letters = Text.Replace("\u00A0", " ").ToCharArray();

            // If randomization is on then change the accuracy and speed by a random amount
            if (thingsorwhatever.randomize)
            {
                var gen = new Random();
                accuracy = accuracy - (int)gen.NextDouble() * 10;
                typingDelay = typingDelay - (int)gen.NextDouble() * 10;
            }
            // Calculate the number of characters to miss based on accuracy
            int toMiss = (int)Math.Floor(letters.Length * ((decimal)(100 - accuracy) / 100));

            // Calculate the number of correct letters in between every missed letter
            int maxIndex = (int)Math.Floor(letters.Length / (decimal)(toMiss + 1));
            int index = 0;

            // Find host window so it can simulate key presses
            var host = CefEmbed.GetBrowser().GetHost();
            foreach (char letter in letters)
            {
                pressKey(letter, host);
                if (index == maxIndex)
                {
                    pressKey('+', host);
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
                await Task.Delay(4500);
                CefEmbed.Reload();
                isCheatRunning = false;
            } else
            {
                isCheatRunning = false;
            }
        }

        private static void pressKey(char key, IBrowserHost host)
        {
            KeyEvent evnt = new KeyEvent()
            {
                Type = KeyEventType.Char,
                WindowsKeyCode = (int)key
            };
            host.SendKeyEvent(evnt);
        }

        private static void pressKeyInt(int code, IBrowserHost host)
        {
            KeyEvent evnt = new KeyEvent()
            {
                Type = KeyEventType.Char,
                WindowsKeyCode = code
            };
            host.SendKeyEvent(evnt);
        }
    }
}
