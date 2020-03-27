using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.DependencyInjection
{
    public interface IServiceProviderAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
}
