using EPT.Tickets.Self.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace EPT.Tickets.Self.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserCommand : IRequest<bool>
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; private set; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string Buy_OrderNo { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public UpdateUserCommand(string orderNo, string buy_OrderNo)
        {
            OrderNo = orderNo;
            Buy_OrderNo = buy_OrderNo;
        }
    }
}
