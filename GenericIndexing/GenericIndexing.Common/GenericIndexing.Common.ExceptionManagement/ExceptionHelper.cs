using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericIndexing.Common.Services.DataContracts;
using GenericIndexing.Common.Logging;

namespace GenericIndexing.Common.ExceptionManagement
{
	public static class ExceptionHelper
	{
		private static void LogException(GenericIndexingException ampException)
		{
			SS4TLogger.WriteLog(ELogLevel.ERROR, ampException.Message + ", Code " + ampException.Code);
		}
		public static void HandleException(Exception exception)
		{
			GenericIndexingException ampException = exception as GenericIndexingException;
			if (ampException != null)
			{
				LogException(ampException);
			}
		}
		public static void HandleException(Exception exception, out SS4TServiceFault fault)
		{
			GenericIndexingException ampException = exception as GenericIndexingException;
			fault = new SS4TServiceFault();
			if (ampException != null)
			{
				fault.Code = ampException.Code;
				fault.Message = ampException.Message;
			}
			else
			{
                fault.Code = GenericIndexing.Common.Services.ServiceConstants.ServiceFault.UNKNOWN_EXCEPTION_CODE;
                fault.Message = GenericIndexing.Common.Services.ServiceConstants.ServiceFault.UNKNOWN_EXCEPTION_MESSAGE;
			}
		}

        public static void HandleCustomException(Exception ex, string LogMessage)
        {
            GenericIndexingException ampEx = ex as GenericIndexingException;
            if (ampEx != null)
            {
                SS4TLogger.WriteLog(ELogLevel.WARN, LogMessage);
            }
            else
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, LogMessage);
            }
        }
	}
}