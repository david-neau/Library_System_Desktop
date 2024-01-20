using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public partial class Catalog : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        string name;

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
    }
}
