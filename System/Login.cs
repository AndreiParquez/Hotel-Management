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
using MySql.Data.MySqlClient;

namespace System
{
    public partial class Login : Form
    {
        private string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;";

        public Login()
        {
            InitializeComponent();
            PopulateAutocompleteSources();
        }

        private void PopulateAutocompleteSources()
        {

            AutoCompleteStringCollection passwords = new AutoCompleteStringCollection();
            passwords.AddRange(GetPasswordValuesFromDatabase()); 
            passtxt.AutoCompleteCustomSource = passwords;
        }




        private string[] GetPasswordValuesFromDatabase()
        {
            string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;";
            string query = "SELECT pass FROM rempass"; 
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        var passwordList = new List<string>();
                        while (reader.Read())
                        {
                            string password = reader["pass"].ToString();
                            passwordList.Add(password);
                        }
                        return passwordList.ToArray();
                    }
                }
            }
        }





       
        private void Login_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

    
        private void loginbtn_Click(object sender, EventArgs e)
        {

            string username = usertxt.Text;
            string password = passtxt.Text;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM login WHERE user = @username AND pass = @password";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int userCount = Convert.ToInt32(command.ExecuteScalar());
                     if (guna2CheckBox1.Checked)
            {
         

                InsertPasswordToMySQL(password);
                
               
            }

                    if (userCount > 0)
                    {
                        SystemSounds.Exclamation.Play();

                       
                        alertSuccess successAlert = new alertSuccess();
                        successAlert.ShowDialog();
                       

                     
                        Timer delayTimer = new Timer();
                        delayTimer.Interval = 200; 
                        delayTimer.Tick += (timerSender, timerEventArgs) =>
                        {
                            delayTimer.Stop(); 

                            
                            Home newForm = new Home();
                            newForm.Show();
                            this.Hide();
                           


                           
                        };

                       
                        delayTimer.Start();
                    }
                    else
                    {
                        SystemSounds.Exclamation.Play();
                       
                        loginfail newForm = new loginfail();
                        newForm.Show();

                    }




                }
            }

        }
        private void InsertPasswordToMySQL(string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO rempass (pass) VALUES (@password)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@password", password);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

         
           Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2TileButton2_Click(object sender, EventArgs e)
        {
            passtxt.Text = "";
            usertxt.Text = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {
           LoginFrontDesk newForm = new LoginFrontDesk();
            newForm.Show();
            this.Hide();
        
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void passtxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void usertxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

            AdminCreate newForm = new AdminCreate();
            newForm.Show();
            this.Hide();
        }
    }
}
