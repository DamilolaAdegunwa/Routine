using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockSms.Core.Model
{
    public class VisitLog
    {
        public string RemoteIpAddress { get; set; }
        public string Url { get; set; }

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public string Method { get; set; }

        public string RequestBody { get; set; }
        
        public DateTime ExcuteStartTime { get; set; }

        public override string ToString()
        {
            string headers = "[" + string.Join(",", this.Headers.Select(i => "{" + $"\"{i.Key}\":\"{i.Value}\"" + "}")) + "]";
            return $"{this.Method} {this.RemoteIpAddress} {this.Url} \r\n Headers: {headers},\r\n RequestBody: {this.RequestBody},\r\n ExcuteStartTime: {this.ExcuteStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
        }
    }
}
