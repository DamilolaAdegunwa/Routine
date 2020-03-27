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
    public class MessageAddCommandHandler : IRequestHandler<MessageAddCommand, bool>
    {
        private readonly IRepository<Message, int> _messageRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        /// <summary>
        /// 
        /// </summary>
        public MessageAddCommandHandler(
            IRepository<Message, int> messageRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(MessageAddCommand message, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var msg = new Message();
                msg.Phone = message.Phone;
                msg.MessageType = message.MessageType;
                msg.Used = false;
                msg.SendTime = DateTime.Now;
                await _messageRepository.InsertAsync(msg, true);
            }
            return true;
        }
    }
}
