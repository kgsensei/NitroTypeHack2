using System;
using System.IO;
using System.Net;
using System.Windows;

using CefSharp;
using CefSharp.Wpf;
using CefSharp.Handler;

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
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Build Cef Embedded Browser settings
            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream", "1");
            settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1");
            settings.CefCommandLineArgs.Add("disable-direct-write", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            
            settings.SetOffScreenRenderingBestPerformanceArgs();

            // Initialize the Cef Embedded Browser with the settings we just built
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("NitroType Cheat Crashed\nError Info:\n" + ex.ToString());
            }
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

            var _ = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        public void AsyncInitialize()
        {
            // Build request handler to block ad resources
            RequestHandler requestHandler = new CustomRequestHandler();
            CefEmbed.RequestHandler = requestHandler;

            // Load auto captcha from extensions
            RequestContext requestContext = new RequestContext();
            ExtensionHandler extensionHandler = new ExtensionHandler();
            requestContext.LoadExtensionFromDirectory("nitrotype-captcha-auto-clicker", extensionHandler);

            // Set callback for JavaScript so letters can be extracted from NitroType
            CefEmbed.JavascriptMessageReceived += CefEmbed_JavascriptMessageReceived;
            
            // Set callback for loading state changes so the auto start script can self inject
            CefEmbed.LoadingStateChanged += CefEmbed_LoadingStateChanged;
        }

        private void CefEmbed_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check if the page is no longer loading
            if (!e.IsLoading)
            {
                // If page is not loading, url is of /race & auto start is true then inject auto start script
                if (e.Browser.MainFrame.Url.IndexOf("nitrotype.com/race") != -1 && thingsorwhatever.autoStart)
                {
                    injectAutoStartScript();
                }
            }
        }

        private void CefEmbed_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            // Check to make sure cheat isn't already running
            if (!App.isCheatRunning)
            {
                string browserData = e.Message.ToString();
                // If the game isn't started throw an error message to user
                // If the game has started then start the cheat automation
                if (browserData == "GAME_NOT_STARTED_ERROR")
                {
                    System.Windows.MessageBox.Show("Game Hasn't Started Yet.", "NitroType AutoTyper", MessageBoxButton.OK);
                }
                else
                {
                    App.simulateTypingText(browserData, thingsorwhatever.typingSpeed, thingsorwhatever.accuracyLvl, CefEmbed);
                }
            }
        }

        // Inject the auto start script, this should call a C# method with the letters it extracts
        private void injectAutoStartScript()
        {
            CefEmbed.ExecuteScriptAsync(@"
                function cheatStart(){
                    if(document.getElementsByClassName('raceChat').length?false:true){
                        z=document.getElementsByClassName('dash-letter');
                        m='';
                        for(let i=0;i<z.length;i++){
                            m=m+z[i].innerText
                        };
                        CefSharp.PostMessage(''+m);
                    }else{
                        setTimeout(()=>{cheatStart()},5);
                    }
                };
                setTimeout(()=>{cheatStart()},2000);
            ");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // If on race page and auto start is false then get attempt to get letters
            // If not on race page throw message telling user to join a race
            if (CefEmbed.Address.IndexOf("nitrotype.com/race") != -1 && thingsorwhatever.autoStart == false)
            {
                CefEmbed.ExecuteScriptAsync(@"
                    if(document.getElementsByClassName('raceChat').length?false:true){
                        z=document.getElementsByClassName('dash-letter');
                        m='';for(let i=0;i<z.length;i++){m=m+z[i].innerText};
                        CefSharp.PostMessage(''+m);
                    }else{CefSharp.PostMessage('GAME_NOT_STARTED_ERROR');}");
            }
            else
            {
                System.Windows.MessageBox.Show("Enter a Race to Use Cheat.", "NitroType AutoTyper", MessageBoxButton.OK);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!thingsorwhatever.godMode && !App.isCheatRunning)
            {
                int change = Convert.ToInt32(e.NewValue);
                int total = Convert.ToInt32(cheatTypeSpeedSlider.Maximum + cheatTypeSpeedSlider.Minimum);
                thingsorwhatever.typingSpeed = total - change;
                if (CT_WPM != null)
                {
                    float wpm = ((change / 5) * 60) / 19;
                    CT_WPM.Content = "[Typing Speed][" + wpm.ToString() + " WPM±20] (?)";
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.typingSpeed = 8;
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
                if (CT_Acc != null)
                {
                    float acc = (int)Math.Floor(100 * ((decimal)(thingsorwhatever.accuracyLvl) / 100));
                    CT_Acc.Content = "[Typing Accuracy][" + acc + "%±5]";
                }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                thingsorwhatever.autoStart = true;
                startCheatBtn.IsEnabled = false;
                if (CefEmbed.Address.IndexOf("nitrotype.com/race") != -1)
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

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://pieclicker.com");
        }
    }

    public class CustomRequestHandler : RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            // Get url in a string variable then check it across ad services
            // note to self: clean this up with a for loop or something
            string uri = request.Url;
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
                // If the url contains an ad service call the class that discards requests
                return new DiscardRequest();
            }
            return null;
        }
    }

    // Class to discard any requests forwarded to it
    public class DiscardRequest : CefSharp.Handler.ResourceRequestHandler
    {
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            request.Url = "";
            request.Dispose();
            return CefReturnValue.Cancel;
        }
    }
}
