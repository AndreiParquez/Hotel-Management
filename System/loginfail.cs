using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public partial class loginfail : Form
    {
        private Timer timer;
        public loginfail()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 2000; // 3000 milliseconds (3 seconds)
            timer.Tick += alerttimer_Tick;
        }
        private void alerttimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Close();
        }
      
        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void loginfail_Load_1(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
