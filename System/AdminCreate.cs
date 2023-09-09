using MySql.Data.MySqlClient;
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
    public partial class AdminCreate : Form
    {
        private string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;";
        public AdminCreate()
        {
            InitializeComponent();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2TileButton2_Click(object sender, EventArgs e)
        {
            Login newForm = new Login();
            newForm.Show();
            this.Hide();
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            string adminUsername = adminUserTxt.Text;
            string adminPassword =pass.Text;
            string confirmAdminPassword = pass2.Text;

            if (adminPassword != confirmAdminPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

               
                string checkAdminQuery = "SELECT COUNT(*) FROM login WHERE user = @adminUsername";
                using (MySqlCommand checkAdminCommand = new MySqlCommand(checkAdminQuery, connection))
                {
                    checkAdminCommand.Parameters.AddWithValue("@adminUsername", adminUsername);
                    int adminCount = Convert.ToInt32(checkAdminCommand.ExecuteScalar());

                    if (adminCount > 0)
                    {
                        MessageBox.Show("An admin with the given username already exists.", "Admin Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                
                string insertAdminQuery = "INSERT INTO login (user, pass) VALUES (@adminUsername, @adminPassword)";
                using (MySqlCommand insertAdminCommand = new MySqlCommand(insertAdminQuery, connection))
                {
                    insertAdminCommand.Parameters.AddWithValue("@adminUsername", adminUsername);
                    insertAdminCommand.Parameters.AddWithValue("@adminPassword", adminPassword);
                    insertAdminCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Admin account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                adminUserTxt.Text = "";
               pass.Text = "";
              pass2.Text = "";
            }
        }
    }
}
