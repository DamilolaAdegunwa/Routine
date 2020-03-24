using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.DependencyInjection
{
    public interface IExposedServiceTypesProvider
    {
        Type[] GetExposedServiceTypes(Type targetType);
    }
}
