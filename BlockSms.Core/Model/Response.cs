using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.Model
{
    public class Response
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        public string Message { get; set; }

        public dynamic Result { get; set; }
    }
}
