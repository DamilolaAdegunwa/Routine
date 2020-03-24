using System;
using System.Collections.Generic;
using System.Text;

namespace BlockSms.Core.Exceptions
{
    public class EPTDbConcurrencyException : EPTException
    {
        public EPTDbConcurrencyException()
        {

        }
        public EPTDbConcurrencyException(string message)
            : base(message)
        {

        }
        public EPTDbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
