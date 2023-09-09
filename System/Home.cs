using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace System
{
    public partial class Home : Form
    {
        Dashboardfrm dashboard;
        Menufrm menufrm;
        laundrySide laundryfrm;
        ReservationSide reservationfrm;
        RestoInventory restoInventory;
        Room room;

        public Home()
        {
            InitializeComponent();
            mdiProp();
            Timer timer = new Timer();
            timer.Interval = 1000; 
            timer.Tick += timertime_Tick;
            timer.Start();

      
        }
        private void timertime_Tick(object sender, EventArgs e)
        {
           
        }
       


        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            dashboard = new Dashboardfrm();
            dashboard.FormClosed += Dashboard_FormClosed;
            dashboard.MdiParent = this;
            dashboard.Dock = DockStyle.Fill;
            dashboard.Show();

         
        }
        private Button lastClickedButton;
        private Color defaultButtonColor = Color.FromArgb(48, 46, 51);
        private void button1_Click(object sender, EventArgs e)
        {

            if (lastClickedButton != null)
            {

                lastClickedButton.BackColor = defaultButtonColor; 
            }

            Button clickedButton = (Button)sender; 


            clickedButton.BackColor = Color.FromArgb(45, 61, 52); 


            lastClickedButton = clickedButton;
            if (dashboard == null)
            {
                dashboard = new Dashboardfrm();
                dashboard.FormClosed += Dashboard_FormClosed;
                dashboard.MdiParent = this;
                dashboard.Dock = DockStyle.Fill;
                dashboard.Show();
            }
            else
            {
                dashboard.Activate();
            }
            sidepanel1.Visible = false;
            sidepanel.Visible = true;
            sidepanel2.Visible = false;
            sidepanel3.Visible = false;

        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            dashboard = null;
        }
        private void RestoInventory_FormClosed(object sender, FormClosedEventArgs e)
        {
            restoInventory = null;
        }
        private void laundrySide_FormClosed(object sender, FormClosedEventArgs e)
        {
            laundryfrm = null;
        }

        bool menuExpand = false;
        private void mdiProp()
        {
            this.SetBevel(false);
            Controls.OfType<MdiClient>().FirstOrDefault().BackColor = Color.FromArgb(171, 171, 171);
        }
        

        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            menufrm = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
        bool sidebarExpand = true;
        private void sidebarTransition_Tick(object sender, EventArgs e)
        {
            if(sidebarExpand) {
                sidebar.Width -= 10;
                if (sidebar.Width <= 76)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();
                    pnDashboard.Width = sidebar.Width;
             
                    pn3.Width = sidebar.Width;
                    pn4.Width = sidebar.Width;
         
                }

            }
            else
            {
                sidebar.Width += 10;
                if (sidebar.Width >= 239)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();

                    pnDashboard.Width = sidebar.Width;

                    pn3.Width = sidebar.Width;
                    pn4.Width = sidebar.Width;

                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
            {
             
                lastClickedButton.BackColor = defaultButtonColor; 
            }

            Button clickedButton = (Button)sender;
            clickedButton.BackColor = Color.FromArgb(45, 61, 52);

      
            lastClickedButton = clickedButton;
            if (restoInventory == null)
            {
                restoInventory = new RestoInventory();
                restoInventory.FormClosed += RestoInventory_FormClosed;
                restoInventory.MdiParent = this;
                restoInventory.Dock = DockStyle.Fill;
                restoInventory.Show();
            }
            else
            {
                restoInventory.Activate();
            }
            sidepanel1.Visible = false;
            sidepanel.Visible = false;
            sidepanel2.Visible = true;
            sidepanel3.Visible = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void sidebar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
            {
           
                lastClickedButton.BackColor = defaultButtonColor; 
            }

            Button clickedButton = (Button)sender;
            clickedButton.BackColor = Color.FromArgb(45, 61, 52); 

           
            lastClickedButton = clickedButton;
            if (reservationfrm == null)
            {
                reservationfrm = new ReservationSide();
                reservationfrm.FormClosed += laundrySide_FormClosed;
                reservationfrm.MdiParent = this;
                reservationfrm.Dock = DockStyle.Fill;
                reservationfrm.Show();
            }
            else
            {
                reservationfrm.Activate();
            }
            sidepanel1.Visible = true;
            sidepanel.Visible = false;
            sidepanel2.Visible = false;
            sidepanel3.Visible = false;

        }

        private void laundrybtn_Click(object sender, EventArgs e)
        {
            
            if (laundryfrm == null)
            {
                laundryfrm = new laundrySide();
                laundryfrm.FormClosed += laundrySide_FormClosed;
                laundryfrm.MdiParent = this;
                laundryfrm.Dock = DockStyle.Fill;
                laundryfrm.Show();
            }
            else
            {
                laundryfrm.Activate();
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
            {
              
                lastClickedButton.BackColor = defaultButtonColor;
            }

            Button clickedButton = (Button)sender;
            clickedButton.BackColor = Color.FromArgb(45, 61, 52); 

         
            lastClickedButton = clickedButton;
            if (room == null)
            {
               room = new Room();
                room.FormClosed += Dashboard_FormClosed;
            room.MdiParent = this;
                room.Dock = DockStyle.Fill;
                room.Show();
            }
            else
            {
               room.Activate();
            }
            sidepanel1.Visible = false;
            sidepanel.Visible = false;
            sidepanel2.Visible = false;
            sidepanel3.Visible = true;
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2CircleButton3_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login newform = new Login();    
            newform.ShowDialog();
            
        }
    }
}
