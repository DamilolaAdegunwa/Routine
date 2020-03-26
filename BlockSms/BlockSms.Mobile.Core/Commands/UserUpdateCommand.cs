using BlockSms.Core.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace BlockSms.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class UserUpdateCommand : IRequest<bool>
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; private set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// 用户配置
        /// </summary>
        public string UserConfig { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public UserUpdateCommand(string phone,string password, string email,string userConfig)
        {
            Password = password;
            Email = email;
            UserConfig = userConfig;
            Phone =phone;
        }
    }
}
