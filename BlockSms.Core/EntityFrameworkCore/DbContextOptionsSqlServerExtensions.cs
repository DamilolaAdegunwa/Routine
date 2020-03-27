using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    public static class DbContextOptionsSqlServerExtensions
    {
        public static DbContextOptionsBuilder UseSqlServer(
            [NotNull] this DbContextConfigurationContext context,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseSqlServer(context.ExistingConnection, sqlServerOptionsAction);
            }
            else
            {
                return context.DbContextOptions.UseSqlServer(context.ConnectionString, sqlServerOptionsAction);
            }
        }

        public static void UseSqlServer(
            [NotNull] this EPTDbContextOptions options,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            options.Configure(context =>
            {
                context.UseSqlServer(sqlServerOptionsAction);
            });
        }

        public static void UseSqlServer<TDbContext>(
            [NotNull] this EPTDbContextOptions options,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
            where TDbContext : EPTDbContext<TDbContext>
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseSqlServer(sqlServerOptionsAction);
            });
        }
    }
}
