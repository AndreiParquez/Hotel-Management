using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.AddItem;

namespace System
{
    public partial class EditProductForm : Form
    {
        public event Action ProductUpdated;
        private MySqlConnection connection;
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;";
        private DataTable originalDataTable;
        private DataView dataView;
        public EditProductForm()
        {
            InitializeComponent();


        }
        public void PopulateCategoryComboBox()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string categoryQuery = "SELECT CategoryID, Name FROM Category";
                    MySqlCommand cmd = new MySqlCommand(categoryQuery, connection);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int categoryId = reader.GetInt32("CategoryID");
                            string categoryName = reader.GetString("Name");
                            cb.Items.Add(new CategoryItem(categoryId, categoryName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void EditProductForm_Load(object sender, EventArgs e)
        {
            PopulateCategoryComboBox();
            RestoInventory restoInventoryForm = new RestoInventory();
            restoInventoryForm.PopulateDataGridView(); 
            DataGridView gridView1 = restoInventoryForm.InventoryGridView;
            timer1.Start();
           
        }
        public void SetProductData(string productId, string productName, string categoryName, int quantity, decimal price)
        {
            pid.Text = productId;
            productNameTextBox.Text = productName;
            cb.Text = categoryName;
            quan.Text = quantity.ToString();
            pr.Text = price.ToString();


        }
        private void PopulateDataGridView()
        {
            RestoInventory restoInventoryForm = new RestoInventory();
            restoInventoryForm.PopulateDataGridView(); 
            DataGridView gridView1 = restoInventoryForm.InventoryGridView;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT products.productID, products.Name, category.Name AS categoryName, products.Quantity, products.Price
                        FROM Products
                        INNER JOIN Category ON products.CategoryID = category.CategoryID";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                originalDataTable = new DataTable();
                adapter.Fill(originalDataTable);

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


            dataView = originalDataTable.DefaultView;
            gridView1.DataSource = dataView;
            DataGridViewImageColumn editButtonColumn = new DataGridViewImageColumn();
            editButtonColumn.Name = "EditButtonColumn";
            editButtonColumn.HeaderText = "Edit";
            editButtonColumn.Image = Properties.Resources.pen;
            gridView1.Columns.Add(editButtonColumn);

            DataGridViewImageColumn deleteButtonColumn = new DataGridViewImageColumn();
            deleteButtonColumn.Name = "DeleteButtonColumn";
            deleteButtonColumn.HeaderText = "Delete";
            deleteButtonColumn.Image = Properties.Resources.bin; 
            gridView1.Columns.Add(deleteButtonColumn);
        }


        private void close_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void quan_TextChanged(object sender, EventArgs e)
        {

        }
     


        private void loginbtn_Click(object sender, EventArgs e)
        {
            int productId = int.Parse(pid.Text);
            string productName = productNameTextBox.Text;
            string categoryName = cb.Text;
            int quantity = int.Parse(quan.Text);
            decimal price = decimal.Parse(pr.Text);

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                   
                    string categoryIdQuery = $"SELECT CategoryID FROM Category WHERE Name = '{categoryName}'";
                    MySqlCommand categoryIdCmd = new MySqlCommand(categoryIdQuery, connection);
                    int categoryId = Convert.ToInt32(categoryIdCmd.ExecuteScalar());

                    
                    string updateQuery = $"UPDATE Products SET Name = '{productName}', Quantity = {quantity}, Price = {price}, CategoryID = {categoryId} WHERE productID = {productId}";

                    MySqlCommand cmd = new MySqlCommand(updateQuery, connection);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product updated successfully.");
                        PopulateDataGridView();
                        ProductUpdated?.Invoke();
                        Close();

                    }
                    else
                    {
                        MessageBox.Show("Product update failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            
        }
        private void btnIncrement_Click(object sender, EventArgs e)
        {
            if (int.TryParse(quan.Text, out int quantity))
            {
                quantity++;
                quan.Text = quantity.ToString();
            }
        }

        private void btnDecrement_Click(object sender, EventArgs e)
        {
            if (int.TryParse(quan.Text, out int quantity) && quantity > 0)
            {
                quantity--;
                quan.Text = quantity.ToString();
            }
        }
        private void guna2TileButton2_Click(object sender, EventArgs e)
        {
            
        }

        private void pid_TextChanged(object sender, EventArgs e)
        {

        }

        private void pr_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

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
