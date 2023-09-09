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
    public partial class RestoInventory : Form
    {
        private MySqlConnection connection;
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; 
        private DataTable originalDataTable;
        private DataView dataView;
 
        public RestoInventory()
        {
            InitializeComponent();
           
     


        }
        private void InitializeGridView()
        {
 

            DataGridViewImageColumn editButtonColumn = new DataGridViewImageColumn();
            editButtonColumn.Name = "EditButtonColumn";
            editButtonColumn.HeaderText = "Edit";
            editButtonColumn.Image = Properties.Resources.pen;
            dataGridView1.Columns.Add(editButtonColumn);

            DataGridViewImageColumn deleteButtonColumn = new DataGridViewImageColumn();
            deleteButtonColumn.Name = "DeleteButtonColumn";
            deleteButtonColumn.HeaderText = "Delete";
            deleteButtonColumn.Image = Properties.Resources.bin; 
            dataGridView1.Columns.Add(deleteButtonColumn);
        }
        private void UpdateTotalItemsLabel()
        {
            int totalItems = dataGridView1.Rows.Count;
            totallbl.Text = $"{totalItems}";
        }



        private void RestoInventory_Load(object sender, EventArgs e)
        {
           
            PopulateDataGridView();
            InitializeGridView();
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellPainting += dataGridView1_CellPainting;
    
            UpdateTotalItemsLabel();



        }
        public DataGridView InventoryGridView
        {
            get { return dataGridView1; }
        }

        DataTable dataTable = new DataTable(); 
        public void PopulateDataGridView()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT products.productID AS ItemID, products.Name, category.Name AS CategoryName, products.Quantity, products.Price AS Cost
                        FROM Products
                        INNER JOIN Category ON products.CategoryID = category.CategoryID";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                originalDataTable = new DataTable();
                adapter.Fill(originalDataTable);
                string countQuery = "SELECT COUNT(*) FROM Category";
                using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
                {
                    int totalCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    label6.Text = totalCount.ToString();
                }
                double totalCost = 0.0;
                foreach (DataRow row in originalDataTable.Rows)
                {
                    totalCost += Convert.ToDouble(row["Cost"]);
                    label8.Text = totalCost.ToString(); 
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            

            dataView = originalDataTable.DefaultView;
            dataGridView1.DataSource = dataView;
            UpdateTotalItemsLabel();

        }
        
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string nameSearchValue = searchtxt.Text.Trim();
 

                DataView dataView = originalDataTable.DefaultView;

                if (!string.IsNullOrEmpty(nameSearchValue))
                {
                    dataView.RowFilter = BuildRowFilter(nameSearchValue);
                }
                else 
                {
                    dataView.RowFilter = "";
                }

                dataGridView1.DataSource = dataView;


                UpdateTotalItemsLabel();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private string BuildRowFilter(string searchTerm)
        {
     
            string filterExpression = "";
            foreach (DataColumn column in originalDataTable.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    if (!string.IsNullOrEmpty(filterExpression))
                    {
                        filterExpression += " OR ";
                    }
                    filterExpression += $"[{column.ColumnName}] LIKE '%{searchTerm}%'";
                }
                else if (IsNumericColumn(column))
                {
                    if (!string.IsNullOrEmpty(filterExpression))
                    {
                        filterExpression += " OR ";
                    }
                    filterExpression += $"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{searchTerm}%'";
                }
            }

            return filterExpression;
        }

        private bool IsNumericColumn(DataColumn column)
        {
            Type dataType = column.DataType;
            return dataType == typeof(int) || dataType == typeof(decimal) || dataType == typeof(double);
        }

        private void searchtxt_Enter(object sender, EventArgs e)
        {

        }

        private void searchtxt_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void searchtxt_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                if (e.ColumnIndex == dataGridView1.Columns["EditButtonColumn"].Index)
                {
                    EditProduct(e.RowIndex);
                }

                else if (e.ColumnIndex == dataGridView1.Columns["DeleteButtonColumn"].Index)
                {
                    DeleteProduct(e.RowIndex);
                }
            }

        }
        private void EditProduct(int rowIndex)
        {
 


            DataGridViewRow selectedRow = dataGridView1.Rows[rowIndex];

       
            string productId = selectedRow.Cells["ItemID"].Value.ToString();
            string productName = selectedRow.Cells["Name"].Value.ToString();
            string categoryName = selectedRow.Cells["categoryName"].Value.ToString();
            int quantity = Convert.ToInt32(selectedRow.Cells["Quantity"].Value);
            decimal price = Convert.ToDecimal(selectedRow.Cells["Cost"].Value);

   
            EditProductForm editForm = new EditProductForm();


            editForm.ProductUpdated += EditForm_ProductUpdated;

            editForm.SetProductData(productId, productName, categoryName, quantity, price);

    
            editForm.ShowDialog();
        }
        private void EditForm_ProductUpdated()
        {
          
            PopulateDataGridView();
        }
        private string productIdToDelete;

        public void GetProductIdToDelete(string productId)
        {
            productIdToDelete = productId;
        }
        private void DeleteProduct(int rowIndex)
        {
            string productId = GetProductToDelete(rowIndex);

            delete deleteConfirmationForm = new delete(productId);
            deleteConfirmationForm.ProductDeletedConfirmed += DeleteConfirmationForm_ProductDeletedConfirmed;
            deleteConfirmationForm.ShowDialog();

        }
        private string GetProductToDelete(int rowIndex)
        {
            DataGridViewRow selectedRow = dataGridView1.Rows[rowIndex];
            string productId = selectedRow.Cells["ItemID"].Value.ToString();
            return productId;
        }
        private void DeleteConfirmationForm_ProductDeletedConfirmed()
        {
   
            PopulateDataGridView();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            AddItem addProduct = new AddItem();
           

            addProduct.ProductUpdated += EditForm_ProductUpdated;
            addProduct.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AddCategory addCategory = new AddCategory();    
            addCategory.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void totallbl_Click(object sender, EventArgs e)
        {

        }
    }
}
