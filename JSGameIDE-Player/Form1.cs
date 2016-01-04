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
using System.IO;

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
            string gamePath = Application.StartupPath + @"\Resources\index.html";
            if (File.Exists(gamePath))
            {
                string data = File.ReadAllText(gamePath);
                int canvasPos = data.IndexOf("canvas id='gameCanvas'");
                if (canvasPos != -1)
                {
                    try
                    {
                        int wIndex = data.IndexOf("width='", canvasPos) + 7;
                        int width = int.Parse(data.Substring(wIndex, data.IndexOf("'", wIndex) - wIndex));
                        int hIndex = data.IndexOf("height='", canvasPos) + 8;
                        int height = int.Parse(data.Substring(hIndex, data.IndexOf("'", hIndex) - hIndex));
                        int wsIndex = data.IndexOf("windowStyle='", canvasPos) + 13;
                        this.Size = new Size(width, height);
                        string windowStyle = data.Substring(wsIndex, data.IndexOf("'", wsIndex) - wsIndex);
                        switch (windowStyle)
                        {
                            case "fixed":
                                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                this.MaximizeBox = false;
                                break;
                            case "fullscreen":
                                GoFullscreen(true);
                                break;
                            default:
                                this.MaximizeBox = true;
                                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                                break;
                        }
                    }
                    catch { }
                }
            }
            Init();
        }

        public void Init()
        {
            Cef.Initialize(new CefSettings());
            Browser = new ChromiumWebBrowser(Application.StartupPath + @"\Resources\index.html");
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
            Browser.SetFocus(true);
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
