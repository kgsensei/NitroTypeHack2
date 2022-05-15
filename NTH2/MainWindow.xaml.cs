using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NitroType2
{
    class thingsorwhatever
    {
        public static int typingSpeed { get; set; } = 5;
        public static bool godMode { get; set; } = false;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!thingsorwhatever.godMode)
            {
                thingsorwhatever.typingSpeed = Convert.ToInt32(e.NewValue);
            }
            webview2.Focus();
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            var html = await webview2.ExecuteScriptAsync("z=document.getElementsByClassName('dash-letter');m='';for(let i=0;i<z.length;i++){m=m+z[i].innerText};window.chrome.webview.postMessage(''+m);");
            webview2.Focus();
        }

        private void Webview2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            webview2.Focus();
            if (!App.isCheatRunning)
            {
                App.simulateTypingText(e.TryGetWebMessageAsString(), thingsorwhatever.typingSpeed);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (App.isCheatRunning)
            {
                webview2.Focus();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!App.isCheatRunning)
            {
                if (thingsorwhatever.godMode)
                {
                    thingsorwhatever.typingSpeed = Convert.ToInt32(cheatTypeSpeedSlider.Value);
                }
                else
                {
                    thingsorwhatever.typingSpeed = 0;
                }
                thingsorwhatever.godMode = !thingsorwhatever.godMode;
            }
            webview2.Focus();
        }
    }
}
