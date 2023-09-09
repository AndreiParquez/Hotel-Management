using Guna.Charts.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace System
{
    public partial class Dashboardfrm : Form
    {
        private const string connectionString = "Server=localhost;Database=db_parquez;Uid=root;Pwd=;"; 
        private MySqlConnection connection;
        private DataSet dataSet;
        public Dashboardfrm()
        {
            InitializeComponent();
            CheckLowInventory();
           timer1.Interval = 5000; 
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckLowInventory();
        }

        private void Dashboardfrm_Load(object sender, EventArgs e)

        {
            CheckLowInventory();
            InitializeChart();


        }
        private void InitializeChart()
        {
            Chart reservationsChart = new Chart();
            reservationsChart.Width = 600;  
            reservationsChart.Height = 343;

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.LabelStyle.Font = new Font("Arial", 10F, FontStyle.Bold); 
            chartArea.AxisY.LabelStyle.Font = new Font("Arial", 6F, FontStyle.Bold); 

            chartArea.AxisY.LineColor = Color.FromArgb(0, 135, 105); 
            chartArea.AxisY.LabelStyle.ForeColor = Color.FromArgb(0, 135, 105);
      
            chartArea.AxisX.LabelStyle.ForeColor = Color.DimGray;
            reservationsChart.BackColor = Color.Honeydew;
            chartArea.Position = new ElementPosition(10, 10, 80, 80);
            chartArea.BackColor = Color.Honeydew;
            reservationsChart.ChartAreas.Add(chartArea);

            Series series = new Series("Reservations");
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Date;
            series.IsValueShownAsLabel = true;

            series.BorderWidth = 3; 
            series.Color = Color.FromArgb(0, 135, 105);
            reservationsChart.Series.Add(series);

            guna2Panel1.Controls.Add(reservationsChart);

            LoadReservationsChartData(reservationsChart);
        }
        private void LoadReservationsChartData(Chart chart)
        {
            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string chartDataQuery = "SELECT DATE(checkin) AS date, COUNT(*) AS total_reservations FROM reservation GROUP BY date";
                    using (MySqlCommand cmd = new MySqlCommand(chartDataQuery, connection))
                    {
                        dataSet = new DataSet();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0)
                        {
                            DataTable chartData = dataSet.Tables[0];
                            Series series = chart.Series["Reservations"];

                            foreach (DataRow row in chartData.Rows)
                            {
                                DateTime date = Convert.ToDateTime(row["date"]);
                                int totalReservations = Convert.ToInt32(row["total_reservations"]);
                                series.Points.AddXY(date, totalReservations);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void CheckLowInventory()
        {
            try
            {
               
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string countQuery = "SELECT COUNT(*) FROM reservation";
                    using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
                    {
                        int totalCount = Convert.ToInt32(countCommand.ExecuteScalar());
                        totalres.Text =  totalCount.ToString();
                    }
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

                        roomlbl.Text =  availableRoomCount.ToString() + "/" + totalRoomCount.ToString();
                        av.Text = availableRoomCount.ToString();
                        oc.Text = occroom.ToString();
                     
                    }

                    string countIn = "SELECT COUNT(*) FROM Products";
                    using (MySqlCommand countCommand = new MySqlCommand(countIn, connection))
                    {
                        int totalCount = Convert.ToInt32(countCommand.ExecuteScalar());
                        label3.Text = totalCount.ToString();
                    }

                    string sumTotalCostQuery = "SELECT SUM(TotalCost) FROM reservation";
                    using (MySqlCommand sumTotalCostCommand = new MySqlCommand(sumTotalCostQuery, connection))
                    {
                        object sumTotalCostResult = sumTotalCostCommand.ExecuteScalar();
                        decimal sumTotalCost = sumTotalCostResult != DBNull.Value ? Convert.ToDecimal(sumTotalCostResult) : 0;

                       rev.Text = sumTotalCost.ToString("C");
                    }

                    string lowInventoryQuery = "SELECT * FROM products WHERE Quantity < 10";
                    using (MySqlCommand command = new MySqlCommand(lowInventoryQuery, connection))
                    {
                        dataSet = new DataSet();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        adapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        {
                      
                            notify.Visible = true;
                        }
                        else
                        {
                            
                           notify.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
          
        }

        private void guna2ShadowPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Shapes5_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }
    }
}
