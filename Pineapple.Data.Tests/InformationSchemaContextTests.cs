using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Data.InformationSchema;

namespace Pineapple.Data.Tests;

[TestClass]
public class InformationSchemaContextTests
{
    [TestMethod]
    public void AllMetadataCollectionsHaveViewMappingsAndKeys()
    {
        using var context = new InformationSchemaContext("Server=localhost;Database=metadata;Integrated Security=true;TrustServerCertificate=true");
        foreach (var type in new[] { typeof(Table), typeof(Column), typeof(KeyColumn), typeof(TableConstraint) })
        {
            var entity = context.Model.FindEntityType(type);
            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.FindPrimaryKey());
            Assert.AreEqual("INFORMATION_SCHEMA", entity.GetViewSchema());
        }
        StringAssert.Contains(context.Tables.ToQueryString(), "[INFORMATION_SCHEMA].[TABLES]");
        StringAssert.Contains(context.Columns.ToQueryString(), "[INFORMATION_SCHEMA].[COLUMNS]");
        StringAssert.Contains(context.KeyColumns.ToQueryString(), "[INFORMATION_SCHEMA].[KEY_COLUMN_USAGE]");
        StringAssert.Contains(context.TableConstraints.ToQueryString(), "[INFORMATION_SCHEMA].[TABLE_CONSTRAINTS]");
        Assert.AreEqual(QueryTrackingBehavior.NoTracking, context.ChangeTracker.QueryTrackingBehavior);
    }
}
