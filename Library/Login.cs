using Oracle.DataAccess.Client;
using System;
using System.Windows.Forms;

namespace Library
{
    public partial class Login : Form
    {
        private DatabaseHelper dbHelper;


        public Login()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper("Data Source=xe; User ID=Kaze; Password=123");
        }

        private void LoginAttempt()
        {
            string email = tbEmail.Text;
            string password = tbPassword.Text;

            using (OracleConnection connection = dbHelper.GetOpenConnection())
            {


                string query = "SELECT id, is_Admin FROM Users WHERE Email = :email AND Password = :password";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":email", OracleDbType.Varchar2).Value = email;
                    command.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(reader.GetOrdinal("id"));
                            string isAdmin = reader.GetString(reader.GetOrdinal("is_Admin"));

                            if (isAdmin == "Y")
                            {

                                Catalog catalog = new Catalog(userId);
                                catalog.Show();
                                this.Hide();
                            }
                            else
                            {
                                Dashboard dashboard = new Dashboard(userId);
                                dashboard.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            lbMessage.Text = "Invalid email or password";
                            tbEmail.Focus();
                        }
                    }
                }
            }
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            LoginAttempt();

        }

        private void label1_Click(object sender, EventArgs e)
        {
            Circulation admin = new Circulation(1);
            admin.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoginAttempt();
            }

        }

        private void tbPassword_Enter(object sender, EventArgs e)
        {
            lbMessage.Text = "Please login to continue";
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
