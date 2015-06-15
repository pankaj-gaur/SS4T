/*
 * File Name    : SS4TEnvironmentContext
 * Author       : pgaur
 * Date Created : 1/14/2009 1:12:50 PM
 */


using System.Runtime.Serialization;

namespace GenericIndexing.Common.Services.DataContracts
{
    /// <summary>
    /// EnvironmentContext has details about the environment of the service. Which file server is used by the service, whats the port number etc.
        /// </summary>
    [DataContract]
    public class SS4TEnvironmentContext
    {

        /// <summary>
        /// This is the base path of the binary file storage location
        /// </summary>
        [DataMember]
        public string BinaryFileBasePath { get; set; }
    }
}
