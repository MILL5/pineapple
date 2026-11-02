using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Data.InformationSchema;

namespace Pineapple.Data.Tests;

[TestClass]
[TestCategory("SqlIntegration")]
public class SqlIntegrationTests
{
    [TestMethod]
    public async Task QueriesAllMetadataCollectionsAgainstSqlServer()
    {
        var connectionString = Environment.GetEnvironmentVariable("PINEAPPLE_SQL_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (Environment.GetEnvironmentVariable("PINEAPPLE_REQUIRE_SQL_TESTS") == "1")
                Assert.Fail("SQL integration is required but PINEAPPLE_SQL_CONNECTION is missing.");
            Assert.Inconclusive("SQL integration not run: set PINEAPPLE_SQL_CONNECTION to provisioned SQL Server.");
        }
        var name = "Pineapple_" + Guid.NewGuid().ToString("N");
        var fixtureConnection = new SqlConnectionStringBuilder(connectionString) { Pooling = false };
        await using var connection = new SqlConnection(fixtureConnection.ConnectionString);
        await connection.OpenAsync();
        // Framework test processes share a database: serialize catalog DDL and reads.
        // This session-scoped lock is released even on failure when connection closes.
        await using (var gate = connection.CreateCommand())
        {
            gate.CommandTimeout = 65;
            gate.CommandText = "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = 'Pineapple_Metadata_Integration', @LockMode = 'Exclusive', " +
                "@LockOwner = 'Session', @LockTimeout = 60000; SELECT @result;";
            var result = (int)await gate.ExecuteScalarAsync();
            Assert.IsGreaterThanOrEqualTo(0, result, "Timed out acquiring the SQL metadata fixture lock.");
        }
        await using var create = connection.CreateCommand();
        create.CommandText = $"CREATE TABLE [dbo].[{name}] ([Id] int NOT NULL PRIMARY KEY, [Value] nvarchar(20) NULL)";
        await create.ExecuteNonQueryAsync();
        try
        {
            using var context = new InformationSchemaContext(connectionString);
            Assert.IsTrue(await context.Tables.AnyAsync(t => t.TableName == name && t.TableSchema == "dbo"));
            Assert.AreEqual(2, await context.Columns.CountAsync(c => c.TableName == name));
            Assert.AreEqual("Id", (await context.KeyColumns.SingleAsync(k => k.TableName == name)).ColumnName);
            Assert.AreEqual("PRIMARY KEY", (await context.TableConstraints.SingleAsync(c => c.TableName == name)).ConstraintType);
        }
        finally
        {
            await using var drop = connection.CreateCommand();
            drop.CommandText = $"DROP TABLE [dbo].[{name}]";
            await drop.ExecuteNonQueryAsync();
        }
    }
}
