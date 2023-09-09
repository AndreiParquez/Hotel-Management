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
    public partial class alertSuccess : Form
    {
        private Timer timer;
        public alertSuccess()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 2000; 
            timer.Tick += alerttimer_Tick;
        }

        private void alert_Load(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void alerttimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Close();
        }
    }
}
