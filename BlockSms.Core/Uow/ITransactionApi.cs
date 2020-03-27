using System;
using System.Threading.Tasks;

namespace BlockSms.Core.Uow
{
    public interface ITransactionApi : IDisposable
    {
        void Commit();

        Task CommitAsync();
    }
}
