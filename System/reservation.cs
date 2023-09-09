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
using System.Drawing.Printing;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace System
{

    public partial class reservation : Form
    {
        private const decimal RatePerDay = 500;
        private string guestName;
        private string contactInfo;
        private string roomType;
        private int roomNo;
        private DateTime checkin;
        private DateTime checkout;
        private string paymentType;
        private int totalDays;
        private decimal totalCost;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; 
        private MySqlConnection connection;
       
        private DataTable originalDataTable;
        private DataView dataView;

        public reservation()
        {
           
            InitializeComponent();
            printDocument.PrintPage += printDocument1_PrintPage;
            printPreviewDialog.Document = printDocument;
            PopulateRoomNumbersComboBox();
            PopulateDataGridView();
            DataGridViewImageColumn editButtonColumn = new DataGridViewImageColumn();
            editButtonColumn.Name = "CheckOutColumn";
            editButtonColumn.HeaderText = "Check Out";
            editButtonColumn.Image = Properties.Resources.btn;
            dataGridView1.Columns.Add(editButtonColumn);
        }

        private void donebtn_Click(object sender, EventArgs e)
        {
            try
            {
                string guestName = nametxt.Text;
                string contactInfo = contacttxt.Text;
              
                int roomNo;
                if (!int.TryParse(roomtxt.Text, out roomNo))
                {
                    MessageBox.Show("Please enter a valid room number.");
                    return;
                }
                DateTime checkin = chekin.Value;
                DateTime checkout = chekout.Value;
                string paymentType = payment.Text;

                
                int totalDays = (int)(checkout - checkin).TotalDays;
                decimal RatePerDay = 500; 
                decimal totalCost = totalDays * RatePerDay;

                decimal paymentAmount;
                if (!decimal.TryParse(amount.Text, out paymentAmount))
                {
                    MessageBox.Show("Please enter a valid payment amount.");
                    return;
                }

                
                decimal change = paymentAmount - totalCost;

                using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    
                    string roomAvailabilityQuery = "SELECT Status FROM room WHERE RoomNo = @RoomNo";
                    using (MySqlCommand availabilityCommand = new MySqlCommand(roomAvailabilityQuery, sqlConnection))
                    {
                        availabilityCommand.Parameters.AddWithValue("@RoomNo", roomNo);
                        string roomStatus = availabilityCommand.ExecuteScalar()?.ToString();

                        if (roomStatus == "available")
                        {
                            string inventoryCheckQuery = "SELECT Quantity FROM products WHERE productID IN (1, 2, 3, 4)";
                            using (MySqlCommand inventoryCheckCommand = new MySqlCommand(inventoryCheckQuery, sqlConnection))
                            {
                                using (MySqlDataReader reader = inventoryCheckCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int inventoryQuantity = Convert.ToInt32(reader["Quantity"]);
                                        if (inventoryQuantity < 1)
                                        {
                                            FailRev newform = new FailRev();
                                            newform.Show();
                                            return;
                                        }
                                    }
                                }
                            }
                           
                            string reservationQuery = "INSERT INTO reservation (guestName, contactInfo, RoomNo, checkin, checkout, paymentType, TotalCost) " +
                                                      "VALUES (@guestName, @contactInfo, @RoomNo, @checkin, @checkout, @paymentType, @TotalCost)";
                            using (MySqlCommand sqlCommand = new MySqlCommand(reservationQuery, sqlConnection))
                            {
                                sqlCommand.Parameters.AddWithValue("@guestName", guestName);
                                sqlCommand.Parameters.AddWithValue("@contactInfo", contactInfo);
                           
                                sqlCommand.Parameters.AddWithValue("@RoomNo", roomNo);
                                sqlCommand.Parameters.AddWithValue("@checkin", checkin);
                                sqlCommand.Parameters.AddWithValue("@checkout", checkout);
                                sqlCommand.Parameters.AddWithValue("@paymentType", paymentType);
                                sqlCommand.Parameters.AddWithValue("@TotalCost", totalCost);

                                int rowsAffected = sqlCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    
                                    string updateStatusQuery = "UPDATE room SET Status = 'occupied' WHERE RoomNo = @RoomNo";
                                    using (MySqlCommand updateStatusCommand = new MySqlCommand(updateStatusQuery, sqlConnection))
                                    {
                                        updateStatusCommand.Parameters.AddWithValue("@RoomNo", roomNo);
                                        updateStatusCommand.ExecuteNonQuery();
                                    }
                                    List<int> specificItemIDs = new List<int> { 1, 2, 3,4 };
                                    string deductInventoryQuery = "UPDATE products SET Quantity = Quantity - 1 WHERE productID IN (" +
                                                                  string.Join(",", specificItemIDs) + ")";
                                    using (MySqlCommand deductInventoryCommand = new MySqlCommand(deductInventoryQuery, sqlConnection))
                                    {
                                        deductInventoryCommand.ExecuteNonQuery();
                                    }
                                   Succesrev newform = new Succesrev();
                                    newform.Show();
                                    amounttxt.Text = change.ToString("C");
                                    PopulateDataGridView();
                                }
                                else
                                {
                                    FailRev newform = new FailRev();
                                    newform.Show();
                                }
                            }
                        }
                        else
                        {
                            FailRev newform = new FailRev();
                            newform.Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
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


        private void PopulateRoomNumbersComboBox()
        {
            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string roomNumberQuery = "SELECT DISTINCT RoomNo FROM room";
                    MySqlCommand roomNumberCmd = new MySqlCommand(roomNumberQuery, connection);
                    MySqlDataReader roomNumberReader = roomNumberCmd.ExecuteReader();

                    roomtxt.Items.Clear(); 

                    while (roomNumberReader.Read())
                    {
                        roomtxt.Items.Add(roomNumberReader["RoomNo"]);
                    }
                    roomNumberReader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void generate_Click(object sender, EventArgs e)
        {
            decimal paymentAmount = decimal.Parse(amount.Text);
            checkin = chekin.Value;
            checkout = chekout.Value;
            totalDays = (int)(checkout - checkin).TotalDays;
            totalCost = totalDays * RatePerDay;
            guestlbl.Text = nametxt.Text;

            roomnlbl.Text = roomtxt.Text;  
            checkinlbl.Text = chekin.Text;
            checkoutlbl.Text = chekout.Text;
            amountlbl.Text = "₱" + amount.Text;
            totalcostlbl.Text = totalCost.ToString("C");
            decimal change = paymentAmount - totalCost;
            amounttxt.Text = change.ToString("C");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            printPreviewDialog.ShowDialog();
        }
        private string hotelName = "Hotel Management System"; 

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            paymentType = payment.Text;

            guestName = nametxt.Text;
            contactInfo = contacttxt.Text;
           
            roomNo = int.Parse(roomtxt.Text);

            using (Font titleFont = new Font("Courier New", 25, FontStyle.Bold))
            using (Font font = new Font("Courier New", 18, FontStyle.Bold))
            using (Font cashierFont = new Font("Courier New", 14, FontStyle.Bold)) 
            using (SolidBrush brush = new SolidBrush(Color.Indigo))
            {
                float y = e.MarginBounds.Top; 

                
                float titleX = e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(hotelName, titleFont).Width) / 2;
                e.Graphics.DrawString(hotelName, titleFont, brush, titleX, y);
                y += 30; 

                string receiptTitle = "Receipt";
                string separator = new string('-', 40);
                string space = new string(' ', 40);
                
                float receiptTitleX = e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(receiptTitle, font).Width) / 2;
                e.Graphics.DrawString(receiptTitle, font, brush, receiptTitleX, y);
                y += 30; 
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;

                e.Graphics.DrawString(separator, font, brush, e.MarginBounds.Left, y);
                y += 20; 
                e.Graphics.DrawString(separator, font, brush, e.MarginBounds.Left, y);
                y += 20; 
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;
                
                foreach (var line in GetFormattedReceiptLines())
                {
                    float lineX = e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(line, font).Width) / 2;
                    e.Graphics.DrawString(line, font, brush, lineX, y);
                    y += 20; 
                    e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                    y += 20;
                }
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;
                e.Graphics.DrawString(separator, font, brush, e.MarginBounds.Left, y);
                y += 20; 
                e.Graphics.DrawString(separator, font, brush, e.MarginBounds.Left, y);
                y += 20; 
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;
                e.Graphics.DrawString(space, font, brush, e.MarginBounds.Left, y);
                y += 20;

                
                string cashierName = "Front Desk: Andrei Parquez"; 
                float cashierX = e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(cashierName, cashierFont).Width) / 2;
                e.Graphics.DrawString(cashierName, cashierFont, brush, cashierX, y);
            }

            e.HasMorePages = false; 
        }
        private IEnumerable<string> GetFormattedReceiptLines()
        {
            yield return "Receipt for: " + guestName;

            yield return "Room No: " + roomNo;
            yield return "Check-In: " + checkin.ToString();
            yield return "Check-Out: " + checkout.ToString();
            yield return "Total Days: " + totalDays;
            yield return "Total Cost: " + totalCost.ToString("C");
            yield return "Payment Type: " + paymentType;
        }

        private void receipt_Click(object sender, EventArgs e)
        {
            printPreviewDialog.ShowDialog();
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reservation_Load(object sender, EventArgs e)
        {
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellPainting += dataGridView1_CellPainting;
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
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["CheckOutColumn"].Index)
            {
                int roomNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["RoomNo"].Value);

                
                using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    string updateStatusQuery = "UPDATE room SET Status = 'available' WHERE RoomNo = @RoomNo";
                    using (MySqlCommand updateStatusCommand = new MySqlCommand(updateStatusQuery, sqlConnection))
                    {
                        updateStatusCommand.Parameters.AddWithValue("@RoomNo", roomNo);
                        updateStatusCommand.ExecuteNonQuery();
                    }
                    List<int> specificItemIDs = new List<int> { 1, 2, 3,4 };
                    string deductInventoryQuery = "UPDATE products SET Quantity = Quantity - 1 WHERE productID IN (" +
                                                  string.Join(",", specificItemIDs) + ")";
                    using (MySqlCommand deductInventoryCommand = new MySqlCommand(deductInventoryQuery, sqlConnection))
                    {
                        deductInventoryCommand.ExecuteNonQuery();
                    }

                    

                    MessageBox.Show("Check-out completed. Room status updated to available.");
                }

                
                PopulateDataGridView();
            }
        }

        private void guna2TileButton2_Click(object sender, EventArgs e)
        {
            amount.Text = 100.ToString();
        }

        private void guna2TileButton3_Click(object sender, EventArgs e)
        {
            amount.Text = 200.ToString();
        }

        private void guna2TileButton5_Click(object sender, EventArgs e)
        {
            amount.Text = 1000.ToString();
        }

        private void guna2TileButton4_Click(object sender, EventArgs e)
        {
            amount.Text = 500.ToString();
        }

        private void guna2TileButton1_Click(object sender, EventArgs e)
        {
            Login newform = new Login();
            newform.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void searchtxt_TextChanged(object sender, EventArgs e)
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
