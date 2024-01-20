using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public OracleConnection GetOpenConnection()
        {
            OracleConnection connection = new OracleConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                // Handle connection errors appropriately
                Console.WriteLine($"Error opening database connection: {ex.Message}");
                connection.Dispose(); // Ensure resources are properly released
                throw; // Rethrow the exception for the calling code to handle
            }

            return connection;
        }
    }
}
