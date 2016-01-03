using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace JSGameIDE_Player
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser Browser;
        public Panel browserPanel;
        public Form1()
        {
            InitializeComponent();
            browserPanel = new Panel();
            browserPanel.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(browserPanel);
            Init();
        }

        public void Init()
        {
            Cef.Initialize(new CefSettings());
            Browser = new ChromiumWebBrowser("www.google.com");
            Browser.Dock = DockStyle.Fill;
            Browser.LoadingStateChanged += Browser_LoadingStateChanged;
            Browser.TitleChanged += Browser_TitleChanged;
            browserPanel.Controls.Add(Browser);
            this.Icon = Icon.ExtractAssociatedIcon(Application.StartupPath + @"\icon.ico");
        }

        private void Browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            SetTitle(e.Title);
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Browser.EvaluateScriptAsync("document.getElementsByTagName('body')[0].style.overflow = 'hidden'");
            ToggleBrowser(!e.IsLoading);
            if (e.IsLoading)
                SetTitle("Loading...");
        }

        public void SetTitle(string str)
        {
            if(this.InvokeRequired)
                this.Invoke(new MethodInvoker(() => { SetTitle(str); }));
            else
            {
                this.Text = str;
            }
        }

        public void ToggleBrowser(bool Visible)
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(() => { ToggleBrowser(Visible); }));
            else
            {
                if(Visible)
                    browserPanel.Show();
                else
                    browserPanel.Hide();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        public void GoFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

    }
}
