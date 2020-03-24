using BlockSms.Core.Model;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace BlockSms.Core.SMS
{
    public class WXKJCommon
    {
        public string PostUrl = "http://manager.wxtxsms.cn/";
        public string Method = "smsport/sendPost.aspx";
        public WXKJCommon() { }
        public WXKJCommon(string postUrl)
        {
            this.PostUrl = postUrl;
        }
        public WXKJCommon(string postUrl, string method)
        {
            this.PostUrl = postUrl;
            this.Method = method;
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="useId">接口ID</param>
        /// <param name="upsd">接口秘钥</param>
        /// <param name="sign">厂商编码</param>
        /// <param name="sendtele">手机号码</param>
        /// <param name="msg">短信内容</param>
        /// <param name="sendtime"></param>
        /// <returns></returns>
        public ResultMsg SendPost(string useId, string upsd, string sign, string sendtele, string msg, string sendtime)
        {
            var sb = new StringBuilder();
            sb.Append("uid=" + useId);
            sb.Append("&upsd=" + GetMD5(upsd));
            sb.Append("&sendtele=" + sendtele);
            sb.Append("&msg=" + msg);
            sb.Append("&sendtime=" + sendtime);
            sb.Append("&sign=" + sign);
            byte[] bTemp = Encoding.UTF8.GetBytes(sb.ToString());
            return DoPostRequest(PostUrl + Method, bTemp);
        }
        private string GetMD5(string str)
        {
            var b = Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
        private ResultMsg DoPostRequest(string url, byte[] bData)
        {
            var res = new ResultMsg { Success = false };
            string strResult = string.Empty;
            try
            {
                var hwRequest = (HttpWebRequest)WebRequest.Create(url);
                hwRequest.Timeout = 10000;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;

                var smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
                var hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.ASCII);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
                res.Msg = GetResult(strResult, out bool isSucess);
                res.Success = isSucess;
            }
            catch (Exception err)
            {
                res.Msg = err.Message;
            }
            res.Result = strResult;
            return res;
        }

        private string GetResult(string result, out bool isSucess)
        {
            isSucess = false;
            string msg = string.Empty;
            switch (result)
            {
                case "error01": msg = " 提交方式不正确。请用POST方式提交"; break;
                case "error02": msg = " 参数输入不完整"; break;
                case "error03": msg = " IP认证失败"; break;
                case "error04": msg = " 用户名、密码格式不正确，用户名密码不正确，用户被禁用"; break;
                case "error05": msg = " 号码数量超出1000个"; break;
                case "error06": msg = " 内容不符合规则"; break;
                case "error07": msg = " 内容超长，超出450字"; break;
                case "error08": msg = " 所用签名不对"; break;
                case "error09": msg = " 余额不足"; break;
                case "error10": msg = " 获取异常，请联系客服"; break;
                default:
                    if (result.Contains("success"))
                    {
                        msg = "提交成功，" + result;
                        isSucess = true;
                    }
                    break;
            }
            return msg;
        }
    }
}
