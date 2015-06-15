///<remarks>
///====================================================================
/// Name:BaseDataContracts.cs
/// Description: 
/// This piece of code collectively defines all the data contracts that would be required to
/// populate base data containing the ConsumerContext inluding DeviceContext and SecurityToken.
/// 
/// Construction Date: 22/10/2009
/// Author: Pankaj Gaur
/// Last Revision Date:		
/// Last Revision By:		
/// Last Revision Change:	
/// </remarks>
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace GenericIndexing.Common.Services.DataContracts
{
    /// <summary>
    /// This class defines a datacontract that contains DeviceContext and 
    /// SecurityTime as its datamembers together comprising the Consumer Context.
    /// </summary>
    [DataContract]
    public class ConsumerContext
    {
        /// <summary>
        /// Get/Set the DeviceContext instance for the consumer context
        /// </summary>
        [DataMember]
        public GenericIndexingDeviceContext DeviceContext { get; set; }

        /// <summary>
        /// Get/Set the SecurityToken instance for the consumer context 
        /// </summary>
        [DataMember]
        public SecurityToken SecurityToken { get; set; }
    }

    /// <summary>
    /// DeviceContext class defines a data contract that populates all the information 
    /// about the device used by the calling method.
    /// </summary>
    [DataContract]
    public class DeviceContext
    {
        /// <summary>
        /// Get/Set the identifier to identify the current device
        /// </summary>
        [DataMember]
        public string DeviceID { get; set; }

        /// <summary>
        /// Get/Set the channel type used by the current device 
        /// </summary>
        [DataMember]
        public string DeviceChannelType { get; set; }

        /// <summary>
        /// Get/Set the operating system version of the current device
        /// </summary>
        [DataMember]
        public string DeviceOSVersion { get; set; }

        /// <summary>
        /// Get/Set the locale of the current device
        /// </summary>
        [DataMember]
        public string DeviceLocale { get; set; }

        /// <summary>
        /// Get/Set the language used by the current device
        /// </summary>
        [DataMember]
        public string DeviceLanguage { get; set; }
    
    }

    /// <summary>
    /// SecurityToken class defines a data contract that populates all the information 
    /// about the security used by the calling method.
    /// </summary>
    [DataContract]
    public class SecurityToken
    {
        /// <summary>
        /// Get/Set the identifier to identify the tenant
        /// </summary>
        [DataMember]
        public string TenantID { get; set; }
        
        /// <summary>
        /// Get/Set the name of the tenant
        /// </summary>
        [DataMember]
        public string TenantName { get; set; }

        /// <summary>
        /// Get/Set the identifier to identify the current User
        /// </summary>
        [DataMember]
        public string UserID { get; set; }

        /// <summary>
        /// Get/Set security token string
        /// </summary>
        [DataMember]
        public string TokenString { get; set; }
    }
}
