using BlockSms.Core.DependencyInjection;
using BlockSms.Core.Domain.Entities;
using BlockSms.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nito.AsyncEx;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BlockSms.Core.EntityFrameworkCore
{
    public abstract class EPTDbContext<TDbContext> : DbContext, IEfCoreDbContext, ITransientDependency
        where TDbContext : DbContext
    {
        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo
            = typeof(EPTDbContext<TDbContext>)
                .GetMethod(
                    nameof(ConfigureGlobalFilters),
                    BindingFlags.Instance | BindingFlags.NonPublic
                );
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediator _mediator;
        protected EPTDbContext(DbContextOptions<TDbContext> options, IMediator mediator, ILoggerFactory loggerFactory)
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureConcurrencyStamp(entityType);

                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            try
            {
                ChangeTracker.AutoDetectChangesEnabled = false; //TODO: Why this is needed?
                ApplyConcepts();
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                AsyncContext.Run(() => _mediator.DispatchDomainEventsAsync<int>(this));
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new EPTDbConcurrencyException(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (_loggerFactory != null)
                optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
        protected virtual void ConfigureConcurrencyStamp(IMutableEntityType entityType)
        {
            if (!typeof(IHasConcurrencyStamp).GetTypeInfo().IsAssignableFrom(entityType.ClrType))
                return;
            entityType.GetProperties()
                .First(p => p.Name == nameof(IHasConcurrencyStamp.ConcurrencyStamp))
                .IsConcurrencyToken = true;
        }

        protected virtual void ApplyConcepts()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyConceptsAsync(entry);
            }
        }

        protected virtual void ApplyConceptsAsync(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    break;
                case EntityState.Modified:
                    HandleConcurrencyStamp(entry);
                    break;
                case EntityState.Deleted:
                    HandleConcurrencyStamp(entry);
                    break;
            }
        }

        protected virtual void HandleConcurrencyStamp(EntityEntry entry)
        {
            if (!(entry.Entity is IHasConcurrencyStamp entity))
            {
                return;
            }

            entity.ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            return expression;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
