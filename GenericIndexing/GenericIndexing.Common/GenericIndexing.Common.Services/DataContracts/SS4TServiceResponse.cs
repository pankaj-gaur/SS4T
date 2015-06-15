/*
 * 
 * File Name    : SS4TServiceResponse
 * Author       : pgaur
 * Date Created : 10/31/2009 12:50:47 PM
 */

using System.Runtime.Serialization;

namespace GenericIndexing.Common.Services.DataContracts
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Service specific response payload data contact</typeparam>
    [DataContract]
    public class SS4TServiceResponse<T>
    {
        public SS4TServiceResponse()
        {
            ResponseContext = new SS4TResponseContext();
        }

        private T servicePayload;

        /// <summary>
        /// The service payload is a generic implementation for any service which would hold the service
        /// response data send to the consumer
        /// </summary>
        [DataMember]
        public T ServicePayload
        {
            get { return servicePayload; }
            set { servicePayload = value; }
        }

        private SS4TResponseContext responseContext;

        /// <summary>
        /// The response context would hold the data related to response which would be populated by  
        /// underlying service/service client
        /// </summary>
        [DataMember]
        public SS4TResponseContext ResponseContext
        {
            get { return responseContext; }
            set { responseContext = value; }
        }
    }
}

