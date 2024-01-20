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
using System.Xml.Linq;

namespace Library
{
    public partial class Circulation : Form
    {
        private DatabaseHelper dbHelper;
        public int userID;
        public string name;
        public Circulation(int userID)
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
    }
}
