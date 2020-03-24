using BlockSms.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EPT.Tickets.Self.Domain
{
    /// <summary>
    /// 支付记录信息表
    /// </summary>
    [Table("User")]
    public class User : Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// 商业机构ID
        /// </summary>
        public int DeptId { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 支付平台ID
        /// </summary>
        public int PlatformId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

    }
}
