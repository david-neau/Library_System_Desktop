using Oracle.DataAccess.Client;
using System;
using System.Data;
using System.Windows.Forms;

namespace Library
{
    public partial class Dashboard : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        public string name;
        public Dashboard(int userID)
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

        public void getBookLastest()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query1 = "SELECT * FROM Books ORDER BY id DESC";
                using (OracleCommand command1 = new OracleCommand(query1, connection))
                {
                    using (OracleDataAdapter adapter1 = new OracleDataAdapter(command1))
                    {
                        DataTable dataTable = new DataTable();
                        adapter1.Fill(dataTable);
                        dataGridView4.DataSource = dataTable;
                    }
                }
            }
        }

        public void getCategory()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query2 = "SELECT * FROM Categories";
                using (OracleCommand command2 = new OracleCommand(query2, connection))
                {
                    using (OracleDataAdapter adapter2 = new OracleDataAdapter(command2))
                    {
                        DataTable dataTable = new DataTable();
                        adapter2.Fill(dataTable);
                        dataGridView3.DataSource = dataTable;
                    }
                }
            }
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Text = name;
            getBookLastest();
            getCategory();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyBook myBook = new MyBook(userID);
            myBook.Show();
            this.Hide();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*  dataGridView1.DataSource = dataSet.Tables["Books"];*/
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MyAccount myAccount = new MyAccount(userID);
            myAccount.Show();
            this.Hide();
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
