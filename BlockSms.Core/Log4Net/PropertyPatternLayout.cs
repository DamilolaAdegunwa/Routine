using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;
using System.Reflection;

namespace BlockSms.Core.Log4Net
{
    public class PropertyPatternLayout : PatternLayout
    {
        public PropertyPatternLayout()
        {
            // 添加自定义属性转换
            this.AddConverter("property", typeof(PropertyPatternLayoutConverter));
        }
    }
    public class PropertyPatternLayoutConverter : PatternLayoutConverter
    {

        /// <summary>
        /// 重写转换方法
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="loggingEvent"></param>
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            if (null != this.Option)
            {
                WriteObject(writer, loggingEvent.Repository, LookupProperty(this.Option, loggingEvent));
            }
            else
            {
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }

        private object LookupProperty(string property, LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            PropertyInfo info = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (null != info)
                propertyValue = info.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }
    }
}
