using AutoMapper;
using BlockSms.Core;
using BlockSms.Core.Helper;
using BlockSms.Core.Web;
using BlockSms.Mobile.Api;
using BlockSms.Mobile.Core.Queries;
using BlockSms.Core.Commands;
using BlockSms.Core.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EPT.SelfAPI.Controllers
{
    /// <summary>
    /// 自助设备管理
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BlockControllerBase
    {
        private const string TicksToken = "_TicksToken";

        private IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUserQueries _queries;
        private readonly IMessageQueries _MessageQueries;
        /// <summary>
        /// 
        /// </summary>
        public UserController(IMemoryCache memoryCache, IMediator mediator, IMapper mapper,
            ILogger<UserController> logger,
            IHttpContextAccessor accessor,
            IUserQueries queries,
            IMessageQueries messageQueries) : base(accessor)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _MessageQueries = messageQueries ?? throw new ArgumentNullException(nameof(messageQueries));
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="password">密码</param>
        /// <returns>Token</returns>
        [HttpGet]
        [Route("Token")]
        public async Task<string> TokenAsync(string phone, string password)
        {
            return await _cache.GetOrCreateAsync(TicksToken + DateTime.Now.ToString("yyyyMMdd") + phone, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                var user = await _queries.GetModelAsync(phone);
                if (user == null)
                    throw new EPTException($"不存在手机号为为：{phone}的用户");
                else
                {
                    if (user.Password != password)
                        throw new EPTException($"密码不正确");
                    else
                    {
                        string keyvalue = DESEncryptHelper.Encrypt(phone, DateTime.Now.ToString("yyyyMMdd"));
                        return keyvalue;
                    }
                }
            });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [HttpGet]
        [Route("GetUser")]
        public async Task<User> GetUserAsync()
        {
            var phone = GetToken();
            var user = await _queries.GetModelAsync(phone);
            if (user == null)
                throw new EPTException($"token不正确");
            return user;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="input">注册信息</param>
        [HttpPost]
        [Route("Register")]
        public async Task<ObjectResult> RegisterAsync([FromBody] UserRegisterDto input)
        {
            if (string.IsNullOrEmpty(input.molile))
                throw new EPTException("手机号不能为空");
            if (string.IsNullOrEmpty(input.code))
                throw new EPTException("验证码不能为空");
            if (string.IsNullOrEmpty(input.password))
                throw new EPTException("密码不能为空");
            var messages = await _MessageQueries.GetModelAsync(input.molile);
            if (messages == null)
                throw new EPTException("尚未发送验证码");
            if (messages.Code != input.code)
                throw new EPTException("验证码不正确");
            if (messages.Used)
                throw new EPTException("验证码已使用");
            if(messages.SendTime.AddMinutes(10)<DateTime.Now)
                throw new EPTException("验证码已过期");
            var command = new UserRegisterCommand(input.molile,input.password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// 用户信息修改
        /// </summary>
        /// <param name="input">注册信息</param>
        [HttpPost]
        [Route("Update")]
        public async Task<ObjectResult> UpdateAsync([FromBody] UserUpdateDto input)
        {
            var phone = GetToken();
            var user = await _queries.GetModelAsync(phone);
            if (user == null)
                throw new EPTException($"token不正确");

            input.Password = input.Password ?? user.Password;
            input.Email = input.Email ?? user.Email;
            input.UserConfig = input.UserConfig ?? user.UserConfig;
            var command = new UserUpdateCommand(user.Phone,input.Password,input.Email, input.UserConfig);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
