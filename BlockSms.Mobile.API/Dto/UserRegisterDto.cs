using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 用户注册(Input)
    /// </summary>
    public class UserRegisterDto
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string molile { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

    }
}
