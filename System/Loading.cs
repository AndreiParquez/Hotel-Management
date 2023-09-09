using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace System
{
    public partial class Loading : Form
    {
        private Login mainForm;
        public Loading()
        {
            InitializeComponent();
        }
        

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public Loading(Login mainForm) : this()
        {
            this.mainForm = mainForm;
        }
        private void Loading_Load(object sender, EventArgs e)
        {
            Thread loadingThread = new Thread(new ThreadStart(PerformLoading));
            loadingThread.Start();
        }

        private void PerformLoading()
        {
            for (int i = 0; i <= 100; i++)
            {
                UpdateProgressBar(i);
                UpdatePercentageLabel(i);
                Thread.Sleep(50); // Simulate loading process
            }

            // Loading is done, close the loading form and show the main form (Login)
            this.Invoke(new Action(() =>
            {
             ;
             Login mainForm = new Login();  
                mainForm.Show(); // Show the Login form
                this.Hide();    // Close the loading form
            }));
        }

        private void UpdateProgressBar(int value)
        {
            if (guna2ProgressBar1.InvokeRequired)
            {
                guna2ProgressBar1.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                guna2ProgressBar1.Value = value;
            }
        }
        private void UpdatePercentageLabel(int percentage)
        {
            if (lblPercentage.InvokeRequired)
            {
                lblPercentage.Invoke(new Action<int>(UpdatePercentageLabel), percentage);
            }
            else
            {
                lblPercentage.Text = $"Loading... {percentage}%";
            }
        }

    }
}
