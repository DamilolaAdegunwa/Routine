using BlockSms.Core.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BlockSms.Core.Web
{
    public class BlockControllerBase : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;
        public BlockControllerBase(IHttpContextAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }
        
        /// <summary>
        /// 解析请求头中的Token
        /// </summary>
        /// <returns></returns>
        protected string GetToken()
        {
            var req = _accessor.HttpContext.Request;
            var token = req.Headers["token"];
            if (string.IsNullOrEmpty(token))
            {
                _accessor.HttpContext.Response.StatusCode = 401;
                throw new Exception("没有找到token");
            }
            string key = DESEncryptHelper.Decrypt(token, DateTime.Now.ToString("yyyyMMdd"));
            if (string.IsNullOrEmpty(key))
            {
                _accessor.HttpContext.Response.StatusCode = 401;
                throw new Exception("无效的token");
            }
            return key;
        }
    }
}
