using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BloodDonorManagementSystem.Infrastructure
{
    public static class Db
    {
        private static readonly string ConnectionString =
            ConfigurationManager
                .ConnectionStrings["BloodDonorDb"]
                .ConnectionString;


        // =========================================================
        // OPEN DATABASE CONNECTION
        // =========================================================

        public static SqlConnection OpenConnection()
        {
            var connection =
                new SqlConnection(ConnectionString);

            connection.Open();

            return connection;
        }


        // =========================================================
        // GET DATATABLE
        // =========================================================

        public static DataTable GetDataTable(
            string sql,
            params SqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var connection = OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                AddParameters(command, parameters);

                adapter.Fill(table);
            }

            return table;
        }


        // =========================================================
        // EXECUTE SCALAR
        // =========================================================

        public static object ExecuteScalar(
            string sql,
            params SqlParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddParameters(command, parameters);

                return command.ExecuteScalar();
            }
        }


        // =========================================================
        // EXECUTE NON QUERY
        // =========================================================

        public static int ExecuteNonQuery(
            string sql,
            params SqlParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddParameters(command, parameters);

                return command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // EXECUTE READER
        // =========================================================

        public static SqlDataReader ExecuteReader(
            string sql,
            params SqlParameter[] parameters)
        {
            var connection = OpenConnection();

            try
            {
                var command =
                    new SqlCommand(sql, connection);

                AddParameters(command, parameters);

                return command.ExecuteReader(
                    CommandBehavior.CloseConnection);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }


        // =========================================================
        // ADD SQL PARAMETERS
        // =========================================================

        private static void AddParameters(
            SqlCommand command,
            SqlParameter[] parameters)
        {
            if (parameters == null ||
                parameters.Length == 0)
            {
                return;
            }

            command.Parameters.AddRange(parameters);
        }
    }
}

