using BlockSms.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlockSms.Core.Domain
{
    /// <summary>
    /// 消息发送记录
    /// </summary>
    [Table("Message")]
    public class Message : Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string  Phone { get; set; }
        /// <summary>
        /// 是否使用
        /// </summary>
        public bool Used { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public EMsgType MessageType { get; set; }

    }


    public enum EMsgType
    { 
        验证码=1,
        消息=2
    }


}
