using BlockSms.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using BlockSms.Core.Extension;

namespace BlockSms.Core.EntityFrameworkCore
{
    public class DefaultConnectionStringResolver : IConnectionStringResolver, ITransientDependency
    {
        protected DbConnectionOptions Options { get; }

        public DefaultConnectionStringResolver(IOptionsSnapshot<DbConnectionOptions> options)
        {
            Options = options.Value;
        }

        public virtual string Resolve(string connectionStringName = null)
        {
            //Get module specific value if provided
            if (!connectionStringName.IsNullOrEmpty())
            {
                var moduleConnString = Options.ConnectionStrings.GetOrDefault(connectionStringName);
                if (!moduleConnString.IsNullOrEmpty())
                {
                    return moduleConnString;
                }
            }

            //Get default value
            return Options.ConnectionStrings.Default;
        }
    }
}
