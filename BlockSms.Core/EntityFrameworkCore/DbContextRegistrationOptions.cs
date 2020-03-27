using BlockSms.Core.DependencyInjection;
using BlockSms.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    public class DbContextRegistrationOptions : CommonDbContextRegistrationOptions, IDbContextRegistrationOptionsBuilder
    {
        public Dictionary<Type, object> EntityOptions { get; }

        public DbContextRegistrationOptions(Type originalDbContextType, IServiceCollection services)
            : base(originalDbContextType, services)
        {
            EntityOptions = new Dictionary<Type, object>();
        }

        public void Entity<TEntity>(Action<EntityOptions<TEntity>> optionsAction) where TEntity : IEntity
        {
            Services.Configure<EntityOptions>(options =>
            {
                options.Entity(optionsAction);
            });
        }
    }
}
