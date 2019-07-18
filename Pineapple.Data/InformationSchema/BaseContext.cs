using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pineapple.Data.InformationSchema
{
    public abstract class BaseContext : DbContext
    {
        protected IQueryable<T> GetEntity<T>() where T : class
        {
            return Set<T>().AsNoTracking();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }
    }
}
