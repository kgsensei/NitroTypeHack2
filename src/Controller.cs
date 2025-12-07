using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace NitroType3 {
    class Controller {
        private static readonly Random RandGen = new();

        // Simulation of typing text, this is the star of
        // the show and where most of the hardcore logic
        // for the cheat is stored
        public async static Task SimulateTypingText(string Text, WebView2 webView) {
            Config.CheatRunning = true;
            Logger.Log("Func SimulateTypingText Running");

            int TypingRate = CalculateVariancy(Config.TypingRate, Config.TypingRateVariancy, Min: 10);
            int Accuracy = CalculateVariancy(Config.Accuracy, Config.AccuracyVariancy, Max: 100);

            Logger.Log($"Calculated Race Specific Typing Rate: {TypingRate}wpm");
            Logger.Log($"Calculated Race Specific Accuracy: {Accuracy}%");

            if (Config.GodMode) Accuracy = 100;

            if (Config.UseNitros && !Config.GodMode) {
                string[] Words = Text.Split('\u00A0');
                Words = ReplaceLongest(Words, "ʜ");
                Text = string.Join(" ", Words);
            }

            char[] InvalidCharacters = { '`', '~', '@', '#', '$', '%', '^', '&', '*', '(', ')' };
            char[] Letters = Text.Replace("\u00A0", " ").ToCharArray();

            int MissIndex = 0;

            int LettersToMiss = (int)Math.Floor(Letters.Length * ((decimal)(100 - Accuracy) / 100));
            int MaxIndex = (int)Math.Floor(Letters.Length / (decimal)(LettersToMiss + 1));

            double[] sinTable = [.. Enumerable.Range(0, 256).Select(x => Math.Sin(x))];
            List<string> commandTable = [];

            // Prebuild commands and put them in a table
            for (int i = 0; i < Letters.Length; i++) {
                char Letter = Letters[i];
                if (Letter == 'ʜ') {
                    commandTable.Add(@"{""type"": ""rawKeyDown"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}");
                    commandTable.Add(@"{""type"": ""keyUp"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}");
                } else if (Letter == 32) {
                    commandTable.Add(@"{ ""type"": ""char"", ""keyIdentifier"": ""U+0020"", ""text"": "" "", ""code"": ""Space"", ""key"": "" "" }");
                } else {
                    commandTable.Add(BuildCharEvent(Letter));
                }
                if (MissIndex == MaxIndex) {
                    commandTable.Add(BuildCharEvent(InvalidCharacters.Sample()));
                    MissIndex = 0;
                } else {
                    MissIndex++;
                }
            }

            // Iterate over the already computed commands and dispatch them to the webview
            for (int i = 0; i < commandTable.Count; i++) {
                await Dispatch(webView, commandTable[i]);
                if (Config.GodMode) {
                    // For some reason removing this causes it to trip up, but waiting
                    // zero seconds fixes it?? I imagine this is because of how it handles
                    // await events or smth, maybe it's compensating for the browsers
                    // interpret duration...
                    await Task.Delay(0);
                } else {
                    await Task.Delay((int)(sinTable[i & 255] * 10 + TypingRate));
                }
            }

            // If auto game is turned on then wait a short period and press enter
            if (Config.AutoGame) {
                await Task.Delay(RandGen.Next(4250, 4500));
                await SendEnter(webView);
            }

            Logger.Log("Finished Typing");
            Config.CheatRunning = false;
        }

        private static async Task Dispatch(WebView2 webView, string json) => await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", json).ConfigureAwait(false);
        private static string BuildCharEvent(char c) => $@"{{ ""type"": ""char"", ""text"": ""{GetEscapeSequence(c)}"" }}";
        private static async Task SendEnter(WebView2 webView) {
            await Dispatch(webView, @"{""type"": ""rawKeyDown"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}");
            await Dispatch(webView, @"{""type"": ""keyUp"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}");
        }
        private static string GetEscapeSequence(char c) => "\\u" + ((int)c).ToString("X4");

        /**
         * @param int       Base amount to change
         * @param int       + or - amount to change
         * @param int       Maximum value
         * @param int       Minimum value
         * @returns int     Fully modified base value
         */
        private static int CalculateVariancy(int Base, int Variancy, int Max = 0, int Min = 0) {
            Base += RandGen.Next(-Variancy, Variancy);
            if (Max != 0 && Base > Max) Base = Max;
            if (Min != 0 && Base < Min) Base = Min;

            return Base;
        }

        /**
         * @param string[]      Words String array to search from
         * @param string        NewValue New value to replace longest item in string array
         * @returns string[]    Original string array with longest value replaced
         */
        private static string[] ReplaceLongest(string[] Words, string NewValue) {
            int Length = 0;
            int Index = 0;

            for (int i = 0; i < Words.Length; i++) {
                if (Words[i].Length > Length) {
                    Length = Words[i].Length;
                    Index = i;
                }
            }

            Words[Index] = NewValue;

            return Words;
        }
    }

    // Adds .sample() as a method to arrays because
    // I refuse to type all that out just to get a
    // random element from an array, utter bullshit
    public static class ArrayExtension {
        public static char Sample(this char[] array) {
            return array[ Random.Shared.Next(0, array.Length - 1) ];
        }
    }
}
