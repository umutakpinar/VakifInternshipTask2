using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VakifInternship_2.controller;
using System.Drawing.Drawing2D;

namespace VakifInternship_2.view
{

    public partial class MainPageView : Form
    {
        UIController _controller;
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
        public MainPageView()
        {
            InitializeComponent();
        }

        private async void btnSelectPath_Click(object sender, EventArgs e)
        {
            if (lblProcessInfo.Text == "LOADING" || lblProcessInfo.Text == "PROCESSING")
            {
                Application.Restart();
                Environment.Exit(1);
            }
            else
            {
                _controller = new UIController(tbxLog, progressBar, lblProcessInfo);
                _controller.SelectPath(tbxInput);
                if (!string.IsNullOrEmpty(tbxInput.Text))
                {
                    _controller.RefreshScreen(dataGridView1, tbxInput, lblProcessInfo);
                }
            }
           
        }

        private void lblProcessInfo_TextChanged(object sender, EventArgs e)
        {
            if (lblProcessInfo.Text == "LOADING" || lblProcessInfo.Text == "PROCESSING")
            {
                btnSelectPath.Text = "CANCEL";
                btnSelectPath.BackColor = Color.Red;
            }
            else
            {
                btnSelectPath.Text = "Select Path";
                btnSelectPath.BackColor = Color.FromArgb(255,255,128,0);
                if (lblProcessInfo.Text == "COMPLETED")
                {
                    FlashWindow(this.Handle, true);
                    dataGridView1.Columns.Remove("IsDynamicSP");
                    dataGridView1.Columns.Remove("HasVarChar2");
                }
            }
        }
    }
}
