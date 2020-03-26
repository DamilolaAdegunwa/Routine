using AutoMapper;
using BlockSms.Core.Helper;
using BlockSms.Core.Web;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Web;
using WebApiClient;
using Submail.AppConfig;
using Submail.Lib;
using BlockSms.Core.Commands;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 短信管理
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : BlockControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ApiOptions _options;
        /// <summary>
        /// 
        /// </summary>
        public MessageController(IMediator mediator, IMapper mapper,
            IHttpContextAccessor accessor,
            IOptions<ApiOptions> optionsAccessor,
            ILogger<MessageController> logger)
            : base(accessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = optionsAccessor.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region 发送短信内容
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        //[HttpPost]
        //[Route("SendMessage")]
        //public string SendMessageAsync([FromBody]MessageCodeInputDto input)
        //{
        //    _logger.LogInformation($"client[post]:发送短信验证码{input.code}给{input.molile}");
        //    IAppConfig appConfig = new MMSConfig("41014", "1c5485f03799ecb37c5d1d00b66523f6", SignType.normal);
        //    MessageSend messageSend = new MessageSend(appConfig);
        //    messageSend.AddTo(input.molile);
        //    messageSend.AddContent($"【每日币推】您的验证码是：{input.code}，请在10分钟内输入");
        //    messageSend.AddTag("123");
        //    string returnMessage = string.Empty;
        //    messageSend.Send(out returnMessage);
        //    return returnMessage;
        //} 
        #endregion

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        [HttpPost]
        [Route("SendCode")]
        public ObjectResult SendCodeAsync([FromBody]MessageCodeInputDto input)
        {
            _logger.LogInformation($"client[post]:发送短信验证码{input.code}给{input.molile}");
            IAppConfig appConfig = new MMSConfig("41014", "1c5485f03799ecb37c5d1d00b66523f6", SignType.normal);
            MessageSend messageSend = new MessageSend(appConfig);
            messageSend.AddTo(input.molile);
            messageSend.AddContent($"【每日币推】您的验证码是：{input.code}，请在10分钟内输入");
            messageSend.AddTag("123");
            string returnMessage = string.Empty;
            messageSend.Send(out returnMessage);

            var command = new MessageAddCommand(input.code, input.code,  BlockSms.Core.Domain.EMsgType.验证码);
            var result =   _mediator.Send(command);
            return Ok(result);
        }

    }
}
