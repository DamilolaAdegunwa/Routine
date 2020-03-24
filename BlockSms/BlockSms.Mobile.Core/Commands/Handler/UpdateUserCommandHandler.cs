using BlockSms.Core.Repositories;
using BlockSms.Core.EntityFrameworkCore;
using BlockSms.Core.Uow;
using EPT.Tickets.Self.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EPT.Tickets.Self.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IRepository<User, int> _payRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        /// <summary>
        /// 
        /// </summary>
        public UpdateUserCommandHandler(
            IRepository<User, int> payRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _payRepository = payRepository ?? throw new ArgumentNullException(nameof(payRepository));
            _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UpdateUserCommand message, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var pay = await _payRepository.ToEfCoreRepository().FirstOrDefaultAsync(o => o.OrderNo == message.OrderNo);
                pay.ChannelId= 1;
                await _payRepository.UpdateAsync(pay, true);
            }
            return true;
        }
    }
}
