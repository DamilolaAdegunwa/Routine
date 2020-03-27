﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BlockSms.Core.Threading
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem);
        void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem, double delaySeconds);
        Task<Func<IBackgroundTaskQueue, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<IBackgroundTaskQueue, CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<IBackgroundTaskQueue, CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            _workItems.Enqueue(workItem);
            _signal.Release();
        }
        public void QueueBackgroundWorkItem(Func<IBackgroundTaskQueue, CancellationToken, Task> workItem, double delaySeconds)
        {
            Task.Delay(TimeSpan.FromSeconds(delaySeconds))
                .ContinueWith(t =>
                {
                    QueueBackgroundWorkItem(workItem);
                });
        }
        public async Task<Func<IBackgroundTaskQueue, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
