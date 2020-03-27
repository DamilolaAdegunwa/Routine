using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.DependencyInjection
{
    public interface IObjectAccessor<out T>
    {
        [CanBeNull]
        T Value { get; }
    }
}
