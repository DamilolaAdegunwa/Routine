using System.Threading;

namespace BlockSms.Core.Threading
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}
