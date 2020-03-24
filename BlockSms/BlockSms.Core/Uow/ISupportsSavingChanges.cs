using System.Threading;
using System.Threading.Tasks;

namespace BlockSms.Core.Uow
{
    public interface ISupportsSavingChanges
    {
        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
