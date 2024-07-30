using Microsoft.Web.WebView2.WinForms;
using System.Numerics;

namespace NitroType3
{
    class Controller
    {
        public async static void SimulateTypingText(string Text, WebView2 webView)
        {
            Config.CheatRunning = true;
            Logger.Log("Func SimulateTypingText Running");

            int TypingRate = CalculateVariancy(Config.TypingRate, Config.TypingRateVariancy, Min: 10);
            int Accuracy = CalculateVariancy(Config.Accuracy, Config.AccuracyVariancy, Max: 100);

            Logger.Log("Calculated Race Specific Typing Rate:" + TypingRate.ToString());
            Logger.Log("Calculated Race Specific Accuracy:" + Accuracy.ToString());

            if (Config.UseNitros) {
                string[] Words = Text.Split('\u00A0');

                Words = ReplaceLongest(Words, "ʜ");

                Text = string.Join(" ", Words);
            }

            char[] InvalidCharacters = { '`', '~', '@', '#', '$', '%', '^', '&', '*', '(', ')' };
            char[] Letters = Text.Replace("\u00A0", " ").ToCharArray();

            int LettersLength = Letters.Length;
            int MissIndex = 0;

            int LettersToMiss = (int)Math.Floor( LettersLength * (decimal)( ( 100 - Accuracy ) / 100 ) );
            int MaxIndex = (int)Math.Floor( LettersLength / (decimal)( LettersToMiss + 1 ) );

            Random RandGen = new();

            for (int i = 0; i < LettersLength; i++)
            {
                char Letter = Letters[i];

                if (Letter == 'ʜ')
                {
                    string Args = @"{""type"": ""char"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}";
                    _ = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", Args);

                    i++;
                }
                else
                {
                    string Args = @"{""type"": ""char"", ""text"": """ + GetEscapeSequence(Letter) + @"""}";
                    _ = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", Args);
                }

                if (MissIndex == MaxIndex)
                {
                    string Args = @"{""type"": ""char"", ""text"": """ + InvalidCharacters.Sample() + @"""}";
                    _ = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent", Args);
                    MissIndex = 0;
                }
                else
                {
                    MissIndex++;
                }

                if (!Config.GodMode) {
                    await Task.Delay((int)( Math.Sin(i) * 10 + TypingRate ));
                }
            }

            if (Config.AutoGame)
            {
                await Task.Delay(RandGen.Next(5250, 5500));
                webView.Reload();
            }

            Logger.Log("Finished Typing");
            Config.CheatRunning = false;
        }

        private static string GetEscapeSequence(char c)
        {
            return "\\u" + ((int)c).ToString("X4");
        }

        /**
         * @param int       Base amount to change
         * @param int       + or - amount to change
         * @param int       Maximum value
         * @param int       Minimum value
         * @returns int     Fully modified base value
         */
        private static int CalculateVariancy(int Base, int Variancy, int Max = 0, int Min = 0)
        {
            Random RandGen = new();

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
        private static string[] ReplaceLongest(string[] Words, string NewValue)
        {
            int Length = 0;
            int Index = 0;

            for (int i = 0; i < Words.Length; i++)
            {
                if (Words[i].Length > Length)
                {
                    Length = Words[i].Length;
                    Index = i;
                }
            }

            Words[Index] = NewValue;

            return Words;
        }
    }

    public static class ArrayExtension
    {
        public static char Sample(this char[] array)
        {
            return array[ Random.Shared.Next(0, array.Length - 1) ];
        }
    }
}
