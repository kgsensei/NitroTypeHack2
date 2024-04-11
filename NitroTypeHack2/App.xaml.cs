using System;
using System.Linq;
using System.Runtime.InteropServices;
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
                string longest = words.OrderByDescending(n => n.Length).First();
                text = string.Join(" ", words);
                text = ReplaceFirst(text, longest, "ʜ");
            }

            var gen = new Random();

            if (Globals.accuracyLevelVary > 0)
            {
                accuracy += gen.Next(-Globals.accuracyLevelVary, Globals.accuracyLevelVary);
                if(accuracy > 100) accuracy = 100;
            }

            char[] letters = text.Replace("\u00A0", " ").ToCharArray();
            char[] allowedCharsToMiss = { '+', '!', '@' };

            int llength = letters.Length;

            int toMiss = (int)Math.Floor(llength * ((decimal)(100 - accuracy) / 100));
            int maxIndex = (int)Math.Floor(llength / (decimal)(toMiss + 1));
            int index = 0;

            for (int i = 0; i < llength; i++)
            {
                char letter = letters[i];

                if (letter == 'ʜ')
                {
                    string args = @"{""type"": ""char"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", args);
                    // Do this to avoid the space as next character,
                    // if we press it then the accuracy level will
                    // be offset.
                    i++;
                }
                else
                {
                    string args;
                    // Letter 32 is a newline character
                    if (letter == 32)
                    {
                        args = @"{""type"": ""char"", ""windowsVirtualKeyCode"": 32, ""unmodifiedText"": "" "", ""text"": "" ""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", args);
                    }
                    // Letter 34 is a double quote | 39 is a single quote for future reference
                    else if (letter == 34)
                    {
                        var keyboard_layout = GetKeyboardLayout(0);
                        var virtual_key = VkKeyScanExA(letter, keyboard_layout);
                        MessageBox.Show(virtual_key.ToString());
                        args = "{\"type\": \"char\", \"windowsVirtualKeyCode\": 222, \"unmodifiedText\": '\x22', \"text\": '\x22'}";
                        MessageBox.Show(args);
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", args);
                    }
                    else
                    {
                        //var keyboard_layout = GetKeyboardLayout(0);
                        //var virtual_key = VkKeyScanExA(letter, keyboard_layout);
                        //args = @"{""type"": ""char"", ""windowsVirtualKeyCode: " + virtual_key + @", unmodifiedText"": """ + letter + @""", ""text"": """ + letter + @"""}";
                        args = @"{""type"": ""char"", ""text"": """ + letter + @"""}";
                        try
                        {
                            _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", args);
                        }
                        catch ( Exception e )
                        {
                            MessageBox.Show($"Input.insertText Args >_\n{args}\n\nError in App.xaml.cs 76 >_\n{e.Message}");
                        }
                    }
                }

                if (index == maxIndex)
                {
                    // This will get one of the random 'allowed to miss' characters,
                    // that way we aren't using the same one for every miss.
                    string args = @"{""type"": ""char"", ""text"": """ + allowedCharsToMiss[gen.Next(0, allowedCharsToMiss.Length - 1)] + @"""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", args);
                    index = 0;
                }
                else
                {
                    index++;
                }

                if(!Globals.godMode) await Task.Delay((int)(Math.Sin(index) * 10 + typingDelay));
            }

            if (Globals.autoGame)
            {
                await Task.Delay(gen.Next(4900, 5600));
                webview2.Reload();
            }

            isCheatRunning = false;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern short VkKeyScanExA(char ch, int dwhkl);

        [DllImport("user32.dll")]
        private static extern short GetKeyboardLayout(int idThread);

        public static string ReplaceFirst(string input, string find, string replace)
        {
            var first = input.IndexOf(find);
            return input.Substring(0, first) + replace + input.Substring(first + find.Length);
        }
    }
}
