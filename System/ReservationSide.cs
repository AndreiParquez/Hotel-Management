using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
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
    public partial class ReservationSide : Form
    {
        private MySqlConnection connection;
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; // Replace with your actual connection string
        private DataTable originalDataTable;
        private DataView dataView;

        public ReservationSide()
        {
            InitializeComponent();
        }

        private void ReservationSide_Load(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string countQuery = "SELECT COUNT(*) FROM reservation";
                using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
                {
                    int totalCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    totalres.Text = totalCount.ToString();
                }
                string sumTotalCostQuery = "SELECT SUM(TotalCost) FROM reservation";
                using (MySqlCommand sumTotalCostCommand = new MySqlCommand(sumTotalCostQuery, connection))
                {
                    object sumTotalCostResult = sumTotalCostCommand.ExecuteScalar();
                    decimal sumTotalCost = sumTotalCostResult != DBNull.Value ? Convert.ToDecimal(sumTotalCostResult) : 0;

                    rev.Text = sumTotalCost.ToString("C");
                }

                string query = "SELECT r.*, rm.RoomType " +
                               "FROM reservation r " +
                               "JOIN room rm ON r.RoomNo = rm.RoomNo"; // Join reservation and room tables
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                originalDataTable = new DataTable();
                adapter.Fill(originalDataTable);

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string nameSearchValue = searchtxt.Text.Trim();


                DataView dataView = originalDataTable.DefaultView;

                // Apply name filter
                if (!string.IsNullOrEmpty(nameSearchValue))
                {
                    dataView.RowFilter = BuildRowFilter(nameSearchValue);
                }
                else
                {
                    dataView.RowFilter = "";
                }

                dataGridView1.DataSource = dataView;



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        private string BuildRowFilter(string searchTerm)
        {
            // Build a filter expression for each column
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

    }
}
