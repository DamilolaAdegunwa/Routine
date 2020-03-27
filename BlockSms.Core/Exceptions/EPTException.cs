using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BlockSms.Core
{
    public class EPTException : Exception
    {
        public EPTException()
        {

        }
        public EPTException(string message)
            : base(message)
        {

        }

        public EPTException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public EPTException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
