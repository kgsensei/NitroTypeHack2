using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NitroTypeHack2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool isCheatRunning = false;

        public async static void simulateTypingText(
            string text,
            int typingDelay,
            int accuracy,
            Microsoft.Web.WebView2.Wpf.WebView2 webview2)
        {
            isCheatRunning = true;

            if (Globals.useNitros)
            {
                var words = text.Split('\u00A0');
                int longest = words.Max(w => w.Length);
                words[longest] = "ʜ";
                text = string.Join(" ", words);
            }

            // .Replace("\u00A0", " ")
            char[] letters = text.ToCharArray();

            if (Globals.randomize)
            {
                var gen = new Random();
                accuracy -= (int)gen.NextDouble() * 20;
                typingDelay -= (int)gen.NextDouble() * 20;
            }

            int toMiss = (int)Math.Floor(letters.Length * ((decimal)(100 - accuracy) / 100));
            int maxIndex = (int)Math.Floor(letters.Length / (decimal)(toMiss + 1));
            int index = 0;

            foreach (char letter in letters)
            {
                textEntry(letter, webview2);
                /*
                if (letter == 'ʜ')
                {
                    sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                }
                else
                {
                    textEntry(letter, webview2);
                }
                */
                if (index == maxIndex)
                {
                    textEntry('+', webview2);
                    index = 0;
                }
                else
                {
                    index = index + 1;
                }
                await Task.Delay(typingDelay);
                if (Globals.autoGame)
                {
                    await Task.Delay(5000);
                    webview2.Reload();
                }
                isCheatRunning = false;
            }
        }

        private async static void textEntry(
            char letter,
            Microsoft.Web.WebView2.Wpf.WebView2 webview2)
        {
            string args;
            if (letter.Equals(" "))
            {
                args = @"{""type"": ""char"", ""text"": ""\u0020""}";
            } else
            {
                args = @"{""type"": ""char"", ""text"": """ + letter + @"""}";
            }
            _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                "Input.dispatchKeyEvent",
                args
                );
        }
    }
}
