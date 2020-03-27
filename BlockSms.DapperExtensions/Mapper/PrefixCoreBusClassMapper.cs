using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.DapperExtension.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    public class PrefixCoreBusClassMapper<T> : ClassMapper<T> where T : class
    {
        public PrefixCoreBusClassMapper()
        {
            Type type = typeof(T);
            Table("Core_Bus_" + type.Name);
            AutoMap();
            UnMap("DomainEvents");
        }
    }
}
