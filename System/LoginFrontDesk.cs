using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public partial class LoginFrontDesk : Form
    {
        public LoginFrontDesk()
        {
            InitializeComponent();
        }
        private string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;";

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Login newForm = new Login();
            newForm.Show();
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void loginbtn_Click(object sender, EventArgs e)
        {

            string username = usertxt.Text;
            string password = passtxt.Text;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM loginpos WHERE user = @username AND pass = @password";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int userCount = Convert.ToInt32(command.ExecuteScalar());
               

                    if (userCount > 0)
                    {
                        SystemSounds.Exclamation.Play();

                        // Show success alert
                        alertSuccess successAlert = new alertSuccess();
                        successAlert.ShowDialog();


                        // Create a timer for the delay
                        Timer delayTimer = new Timer();
                        delayTimer.Interval = 200; // 3000 milliseconds (3 seconds)
                        delayTimer.Tick += (timerSender, timerEventArgs) =>
                        {
                            delayTimer.Stop(); // Stop the timer

                            // Show the Home form
                           reservation newForm = new reservation();
                            newForm.Show();
                            this.Hide();
                            // Close the current form


                            // Add your redirection logic here if needed
                        };

                        // Start the timer to introduce the delay
                        delayTimer.Start();
                    }
                    else
                    {
                        SystemSounds.Exclamation.Play();
                        // Show failure alert
                        loginfail newForm = new loginfail();
                        newForm.Show();

                    }




                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void LoginFrontDesk_Load(object sender, EventArgs e)
        {
           
        }
    }
    }

