/*
 * File Name    : IndexService
 * Author       : pagaur
 * 
 */
using System.Runtime.Serialization;
using GenericIndexing.Common.Services.DataContracts;

namespace GenericIndexing.IndexService.DataContracts
{      
    [DataContract]
    public class IndexResponse
    {
        [DataMember]
        public int Result { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
