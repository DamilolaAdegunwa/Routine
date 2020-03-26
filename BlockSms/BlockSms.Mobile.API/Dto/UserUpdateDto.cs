using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 用户注册(Input)
    /// </summary>
    public class UserUpdateDto
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户配置
        /// </summary>
        public string UserConfig { get; set; }

    }
}
