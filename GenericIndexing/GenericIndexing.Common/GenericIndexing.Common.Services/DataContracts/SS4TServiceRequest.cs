/*
 * File Name    : SS4TServiceRequest
 * Author       : pgaur
 * Date Created : 10/31/2009 12:39:16 PM
 */

using System.Runtime.Serialization;

namespace GenericIndexing.Common.Services.DataContracts
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Service specific request payload data contact</typeparam>
    [DataContract]
    public class SS4TServiceRequest<T>
    {
        public SS4TServiceRequest()
        {
        }

        private T servicePayload;

        /// <summary>
        /// The service payload is a generic implementation for any service which would hold the service
        /// request data send by the consumer
        /// </summary>
        [DataMember]
        public T ServicePayload
        {
            get { return servicePayload; }
            set { servicePayload = value; }
        }
    }
}
