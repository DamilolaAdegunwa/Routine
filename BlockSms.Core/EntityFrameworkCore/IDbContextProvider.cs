using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : IEfCoreDbContext
    {
        TDbContext GetDbContext();
    }
}
