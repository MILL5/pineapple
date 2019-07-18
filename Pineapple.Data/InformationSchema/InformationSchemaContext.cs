using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Data.InformationSchema
{
    public class InformationSchemaContext : BaseContext
    {
        private readonly string _connectionString;

        public InformationSchemaContext(string connectionString)
        {
            CheckIsNotNullOrWhitespace(nameof(connectionString), connectionString);

            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public virtual IQueryable<Table> Tables { get { return Set<Table>().AsNoTracking(); } }
        public virtual IQueryable<Column> Columns { get { return Set<Column>().AsNoTracking(); } }
        public virtual IQueryable<KeyColumn> KeyColumns { get { return Set<KeyColumn>().AsNoTracking(); } }
        public virtual IQueryable<TableConstraint> TableConstraints { get { return Set<TableConstraint>().AsNoTracking(); } }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Column>(entity =>
            {
                entity.ToTable("COLUMNS", "INFORMATION_SCHEMA");

                entity.HasKey(e => new { e.TableCatalog, e.TableSchema, e.TableName, e.ColumnName });

                entity.Property(e => e.TableCatalog)
                    .HasColumnName("TABLE_CATALOG")
                    .HasMaxLength(256);
                entity.Property(e => e.ColumnName)
                    .HasColumnName("COLUMN_NAME")
                    .HasMaxLength(256);
                entity.Property(e => e.DataType)
                    .HasColumnName("DATA_TYPE")
                    .HasMaxLength(256);
                entity.Property(e => e.TableName)
                    .HasColumnName("TABLE_NAME")
                    .HasMaxLength(256);
                entity.Property(e => e.TableSchema)
                    .HasColumnName("TABLE_SCHEMA")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<KeyColumn>(entity =>
            {
                entity.ToTable("KEY_COLUMN_USAGE", "INFORMATION_SCHEMA");

                entity.HasKey(e => new { e.TableCatalog, e.TableSchema, e.TableName, e.ColumnName });

                entity.Property(e => e.TableCatalog)
                    .HasColumnName("TABLE_CATALOG")
                    .HasMaxLength(256);
                entity.Property(e => e.ColumnName)
                    .HasColumnName("COLUMN_NAME")
                    .HasMaxLength(256);
                entity.Property(e => e.TableName)
                    .HasColumnName("TABLE_NAME")
                    .HasMaxLength(256);
                entity.Property(e => e.TableSchema)
                    .HasColumnName("TABLE_SCHEMA")
                    .HasMaxLength(256);
                entity.Property(e => e.Position)
                    .HasColumnName("ORDINAL_POSITION");
                entity.Property(e => e.ConstraintName)
                    .HasColumnName("CONSTRAINT_NAME");
            });

            modelBuilder.Entity<TableConstraint>(entity =>
            {
                entity.ToTable("TABLE_CONSTRAINTS", "INFORMATION_SCHEMA");

                entity.HasKey(e => new { e.TableCatalog, e.TableSchema, e.TableName, e.ConstraintName });

                entity.Property(e => e.TableCatalog)
                    .HasColumnName("TABLE_CATALOG")
                    .HasMaxLength(256);
                entity.Property(e => e.TableName)
                    .HasColumnName("TABLE_NAME")
                    .HasMaxLength(256);
                entity.Property(e => e.TableSchema)
                    .HasColumnName("TABLE_SCHEMA")
                    .HasMaxLength(256);
                entity.Property(e => e.ConstraintType)
                    .HasColumnName("CONSTRAINT_TYPE")
                    .HasMaxLength(256);
                entity.Property(e => e.ConstraintName)
                    .HasColumnName("CONSTRAINT_NAME")
                    .HasMaxLength(256);
            });
        }
    }
}
