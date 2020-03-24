using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.DapperExtension.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    public class PrefixCoreClassMapper<T> : ClassMapper<T> where T : class
    {
        public PrefixCoreClassMapper()
        {
            Type type = typeof(T);
            Table("Core_"+type.Name);
            AutoMap();
            UnMap("DomainEvents");
        }
    }
}
