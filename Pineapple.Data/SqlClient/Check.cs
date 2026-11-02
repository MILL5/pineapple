using Microsoft.Data.SqlClient;
using System;
using static Pineapple.Common.Preconditions;
using static Pineapple.Health.Check;

namespace Pineapple.Data.SqlClient
{
    public static class Check
    {
        public static bool IsSqlServerAvailable(string connectionString)
        {
            CheckIsNotNullOrWhitespace(nameof(connectionString), connectionString);

            var csb = new SqlConnectionStringBuilder(connectionString);
            var datasource = csb.DataSource;
            if (datasource.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
                datasource = datasource.Substring(4);

            var parts = datasource.Split(',');

            var server = parts[0];

            const int DEFAULT_PORT = 1433;

            int port = DEFAULT_PORT;

            if (parts.Length == 2)
            {
                if (!int.TryParse(parts[1], out port) || port < 1 || port > 65535)
                    return false;
            }

            const int NUM_OF_RETRIES = 3;

            int count = 0;

            while (count++ < NUM_OF_RETRIES)
            {
                if (IsAlive(server, port))
                    return true;
            }

            return false;
        }

        public static bool IsSqlServerAvailable(this SqlConnection connection)
        {
            return IsSqlServerAvailable(connection.ConnectionString);
        }
    }
}
