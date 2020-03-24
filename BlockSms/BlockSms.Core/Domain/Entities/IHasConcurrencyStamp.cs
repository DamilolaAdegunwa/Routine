using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.Domain.Entities
{
    public interface IHasConcurrencyStamp
    {
        string ConcurrencyStamp { get; set; }
    }
}
