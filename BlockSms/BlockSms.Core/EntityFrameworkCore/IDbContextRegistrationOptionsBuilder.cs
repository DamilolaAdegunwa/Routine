using BlockSms.Core.DependencyInjection;
using BlockSms.Core.Domain.Entities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    public interface IDbContextRegistrationOptionsBuilder : ICommonDbContextRegistrationOptionsBuilder
    {
        void Entity<TEntity>([NotNull] Action<EntityOptions<TEntity>> optionsAction)
            where TEntity : IEntity;
    }
}
