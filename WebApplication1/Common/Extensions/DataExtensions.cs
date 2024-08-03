using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Common.Extensions
{
    public static class DataExtensions
    {
        public static bool ExecuteStoredProcedure(this DbContext context, string query, object parameter = null)
        {
            using var connection = (SqlConnection)context.Database.GetDbConnection();
            using var command = new SqlCommand(query, connection);
            var parameters = parameter.GetType().GetProperties()
                .Select(p => new SqlParameter($"@{p.Name}", p.GetValue(parameter)))
                .ToArray();
            command.Parameters.AddRange(parameters);
            command.CommandType = CommandType.StoredProcedure;
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var effected = command.ExecuteNonQuery();
            connection.Close();
            return effected > 0;
        }
    }
}
