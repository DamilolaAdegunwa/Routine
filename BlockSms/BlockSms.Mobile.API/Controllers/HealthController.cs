using BlockSms.Core.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 服务状态监测
    /// </summary>
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("api/Health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// 检查服务状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get() => Ok("ok");
    }
}
