using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput.Native;
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
            //Random rndm = new Random();
            foreach (char letter in letters)
            {
                //int missLetter = Convert.ToInt32(Math.Floor(rndm.NextDouble() * (accuracy / 100 + 1)));
                //if (missLetter == 1)
                //{
                //    sim.Keyboard.TextEntry("a");
                //}
                sim.Keyboard.TextEntry(letter);
                await Task.Delay(typingDelay);
            }
            isCheatRunning = false;
        }
    }
}
