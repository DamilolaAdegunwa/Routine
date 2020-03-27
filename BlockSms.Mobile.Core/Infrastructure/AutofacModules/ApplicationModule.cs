using Autofac;

namespace BlockSms.Mobile.Core.Infrastructure.AutofacModules
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationModule : Autofac.Module
    {
        /// <summary>
        /// 
        /// </summary>
        public string QueriesConnectionString { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qconstr"></param>
        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterAssemblyTypes(typeof(CreatOrderCommandHandler).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        }
    }
}
