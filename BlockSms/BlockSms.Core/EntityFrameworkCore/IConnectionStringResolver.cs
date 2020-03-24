using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    public interface IConnectionStringResolver
    {
        [NotNull]
        string Resolve(string connectionStringName = null);
    }
}
