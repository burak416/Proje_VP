using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Proje_VP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string username = nameText.Text;
            string password = textBox1.Text;
            string connectionString = @"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\User_Info.db;Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT IsAdmin FROM UserInfo WHERE Username = @username AND Password = @password";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            int isAdmin = Convert.ToInt32(result);

                            if (isAdmin == 1)
                            {
                                MessageBox.Show("Admin login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Form4 adminForm = new Form4();
                                adminForm.Show();
                                Form3 form3 = new Form3(username);
                                form3.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("User login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Form3 userform = new Form3(username);
                                userform.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:" +ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox4.Text;
            string password = textBox2.Text;
            string phoneNumber = maskedTextBox1.Text; 
            string securityQuestion = comboBox1.SelectedItem?.ToString();
            string answer = textBox3.Text;
            string gender;
            if (radioButton1.Checked)
            {
                gender = "Female";
            }
            else if (radioButton2.Checked)
            {
                gender = "Male";
            }
            else
            {
                MessageBox.Show("Please choose one gender.");
                return;
            }

            DateTime birthDate = dateTimePicker1.Value;
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.Date < birthDate.AddYears(age))
            {
                age--;
            }

            if (age < 18)
            {
                MessageBox.Show("You must be at least 18 years old to register.", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connectionString = @"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\User_Info.db;Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO UserInfo (Username, Password, \"Phone Number\", Gender, \"Security Question\", Answer) " +
                                   "VALUES (@username, @password, @phone_number, @gender, @security_question, @answer)";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@phone_number", phoneNumber);
                        command.Parameters.AddWithValue("@gender", gender); 
                        command.Parameters.AddWithValue("@security_question", securityQuestion);
                        command.Parameters.AddWithValue("@answer", answer);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("User information saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to save user information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: "+  ex.Message);
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            string username = textBox6.Text;
            string securityQuestion = comboBox2.SelectedItem?.ToString();
            string answer = textBox5.Text;
            string connectionString = @"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\User_Info.db;Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM UserInfo WHERE Username = @username AND" +
                        " \"Security Question\" = @securityQuestion AND Answer = @answer";


                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@securityQuestion", securityQuestion);
                        command.Parameters.AddWithValue("@answer", answer);
                        int result = Convert.ToInt32(command.ExecuteScalar());

                        if (result > 0)
                        {
                            MessageBox.Show("Security question and answer verified. Proceed to reset your password.", "Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Form2 passwordResetForm = new Form2(username);
                            passwordResetForm.Show();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect security question or answer.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:" + ex.Message);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
