using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NitroType3
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            controls = new Panel();
            discord = new Button();
            godmode = new CheckBox();
            usenitros = new CheckBox();
            autogame = new CheckBox();
            autostart = new CheckBox();
            accuracyVarianceSlider = new TrackBar();
            accuracyVarianceLabel = new Label();
            accuracySlider = new TrackBar();
            accuracySliderLabel = new Label();
            typingRateVarianceSlider = new TrackBar();
            typingRateVarianceLabel = new Label();
            typingRateSlider = new TrackBar();
            typingRateSliderLabel = new Label();
            startButton = new Button();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            controls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)accuracyVarianceSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)accuracySlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)typingRateVarianceSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)typingRateSlider).BeginInit();
            SuspendLayout();
            // 
            // webView
            // 
            webView.AccessibleName = "webView";
            webView.AllowExternalDrop = true;
            webView.BackColor = Color.Black;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.Black;
            webView.Dock = DockStyle.Fill;
            webView.Location = new Point(0, 0);
            webView.Margin = new Padding(0);
            webView.Name = "webView";
            webView.Size = new Size(1100, 800);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            // 
            // controls
            // 
            controls.BackColor = Color.DimGray;
            controls.Controls.Add(discord);
            controls.Controls.Add(godmode);
            controls.Controls.Add(usenitros);
            controls.Controls.Add(autogame);
            controls.Controls.Add(autostart);
            controls.Controls.Add(accuracyVarianceSlider);
            controls.Controls.Add(accuracyVarianceLabel);
            controls.Controls.Add(accuracySlider);
            controls.Controls.Add(accuracySliderLabel);
            controls.Controls.Add(typingRateVarianceSlider);
            controls.Controls.Add(typingRateVarianceLabel);
            controls.Controls.Add(typingRateSlider);
            controls.Controls.Add(typingRateSliderLabel);
            controls.Controls.Add(startButton);
            controls.Dock = DockStyle.Right;
            controls.Location = new Point(1100, 0);
            controls.Name = "controls";
            controls.Size = new Size(200, 800);
            controls.TabIndex = 1;
            // 
            // discord
            // 
            discord.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            discord.BackColor = Color.DimGray;
            discord.FlatStyle = FlatStyle.Flat;
            discord.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            discord.ForeColor = Color.White;
            discord.Location = new Point(5, 765);
            discord.Margin = new Padding(5);
            discord.Name = "discord";
            discord.Size = new Size(190, 30);
            discord.TabIndex = 0;
            discord.Text = "Join Discord";
            discord.UseVisualStyleBackColor = false;
            discord.Click += UI_Click_Discord;
            // 
            // godmode
            // 
            godmode.Font = new Font("Segoe UI", 10F);
            godmode.ForeColor = Color.White;
            godmode.Location = new Point(5, 327);
            godmode.Name = "godmode";
            godmode.Size = new Size(104, 24);
            godmode.TabIndex = 0;
            godmode.Text = "God Mode";
            godmode.CheckedChanged += UI_Update_Godmode;
            // 
            // usenitros
            // 
            usenitros.Font = new Font("Segoe UI", 10F);
            usenitros.ForeColor = Color.White;
            usenitros.Location = new Point(5, 297);
            usenitros.Name = "usenitros";
            usenitros.Size = new Size(104, 24);
            usenitros.TabIndex = 0;
            usenitros.Text = "Use Nitros";
            usenitros.CheckedChanged += UI_Update_Usenitros;
            // 
            // autogame
            // 
            autogame.Font = new Font("Segoe UI", 10F);
            autogame.ForeColor = Color.White;
            autogame.Location = new Point(5, 267);
            autogame.Name = "autogame";
            autogame.Size = new Size(104, 24);
            autogame.TabIndex = 0;
            autogame.Text = "Auto Game";
            autogame.CheckedChanged += UI_Update_Autogame;
            // 
            // autostart
            // 
            autostart.Checked = true;
            autostart.CheckState = CheckState.Checked;
            autostart.Font = new Font("Segoe UI", 10F);
            autostart.ForeColor = Color.White;
            autostart.Location = new Point(5, 237);
            autostart.Name = "autostart";
            autostart.Size = new Size(104, 24);
            autostart.TabIndex = 0;
            autostart.Text = "Auto Start";
            autostart.CheckedChanged += UI_Update_Autostart;
            // 
            // accuracyVarianceSlider
            // 
            accuracyVarianceSlider.BackColor = Color.DimGray;
            accuracyVarianceSlider.Location = new Point(0, 198);
            accuracyVarianceSlider.Margin = new Padding(0);
            accuracyVarianceSlider.Maximum = 15;
            accuracyVarianceSlider.Name = "accuracyVarianceSlider";
            accuracyVarianceSlider.Size = new Size(200, 45);
            accuracyVarianceSlider.TabIndex = 5;
            accuracyVarianceSlider.TickStyle = TickStyle.None;
            accuracyVarianceSlider.ValueChanged += UI_Slider_AccuracyVariance;
            // 
            // accuracyVarianceLabel
            // 
            accuracyVarianceLabel.Font = new Font("Segoe UI", 10F);
            accuracyVarianceLabel.ForeColor = Color.White;
            accuracyVarianceLabel.Location = new Point(5, 175);
            accuracyVarianceLabel.Name = "accuracyVarianceLabel";
            accuracyVarianceLabel.Size = new Size(190, 23);
            accuracyVarianceLabel.TabIndex = 6;
            accuracyVarianceLabel.Text = "Accuracy Variance: ±0";
            accuracyVarianceLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // accuracySlider
            // 
            accuracySlider.BackColor = Color.DimGray;
            accuracySlider.Location = new Point(0, 153);
            accuracySlider.Margin = new Padding(0);
            accuracySlider.Maximum = 100;
            accuracySlider.Name = "accuracySlider";
            accuracySlider.Size = new Size(200, 45);
            accuracySlider.TabIndex = 4;
            accuracySlider.TickStyle = TickStyle.None;
            accuracySlider.Value = 100;
            accuracySlider.ValueChanged += UI_Slider_AccuracySlider;
            // 
            // accuracySliderLabel
            // 
            accuracySliderLabel.Font = new Font("Segoe UI", 10F);
            accuracySliderLabel.ForeColor = Color.White;
            accuracySliderLabel.Location = new Point(5, 130);
            accuracySliderLabel.Name = "accuracySliderLabel";
            accuracySliderLabel.Size = new Size(190, 23);
            accuracySliderLabel.TabIndex = 0;
            accuracySliderLabel.Text = "Accuracy: 100%";
            accuracySliderLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // typingRateVarianceSlider
            // 
            typingRateVarianceSlider.BackColor = Color.DimGray;
            typingRateVarianceSlider.Location = new Point(0, 108);
            typingRateVarianceSlider.Margin = new Padding(0);
            typingRateVarianceSlider.Maximum = 15;
            typingRateVarianceSlider.Name = "typingRateVarianceSlider";
            typingRateVarianceSlider.Size = new Size(200, 45);
            typingRateVarianceSlider.TabIndex = 3;
            typingRateVarianceSlider.TickStyle = TickStyle.None;
            typingRateVarianceSlider.ValueChanged += UI_Slider_TypingRateVariance;
            // 
            // typingRateVarianceLabel
            // 
            typingRateVarianceLabel.Font = new Font("Segoe UI", 10F);
            typingRateVarianceLabel.ForeColor = Color.White;
            typingRateVarianceLabel.Location = new Point(5, 85);
            typingRateVarianceLabel.Name = "typingRateVarianceLabel";
            typingRateVarianceLabel.Size = new Size(190, 23);
            typingRateVarianceLabel.TabIndex = 2;
            typingRateVarianceLabel.Text = "Typing Rate Variance: ±0";
            typingRateVarianceLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // typingRateSlider
            // 
            typingRateSlider.BackColor = Color.DimGray;
            typingRateSlider.Location = new Point(0, 63);
            typingRateSlider.Margin = new Padding(0);
            typingRateSlider.Maximum = 350;
            typingRateSlider.Minimum = 10;
            typingRateSlider.Name = "typingRateSlider";
            typingRateSlider.Size = new Size(200, 45);
            typingRateSlider.TabIndex = 1;
            typingRateSlider.TickStyle = TickStyle.None;
            typingRateSlider.Value = 100;
            typingRateSlider.ValueChanged += UI_Slider_TypingRate;
            // 
            // typingRateSliderLabel
            // 
            typingRateSliderLabel.Font = new Font("Segoe UI", 10F);
            typingRateSliderLabel.ForeColor = Color.White;
            typingRateSliderLabel.Location = new Point(5, 40);
            typingRateSliderLabel.Name = "typingRateSliderLabel";
            typingRateSliderLabel.Size = new Size(190, 23);
            typingRateSliderLabel.TabIndex = 0;
            typingRateSliderLabel.Text = "Typing Rate: ~45";
            typingRateSliderLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // startButton
            // 
            startButton.BackColor = Color.DimGray;
            startButton.Enabled = false;
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            startButton.ForeColor = Color.White;
            startButton.Location = new Point(5, 5);
            startButton.Margin = new Padding(5);
            startButton.Name = "startButton";
            startButton.Size = new Size(190, 30);
            startButton.TabIndex = 0;
            startButton.Text = "Start Cheat";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += UI_Click_Start;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1300, 800);
            Controls.Add(webView);
            Controls.Add(controls);
            ForeColor = SystemColors.ControlText;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1300, 800);
            Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            controls.ResumeLayout(false);
            controls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)accuracyVarianceSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)accuracySlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)typingRateVarianceSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)typingRateSlider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel controls;
        private Button startButton;
        private TrackBar typingRateSlider;
        private Label typingRateSliderLabel;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private TrackBar typingRateVarianceSlider;
        private Label typingRateVarianceLabel;
        private TrackBar accuracySlider;
        private Label accuracySliderLabel;
        private TrackBar accuracyVarianceSlider;
        private Label accuracyVarianceLabel;
        private CheckBox autostart;
        private CheckBox autogame;
        private CheckBox usenitros;
        private CheckBox godmode;
        private Button discord;
    }
}
