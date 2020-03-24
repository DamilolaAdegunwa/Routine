﻿using BlockSms.Core.Extension;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockSms.Core.Uow
{
    internal class ChildUnitOfWork : IUnitOfWork
    {
        public Guid Id => _parent.Id;

        public IUnitOfWorkOptions Options => _parent.Options;

        public IUnitOfWork Outer => _parent.Outer;

        public bool IsReserved => _parent.IsReserved;

        public string ReservationName => _parent.ReservationName;

        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        public event EventHandler<UnitOfWorkEventArgs> Disposed;

        public IServiceProvider ServiceProvider => _parent.ServiceProvider;

        private readonly IUnitOfWork _parent;

        public ChildUnitOfWork([NotNull] IUnitOfWork parent)
        {
            Check.NotNull(parent, nameof(parent));

            _parent = parent;

            _parent.Failed += (sender, args) => { Failed.InvokeSafely(sender, args); };
            _parent.Disposed += (sender, args) => { Disposed.InvokeSafely(sender, args); };
        }

        public void SetOuter(IUnitOfWork outer)
        {
            _parent.SetOuter(outer);
        }

        public void Initialize(UnitOfWorkOptions options)
        {
            _parent.Initialize(options);
        }

        public void Reserve(string reservationName)
        {
            _parent.Reserve(reservationName);
        }

        public void SaveChanges()
        {
            _parent.SaveChanges();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _parent.SaveChangesAsync(cancellationToken);
        }

        public void Complete()
        {

        }

        public Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Rollback()
        {
            _parent.Rollback();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _parent.RollbackAsync(cancellationToken);
        }

        public void OnCompleted(Func<Task> handler)
        {
            _parent.OnCompleted(handler);
        }

        public IDatabaseApi FindDatabaseApi(string key)
        {
            return _parent.FindDatabaseApi(key);
        }

        public void AddDatabaseApi(string key, IDatabaseApi api)
        {
            _parent.AddDatabaseApi(key, api);
        }

        public IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory)
        {
            return _parent.GetOrAddDatabaseApi(key, factory);
        }

        public ITransactionApi FindTransactionApi(string key)
        {
            return _parent.FindTransactionApi(key);
        }

        public void AddTransactionApi(string key, ITransactionApi api)
        {
            _parent.AddTransactionApi(key, api);
        }

        public ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory)
        {
            return _parent.GetOrAddTransactionApi(key, factory);
        }

        public void Dispose()
        {

        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}