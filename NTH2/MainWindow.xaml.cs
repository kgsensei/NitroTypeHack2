using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace NitroType2
{
    class thingsorwhatever
    {
        public static int typingSpeed { get; set; } = 5;
        public static int accuracyLvl { get; set; } = 100;
        public static bool godMode { get; set; } = false;
        public static bool autoStart { get; set; } = false;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            AsyncInitialize();
        }

        public async void AsyncInitialize()
        {
            await webview2.EnsureCoreWebView2Async();
            webview2.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webview2.CoreWebView2.AddWebResourceRequestedFilter(null, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
            webview2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
            var answer = MessageBox.Show("Would you like to donate to support the creator?", "Donate", MessageBoxButton.YesNo);
            if (answer == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start("https://www.paypal.com/donate/?business=8MYQCEFXWXTKQ&no_recurring=0&item_name=Donate+to+support+current+projects+and+enable+more+projects%2Ffeatures+in+the+future.&currency_code=USD");
            }
        }

        private void CoreWebView2_WebResourceResponseReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            if (e.Request.Uri.IndexOf("nitrotype.com/race") != -1 && thingsorwhatever.autoStart == true)
            {
                webview2.ExecuteScriptAsync(@"
                    function cheatStart(){
                        if(document.getElementsByClassName('raceChat').length?false:true){
                            z=document.getElementsByClassName('dash-letter');
                            m='';for(let i=0;i<z.length;i++){m=m+z[i].innerText};
                            window.chrome.webview.postMessage(''+m);
                        }else{setTimeout(()=>{cheatStart()},5);}
                    };setTimeout(()=>{cheatStart()},2000);");
            }
        }

        private void CoreWebView2_WebResourceRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs e)
        {
            string uri = e.Request.Uri;
            if (uri.IndexOf("googlesyndication") != -1 ||
                uri.IndexOf("adservice")         != -1 ||
                uri.IndexOf("adsystem")          != -1 ||
                uri.IndexOf("adsafeprotected")   != -1 ||
                uri.IndexOf("facebook")          != -1 ||
                uri.IndexOf("googletagmanager")  != -1 ||
                uri.IndexOf("google-analytics")  != -1 ||
                uri.IndexOf("ad-delivery")       != -1 ||
                uri.IndexOf("doubleclick")       != -1 ||
                uri.IndexOf("adlightning")       != -1 ||
                uri.IndexOf("smartadserver")     != -1 ||
                uri.IndexOf("quantserve")        != -1 ||
                uri.IndexOf("qccerttest")        != -1 ||
                uri.IndexOf("qualaroo")          != -1 ||
                uri.IndexOf("criteo")            != -1 ||
                uri.IndexOf("moatads")           != -1 ||
                uri.IndexOf("intergi")           != -1 ||
                uri.IndexOf("playwire")          != -1)
            {
                e.Response = webview2.CoreWebView2.Environment.CreateWebResourceResponse(null, 404, "Not found", null);
            }
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

        //document.getElementsByClassName('raceChat')

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
                    App.simulateTypingText(browserData, thingsorwhatever.typingSpeed, thingsorwhatever.accuracyLvl);
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
    }
}
