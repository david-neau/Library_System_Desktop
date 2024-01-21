using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Library
{
    public partial class User : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        string name;
        public int selectedUserID;
        public bool newImageCheck;

        public User(int userID)
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

        private void Dashboard_Load(object sender, EventArgs e)
        {
            getUserList();
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

        }

        private void getUserList()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query2 = "SELECT * FROM Users";

                using (OracleCommand command = new OracleCommand(query2, connection))
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

        private void getUserInfo()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
              

                string query = "SELECT id, is_admin, name, email, password, created_at, updated_at, image FROM Users WHERE id = :id";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = selectedUserID;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            textBox6.Text = row["id"].ToString();
                            checkBox2.Checked = row["is_admin"].ToString() == "Y";
                            textBox7.Text = row["name"].ToString();
                            textBox5.Text = row["email"].ToString();
                            textBox4.Text = row["password"].ToString();
                            label13.Text = row["created_at"].ToString();
                            label17.Text = row["updated_at"].ToString();
                            object imageObject = row["image"];

                            if (imageObject != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])imageObject;

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
                        }
                    }
                }
            }
        }
            

        


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage3)
            {
                getUserList();
            }

            if (tabControl1.SelectedTab == tabPage1)
            {

                getUserInfo();

            }

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedUserID = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["id"].Value.ToString());
            tabControl1.SelectedTab = tabPage1;
            getUserInfo();
        }



        private void button7_Click(object sender, EventArgs e)
        {
            
            if(newImageCheck == true)
            {
                string query = "UPDATE Users SET image = :image, name = :name, email = :email, password = :password, is_admin = :is_admin WHERE id = :id";

                try
                {
                    using (OracleConnection connection = dbHelper.GetOpenConnection())
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(":image", OracleDbType.Blob).Value = ImageToByteArray2(pictureBox3.Image);
                            command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox7.Text;
                            command.Parameters.Add(":email", OracleDbType.Varchar2).Value = textBox5.Text;
                            command.Parameters.Add(":password", OracleDbType.Varchar2).Value = textBox4.Text;
                            command.Parameters.Add(":is_admin", OracleDbType.Char).Value = checkBox2.Checked ? "Y" : "N";
                            command.Parameters.Add(":id", OracleDbType.Int32).Value = selectedUserID;

                            command.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("User Updated!");
                    newImageCheck = false;

                }
                catch
                {
                    MessageBox.Show("Failed to update User!");
                }
            }
            else
            {
                string query = "UPDATE Users SET name = :name, email = :email, password = :password, is_admin = :is_admin WHERE id = :id";

                try
                {
                    using (OracleConnection connection = dbHelper.GetOpenConnection())
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox7.Text;
                            command.Parameters.Add(":email", OracleDbType.Varchar2).Value = textBox5.Text;
                            command.Parameters.Add(":password", OracleDbType.Varchar2).Value = textBox4.Text;
                            command.Parameters.Add(":is_admin", OracleDbType.Char).Value = checkBox2.Checked ? "Y" : "N";
                            command.Parameters.Add(":id", OracleDbType.Int32).Value = selectedUserID;

                            command.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("User Updated!");

                }
                catch
                {
                    MessageBox.Show("Failed to update User!");
                }
            }


    
            
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog2.ShowDialog();

            if (result == DialogResult.OK)
            {
                pictureBox3.Image = Image.FromFile(openFileDialog2.FileName);
                newImageCheck = true;
            }
        }
        private byte[] ImageToByteArray2(Image image)
        {
            if (image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    return ms.ToArray();
                }
            }
            else
            {
                return null; // or throw an exception, depending on your requirements
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                pictureBox2.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Users (name, email, password, is_admin, image) VALUES (:name, :email, :password, :is_admin, :image)";

            try
            {
                using (OracleConnection connection = dbHelper.GetOpenConnection())
                {
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(":name", OracleDbType.Varchar2).Value = textBox8.Text;
                        command.Parameters.Add(":email", OracleDbType.Varchar2).Value = textBox2.Text;
                        command.Parameters.Add(":password", OracleDbType.Varchar2).Value = textBox1.Text;
                        command.Parameters.Add(":is_admin", OracleDbType.Char).Value = checkBox1.Checked ? "Y" : "N";
                        if (pictureBox3.Image != null)
                        {
                            command.Parameters.Add(":image", OracleDbType.Blob).Value = ImageToByteArray2(pictureBox3.Image);
                        }
                        else
                        {
                            // Handle the case where the image is null
                            command.Parameters.Add(":image", OracleDbType.Blob).Value = DBNull.Value;
                        }
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("User Added!");

            }
            catch
            {
                MessageBox.Show("Failed to add User!");
            }
        }

      

       
    }
}
