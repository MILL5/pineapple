using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Data.InformationSchema;

namespace Pineapple.Tests;

[TestClass]
public class InformationSchemaTests
{
    [TestMethod]
    [DataRow("TABLES")]
    [DataRow("COLUMNS")]
    [DataRow("KEY_COLUMN_USAGE")]
    [DataRow("TABLE_CONSTRAINTS")]
    public void PublicSchemaQueriesTranslateWithoutOpeningAConnection(string view)
    {
        using var context = new InformationSchemaContext("Server=localhost;Database=unused;Integrated Security=true");
        var sql = view switch
        {
            "TABLES" => context.Tables.Where(x => x.TableName == "Orders").ToQueryString(),
            "COLUMNS" => context.Columns.Where(x => x.TableName == "Orders").ToQueryString(),
            "KEY_COLUMN_USAGE" => context.KeyColumns.Where(x => x.TableName == "Orders").ToQueryString(),
            "TABLE_CONSTRAINTS" => context.TableConstraints.Where(x => x.TableName == "Orders").ToQueryString(),
            _ => throw new ArgumentOutOfRangeException(nameof(view))
        };

        StringAssert.Contains(sql, $"[INFORMATION_SCHEMA].[{view}]");
        StringAssert.Contains(sql, "[TABLE_CATALOG]");
        StringAssert.Contains(sql, "[TABLE_SCHEMA]");
        StringAssert.Contains(sql, "[TABLE_NAME]");
        Assert.AreEqual(QueryTrackingBehavior.NoTracking, context.ChangeTracker.QueryTrackingBehavior);
    }
}
