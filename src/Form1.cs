using Microsoft.Web.WebView2.Core;

namespace NitroType3 {
    public partial class Form1 : Form {
        // Contains a list of the IFrames on the page,
        // will need to inject into all of them
        private List<CoreWebView2Frame> WebviewIFrame = [];

        public Form1() {
            try {
                _ = CoreWebView2Environment.GetAvailableBrowserVersionString();
            } catch (WebView2RuntimeNotFoundException) {
                Logger.Log("Missing WebView2 Runtime", Logger.Level.Error);
                MessageBox.Show(
                    "You don't have the Microsoft WebView2 Component installed.\n" +
                    "This is a requirement to run the cheat.\n" +
                    "Please install it then run the cheat again.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Environment.Exit(0); // Don't call Close() because window is not initialized yet
            } catch (DllNotFoundException) {
                Logger.Log("Unable to load DLL WebViewLoader2.dll", Logger.Level.Error);
                MessageBox.Show(
                    "Unable to load DLL WebViewLoader2.dll or one of its dependencies." +
                    "Are you using another program that requires it or running two" +
                    "versions of the cheat at once?",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Environment.Exit(0); // Don't call Close() because window is not initialized yet
            }

            InitializeComponent();
            LoadPreviousUser();
            SetupWebview();

            Text = "NitroType Cheat v" + Updates.VersionCode;
        }

        // Loads settings from last session if it exists
        private void LoadPreviousUser() {
            int Opens = UserConfig.Get("UsrCnf_OpenAmount");
            Logger.Log("Open Number:" + Opens);
            if (Opens != 0) {
                try {
                    Config.TypingRate = UserConfig.Get("UsrCnf_TypingRate_Real");
                    typingRateSlider.Value = UserConfig.Get("UsrCnf_TypingRate_Visual");

                    Config.TypingRateVariancy = UserConfig.Get("UsrCnf_TypingRateV");
                    typingRateVarianceSlider.Value = Config.TypingRateVariancy;

                    Config.Accuracy = UserConfig.Get("UsrCnf_Accuracy");
                    accuracySlider.Value = Config.Accuracy;

                    Config.AccuracyVariancy = UserConfig.Get("UsrCnf_AccuracyV");
                    accuracyVarianceSlider.Value = Config.AccuracyVariancy;

                    Config.AutoStart = UserConfig.Get("UsrCnf_AutoStart");
                    autostart.Checked = Config.AutoStart;
                    startButton.Enabled = !Config.AutoStart;
                    if (Config.AutoStart) {
                        UI_Change_Start_Colors(24, 85, 133);
                    } else {
                        UI_Change_Start_Colors(214, 47, 58);
                    }

                    Config.AutoGame = UserConfig.Get("UsrCnf_AutoGame");
                    autogame.Checked = Config.AutoGame;

                    Config.UseNitros = UserConfig.Get("UsrCnf_UseNitros");
                    usenitros.Checked = Config.UseNitros;
                } catch (Exception) {
                    UserConfig.Reset();
                }
            }
            Opens++;
            UserConfig.Set("UsrCnf_OpenAmount", Opens);
        }

        // Sets the browser settings for the webview and injects captcha solver
        private async void SetupWebview() {
            Logger.Log("Building WebView2");
            CoreWebView2EnvironmentOptions coreWebView2EnvironmentOptions = new() {
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

            Logger.Log("Hooking Request, Response, and Frame Events");
            webView.CoreWebView2.WebResourceRequested += RequestBlocker;
            webView.CoreWebView2.FrameCreated += CoreWebView2_FrameCreated;
            webView.CoreWebView2.WebResourceResponseReceived += ManualAdBlocker;
            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

            Logger.Log("Loading NitroType.com");
            webView.Source = new Uri("https://nitrotype.com");
        }

        // Detect when a new frame is created on the page and add it to our list
        void CoreWebView2_FrameCreated(object? sender, CoreWebView2FrameCreatedEventArgs args) {
            WebviewIFrame.Add(args.Frame);
            if (Config.AutoStart) {
                args.Frame.NavigationCompleted += InjectAutoStart;
            }
            args.Frame.WebMessageReceived += WebMessageRecieved;
            args.Frame.Destroyed += WebViewFrames_DestoryedNestedIFrames;
        }

        // Detect when a frame is destroyed on the page and remove it from out list
        void WebViewFrames_DestoryedNestedIFrames(object? sender, object args) {
            try {
                Logger.Log("Removing IFrame from List");
                var frameToRemove = WebviewIFrame.SingleOrDefault(r => r.IsDestroyed() == 1);
                if (frameToRemove != null) WebviewIFrame.Remove(frameToRemove);
            } catch (InvalidOperationException ex) {
                MessageBox.Show(
                    "Failed to remove WebviewIFrame ID from list\n" + ex,
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Change the colours of the start button
        private void UI_Change_Start_Colors(int r, int b, int g) {
            Color color = Color.FromArgb(r, b, g);
            startButton.BackColor = color;
            startButton.FlatAppearance.BorderColor = color;
            startButton.FlatAppearance.MouseOverBackColor = color;
            startButton.FlatAppearance.MouseDownBackColor = color;
        }

        // Autostart Checkbox Logic
        private void UI_Update_Autostart(object sender, EventArgs e) {
            Logger.Log("Auto Start Value Changed:" + autostart.Checked.ToString());
            startButton.Enabled = !autostart.Checked;
            Config.AutoStart = autostart.Checked;
            if (Config.AutoStart) {
                UI_Change_Start_Colors(24, 85, 133);
            } else {
                UI_Change_Start_Colors(214, 47, 58);
            }
        }

        // Autogame Checkbox Logic
        private void UI_Update_Autogame(object sender, EventArgs e) {
            Logger.Log("Auto Game Value Changed:" + autogame.Checked.ToString());
            Config.AutoGame = autogame.Checked;
        }

        // Use Nitros Checkbox Logic
        private void UI_Update_Usenitros(object sender, EventArgs e) {
            Logger.Log("Use Nitros Value Changed:" + usenitros.Checked.ToString());
            Config.UseNitros = usenitros.Checked;
        }

        // Godmode Checkbox Logic
        private void UI_Update_Godmode(object? sender, EventArgs e) {
            if (Config.GodMode) {
                Logger.Log("God Mode Value Changed:False");
                godmode.Checked = false;
                Config.GodMode = false;
            } else {
                DialogResult GodmodeConsent = MessageBox.Show(
                    "God Mode removes all typing limits and uses max accuracy. " +
                    "It's highly likely your account will be banned if you use " +
                    "it, continue?",
                    "God Mode Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (GodmodeConsent == DialogResult.Yes) {
                    Logger.Log("God Mode Value Changed:True");
                    Config.GodMode = true;
                } else {
                    godmode.CheckedChanged -= UI_Update_Godmode;
                    godmode.Checked = false;
                    godmode.CheckedChanged += UI_Update_Godmode;
                }
            }
        }

        // Discord Button Logic
        private void UI_Click_Discord(object sender, EventArgs e) {
            Logger.Log("Discord Button Clicked");
            Connections.OpenLink(BuildEnvironment.DiscordLink);
        }

        // Manual Start Logic + Script Injection
        private void UI_Click_Start(object sender, EventArgs e) {
            Logger.Log("Manual Start Clicked");
            if (webView.Source.AbsolutePath == "/race" && !Config.AutoStart) {
                for (int i = 0; i < WebviewIFrame.Count; i++) {
                    if (WebviewIFrame[i] != null) {
                        Logger.Log("Injecting Manual Start Script into: (" + i + ") of " + WebviewIFrame.Count);
                        WebviewIFrame[i].ExecuteScriptAsync(
@"(() => {
    if (document.getElementsByClassName('raceChat').length == 0) {
        let z = document.getElementsByClassName('dash-copy')[0].innerText.replaceAll('\n', '');
        if (z != '') {
            window.chrome.webview.postMessage(z);
            return;
        }
    }
    window.chrome.webview.postMessage('GAME_NOT_STARTED_ERROR');
})();"
                        );
                    }
                }
            } else {
                MessageBox.Show(
                    "Enter A race before starting the cheat.",
                    "Internal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Accuracy Variance Slider Logic
        private void UI_Slider_AccuracyVariance(object sender, EventArgs e) {
            accuracyVarianceLabel.Text = "Accuracy Variance: " + accuracyVarianceSlider.Value;
            Config.AccuracyVariancy = accuracyVarianceSlider.Value;
        }

        // Accuracy Slider Logic
        private void UI_Slider_AccuracySlider(object sender, EventArgs e) {
            accuracySliderLabel.Text = "Accuracy: " + accuracySlider.Value + "%";
            Config.Accuracy = accuracySlider.Value;
        }

        // Typing Rate Variance Logic
        private void UI_Slider_TypingRateVariance(object sender, EventArgs e) {
            typingRateVarianceLabel.Text = "Typing Rate Variance: �" + typingRateVarianceSlider.Value;
            Config.TypingRateVariancy = typingRateVarianceSlider.Value;
        }

        // Typing Rate Logic
        private void UI_Slider_TypingRate(object sender, EventArgs e) {
            int Total = typingRateSlider.Maximum + typingRateSlider.Minimum;
            int RealRate = Total - typingRateSlider.Value;
            Config.TypingRate = RealRate;
            int WPMCalculation = (int)(60 / ((double)RealRate / 1000) / 5);
            typingRateSliderLabel.Text = "Typing Rate: ~" + WPMCalculation;
            UserConfig.Set("UsrCnf_TypingRate_Visual", typingRateSlider.Value);
        }

        // Auto start script
        private string GetAutoStart() {
            return
@"(() => {
    function tryAndPatchThisNitroTypeLMFAOOO() {
        if (document.getElementsByClassName('raceChat').length == 0) {
            let z = document.getElementsByClassName('dash-copy')[0].innerText.replaceAll('\n', '');
            if (z != '') {
                window.chrome.webview.postMessage(z);
                return;
            }
        }
        setTimeout(() => {tryAndPatchThisNitroTypeLMFAOOO();}, 10);
    }

    setTimeout(() => {tryAndPatchThisNitroTypeLMFAOOO();}, 3000);

    setInterval(() => {
        const m = 'Validated!Playon.';
        const x = document.querySelector('h1.tsxxl.mbs');
        const f = Array.from(document.querySelectorAll('div.tc-i')).find(l => l.textContent.replace(/ /g, '') === m);
        if (x) {
            if (x.textContent === 'Communications Error') {
                window.location.reload();
            }
        }
        if (f) {
            if (f.textContent.replace(/ /g, '') === m) {
                window.location.reload();
            }
        }
    }, 100);
})();";
        }

        // If a web message is recieved there's a possibility it
        // contains the text to type so we need to check if it is
        // the GAME_NOT_STARTED_ERROR or empty, otherwise send the
        // text over to the controller.
        private void WebMessageRecieved(object? sender, CoreWebView2WebMessageReceivedEventArgs e) {
            if (!Config.CheatRunning) {
                string BrowserData = e.TryGetWebMessageAsString();
                if (BrowserData == "") return;
                Logger.Log("Web Message Recieved: " + BrowserData);

                if (BrowserData == "GAME_NOT_STARTED_ERROR") {
                    MessageBox.Show(
                        "The game hasn't started yet.",
                        "Internal Error",
                        MessageBoxButtons.OK
                    );
                } else {
                    _ = Controller.SimulateTypingText(BrowserData, webView);
                }
            }
        }

        // Inject the auto start script into all current frames
        private void InjectAutoStart(object? sender, CoreWebView2NavigationCompletedEventArgs? e) {
            for (int i = 0; i < WebviewIFrame.Count; i++) {
                if (WebviewIFrame[i] != null) {
                    Logger.Log("Injecting Auto Start Script into: (" + i + ") of " + WebviewIFrame.Count);
                    WebviewIFrame[i].ExecuteScriptAsync(GetAutoStart());
                }
            }
        }

        // Remove html elements that may be ad containers,
        // we need this to remove shadows and NitroType's
        // own ads for nitro and whatnot
        private void ManualAdBlocker(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e) {
            webView.ExecuteScriptAsync(
                @"setInterval(() => {
                    const tmpx = document.querySelectorAll('.profile-ad, .ad, .goldTeaser');
                    for (let i = 0; i < tmpx.length; i++) {
                        tmpx[i].remove();
                    }
                }, 100);"
            );
        }

        // Checks all requests against the dedicated adblock
        // list and will reject anything potentially "malicious"
        private void RequestBlocker(object? sender, CoreWebView2WebResourceRequestedEventArgs e) {
            if (AdBlocker.IsBlocked(e.Request.Uri)) {
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
