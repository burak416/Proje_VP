using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proje_VP
{
    public partial class Form2 : Form
    {
        private string _username;
        public Form2(string username)
        {
            InitializeComponent();
            _username = username;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            label4.Text = "Reset Password for " + _username;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string newPassword = textBox2.Text;
            string confirmPassword = textBox1.Text;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please enter the same password in both fields.");
                return;
            }

            string connectionString = @"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\User_Info.db;Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {                    
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.Parameters.AddWithValue("@newPassword", newPassword);
                        command.Parameters.AddWithValue("@username", _username);                     
                        command.CommandText = "UPDATE UserInfo SET Password = @newPassword WHERE Username = @username";

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Password updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update password. Please try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }

}
