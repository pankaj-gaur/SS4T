using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericIndexing.Common.Services
{
    public enum SchemaTitle
    {
        Promotion
    }
    /// <summary>
    ///This class represents the various constant expressions used in the Common Services. All constants defined here are static 
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Common error message; used before every Log
        /// </summary>
        public static readonly string LOG_MESSAGE = "An error has occurred. The request trace is: ";

        public static readonly string ROOT_ELEMENT_URI = "ROOT";
        public static readonly string PARENT_CATEGORY_XPATH = @"infocategory/parentcategory";
        public static readonly string NEW_ELEMENT_NAME = "parenturi";
        public static readonly string TCM_URI_ATTRIBUTE = "Id";
        public static readonly string CATEGORY_ITEM_XPATH = @"infoitem/parentcategory";

        public const string MULTICHAR_DELIMITTER = "###";

   
        /// <summary>
        /// // Swervice Fault
        /// </summary>
        public static class ServiceFault
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly string UNKNOWN_EXCEPTION_CODE = "FC0001";
            
            /// <summary>
            /// 
            /// </summary>
            public static readonly string UNKNOWN_EXCEPTION_MESSAGE = "System Error occurred";

            /// <summary>
            /// 
            /// </summary>
            /// <summary>
            /// 
            /// </summary>
            public static readonly string SECURITY_EXCEPTION_CODE = "PL0001";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string SECURITY_EXCEPTION_MESSAGE = "Authentication failed";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string MAX_RESULT_EXCEPTION_CODE = "FC0002";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string MAX_RESULT_EXCEPTION_MESSAGE = "Too many search results.";

        }
    }
}
