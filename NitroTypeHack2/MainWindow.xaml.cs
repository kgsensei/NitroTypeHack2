using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data;
using System.Diagnostics.Metrics;

namespace NitroTypeHack2
{
    class Updater
    {
        public static string version_code = "4.5.1";
    }

    /// <summary>
    /// Variables in the cheat that need to be referenced everywhere.
    /// </summary>
    class Globals
    {
        public static int typingSpeed { get; set; } = 5;
        public static int accuracyLevel { get; set; } = 100;
        public static int accuracyLevelVary { get; set; } = 0;
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
        public static string title = $"NitroType Cheat {Updater.version_code} - kgsensei";
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
            AppDomain.CurrentDomain.UnhandledException += OnDispatcherUnhandledException;
            try
            {
                _ = CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (WebView2RuntimeNotFoundException)
            {
                MessageBox.Show(
                    "You don't have the Microsoft WebView2 Component installed.\nThis is a requirement to run the cheat.\nPlease install it then run the cheat again.", "Critical Error", MessageBoxButton.OK);
                this.Close();
            }

            InitializeComponent();
            AsyncInitialize();

            var sr = (HttpWebRequest)WebRequest.Create("https://analytics.kgsensei.dev/api/software");
            sr.ContentType = "application/json";
            sr.Method = "POST";
            sr.Headers.Add("origin", "https://kgsensei.dev");
            using (var sw = new StreamWriter(sr.GetRequestStream())) { sw.Write("{\"project\":\"NitroTypeHack2\"}"); }
            _ = (HttpWebResponse)sr.GetResponse();
        }

        // Async start, for fancy stuff.
        public async void AsyncInitialize()
        {
            CoreWebView2EnvironmentOptions coreWebView2EnvironmentOptions = new CoreWebView2EnvironmentOptions();
            coreWebView2EnvironmentOptions.AreBrowserExtensionsEnabled = true;

            var webViewEnvOptions = await CoreWebView2Environment.CreateAsync(null, null, coreWebView2EnvironmentOptions);

            await webview2.EnsureCoreWebView2Async(webViewEnvOptions);

            try
            {
                // Injects the auto-captcha extension into the browser
                await webview2.CoreWebView2.Profile.AddBrowserExtensionAsync(System.IO.Directory.GetCurrentDirectory() + @"\extensions\hlifkpholllijblknnmbfagnkjneagid");
            } catch (Exception)
            {
                MessageBox.Show("Error occured when trying to inject the captcha solver.", "Critical Error", MessageBoxButton.OK);
                this.Close();
            }

            try
            {
                webview2.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
                webview2.CoreWebView2.AddWebResourceRequestedFilter(null, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
                webview2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
            } catch (Exception)
            {
                MessageBox.Show("Error occured when trying to setup the request filters.", "Critical Error", MessageBoxButton.OK);
                this.Close();
            }

            webview2.Source = new Uri("https://nitrotype.com");

            // Check for updates, if one exists then make a popup
            // message prompting the user to upgrade their client
            HttpClient httpClient = new HttpClient();
            var req = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.github.com/repos/kgsensei/NitroTypeHack2/releases/latest"),
                Method = HttpMethod.Get
            };
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req.Headers.UserAgent.Add(new ProductInfoHeaderValue("nth-version-checker", Updater.version_code));
            var res = await httpClient.SendAsync(req);

            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync();
            var ver = json.Split(':')[42].Split('"')[1];
            if(ver != Updater.version_code)
            {
                if(MessageBox.Show(
                    $"New update available!\n\nCurrent version v{Updater.version_code}\nNew version v{ver}\n\nWould you like to install it now?",
                    "Update Available",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/kgsensei/NitroTypeHack2/releases/latest");
                }
            }
        }

        // On button click start the cheat.
        async private void button_start_cheat(
            object sender,
            RoutedEventArgs e)
        {
            if (
                webview2.Source.ToString().IndexOf("nitrotype.com/race") != -1 &&
                webview2.Source.ToString().IndexOf("nitrotype.com/racer/") == -1 &&
                Globals.autoStart == false)
            {
                await webview2.ExecuteScriptAsync(@"
                    if(document.getElementsByClassName('raceChat').length ? false : true) {
                        z = document.getElementsByClassName('dash-letter');
                        m = '';
                        for(let i = 0 ; i < z.length; i++) {
                            m = m + z[i].innerText
                        };
                        window.chrome.webview.postMessage('' + m);
                    } else {
                        window.chrome.webview.postMessage('GAME_NOT_STARTED_ERROR');
                    }");
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
                if(wpmDisplay != null)
                {
                    wpmDisplay.Content = "~" + (int)(60 / ((double)Globals.typingSpeed / 1000) / 5);
                }
            }
        }

        private void slider_change_accuracy(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if(!Globals.godMode && !App.isCheatRunning)
            {
                Globals.accuracyLevel = Convert.ToInt32(e.NewValue);
                if(accDisplay != null)
                {
                    accDisplay.Content = Globals.accuracyLevel + "%";
                }
            }
        }

        private void slider_change_accuracy_vary(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if(!Globals.godMode && !App.isCheatRunning)
            {
                Globals.accuracyLevelVary = Convert.ToInt32(e.NewValue);
                if(accVaryDisplay != null)
                {
                    accVaryDisplay.Content = "±" + Globals.accuracyLevelVary;
                }
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
                if(webview2.Source != null)
                {
                    // Verify that we're actually in a race and not viewing a public racer profile
                    if (
                        webview2.Source.ToString().IndexOf("nitrotype.com/race") != -1 &&
                        webview2.Source.ToString().IndexOf("nitrotype.com/racer/") == -1)
                    {
                        injectAutoStartScript();
                    }
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

        private void button_Discord(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("https://link.kgsensei.dev/discord");
            string args = @"{""text"": ""The""}";
            webview2.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.insertText", args);
        }

        // Inject a script that will auto start the cheat.
        private void injectAutoStartScript()
        {
            var r = new Random();
            string funcName = new string(Enumerable.Range(0, 25).Select(n => (Char)r.Next(65, 90)).ToArray());
            string contName = new string(Enumerable.Range(0, 25).Select(n => (Char)r.Next(65, 90)).ToArray());
            webview2.ExecuteScriptAsync(contName + @" = 0;
                function " + funcName + @"() {
                    if(document.getElementsByClassName('raceChat').length ? false : true) {
                        let z = document.getElementsByClassName('dash-letter');
                        let m = '';
                        for(let i = 0; i < z.length; i++) {m = m + z[i].innerText};
                        window.chrome.webview.postMessage('' + m);
                    } else {
                        setTimeout(() => {" + funcName + @"()}, 10);                    
                    }
                }
                setTimeout(() => {" + funcName + @"()}, 2000);
                setInterval(() => {
                    const m = ""Validated! Play on.""
                    const x = document.querySelector(""h1.tsxxl.mbs"");
                    const f = Array.from(document.querySelectorAll(""div.tc-i"")).find(l => l.textContent === m);
                    if(x) {
                        if(x.textContent === ""Communications Error"") {
                            window.location.reload()
                        }
                    }
                    if(f) {
                        if(f.textContent === m) {
                            window.location.reload()
                        }
                    }
                }, 250);");
        }

        // Detect if cheat should start running.
        private void Webview2_WebMessageReceived(
            object sender,
            CoreWebView2WebMessageReceivedEventArgs e)
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
            if (
                e.Request.Uri.IndexOf("nitrotype.com/race") != -1 &&
                e.Request.Uri.IndexOf("nitrotype.com/racer/") == -1 &&
                Globals.autoStart)
            {
                injectAutoStartScript();
            }
            webview2.ExecuteScriptAsync(@"
                NTGLOBALS.NO_ADS = true;
                setInterval(() => {
                    const tmpx = document.getElementsByClassName('ad');
                    for (let i = 0; i < tmpx.length; i++) {
                        tmpx[i].remove();}},100)");
        }

        // Ad blocker.
        private void CoreWebView2_WebResourceRequested(
            object sender,
            CoreWebView2WebResourceRequestedEventArgs e)
        {
            string uri = e.Request.Uri;
            if (
                uri.IndexOf("doubleclick.net") != -1 ||
                uri.IndexOf("vuukle.com") != -1 ||
                uri.IndexOf("cloudfront.net") != -1 ||
                uri.IndexOf("proper.io") != -1 ||
                uri.IndexOf("pub.network") != -1 ||
                uri.IndexOf("btloader.com") != -1 ||
                uri.IndexOf("intergient.com") != -1 ||
                uri.IndexOf("qualaroo.com") != -1 ||
                uri.IndexOf("facebook.net") != -1 ||
                uri.IndexOf("facebook.com") != -1 ||
                uri.IndexOf("googletagmanager.com") != -1 ||
                uri.IndexOf("cloudflareinsights.com") != -1
            )
            {
                e.Response = webview2.CoreWebView2.Environment.CreateWebResourceResponse(null, 404, "Not Found", null);
            }
        }

        private void OnDispatcherUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled exception occurred\n\n> Sender\n" + sender.ToString() + "\n> Exception\n" + e.ExceptionObject.ToString(), "Critical Error", MessageBoxButton.OK);
        }
    }
}
