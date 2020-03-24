using BlockSms.Core.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.EntityFrameworkCore
{
    [Serializable]
    public class ConnectionStrings : Dictionary<string, string>
    {
        public const string DefaultConnectionStringName = "Default";

        public string Default
        {
            get => this.GetOrDefault(DefaultConnectionStringName);
            set => this[DefaultConnectionStringName] = value;
        }
    }
}
