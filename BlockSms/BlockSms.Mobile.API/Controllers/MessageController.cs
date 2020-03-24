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

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        [HttpPost]
        [Route("SendMessage")]
        public async Task<MessageOuputDto> SendMessageAsync([FromBody]MessageInputDto input)
        {
            _logger.LogInformation($"client[post]:发送短信验证码");
            var ts = DateTime.Now.Ticks.ToString();
            var sign = DESEncryptHelper.Get32MD5One($"{_options.MessageUserId}{ts}{_options.MessageApiKey}").ToLower();
            var content = HttpUtility.UrlEncode(string.Format(_options.Template1, input.code));
            _logger.LogInformation($"开始短信验证码-->mobile={input.molile},msgcontent={content}");
            var url = $"{_options.MessageApiURL}/api/sms/send?userid={_options.MessageUserId}&ts={ts}&sign={sign}&mobile={input.molile}&msgcontent={content}";
            using (var client = HttpApiClient.Create<WebApis.IMessageApi>())
            {
                var result = await client.SendAsync(url);
                return result;
            }
        }

        /// <summary>
        /// 发送短信消费码
        /// </summary>
        [HttpPost]
        [Route("SendConsumeMessage")]
        public async Task<MessageOuputDto> SendConsumeMessageAsync([FromBody]MessageInputDto input)
        {
            _logger.LogInformation($"client[post]:发送短信消费码");
            var ts = DateTime.Now.Ticks.ToString();
            var sign = DESEncryptHelper.Get32MD5One($"{_options.MessageUserId}{ts}{_options.MessageApiKey}").ToLower();
            var content = HttpUtility.UrlEncode(string.Format(_options.Template2, input.code));
            var url = $"{_options.MessageApiURL}/api/sms/send?userid={_options.MessageUserId}&ts={ts}&sign={sign}&mobile={input.molile}&msgcontent={content}";
            using (var client = HttpApiClient.Create<WebApis.IMessageApi>())
            {
                var result = await client.SendAsync(url);
                return result;
            }
        }

    }
}
