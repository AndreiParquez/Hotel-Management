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
    public partial class Room : Form
    {

        private MySqlConnection connection;
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; 
        private DataTable originalDataTable;
        private DataView dataView;
        public Room()
        {
            InitializeComponent();
            PopulateDataGridView();
            load1();
        }
        public void PopulateDataGridView()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT * from Room";

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
            dataGridView1.DataSource = dataView;

        }
        private void load1()
        {
            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string availableRoomsQuery = "SELECT COUNT(*) FROM room WHERE Status = 'available'";
                    string occuRoomsQuery = "SELECT COUNT(*) FROM room WHERE Status = 'occupied'";
                    string totalRoomsQuery = "SELECT COUNT(*) FROM room";
                    using (MySqlCommand availableRoomsCommand = new MySqlCommand(availableRoomsQuery, connection))
                    using (MySqlCommand totalRoomsCommand = new MySqlCommand(totalRoomsQuery, connection))
                    using (MySqlCommand occuRoomsQueryCommand = new MySqlCommand(occuRoomsQuery, connection))
                    {
                        int availableRoomCount = Convert.ToInt32(availableRoomsCommand.ExecuteScalar());
                        int totalRoomCount = Convert.ToInt32(totalRoomsCommand.ExecuteScalar());
                        int occroom = Convert.ToInt32(occuRoomsQueryCommand.ExecuteScalar());

                        lbl1.Text = totalRoomCount.ToString();
                        av.Text = availableRoomCount.ToString();
                        oc.Text = occroom.ToString();

                    }

                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string availableRoomsQuery = "SELECT COUNT(*) FROM room WHERE Status = 'available'";
                    string occuRoomsQuery = "SELECT COUNT(*) FROM room WHERE Status = 'occupied'";
                    string totalRoomsQuery = "SELECT COUNT(*) FROM room";
                    using (MySqlCommand availableRoomsCommand = new MySqlCommand(availableRoomsQuery, connection))
                    using (MySqlCommand totalRoomsCommand = new MySqlCommand(totalRoomsQuery, connection))
                    using (MySqlCommand occuRoomsQueryCommand = new MySqlCommand(occuRoomsQuery, connection))
                    {
                        int availableRoomCount = Convert.ToInt32(availableRoomsCommand.ExecuteScalar());
                        int totalRoomCount = Convert.ToInt32(totalRoomsCommand.ExecuteScalar());
                        int occroom = Convert.ToInt32(occuRoomsQueryCommand.ExecuteScalar());

                        lbl1.Text =  totalRoomCount.ToString();
                        av.Text = availableRoomCount.ToString();
                        oc.Text = occroom.ToString();

                    }

                    string roomNumber = roomtxt.Text;
                    string roomType = cb.SelectedItem.ToString();
                    string status = "available"; 

                    string insertQuery = "INSERT INTO room (RoomNo, Roomtype, Status) VALUES (@RoomNo, @Roomtype, @Status)";

                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                    cmd.Parameters.AddWithValue("@RoomNo", roomNumber);
                    cmd.Parameters.AddWithValue("@Roomtype", roomType);
                    cmd.Parameters.AddWithValue("@Status", status);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Room created successfully.");
                        PopulateDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create room.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Room_Load(object sender, EventArgs e)
        {
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellPainting += dataGridView1_CellPainting;

        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
    }
}
