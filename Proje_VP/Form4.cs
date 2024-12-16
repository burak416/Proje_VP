using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Proje_VP
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=C:\\Users\\burak\\source\\repos\\Proje_VP\\Proje_VP\\bin\\Debug\\Product_Info.db;Version=3;"))
                {
                    conn.Open();


                    SQLiteCommand gain = new SQLiteCommand(
                        "SELECT Product, (Price * (InitialStock - Stock)) AS TotalGain, " +
                        "(InitialStock - Stock) AS SoldUnits FROM ProductInfo WHERE InitialStock > Stock",
                        conn);
                    SQLiteDataAdapter gainAdapter = new SQLiteDataAdapter(gain);
                    DataTable gainTable = new DataTable();
                    gainAdapter.Fill(gainTable);

                    chart1.Series[0].Points.Clear();
                    chart1.Titles.Clear();
                    chart1.Titles.Add("Total Earnings per Product");

                    foreach (DataRow row in gainTable.Rows)
                    {
                        string product = row["Product"].ToString();
                        decimal totalGain = Convert.ToDecimal(row["TotalGain"]);
                        int soldUnits = Convert.ToInt32(row["SoldUnits"]);
                        var point = new DataPoint();
                        point.YValues = new double[] { (double)totalGain };
                        chart1.Series[0].Points.Add(point);
                        point.AxisLabel = product;
                        point.ToolTip = $"{product}: ${totalGain} gain, {soldUnits} sold";
                        point.Label = $"{product}\n${totalGain}";

                    }


                    SQLiteCommand sale = new SQLiteCommand(
                        "SELECT Product, (InitialStock - Stock) AS SoldUnits FROM ProductInfo WHERE InitialStock > Stock",
                        conn);
                    SQLiteDataAdapter saleAdapter = new SQLiteDataAdapter(sale);
                    DataTable saleTable = new DataTable();
                    saleAdapter.Fill(saleTable);

                    chart2.Series[0].Points.Clear();
                    chart2.Titles.Clear();
                    chart2.Titles.Add("Units Sold per Product");

                    foreach (DataRow row in saleTable.Rows)
                    {
                        string product = row["Product"].ToString();
                        int soldUnits = Convert.ToInt32(row["SoldUnits"]);
                        var point = new DataPoint();
                        point.YValues = new double[] { (double)soldUnits };
                        chart2.Series[0].Points.Add(point);
                        point.AxisLabel = product;
                        point.ToolTip = $"{product}: {soldUnits} sold";
                        point.Label = $"{product}\n{soldUnits} units";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }

        private void chart2_Click(object sender, EventArgs e)
        {
        }
    }
}