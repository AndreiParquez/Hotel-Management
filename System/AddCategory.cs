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
    public partial class AddCategory : Form
    {
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; 

        public AddCategory()
        {
            InitializeComponent();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            string categoryName = catname.Text;
            string description = destxt.Text;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                
                    string insertQuery = $"INSERT INTO Category (Name, Description) VALUES ('{categoryName}', '{description}')";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Category inserted successfully.");
                     

                    }
                    else
                    {
                        MessageBox.Show("Category insertion failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
       
        private void AddCategory_Load(object sender, EventArgs e)
        {
            timer1.Start();
            AddItem restoInventoryForm = new AddItem();
            restoInventoryForm.PopulateCategoryComboBox(); ;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }
    }
}
