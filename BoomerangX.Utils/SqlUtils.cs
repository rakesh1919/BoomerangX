using System.Data.SqlClient;

namespace BoomerangX.Utils
{
    public class SqlUtils
    {
        public string serverName = string.Empty;

        public string databaseName = string.Empty;

        public string userName = string.Empty;

        public string passKey = string.Empty;

        public SqlConnection conn = null;

        public SqlUtils()
        {
            this.serverName = "motncwldjn.database.windows.net";
            this.databaseName = "boomerangx_db";
            this.userName = "boomerangx@motncwldjn";
            this.passKey = "V83b39xsU8Lp";
            this.conn = null;
        }

        public string GetConnectionString()
        {
            // To avoid storing the connection string in your code,  
            // you can retrieve it from a configuration file, using the  
            // System.Configuration.ConfigurationSettings.AppSettings property  
            string connectionStringFormat = "Data Source=tcp:{0},1433;Initial Catalog={1};User Id={2};Password={3}";
            return string.Format(connectionStringFormat, this.serverName, this.databaseName, this.userName, this.passKey);
        }

        public SqlConnection GetNewSqlConnection()
        {
            SqlConnection conn = new SqlConnection(this.GetConnectionString());
            return conn;
        }

        public int ExecuteNonQuery(string query)
        {
            if (conn == null)
            {
                conn = this.GetNewSqlConnection();
            }
            SqlCommand command = new SqlCommand(query, conn);
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            int noOfRowsaffected = command.ExecuteNonQuery();
            return noOfRowsaffected;
        }

        public void CloseConnection(SqlConnection conn)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();

            }
        }


        public SqlDataReader ExecuteQuery(string query)
        {
            if (conn == null)
            {
                conn = this.GetNewSqlConnection();
            }

            SqlCommand command = new SqlCommand(query, conn);
            if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();
            return command.ExecuteReader();
        }

    }
}
