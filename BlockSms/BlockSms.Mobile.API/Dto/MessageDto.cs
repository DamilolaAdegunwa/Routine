using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 短信实体(Input)
    /// </summary>
    public class MessageInputDto
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string molile { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string code { get; set; }
    }
    /// <summary>
    /// 短信实体(Ouput)
    /// </summary>
    public class MessageOuputDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public MessageOutCode code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }
    }

    /// <summary>
    /// 短信实体(Ouput)CODE
    /// </summary>
    public enum MessageOutCode
    {
#pragma warning disable CS1591
        成功 = 0,
        失败 = 1,
        参数校验失败_参数不能为空 = 2,
        余额不足 = 3,
        用户不存在 = 4,
        未开通HTTP协议接口权限 = 5,
        sign校验失败 = 6,
        IP校验失败 = 7,
        time时间格式错误 = 8,
        扩展号格式错误 = 9,
        发送内容包含敏感词 = 10,
        短信内容过长 = 11,
        ts校验有误要求5分钟内有效 = 14,
        号码个数超过限制 = 15,
        超出可定时发送时间 = 16,
        超出可发送时间范围 = 17
#pragma warning restore CS1591

    }
}
