using Oracle.DataAccess.Client;
using System;
using System.Data;
using System.Windows.Forms;

namespace Library
{
    public partial class MyBook : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        public string name;

        public MyBook(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            dbHelper = new DatabaseHelper("Data Source=xe; User ID=Kaze; Password=123");
            getName();
            getItemOut();
            getHistory();
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

        private void getItemOut()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = @"SELECT b.title,bc.barcode, t.borrow_date
                                FROM transactions t
                                JOIN book_copy bc ON t.book_copy_id = bc.id
                                JOIN books b ON bc.book_id = b.id
                                WHERE t.user_id = :id AND t.return_date IS NULL";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = userID;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView2.DataSource = dataTable; // Populate dataGridView2 with the data
                    }
                }
            }
        }

        private void getHistory()
        {
            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {
                string query = @"SELECT b.title,bc.barcode, t.borrow_date, t.return_date
                                FROM transactions t
                                JOIN book_copy bc ON t.book_copy_id = bc.id
                                JOIN books b ON bc.book_id = b.id
                                WHERE t.user_id = :id AND t.return_date IS NOT NULL";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = userID;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable; // Populate dataGridView2 with the data
                    }
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            MyAccount myAccount = new MyAccount(userID);
            myAccount.Show();
            this.Hide();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
