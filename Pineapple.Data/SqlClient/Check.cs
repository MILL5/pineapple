using Microsoft.Data.SqlClient;
using System;
using static Pineapple.Common.Preconditions;
#pragma warning disable PINE006 // Both checks are deprecated together.
using static Pineapple.Health.Check;
#pragma warning restore PINE006

namespace Pineapple.Data.SqlClient
{
    [Obsolete("Use Microsoft.Extensions.Diagnostics.HealthChecks with AspNetCore.HealthChecks.Uris, .Network, or .SqlServer; for Azure SQL serverless, use EF Core EnableRetryOnFailure.", DiagnosticId = "PINE006", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public static class Check
    {
        public static bool IsSqlServerAvailable(string connectionString)
        {
            CheckIsNotNullOrWhitespace(nameof(connectionString), connectionString);

            var csb = new SqlConnectionStringBuilder(connectionString);
            var datasource = csb.DataSource;

            var parts = datasource.Split(',');

            var server = parts[0];

            const int DEFAULT_PORT = 1433;

            int port = DEFAULT_PORT;

            if (parts.Length == 2)
            {
                int.TryParse(parts[1], out port);
            }

            const int NUM_OF_RETRIES = 3;

            int count = 0;

            while (count < NUM_OF_RETRIES && !IsAlive(server, port))
            {
                count++;
            }
            
            return true;
        }

        public static bool IsSqlServerAvailable(this SqlConnection connection)
        {
            return IsSqlServerAvailable(connection.ConnectionString);
        }
    }
}
