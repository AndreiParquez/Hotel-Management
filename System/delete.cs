using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public partial class delete : Form

    {
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; // Replace with your actual connection string

        public event Action ProductDeletedConfirmed;
        private string productIdToDelete;

        public delete(string productId)
        {
            InitializeComponent();
            productIdToDelete = productId;
        }
        public void GetProductIdToDelete(string productId)
        {
            string productIdToDelete = productId;
           


        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Delete the product with the specified productID
                    string deleteQuery = $"DELETE FROM Products WHERE productID = {productIdToDelete}";
                    MySqlCommand cmd = new MySqlCommand(deleteQuery, connection);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product deleted successfully.");
                        ProductDeletedConfirmed?.Invoke(); // Notify other parts of the application
                        this.Close(); // Close the DeleteConfirmationForm
                    }
                    else
                    {
                        MessageBox.Show("Product deletion failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
