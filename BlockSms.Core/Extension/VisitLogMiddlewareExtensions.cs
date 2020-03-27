using BlockSms.Core.Model;
using BlockSms.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockSms.Core.Middlewares
{
    public class VisitLogMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private VisitLog visitLog;

        public VisitLogMiddleware(RequestDelegate next, ILogger<VisitLogMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                visitLog = new VisitLog();
                var request = context.Request;
                visitLog.RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString();
                visitLog.Url = request.Path.ToString() + context.Request.QueryString.Value;
                visitLog.Headers = request.Headers.ToDictionary(k => k.Key, v => string.Join(";", v.Value.ToList()));
                visitLog.Method = request.Method;
                visitLog.ExcuteStartTime = DateTime.Now;
                context.Request.EnableRewind();
                var encoding = GetEncoding(request.ContentType);
                await ReadRequestBodyAsync(context.Request.Body, encoding);
                _logger.LogInformation(visitLog.ToString());
                if (context.Request != null && context.Request.Path != null && !context.Request.Path.Value.Contains("swagger"))
                {
                    EnableReadAsync(context.Response);
                    context.Response.OnCompleted(async o =>
                    {
                        if (o is HttpContext c)
                        {
                           await ReadBodyAsync(c.Response).ConfigureAwait(false);
                        }
                    }, context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "记录请求内容时出错");
            }
            finally
            {
                await _next.Invoke(context);
            }
        }
        private async Task ReadRequestBodyAsync(Stream inputStream,Encoding encoding)
        {
            if (!inputStream.CanRead) return;
            var memery = new System.IO.MemoryStream();
            inputStream.CopyTo(memery);
            
            using (var reader = new StreamReader(memery, encoding))
            {
                memery.Position = 0;
                visitLog.RequestBody = await reader.ReadToEndAsync();
            }
            inputStream.Position = 0;
        }
        private async Task ReadBodyAsync(HttpResponse response)
        {
            if (response.Body.Length <= 0) return;
            response.Body.Seek(0, SeekOrigin.Begin);
            var encoding = GetEncoding(response.ContentType);
            var body = await ReadStreamAsync(response.Body, encoding, false).ConfigureAwait(false);
            _logger.LogInformation($"Response {response.StatusCode} {body}");
        }
        private async Task<string> ReadStreamAsync(Stream stream, Encoding encoding, bool forceSeekBeginZero = true)
        {
            using (var sr = new StreamReader(stream, encoding, true, 1024, true))
            {
                var str = await sr.ReadToEndAsync();
                if (forceSeekBeginZero)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                return str;
            }
        }
        private static Encoding GetEncoding(string contentType)
        {
            var mediaType = contentType == null ? default(MediaType) : new MediaType(contentType);
            var encoding = mediaType.Encoding;
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding;
        }

        private static void EnableReadAsync(HttpResponse response)
        {
            if (!response.Body.CanRead || !response.Body.CanSeek)
            {
                response.Body = new MemoryWrappedHttpResponseStream(response.Body);
            }
        }
    }
    public static class VisitLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseVisitLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VisitLogMiddleware>();
        }
    }
}
