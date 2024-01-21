using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Library
{
    public partial class Catalog : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        string name;
        public int selectedBookID;
        public int selectedCategoryID;
        public int selectedCopyID;

        public Catalog(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            dbHelper = new DatabaseHelper("Data Source=xe; User ID=Kaze; Password=123");
            getName();
        }

        public void getName()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT name FROM Users WHERE id = :id";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = userID;

                    object userName = command.ExecuteScalar();

                    if (userName != null && userName != DBNull.Value)
                    {
                        name = userName.ToString();

                    }
                }
            }
        }

        private void getInLibrary()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT * FROM Books ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable5 = new DataTable();
                        adapter.Fill(dataTable5);
                        dataGridView4.DataSource = dataTable5;
                    }
                }
            }
        }
        private string generateBarcode()
        {
            string barcode = "";
            Random random = new Random();
            bool barcodeExists = false;

            do
            {
                barcode = "";
                for (int i = 0; i < 10; i++)
                {
                    barcode += random.Next(0, 9).ToString();
                }

                // Check if barcode already exists
                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    string query = "SELECT COUNT(*) FROM book_copy WHERE barcode = :barcode";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            barcodeExists = true;
                        }
                        else
                        {
                            barcodeExists = false;
                        }
                    }
                }
            } while (barcodeExists);

            return barcode;
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            getInLibrary();
            toolStripDropDownButton1.Text = name;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Circulation circulation = new Circulation(userID);
            circulation.Show();
            this.Hide();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard(userID);
            dashboard.Show();
            this.Hide();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            User user = new User(userID);
            user.Show();
            this.Hide();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

          

            if (tabControl1.SelectedTab == tabPage3)
            {
                getInLibrary();
            }
            else if (tabControl1.SelectedTab == tabPage1)
            {
                getCategory();
            }
            else if (tabControl1.SelectedTab == tabPage6)
            {
              
                getCopyList();
            }
            else if(tabControl1.SelectedTab == tabPage5)
            {
                getBarcodeList();
            }
            else if (tabControl1.SelectedTab == tabPage4)
            {
                loadCategoryInCombo();
                comboBox2.SelectedIndex = 0;
            }

        }

        private void loadCategoryInCombo()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT id, name FROM categories";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        comboBox2.DataSource = dataTable;
                        comboBox2.ValueMember = "id";
                        comboBox2.DisplayMember = "name";
                    }
                }
            }
        }

        private void loadCategoryInDetail()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT id, name FROM categories";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable2 = new DataTable();
                        adapter.Fill(dataTable2);
                        comboBox3.DataSource = dataTable2;
                        comboBox3.ValueMember = "id";
                        comboBox3.DisplayMember = "name";
                    }
                }
            }
        }

        private void getBarcodeList()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT * FROM BOOK_COPY WHERE is_labeled = 'N' AND is_lost = 'N'";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable2 = new DataTable();
                        adapter.Fill(dataTable2);
                        dataGridView2.DataSource = dataTable2;
                    }
                }
            }
        }


        private void getBookDetail()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT isbn, title, author, category_id, year FROM books WHERE id = :bookID";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":bookID", OracleDbType.Int32).Value = selectedBookID;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBox9.Text = reader["isbn"].ToString();
                            textBox11.Text = reader["title"].ToString();
                            textBox10.Text = reader["author"].ToString();
                            comboBox3.SelectedValue = Convert.ToInt32(reader["category_id"]);
                            textBox1.Text = reader["year"].ToString();
                        }
                    }
                }
            }
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                selectedBookID = Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells["id"].Value.ToString());
                tabControl1.SelectedTab = tabPage6;
                loadCategoryInDetail();
                getBookDetail();
               

            }
            catch { 
            MessageBox.Show("Please select a book");
            }

        }

        private void getCategory()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT * FROM categories";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable4 = new DataTable();
                        adapter.Fill(dataTable4);
                        dataGridView3.DataSource = dataTable4;
                    }
                }
            }
        }
      

        private void button11_Click(object sender, EventArgs e)
        {
            // Get the values from the text boxes and combo box
            string textBox9Value = textBox9.Text;
            string textBox11Value = textBox11.Text;
            string textBox10Value = textBox10.Text;
            int comboBox3Value = Convert.ToInt32(comboBox3.SelectedValue);
            string textBox1Value = textBox1.Text;

            try
            { // Update the book details in the database
                getCategory();
                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    string query = "UPDATE books SET isbn = :isbn, title = :title, author = :author, category_id = :categoryId, year = :year WHERE id = :bookID";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(":isbn", OracleDbType.Varchar2).Value = textBox9Value;
                        command.Parameters.Add(":title", OracleDbType.Varchar2).Value = textBox11Value;
                        command.Parameters.Add(":author", OracleDbType.Varchar2).Value = textBox10Value;
                        command.Parameters.Add(":categoryId", OracleDbType.Int32).Value = comboBox3Value;
                        command.Parameters.Add(":year", OracleDbType.Varchar2).Value = textBox1Value;
                        command.Parameters.Add(":bookID", OracleDbType.Int32).Value = selectedBookID;

                        command.ExecuteNonQuery();
                    }
                }
               
                getBookDetail(); MessageBox.Show("Updated!");
            }
            catch {
            MessageBox.Show("Cannot update!");
            }
           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string textBox2Value = textBox2.Text;

            try
            {
                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    string query = "INSERT INTO categories (name) VALUES (:name)";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox2Value;

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Category inserted successfully!");
                getCategory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting category: " + ex.Message);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string query = "UPDATE CATEGORIES SET name = :name WHERE id = :categoryID";



            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox2.Text;
                    command.Parameters.Add(":categoryID", OracleDbType.Int32).Value = selectedCategoryID;

                    command.ExecuteNonQuery();
                }
            }
            getCategory();
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try {
                selectedCategoryID = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["id"].Value.ToString());
                textBox2.Text = Convert.ToString(dataGridView3.Rows[e.RowIndex].Cells["name"].Value.ToString());
            }
            catch {
                MessageBox.Show("Invalid selection!");
            }
            

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try {
                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    string query = "INSERT INTO book_copy (book_id,is_lost) VALUES (:bookID, 'N')";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(":bookID", OracleDbType.Int32).Value = selectedBookID;

                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Copy added!"); getCopyList();

            }
            catch { 
            MessageBox.Show("Cannot add!");
            }
            
        }

        private void getCopyList()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT bc.*, u.name AS LastBorrowed FROM BOOK_COPY bc LEFT JOIN transactions t ON bc.id = t.book_copy_id LEFT JOIN Users u ON t.user_id = u.id WHERE bc.book_id = :bookID";


                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":bookID", OracleDbType.Int32).Value = selectedBookID;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable3 = new DataTable();
                        adapter.Fill(dataTable3);
                        dataGridView1.DataSource = dataTable3;
                    }
                }
            }
        }


        private void button13_Click(object sender, EventArgs e)
        {

            if (selectedCopyID != 0)
            { 
                string query = "SELECT is_lost FROM book_copy WHERE id = :copyID";

                try
                {
                    using (OracleConnection connection = dbHelper.GetOpenConnection())
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(":copyID", OracleDbType.Int32).Value = selectedCopyID;

                            object isLost = command.ExecuteScalar();

                            if (isLost != null && isLost != DBNull.Value)
                            {
                                string currentStatus = isLost.ToString();

                              
                                string updateQuery = "UPDATE book_copy SET is_lost = :newStatus WHERE id = :copyID";

                                using (OracleCommand updateCommand = new OracleCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.Add(":newStatus", OracleDbType.Varchar2).Value = currentStatus == "N" ? "Y" : "N";
                                    updateCommand.Parameters.Add(":copyID", OracleDbType.Int32).Value = selectedCopyID;

                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    MessageBox.Show("Updated!"); getCopyList();
                }
                catch
                {
                    MessageBox.Show("Could not update!");
                }
            }




        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try { selectedCopyID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString()); }
            catch { MessageBox.Show("Invalid selection!"); }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (selectedCopyID != 0)
            {
                string barcode = generateBarcode();

                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    string query = "UPDATE book_copy SET barcode = :barcode, is_labeled = 'N', barcode_img = NULL WHERE id = :copyID";


                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                        command.Parameters.Add(":copyID", OracleDbType.Int32).Value = selectedCopyID;

                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Barcode generated!");
                    getCopyList();


                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT barcode FROM BOOK_COPY WHERE is_labeled = 'N' AND is_lost = 'N'";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        List<string> barcodes = new List<string>();

                        while (reader.Read())
                        {
                            barcodes.Add(reader["barcode"].ToString());
                        }

                        foreach (string barcode in barcodes)
                        {
                            // Generate barcode image using ZXing.Net library
                            BarcodeWriter writer = new BarcodeWriter
                            {
                                Format = BarcodeFormat.CODE_128, // Replace with the desired barcode format
                                Options = new ZXing.Common.EncodingOptions
                                {
                                    Width = 300, // Set the desired image width
                                    Height = 100 // Set the desired image height
                                }
                            };

                            Bitmap barcodeImage = writer.Write(barcode);

                            // Save barcode image to the database
                            SaveBarcodeImageToDatabase(barcode, barcodeImage);
                        }
                        MessageBox.Show("Barcode generated");
                    }
                }
            }
        }

        private void SaveBarcodeImageToDatabase(string barcode, Bitmap barcodeImage)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    barcodeImage.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();

                    using (OracleConnection connection = dbHelper.GetOpenConnection())
                    {
                        string query = "UPDATE book_copy SET Barcode_IMG = :barcodeImage, IS_LABELED = 'Y' WHERE barcode = :barcode";

                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(":barcodeImage", OracleDbType.Blob).Value = imageBytes;
                            command.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;

                            command.ExecuteNonQuery();
                        }
                    }
                }
               
            }
            catch
            {
                MessageBox.Show("Cannot save!");
            }
           
        }

    }
}
