using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EstacionamentoCRUD.DAL
{
    public static class DataAccess
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EstacionamentoDB"].ConnectionString;
        }

        public static int ExecuteNonQuery(string commandText, 
            SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteDataTable(string commandText, 
            SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var dataAdapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
        
        public static object ExecuteScalar(string commandText, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
