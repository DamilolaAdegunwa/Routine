using BlockSms.Core.EntityFrameworkCore;
using BlockSms.Core.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace  BlockSms.Mobile.Core.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class MobileContext : EPTDbContext<MobileContext>
    {
        /// <summary>
        /// 
        /// </summary>
        public DbSet<User> Self_Equipments { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public MobileContext(DbContextOptions<MobileContext> options, IMediator mediator, ILoggerFactory loggerFactory)
            : base(options, mediator, loggerFactory)
        { }
    }
}
