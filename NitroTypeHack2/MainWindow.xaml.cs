using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

namespace NitroTypeHack2
{
    /// <summary>
    /// Variables in the cheat that need to be referenced everywhere.
    /// </summary>
    class Globals
    {
        public static int typingSpeed { get; set; } = 5;
        public static int accuracyLevel { get; set; } = 100;
        public static bool godMode { get; set; } = false;
        public static bool autoStart { get; set; } = true;
        public static bool autoGame { get; set; } = false;
        public static bool useNitros { get; set; } = false;
    }

    /// <summary>
    /// Basically just localization variables for the cheat.
    /// </summary>
    class Messages
    {
        public static string title = "NitroType Cheat 4 - kgsensei";
        public static string err_game_not_started = "The game hasn't started yet.";
        public static string err_enter_a_race = "Enter a race to use the cheat.";
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Main Window start function.
        public MainWindow()
        {
            InitializeComponent();
            AsyncInitialize();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://rainydais.com/software.php");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("origin", "https://kgsensei.dev");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"project\":\"NitroTypeHack2\"}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        // Async start, for fancy stuff.
        public async void AsyncInitialize()
        {
            await webview2.EnsureCoreWebView2Async();
            webview2.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webview2.CoreWebView2.AddWebResourceRequestedFilter(null, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
            webview2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
        }

        // On button click start the cheat.
        async private void button_start_cheat(
            object sender,
            RoutedEventArgs e)
        {
            if (webview2.Source.ToString().IndexOf("nitrotype.com/race") != -1 && Globals.autoStart == false)
            {
                await webview2.ExecuteScriptAsync(@"
                    if(document.getElementsByClassName('raceChat').length?false:true){
                        z=document.getElementsByClassName('dash-letter');
                        m='';for(let i=0;i<z.length;i++){m=m+z[i].innerText};
                        window.chrome.webview.postMessage(''+m);
                    }else{window.chrome.webview.postMessage('GAME_NOT_STARTED_ERROR');}");
            }
            else
            {
                MessageBox.Show(Messages.err_enter_a_race, Messages.title, MessageBoxButton.OK);
            }
        }

        // On slider update change the speed.
        private void slider_change_speed(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (!Globals.godMode && !App.isCheatRunning)
            {
                int change = Convert.ToInt32(e.NewValue);
                int total = Convert.ToInt32(cheatTypeSpeedSlider.Maximum + cheatTypeSpeedSlider.Minimum);
                Globals.typingSpeed = total - change;
            }
        }

        private void slider_change_accuracy(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if(!Globals.godMode && !App.isCheatRunning)
            {
                Globals.accuracyLevel = Convert.ToInt32(e.NewValue);
            }
        }

        private void button_GodModeEnabled(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.typingSpeed = 1;
                Globals.accuracyLevel = 100;
                Globals.godMode = true;
                cheatTypeSpeedSlider.IsEnabled = false;
                cheatTypeAccSlider.IsEnabled = false;
            }
            else
            {
                button_GodMode.IsChecked = false;
            }
        }

        private void button_GodModeDisabled(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                int change = Convert.ToInt32(cheatTypeSpeedSlider.Value);
                int total = Convert.ToInt32(cheatTypeSpeedSlider.Maximum + cheatTypeSpeedSlider.Minimum);
                Globals.typingSpeed = total - change;
                Globals.accuracyLevel = Convert.ToInt32(cheatTypeAccSlider.Value);
                Globals.godMode = false;
                cheatTypeSpeedSlider.IsEnabled = true;
                cheatTypeAccSlider.IsEnabled = true;
            }
            else
            {
                button_GodMode.IsChecked = true;
            }
        }

        private void button_AutoStartOn(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.autoStart = true;
                startCheatBtn.IsEnabled = false;
                if (webview2.Source.ToString().IndexOf("nitrotype.com/race") != -1)
                {
                    injectAutoStartScript();
                }
            }
        }

        private void button_AutoStartOff(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.autoStart = false;
                startCheatBtn.IsEnabled = true;
            }
        }

        private void button_AutoGameOn(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.autoGame = true;
            }
        }

        private void button_AutoGameOff(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.autoGame = false;
            }
        }

        private void button_UseNitrosOn(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.useNitros = true;
            }
            else
            {
                button_UseNitros.IsChecked = false;
            }
        }

        private void button_UseNitrosOff(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                Globals.useNitros = false;
            }
            else
            {
                button_UseNitros.IsChecked = true;
            }
        }

        // Inject a script that will auto start the cheat.
        private void injectAutoStartScript()
        {
            var r = new Random();
            string funcName = new String(Enumerable.Range(0, 25).Select(n => (Char)(r.Next(65, 90))).ToArray());
            webview2.ExecuteScriptAsync(@"
                function " + funcName + @"(){
                    if(document.getElementsByClassName('raceChat').length?false:true){
                        z=document.getElementsByClassName('dash-letter');
                        m='';for(let i=0;i<z.length;i++){m=m+z[i].innerText};
                        window.chrome.webview.postMessage(''+m);
                    }else{setTimeout(()=>{" + funcName + @"()},10);}
                };setTimeout(()=>{" + funcName + @"()},2000);");
        }

        // Detect if cheat should start running.
        private void Webview2_WebMessageReceived(
            object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                string browserData = e.TryGetWebMessageAsString();
                if (browserData == "GAME_NOT_STARTED_ERROR")
                {
                    MessageBox.Show(Messages.err_game_not_started, Messages.title, MessageBoxButton.OK);
                }
                else
                {
                    this.Activate();
                    App.simulateTypingText(browserData, Globals.typingSpeed, Globals.accuracyLevel, webview2);
                }
            }
        }

        // Detect if auto start script should be injected.
        private void CoreWebView2_WebResourceResponseReceived(
            object sender,
            CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            if (e.Request.Uri.IndexOf("nitrotype.com/race") != -1 && Globals.autoStart)
            {
                injectAutoStartScript();
            }
        }

        // Ad blocker.
        private void CoreWebView2_WebResourceRequested(
            object sender,
            CoreWebView2WebResourceRequestedEventArgs e)
        {
            string uri = e.Request.Uri;
            if (uri.IndexOf("googlesyndication") != -1 ||
                uri.IndexOf("adservice") != -1 ||
                uri.IndexOf("adsystem") != -1 ||
                uri.IndexOf("adsafeprotected") != -1 ||
                uri.IndexOf("facebook") != -1 ||
                uri.IndexOf("googletagmanager") != -1 ||
                uri.IndexOf("google-analytics") != -1 ||
                uri.IndexOf("ad-delivery") != -1 ||
                uri.IndexOf("doubleclick") != -1 ||
                uri.IndexOf("adlightning") != -1 ||
                uri.IndexOf("smartadserver") != -1 ||
                uri.IndexOf("quantserve") != -1 ||
                uri.IndexOf("qccerttest") != -1 ||
                uri.IndexOf("qualaroo") != -1 ||
                uri.IndexOf("criteo") != -1 ||
                uri.IndexOf("moatads") != -1 ||
                uri.IndexOf("intergi") != -1 ||
                uri.IndexOf("playwire") != -1)
            {
                e.Response = webview2.CoreWebView2.Environment.CreateWebResourceResponse(null, 404, "Not Found", null);
            }
        }
    }
}
