using BlockSms.Core.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace BlockSms.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRegisterCommand : IRequest<bool>
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string Phone { get; private set; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public UserRegisterCommand(string phone, string password)
        {
            Phone = phone;
            Password = password;
        }
    }
}
