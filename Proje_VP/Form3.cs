using System;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace Proje_VP
{
    public partial class Form3 : Form
    {
        private string currentUserID; 

        public Form3(string userID)
        {
            InitializeComponent();
            currentUserID = userID;
            listBox1.SelectionMode = SelectionMode.MultiExtended;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            UpdatePurchaseHistory(currentUserID);
        }
        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("Page has been closed. The application is shutting down.");
            Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e) // Add to Cart
        {
            bool itemAdded = false; 
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\Product_Info.db;Version=3;"))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM ProductInfo", conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id;
                        if (reader["ID"] != DBNull.Value)
                        {
                            id = Convert.ToInt32(reader["ID"]);
                        }
                        else
                        {
                            id = 0;
                        }

                        string product;
                        if (reader["Product"] != DBNull.Value)
                        {
                            product = reader["Product"].ToString();
                        }
                        else
                        {
                            product = string.Empty;
                        }

                        decimal price;
                        if (reader["Price"] != DBNull.Value)
                        {
                            price = Convert.ToDecimal(reader["Price"]);
                        }
                        else
                        {
                            price = 0m;
                        }

                        int stock;
                        if (reader["Stock"] != DBNull.Value)
                        {
                            stock = Convert.ToInt32(reader["Stock"]);
                        }
                        else
                        {
                            stock = 0;
                        }

                        NumericUpDown numericUpDown = this.Controls.Find($"numericUpDown{id}", true).
                            FirstOrDefault() as NumericUpDown;

                        if (numericUpDown == null)
                        {
                            continue;
                        }

                        int quantity = (int)numericUpDown.Value;

                        if (quantity > 0 && stock >= quantity)
                        {
                            listBox1.Items.Add($"Product: {product}, Quantity: {quantity}, Unit Price: {price}");
                            itemAdded = true;
                        }
                        else if (quantity > stock)
                        {
                            MessageBox.Show("Not enough stock for "+product);
                        }
                    }
                }

                if (itemAdded)
                {
                    MessageBox.Show("Products are added to cart.");
                }
                else
                {
                    MessageBox.Show("Please choose something.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e) // Total
        {
            try
            {
                decimal totalPrice = 0;

                if (listBox1.Items.Count == 0)
                {
                    MessageBox.Show("Your cart is empty.");
                    return;
                }

                foreach (string item in listBox1.Items)
                {
                    string[] parts = item.Split(',');

                    if (parts.Length >= 3)
                    {
                        string adetPart = parts[1].Split(':')[1].Trim();
                        string fiyatPart = parts[2].Split(':')[1].Trim();

                        if (int.TryParse(adetPart, out int quantity) && decimal.TryParse(fiyatPart, out decimal price))
                        {
                            totalPrice += quantity * price;
                        }
                        else
                        {
                            MessageBox.Show("Error in product: "+item);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid format: " + item);
                    }
                }

                label17.Text = $"Total Price: {totalPrice:0.00}$";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Pay with Card
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\Product_Info.db;Version=3;"))
                using (SQLiteConnection historyConn = new SQLiteConnection(@"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\User_Purchase_History.db;Version=3;"))
                {
                    conn.Open();
                    historyConn.Open();

                    foreach (string item in listBox1.Items)
                    {
                        string[] parts = item.Split(',');

                        if (parts.Length >= 2)
                        {
                            string product = parts[0].Split(':')[1].Trim();
                            string adetPart = parts[1].Split(':')[1].Trim();

                            if (int.TryParse(adetPart, out int quantity))
                            {
                                SQLiteCommand update = new SQLiteCommand(
                                    "UPDATE ProductInfo SET Stock = Stock - @quantity WHERE TRIM(Product) = TRIM(@product)", conn);
                                update.Parameters.AddWithValue("@quantity", quantity);
                                update.Parameters.AddWithValue("@product", product);

                                int affectedRows = update.ExecuteNonQuery();
                                if (affectedRows == 0)
                                {
                                    MessageBox.Show($"Stock update failed for '{product}'!");
                                    continue;
                                }

                                SQLiteCommand history = new SQLiteCommand(
                                    "INSERT INTO PurchaseHistory (UserID, Product, Quantity, Price, PurchaseDate) " +
                                    "VALUES (@userID, @product, @quantity, @price, @date)", historyConn);
                                history.Parameters.AddWithValue("@userID", currentUserID);
                                history.Parameters.AddWithValue("@product", product);
                                history.Parameters.AddWithValue("@quantity", quantity);
                                history.Parameters.AddWithValue("@price", parts[2].Split(':')[1].Trim()); 
                                history.Parameters.AddWithValue("@date", DateTime.Now);

                                history.ExecuteNonQuery();
                            }
                            else
                            {
                                MessageBox.Show("Invalid format: " + adetPart);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid format: "+item);
                        }
                    }

                    listBox1.Items.Clear();
                    label17.Text = "Total Price: 0$";

                    UpdatePurchaseHistory(currentUserID);

                    MessageBox.Show("Purchase completed successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void UpdatePurchaseHistory(string userID)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=C:\Users\burak\source\repos\Proje_VP\Proje_VP\bin\Debug\User_Purchase_History.db;Version=3;"))
                {
                    conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(
                        "SELECT Product, Quantity, Price, PurchaseDate FROM PurchaseHistory WHERE UserID = " +
                        "@userID ORDER BY PurchaseDate DESC", conn);
                    cmd.Parameters.AddWithValue("@userID", userID);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    listBox2.Items.Clear(); 
                    while (reader.Read())
                    {
                        string product = reader["Product"].ToString();
                        int quantity = Convert.ToInt32(reader["Quantity"]);
                        int price = Convert.ToInt32(reader["Price"]);
                        DateTime date = Convert.ToDateTime(reader["PurchaseDate"]);

                        listBox2.Items.Add("Product: " + product + ", " + "Quantity: " + quantity + ", " + "Price: " + "$" + 
                            price + ", " + "Date: "+date);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedItems.Count > 0)
                {
                    while (listBox1.SelectedItems.Count > 0)
                    {
                        listBox1.Items.Remove(listBox1.SelectedItem);
                    }
                    listBox1.ClearSelected();
                }
                else
                {
                    MessageBox.Show("Please select one or more items to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
