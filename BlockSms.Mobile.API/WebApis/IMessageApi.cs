using WebApiClient;
using WebApiClient.Attributes;


namespace BlockSms.Mobile.Api.WebApis
{
    /// <summary>
    /// 短信接口
    /// </summary>
    [LogFilter]
    public interface IMessageApi : IHttpApi
    {
        /// <summary>
        /// 短信发送接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        ITask<MessageOuputDto> SendAsync([Uri] string url);
    }
}
