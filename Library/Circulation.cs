﻿using Oracle.DataAccess.Client;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Library
{
    public partial class Circulation : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        public string name;
        public int selectedID;
        public string barcode;
        public string checkInBarcode;
        public string barcodeCopy;
        public Circulation(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            dbHelper = new DatabaseHelper("Data Source=xe; User ID=Kaze; Password=123");
            getName();


        }

        public void borrowBook(string barcode, int userID)
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                // Check if the barcode exists in BOOK_COPY table
                string query = "SELECT COUNT(*) FROM BOOK_COPY WHERE BARCODE = :barcode AND IS_AVAILABLE = 'Y'";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count != 0)
                    {
                        // Get the copyID from the barcode
                        string copyIDQuery = "SELECT ID FROM BOOK_COPY WHERE BARCODE = :barcode";
                        using (OracleCommand copyIDCommand = new OracleCommand(copyIDQuery, connection))
                        {
                            copyIDCommand.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                            int copyID = Convert.ToInt32(copyIDCommand.ExecuteScalar());

                            // Insert into TRANSACTIONS table
                            string insertQuery = "INSERT INTO TRANSACTIONS (USER_ID, BORROW_DATE, BOOK_COPY_ID) VALUES (:userID, SYSDATE, :copyID)";
                            using (OracleCommand insertCommand = new OracleCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.Add(":userID", OracleDbType.Int32).Value = userID;
                                insertCommand.Parameters.Add(":copyID", OracleDbType.Int32).Value = copyID;
                                insertCommand.ExecuteNonQuery();
                            }
                            getTransactions();

                            // Update the status of the book in BOOK_COPY table
                            string updateStatusQuery = "UPDATE BOOK_COPY SET is_available = 'N' WHERE BARCODE = :barcode";
                            using (OracleCommand updateStatusCommand = new OracleCommand(updateStatusQuery, connection))
                            {
                                updateStatusCommand.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                                updateStatusCommand.ExecuteNonQuery();
                            }

                        }

                    }
                    else
                    {
                        MessageBox.Show("Item not available");
                    }
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {


            if (selectedID != 0)
            {

                barcode = textBox2.Text;
                borrowBook(barcode, selectedID);
                getTransactions();



            }
            else
            {
                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    string query1 = "SELECT name,email,id FROM Users WHERE name LIKE '%' || :name || '%'";

                    using (OracleCommand command = new OracleCommand(query1, connection))
                    {
                        command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox2.Text;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable1 = new DataTable();
                            adapter.Fill(dataTable1);
                            dataGridView6.DataSource = dataTable1;
                        }
                    }
                }


            }
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

        private void Dashboard_Load(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Text = name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Catalog catalog = new Catalog(userID);
            catalog.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

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

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard(userID);
            dashboard.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            User user = new User(userID);
            user.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query2 = "SELECT * FROM Books WHERE name LIKE '%' || :name || '%'";

                using (OracleCommand command = new OracleCommand(query2, connection))
                {
                    command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox2.Text;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable2 = new DataTable();
                        adapter.Fill(dataTable2);
                        dataGridView6.DataSource = dataTable2;
                    }
                }
            }
        }

        public void checkIn(string barcode)
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                // Check if the barcode exists in BOOK_COPY table
                string query = "SELECT COUNT(*) FROM BOOK_COPY WHERE BARCODE = :barcode";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count != 0)
                    {
                        // Get the book ID from the barcode
                        string bookIDQuery = "SELECT BOOK_ID FROM BOOK_COPY WHERE BARCODE = :barcode";
                        using (OracleCommand bookIDCommand = new OracleCommand(bookIDQuery, connection))
                        {
                            bookIDCommand.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                            int bookID = Convert.ToInt32(bookIDCommand.ExecuteScalar());

                            // Update the return date in TRANSACTIONS table
                            string updateQuery = "UPDATE TRANSACTIONS SET RETURN_DATE = SYSDATE WHERE BOOK_COPY_ID IN (SELECT ID FROM BOOK_COPY WHERE BARCODE = :barcode) AND RETURN_DATE IS NULL";


                            using (OracleCommand updateCommand = new OracleCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                                updateCommand.ExecuteNonQuery();
                            }

                            // Update the status of the book in BOOK_COPY table
                            string updateStatusQuery = "UPDATE BOOK_COPY SET is_available = 'Y' WHERE BARCODE = :barcode";
                            using (OracleCommand updateStatusCommand = new OracleCommand(updateStatusQuery, connection))
                            {
                                updateStatusCommand.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;
                                updateStatusCommand.ExecuteNonQuery();
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid barcode");
                    }
                }
            }
        }

        private void getTransactions()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = @"SELECT  B.TITLE, BC.Barcode, T.ID, T.BORROW_DATE,T.RETURN_DATE  
                        FROM TRANSACTIONS T
                        JOIN BOOK_COPY BC ON T.BOOK_COPY_ID = BC.ID
                        JOIN BOOKS B ON BC.BOOK_ID = B.ID
                        WHERE T.USER_ID = :userID AND T.RETURN_DATE IS NULL";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":userID", OracleDbType.Int32).Value = selectedID;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView7.DataSource = dataTable;
                    }
                }
            }
        }

        private void getRecentCheckin()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = @"SELECT BC.BARCODE, B.TITLE, U.NAME, T.*
                FROM TRANSACTIONS T
                JOIN BOOK_COPY BC ON T.BOOK_COPY_ID = BC.ID
                JOIN BOOKS B ON BC.BOOK_ID = B.ID
                JOIN USERS U ON T.USER_ID = U.ID
                WHERE T.RETURN_DATE IS NOT NULL
                ORDER BY T.RETURN_DATE DESC";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable4 = new DataTable();
                        adapter.Fill(dataTable4);
                        dataGridView8.DataSource = dataTable4;
                    }
                }
            }
        }


        private void dataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRowIndex = e.RowIndex;
            if (selectedRowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView6.Rows[selectedRowIndex];
                try
                {
                    selectedID = Convert.ToInt32(selectedRow.Cells["id"].Value);
                    // Get the name from the selectedID
                    string name = GetNameFromID(selectedID);

                    label29.Text = name;
                    button3.Text = "Borrow";

                    textBox2.Focus();
                    textBox2.Clear();
                    getTransactions();

                }
                catch
                {
                    selectedID = 0;
                    barcode = string.Empty;

                    label29.Text = "";
                    button3.Text = "Patron";
                }




            }
        }

        private string GetNameFromID(int id)
        {
            string name = string.Empty;

            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = "SELECT name FROM Users WHERE id = :id";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    object userName = command.ExecuteScalar();

                    if (userName != null && userName != DBNull.Value)
                    {
                        name = userName.ToString();
                    }
                }
            }

            return name;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                borrowBook(barcode, selectedID);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            checkInBarcode = textBox3.Text;
            checkIn(checkInBarcode);
            getRecentCheckin();
            textBox3.Focus();

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            getRecentCheckin();
        }

        private void getCopyStatus()
        {

            // Assuming you want to retrieve the book details based on the provided barcode

            string query = @"SELECT BC.BARCODE, BB.ISBN, BB.TITLE, BB.IMAGE, BC.IS_AVAILABLE
                 FROM BOOK_COPY BC
                 JOIN BOOKS BB ON BC.BOOK_ID = BB.ID
                 WHERE BC.BARCODE = :barcode";


            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcodeCopy;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            // Retrieve the BLOB image from the DataTable
                            byte[] imageBytes = dataTable.Rows[0]["IMAGE"] as byte[];

                            if (imageBytes != null)
                            {
                                // Convert the byte array to an Image
                                Image image;
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    image = Image.FromStream(ms);
                                }

                                // Assign the image to the PictureBox
                                pictureBox3.Image = image;
                            }
                            else
                            {
                                // Clear the PictureBox if the image is null
                                pictureBox3.Image = null;
                            }

                            textBox4.Text = dataTable.Rows[0]["BARCODE"].ToString();
                            textBox10.Text = dataTable.Rows[0]["ISBN"].ToString();
                            textBox12.Text = dataTable.Rows[0]["TITLE"].ToString();
                            textBox1.Text = dataTable.Rows[0]["IS_AVAILABLE"].ToString() == "Y" ? "Available" : "Not Available";
                        }
                        else
                        {
                            // Handle case when no book is found with the provided barcode
                            pictureBox3.Image = null;
                            textBox4.Text = "";
                            textBox10.Text = "";
                            textBox12.Text = "";
                            textBox1.Text = "Not Available";
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            barcodeCopy = toolStripTextBox1.Text;
            getCopyStatus();
            tabControl1.SelectedTab = tabPage3;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
