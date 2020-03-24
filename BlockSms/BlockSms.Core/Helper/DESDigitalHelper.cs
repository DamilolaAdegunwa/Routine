using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.Helper
{
    public class DESDigitalHelper
    {
        /// <summary>
        /// 数字加密成数字
        /// </summary>
        /// <param name="_CardNo">卡号/条码/二维码(数字范围100000000~21474000000)</param>
        /// <returns>加密后的数字</returns>
        public static Int64 Encryption(Int64 _CardNo, int _Newkey)
        {
            //int _Newkey=500;
            int[,] _State = new int[4, 3] { { 8, 2, 5 }, { 8, 1, 7 }, { 8, 5, 6 }, { 6, 3, 5 } };
            return Encryption(_CardNo, _Newkey, _State);
        }

        /// <summary>
        /// 数字加密成数字
        /// </summary>
        /// <param name="_CardNo">卡号/条码/二维码</param>
        /// <param name="_NewKey">干扰值（数字范围0~10000）</param>
        /// <param name="state">反转公式</param>
        /// <returns>加密后的数字</returns>
        public static Int64 Encryption(Int64 _CardNo, int _NewKey, int[,] state)
        {
            Int64 x = _CardNo - _NewKey;
            string y = Convert.ToString(x, 2);
            int lengthw = 0;
            StringBuilder sb = new StringBuilder(y);
            for (int i = 0; i < state.GetLength(0); i++)
            {
                if (y.Substring(lengthw + state[i, 1], 1) == "0")
                {
                    sb.Replace("0", "1", lengthw + state[i, 1], 1);
                }
                if (y.Substring(lengthw + state[i, 1], 1) == "1")
                {
                    sb.Replace("1", "0", lengthw + state[i, 1], 1);
                }

                if (y.Substring(lengthw + state[i, 2], 1) == "0")
                {
                    sb.Replace("0", "1", lengthw + state[i, 2], 1);
                }
                if (y.Substring(lengthw + state[i, 2], 1) == "1")
                {
                    sb.Replace("1", "0", lengthw + state[i, 2], 1);
                }
                lengthw += state[i, 0];
            }
            return Convert.ToInt64(sb.ToString(), 2);

        }

        /// <summary>
        /// 数字解密成数字
        /// </summary>
        /// <param name="_IdentifiCode">识别码</param>
        /// <returns>解密后的数字</returns>
        public static Int64 Decrypt(Int64 _IdentifiCode, int _Newkey)
        {
            //int _Newkey = 500;
            int[,] _State = new int[4, 3] { { 8, 2, 5 }, { 8, 1, 7 }, { 8, 5, 6 }, { 6, 3, 5 } };
            return Decrypt(_IdentifiCode, _Newkey, _State);
        }

        /// <summary>
        /// 数字解密成数字
        /// </summary>
        /// <param name="_IdentifiCode">识别码</param>
        /// <param name="_NewKey">干扰值（数字范围0~10000）</param>
        /// <param name="state">反转公式</param>
        /// <returns>解密后的数字</returns>
        public static Int64 Decrypt(Int64 _IdentifiCode, int _NewKey, int[,] state)
        {
            Int64 x = _IdentifiCode;
            string y = Convert.ToString(x, 2);

            int lengthw = 0;
            StringBuilder sb = new StringBuilder(y);
            for (int i = 0; i < state.GetLength(0); i++)
            {
                if (y.Substring(lengthw + state[i, 1], 1) == "0")
                {
                    sb.Replace("0", "1", lengthw + state[i, 1], 1);
                }
                if (y.Substring(lengthw + state[i, 1], 1) == "1")
                {
                    sb.Replace("1", "0", lengthw + state[i, 1], 1);
                }

                if (y.Substring(lengthw + state[i, 2], 1) == "0")
                {
                    sb.Replace("0", "1", lengthw + state[i, 2], 1);
                }
                if (y.Substring(lengthw + state[i, 2], 1) == "1")
                {
                    sb.Replace("1", "0", lengthw + state[i, 2], 1);
                }
                lengthw += state[i, 0];
            }
            return (Convert.ToInt64(sb.ToString(), 2) + _NewKey);
        }

        private static char[] constant =
        {
          '0','1','2','3','4','5','6','7','8','9',
          'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
          'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };
        public static string GenerateRandom(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
    }
}
