using System;
using System.IO;
using System.Net;
using System.Windows;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;

namespace NitroType2
{
    class thingsorwhatever
    {
        public static int typingSpeed { get; set; } = 5;
        public static int accuracyLvl { get; set; } = 100;
        public static bool godMode { get; set; } = false;
        public static bool autoStart { get; set; } = true;
        public static bool randomize { get; set; } = false;
        public static bool autoGame { get; set; } = false;
        public static bool useNitros { get; set; } = false;
        public static IWebDriver selenium { get; set; } = null;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
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

        public async void AsyncInitialize()
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9936");

            await webview2.EnsureCoreWebView2Async();
            webview2.Initialized += Webview2_Initialized;

            webview2.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webview2.CoreWebView2.AddWebResourceRequestedFilter(null, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
            webview2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
        }

        private void Webview2_Initialized(object sender, EventArgs e)
        {
            EdgeOptions eo = new EdgeOptions();

            eo.AddArgument("--headless=new");
            eo.UseWebView = true;
            eo.DebuggerAddress = "localhost:9936";

            thingsorwhatever.selenium = new EdgeDriver(eo);
        }

        private void CoreWebView2_WebResourceResponseReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            if (e.Request.Uri.IndexOf("nitrotype.com/race") != -1)
            {
                MessageBox.Show(thingsorwhatever.selenium.Title);
                if (thingsorwhatever.autoStart)
                {
                    injectAutoStartScript();
                }
            }
        }

        private void injectAutoStartScript()
        {
            webview2.ExecuteScriptAsync(@"
                function cheatStart(){
                    if(document.getElementsByClassName('raceChat').length?false:true){
                        z=document.getElementsByClassName('dash-letter');
                        m='';for(let i=0;i<z.length;i++){m=m+z[i].innerText};
                        window.chrome.webview.postMessage(''+m);
                    }else{setTimeout(()=>{cheatStart()},10);}
                };setTimeout(()=>{cheatStart()},2000);");
        }

        private void CoreWebView2_WebResourceRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs e)
        {
            string uri = e.Request.Uri;
            string[] blocked = new string[] { "bugsnag", "googlesyndication", "adservice", "adsystem", "adsafeprotected",
                "facebook", "googletagmanager", "google-analytics", "ad-delivery", "doubleclick", "adlightning",
                "smartadserver", "quantserve", "qccerttest", "qualaroo", "criteo", "moatads", "intergi", "playwire",
                "captcha", "cloudflareinsights", "btloader" };
            for (int i = 0; i < blocked.Length; i++)
            {
                if (uri.IndexOf(blocked[i]) != -1)
                {
                    e.Response = webview2.CoreWebView2.Environment.CreateWebResourceResponse(null, 404, "Not Found", null);
                }
            }
            _ = webview2.ExecuteScriptAsync("console.log(" + uri + ")");
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!thingsorwhatever.godMode && !App.isCheatRunning)
            {
                int change = Convert.ToInt32(e.NewValue);
                int total = Convert.ToInt32(cheatTypeSpeedSlider.Maximum + cheatTypeSpeedSlider.Minimum);
                thingsorwhatever.typingSpeed = total - change;
            }
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (webview2.Source.ToString().IndexOf("nitrotype.com/race") != -1 && thingsorwhatever.autoStart == false)
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
                MessageBox.Show("Enter a Race to Use Cheat.", "NitroType AutoTyper", MessageBoxButton.OK);
            }
            webview2.Focus();
        }

        private void Webview2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            webview2.Focus();
            if (!App.isCheatRunning)
            {
                string browserData = e.TryGetWebMessageAsString();
                if (browserData == "GAME_NOT_STARTED_ERROR")
                {
                    MessageBox.Show("Game Hasn't Started Yet.", "NitroType AutoTyper", MessageBoxButton.OK);
                }
                else
                {
                    this.Activate();
                    App.simulateTypingText(browserData, thingsorwhatever.typingSpeed, thingsorwhatever.accuracyLvl, webview2);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.typingSpeed = 0;
                thingsorwhatever.accuracyLvl = 100;
                thingsorwhatever.godMode = true;
                cheatTypeSpeedSlider.IsEnabled = false;
                cheatTypeAccSlider.IsEnabled = false;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                int change = Convert.ToInt32(cheatTypeSpeedSlider.Value);
                int total = Convert.ToInt32(cheatTypeSpeedSlider.Maximum + cheatTypeSpeedSlider.Minimum);
                thingsorwhatever.typingSpeed = total - change;
                thingsorwhatever.accuracyLvl = Convert.ToInt32(cheatTypeAccSlider.Value);
                thingsorwhatever.godMode = false;
                cheatTypeSpeedSlider.IsEnabled = true;
                cheatTypeAccSlider.IsEnabled = true;
            }
        }

        private void cheatTypeAccSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App.isCheatRunning && !thingsorwhatever.godMode)
            {
                thingsorwhatever.accuracyLvl = Convert.ToInt32(e.NewValue);
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.autoStart = true;
                startCheatBtn.IsEnabled = false;
                if (webview2.Source.ToString().IndexOf("nitrotype.com/race") != -1)
                {
                    injectAutoStartScript();
                }
            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.autoStart = false;
                startCheatBtn.IsEnabled = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://paypal.me/publickgsensei");
        }

        private void CheckBox_Unchecked_2(object sender, RoutedEventArgs e)
        {
            thingsorwhatever.randomize = false;
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            thingsorwhatever.randomize = true;
        }

        private void CheckBox_Checked_3(object sender, RoutedEventArgs e)
        {
            thingsorwhatever.autoGame = true;
        }

        private void CheckBox_Unchecked_3(object sender, RoutedEventArgs e)
        {
            thingsorwhatever.autoGame = false;
        }

        private void CheckBox_Unchecked_4(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.useNitros = false;
            }
        }

        private void CheckBox_Checked_4(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.useNitros = true;
            }
        }
    }
}
