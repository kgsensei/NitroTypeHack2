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
            char[] allowedCharsToMiss = { '+', '-', '=' };

            int toMiss = (int)Math.Floor(letters.Length * ((decimal)(100 - accuracy) / 100));
            int maxIndex = (int)Math.Floor(letters.Length / (decimal)(toMiss + 1));
            int index = 0;

            for (int i = 0; i < letters.Length; i++)
            {
                char letter = letters[i];

                if (letter == 'ʜ')
                {
                    string args;
                    args = @"{""type"": ""rawKeyDown"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Input.dispatchKeyEvent",
                            args
                        );

                    args = @"{""type"": ""keyUp"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Input.dispatchKeyEvent",
                            args
                        );

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
                        args = @"{""type"": ""rawKeyDown"", ""windowsVirtualKeyCode"": 32, ""unmodifiedText"": "" "", ""text"": "" ""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                                "Input.dispatchKeyEvent",
                                args
                            );

                        args = @"{""type"": ""keyUp"", ""windowsVirtualKeyCode"": 32, ""unmodifiedText"": "" "", ""text"": "" ""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                                "Input.dispatchKeyEvent",
                                args
                            );
                    }
                    else
                    {
                        args = @"{""type"": ""char"", ""text"": """ + letter + @"""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                                "Input.dispatchKeyEvent",
                                args
                            );
                    }
                }

                if (index == maxIndex)
                {
                    // This will get one of the random 'allowed to miss' characters,
                    // that way we aren't using the same one for every miss.
                    string args = @"{""type"": ""char"", ""text"": """ + allowedCharsToMiss[gen.Next(0, allowedCharsToMiss.Length - 1)] + @"""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Input.dispatchKeyEvent",
                            args
                        );
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

        public static string ReplaceFirst(string input, string find, string replace)
        {
            var first = input.IndexOf(find);
            return input.Substring(0, first) + replace + input.Substring(first + find.Length);
        }
    }
}
