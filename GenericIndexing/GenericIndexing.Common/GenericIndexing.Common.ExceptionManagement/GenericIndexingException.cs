using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericIndexing.Common.ExceptionManagement
{
	public class GenericIndexingException : ApplicationException
	{
	    public GenericIndexingException():base()
	    {
            
	    }

	    public string Code { get; set; }

		public GenericIndexingException(string code, string errorMessage)
			: base(errorMessage)
		{
			Code = code;
		}

        public GenericIndexingException(string msg, Exception ex)
            : base(msg, ex)
        {
        }

        public GenericIndexingException(string msg)
            : base(msg)
        {
        }
	}
}
