using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 记录日志的过滤器
    /// </summary>
    public class LogFilter : ApiActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnBeginRequestAsync(ApiActionContext context)
        {
            try
            {
                var content = context.RequestMessage.Content.ReadAsStringAsync().Result;
                var dic = context.RequestMessage.Headers.ToDictionary(k => k.Key, v => string.Join(";", v.Value.ToList()));
                string headers = "[" + string.Join(",", dic.Select(i => "{" + $"\"{i.Key}\":\"{i.Value}\"" + "}")) + "]";
                var msg = $"{context.RequestMessage.Method}{headers} {context.RequestMessage.RequestUri} {content}";
                Logger?.LogDebug(msg);
                //context.Tags.Set("BeginTime", DateTime.Now);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "记录请求内容出错：" + ex.Message);
            }
            return base.OnBeginRequestAsync(context);
        }

        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnEndRequestAsync(ApiActionContext context)
        {
            try
            {
                var request = context.RequestMessage;
                var result = await context.ResponseMessage.Content.ReadAsStringAsync();
                var msg = $"{request.Method} {request.RequestUri.AbsolutePath} 响应内容：{result}";
                Logger?.LogDebug(msg);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "记录回调响应内容出错：" + ex.Message);
            }
        }
    }
}
