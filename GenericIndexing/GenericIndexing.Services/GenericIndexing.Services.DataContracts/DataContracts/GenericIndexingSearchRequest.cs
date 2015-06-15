using System;
using System.Runtime.Serialization;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    public class GenericIndexingSearchRequest<T>
    {
        public GenericIndexingSearchRequest() { }
        
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
    }
}
