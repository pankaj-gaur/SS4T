/*
 * File Name    : GenericIndexingDeviceContext
 * Author:        Pankaj Gaur
 * Date Created : 10/31/2009 1:12:50 PM
 */


using System.Runtime.Serialization;

namespace GenericIndexing.Common.Services.DataContracts
{
    /// <summary>
    /// DeviceContext class defines a data contract that populates all the information 
    /// about the device used by the calling method.
    /// </summary>
    [DataContract]
    public class GenericIndexingDeviceContext
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

        /// <summary>
        /// Device Type represents the type of the channel. E.g. Web, iPhone, BB etc
        /// </summary>
        [DataMember]
        public string  DeviceType { get; set; }

        /// <summary>
        /// DeviceAppVersion represents the version of application for channel applications.
        /// </summary>
        [DataMember]
        public string DeviceAppVersion { get; set; }

        /// <summary>
        /// IP Address represents ip address of the client machine which not the channel
        /// </summary>
        [DataMember]
        public string ClientIPAddress { get; set; }

    }
}
