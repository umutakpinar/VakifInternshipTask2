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

        public MainPageView()
        {
            InitializeComponent();
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            UIController controller = new UIController();
            controller.SelectPath(tbxInput);
            controller.RefreshScreen(dataGridView1, tbxInput);
        }
    }
}
