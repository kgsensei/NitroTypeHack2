using Microsoft.Web.WebView2.Core;

namespace NitroType3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            try
            {
                _ = CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (WebView2RuntimeNotFoundException)
            {
                Logger.Log("Missing WebView2 Runtime", Logger.Level.Error);
                MessageBox.Show(
                    "You don't have the Microsoft WebView2 Component installed.\nThis is a requirement to run the cheat.\nPlease install it then run the cheat again.",
                    "Fatal Error",
                    MessageBoxButtons.OK
                );
                System.Diagnostics.Process.Start("WebView2Setup.exe");
                Environment.Exit(0); // Don't call Close() because window is not initialized yet
            }

            InitializeComponent();
            SetupWebview();

            Text = "NitroType Cheat v" + Updates.VersionCode;
        }

        private async void SetupWebview()
        {
            Logger.Log("Building WebView2");
            CoreWebView2EnvironmentOptions coreWebView2EnvironmentOptions = new()
            {
                AreBrowserExtensionsEnabled = true,
            };

            var webViewEnvOptions = await CoreWebView2Environment.CreateAsync(
                null,
                null,
                coreWebView2EnvironmentOptions
            );

            Logger.Log("Ensuring Initialization");
            await webView.EnsureCoreWebView2Async(webViewEnvOptions);

            Logger.Log("Injecting Captcha Solver");
            await webView.CoreWebView2.Profile.AddBrowserExtensionAsync(
                System.IO.Directory.GetCurrentDirectory() + @"\extensions\hlifkpholllijblknnmbfagnkjneagid"
            );

            Logger.Log("Hooking Request, Response, and Message Events");
            webView.CoreWebView2.WebResourceRequested += RequestBlocker;
            webView.CoreWebView2.WebResourceResponseReceived += RacePageLoadedChecker;
            webView.CoreWebView2.WebMessageReceived += WebMessageRecieved;
            webView.CoreWebView2.AddWebResourceRequestedFilter(
                null,
                CoreWebView2WebResourceContext.All
            );

            Logger.Log("Loading NitroType.com");
            webView.Source = new Uri("https://nitrotype.com");
        }

        private void UI_Update_Autostart(object sender, EventArgs e)
        {
            Logger.Log("Auto Start Value Changed:" + autostart.Checked.ToString());
            startButton.Enabled = !autostart.Checked;
            Config.AutoStart = autostart.Checked;
        }

        private void UI_Update_Autogame(object sender, EventArgs e)
        {
            Logger.Log("Auto Game Value Changed:" +  autogame.Checked.ToString());
            Config.AutoGame = autogame.Checked;
        }

        private void UI_Update_Usenitros(object sender, EventArgs e)
        {
            Logger.Log("Use Nitros Value Changed:" + usenitros.Checked.ToString());
            Config.UseNitros = usenitros.Checked;
        }

        private void UI_Update_Godmode(object? sender, EventArgs e)
        {
            if (Config.GodMode)
            {
                Logger.Log("God Mode Value Changed:False");
                godmode.Checked = false;
                Config.GodMode = false;
            }
            else
            {
                DialogResult GodmodeConsent = MessageBox.Show(
                    "God Mode removes all typing limits and uses max accuracy. It's highly likely your account will be banned if you use it, continue?",
                    "God Mode Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (GodmodeConsent == DialogResult.Yes)
                {
                    Logger.Log("God Mode Value Changed:True");
                    Config.GodMode = true;
                }
                else
                {
                    godmode.CheckedChanged -= UI_Update_Godmode;
                    godmode.Checked = false;
                    godmode.CheckedChanged += UI_Update_Godmode;
                }
            }
        }

        private void UI_Click_Discord(object sender, EventArgs e)
        {
            Logger.Log("Discord Button Clicked");
            System.Diagnostics.Process.Start("explorer.exe", "https://link.kgsensei.dev/discord");
        }

        private void UI_Slider_AccuracyVariance(object sender, EventArgs e)
        {
            accuracyVarianceLabel.Text = "Accuracy Variance: ±" + accuracyVarianceSlider.Value;
            Config.AccuracyVariancy = accuracyVarianceSlider.Value;
        }

        private void UI_Slider_AccuracySlider(object sender, EventArgs e)
        {
            accuracySliderLabel.Text = "Accuracy: " + accuracySlider.Value + "%";
            Config.Accuracy = accuracySlider.Value;
        }

        private void UI_Slider_TypingRateVariance(object sender, EventArgs e)
        {
            typingRateVarianceLabel.Text = "Typing Rate Variance: ±" + typingRateVarianceSlider.Value;
            Config.TypingRateVariancy = typingRateVarianceSlider.Value;
        }

        private void UI_Slider_TypingRate(object sender, EventArgs e)
        {
            int Total = typingRateSlider.Maximum + typingRateSlider.Minimum;
            int RealRate = Total - typingRateSlider.Value;
            Config.TypingRate = RealRate;
            int WPMCalculation = (int)( 60 / ( (double)RealRate / 1000 ) / 5 );
            typingRateSliderLabel.Text = "Typing Rate: ~" + WPMCalculation;
        }

        private void InjectAutoStart()
        {
            var r = new Random();
            string funcName = new(Enumerable.Range(0, 30).Select(n => (Char)r.Next(65, 90)).ToArray());

            Logger.Log("Injecting Auto Start Script");

            webView.ExecuteScriptAsync(
                @"function " + funcName + @"() {
                    if(document.getElementsByClassName('raceChat').length ? false : true) {
                        let z = document.getElementsByClassName('dash-letter');
                        let m = '';
                        for(let i = 0; i < z.length; i++) {
                            m = m + z[i].innerText
                        };
                        window.chrome.webview.postMessage('' + m);
                    } else {
                        setTimeout(() => {" + funcName + @"();}, 10);                    
                    }
                }

                setTimeout(() => {" + funcName + @"();}, 2000);

                setInterval(() => {
                    const m = ""Validated! Play on."";
                    const x = document.querySelector(""h1.tsxxl.mbs"");
                    const f = Array.from(document.querySelectorAll(""div.tc-i"")).find(l => l.textContent === m);
                    if(x) {
                        if(x.textContent === ""Communications Error"") {
                            window.location.reload();
                        }
                    }
                    if(f) {
                        if(f.textContent === m) {
                            window.location.reload();
                        }
                    }
                }, 250);"
            );
        }

        private void WebMessageRecieved(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (!Config.CheatRunning)
            {
                string BrowserData = e.TryGetWebMessageAsString();
                Logger.Log("Web Message Recieved:" + BrowserData);

                if (BrowserData == "GAME_NOT_STARTED_ERROR")
                {
                    MessageBox.Show(
                        "The game hasn't started yet.",
                        "Internal Error",
                        MessageBoxButtons.OK
                    );
                }
                else
                {
                    Controller.SimulateTypingText(BrowserData, webView);
                }
            }
        }

        private void RacePageLoadedChecker(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            string Uri = e.Request.Uri;

            if (
                Uri.Contains("nitrotype.com/race") &&
                !Uri.Contains("nitrotype.com/racer/") &&
                Config.AutoStart
            )
            {
                InjectAutoStart();
            }

            webView.ExecuteScriptAsync(
                @"setInterval(() => {
                    const tmpx = document.getElementsByClassName('ad');
                    for (let i = 0; i < tmpx.length; i++) {
                        tmpx[i].remove();
                    }
                }, 100);"
            );
        }

        private void RequestBlocker(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            bool Blocked = AdBlocker.IsBlocked(e.Request.Uri);

            if (Blocked)
            {
                e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                    null,
                    404,
                    "Resource Blocked by Client",
                    null
                );
            }
        }
    }
}
