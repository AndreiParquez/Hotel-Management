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
    public partial class AddItem : Form
    {
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; 
        public event Action ProductUpdated;
        private MySqlConnection connection;
        private DataTable originalDataTable;
        private DataView dataView;
        
        public AddItem()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            if (int.TryParse(quan.Text, out int quantity))
            {
                quantity++;
                quan.Text = quantity.ToString();
               
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            
                if (int.TryParse(quan.Text, out int quantity) && quantity > 0)
                {
                    quantity--;
                    quan.Text = quantity.ToString();
                }
            
        }

        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddProduct_Load(object sender, EventArgs e)
        {
            PopulateCategoryComboBox();
            RestoInventory restoInventoryForm = new RestoInventory();
            restoInventoryForm.PopulateDataGridView(); 
            DataGridView gridView1 = restoInventoryForm.InventoryGridView;
            timer1.Start();
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
        public void AddForm_CategoryUpdated()
        {
           
            PopulateCategoryComboBox();
        }

        
        public class CategoryItem
        {
            public int CategoryID { get; set; }
            public string Name { get; set; }

            public CategoryItem(int categoryId, string name)
            {
                CategoryID = categoryId;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
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


                    string insertQuery = $"INSERT INTO Products (ProductID, Name, Quantity, Price, CategoryID) VALUES ({productId}, '{productName}', {quantity}, {price}, {categoryId})";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product inserted successfully.");
                        PopulateDataGridView();
                        ProductUpdated?.Invoke();
                    }
                    else
                    {
                        MessageBox.Show("Product insertion failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
