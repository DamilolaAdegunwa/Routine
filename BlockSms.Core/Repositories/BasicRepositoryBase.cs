using BlockSms.Core.DependencyInjection;
using BlockSms.Core.Domain.Entities;
using BlockSms.Core.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockSms.Core.Repositories
{
    public abstract class BasicRepositoryBase<TEntity> :
        IBasicRepository<TEntity>,
        IServiceProviderAccessor,
        ITransientDependency
        where TEntity : class, IEntity
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ICancellationTokenProvider CancellationTokenProvider { get; set; }

        protected BasicRepositoryBase()
        {
            CancellationTokenProvider = NullCancellationTokenProvider.Instance;
        }

        public abstract TEntity Insert(TEntity entity, bool autoSave = false);

        public virtual Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Insert(entity, autoSave));
        }

        public abstract TEntity Update(TEntity entity, bool autoSave = false);

        public virtual Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity));
        }

        public abstract void Delete(TEntity entity, bool autoSave = false);

        public virtual Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            Delete(entity);
            return Task.CompletedTask;
        }

        protected virtual CancellationToken GetCancellationToken(CancellationToken prefferedValue = default)
        {
            return CancellationTokenProvider.FallbackToProvider(prefferedValue);
        }

        public abstract List<TEntity> GetList(bool includeDetails = false);

        public virtual Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetList(includeDetails));
        }

        public abstract long GetCount();

        public virtual Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetCount());
        }
    }
}
