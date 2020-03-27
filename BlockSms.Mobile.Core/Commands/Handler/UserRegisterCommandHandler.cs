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
    public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, bool>
    {
        private readonly IRepository<User, int> _userRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        /// <summary>
        /// 
        /// </summary>
        public UserRegisterCommandHandler(
            IRepository<User, int> userRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UserRegisterCommand message, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var user = new User();
                user.Name = message.Phone;
                user.Password = message.Password;
                user.Phone = message.Phone;
                user.UserState = EUserState.Open;
                user.UserType = UserType.Visitor;
                user.CreateTime = DateTime.Now;
                await _userRepository.InsertAsync(user, true);
            }
            return true;
        }
    }
}
