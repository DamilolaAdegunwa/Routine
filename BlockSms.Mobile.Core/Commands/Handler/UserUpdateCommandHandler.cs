using BlockSms.Core.Repositories;
using BlockSms.Core.EntityFrameworkCore;
using BlockSms.Core.Uow;
using BlockSms.Core.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockSms.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class UserUpdateCommandHandler : IRequestHandler<UserUpdateCommand, bool>
    {
        private readonly IRepository<User, int> _payRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        /// <summary>
        /// 
        /// </summary>
        public UserUpdateCommandHandler(
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
        public async Task<bool> Handle(UserUpdateCommand message, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var user = await _payRepository.ToEfCoreRepository().FirstOrDefaultAsync(o => o.Phone == message.Phone);
                user.Password= message.Password;
                user.UserConfig = message.UserConfig;
                user.Email = message.Email;
                await _payRepository.UpdateAsync(user, true);
            }
            return true;
        }
    }
}
