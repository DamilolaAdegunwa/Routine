using BlockSms.Core.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace BlockSms.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageAddCommand : IRequest<bool>
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; private set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public EMsgType MessageType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public MessageAddCommand(string phone, string code,EMsgType msgType)
        {
            Phone = phone;
            Code = code;
            MessageType = msgType;
        }
    }
}
