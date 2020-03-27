using BlockSms.Core.Domain.Entities;
using BlockSms.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    public static class EfCoreRepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity, TKey>(this IBasicRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            return repository.ToEfCoreRepository().DbContext;
        }

        public static DbSet<TEntity> GetDbSet<TEntity, TKey>(this IBasicRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            return repository.ToEfCoreRepository().DbSet;
        }

        public static IEfCoreRepository<TEntity, TKey> ToEfCoreRepository<TEntity, TKey>(this IBasicRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            var efCoreRepository = repository as IEfCoreRepository<TEntity, TKey>;
            if (efCoreRepository == null)
            {
                throw new ArgumentException("Given repository does not implement " + typeof(IEfCoreRepository<TEntity, TKey>).AssemblyQualifiedName, nameof(repository));
            }

            return efCoreRepository;
        }
    }
}
