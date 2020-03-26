using BlockSms.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlockSms.Core.Domain
{
    /// <summary>
    /// 支付记录信息表
    /// </summary>
    [Table("User")]
    public class User : Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string  Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        public EUserState UserState { get; set; }
        /// <summary>
        /// 用户配置
        /// </summary>
        public string UserConfig { get; set; }

    }

    public enum EUserState
    {
        Close=0,
        Open=1
    }
    public enum UserType
    {
        Visitor=0,
        Customer=1,
        Vip=2,
    }
}
