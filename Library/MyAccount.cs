using Oracle.DataAccess.Client;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Library
{
    public partial class MyAccount : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        public string name;
        public bool newImageCheck = false;

        public MyAccount(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            dbHelper = new DatabaseHelper("Data Source=xe; User ID=Kaze; Password=123");
            getName();
            getUser();

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
            getUser();
            toolStripDropDownButton1.Text = name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyBook myBook = new MyBook(userID);
            myBook.Show();
            this.Hide();
        }

        private void getUser()
        {
            string query = "SELECT * FROM Users WHERE id = :id";

            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = userID;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBox7.Text = reader["name"].ToString();
                            textBox6.Text = reader["id"].ToString();
                            textBox5.Text = reader["email"].ToString();
                            textBox4.Text = reader["password"].ToString();
                            object imageObject = reader["image"];

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


        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard(userID);
            dashboard.Show();
            this.Hide();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (newImageCheck == true)
            {
                string query = "UPDATE Users SET image = :image, email = :email, password = :password WHERE id = :id";

                try
                {
                    using (OracleConnection connection = dbHelper.GetOpenConnection())
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(":image", OracleDbType.Blob).Value = ImageToByteArray2(pictureBox3.Image);
                            command.Parameters.Add(":email", OracleDbType.Varchar2).Value = textBox5.Text; // Access the Text property
                            command.Parameters.Add(":password", OracleDbType.Varchar2).Value = textBox4.Text;
                            command.Parameters.Add(":id", OracleDbType.Int32).Value = userID;

                            command.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Information Updated!");
                    newImageCheck = false;
                    getUser();

                }
                catch
                {
                    MessageBox.Show("Failed to update User!");
                }
            }
            else
            {
                string query = "UPDATE Users SET email = :email, password = :password WHERE id = :id";

                try
                {
                    using (OracleConnection connection = dbHelper.GetOpenConnection())
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {

                            command.Parameters.Add(":email", OracleDbType.Varchar2).Value = textBox5.Text; // Access the Text property
                            command.Parameters.Add(":password", OracleDbType.Varchar2).Value = textBox4.Text;
                            command.Parameters.Add(":id", OracleDbType.Int32).Value = userID;

                            command.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Information Updated!");
                    getUser();

                }
                catch
                {
                    MessageBox.Show("Failed to update User!");
                }
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

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                pictureBox3.Image = Image.FromFile(openFileDialog1.FileName);
                newImageCheck = true;
            }
        }
    }
}
