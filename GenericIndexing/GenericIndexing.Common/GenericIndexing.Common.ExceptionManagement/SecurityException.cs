using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericIndexing.Common.ExceptionManagement
{
    public class SecurityException:GenericIndexingException
    {
        public SecurityException(string code, string errorMessage) : base(code, errorMessage)
        {
            Code = code;
        }

        public SecurityException()
        {
        }
    }
}
