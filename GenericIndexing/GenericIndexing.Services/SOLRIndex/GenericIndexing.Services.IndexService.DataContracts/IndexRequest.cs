/*
 * File Name    : IndexService
 * Author       : pagaur
 * 
 */
using System.Runtime.Serialization;
using GenericIndexing.Common.Services.DataContracts;
using System.Collections.Generic;

namespace GenericIndexing.IndexService.DataContracts
{      
    [DataContract]
    public class IndexRequest
    {
        [DataMember]
        public string ItemURI { get; set; }

        [DataMember]
        public string DCP { get; set; }

        [DataMember]
        public string ContentType { get; set; }

        [DataMember]
        public string LanguageInRequest { get; set; }

        
    }
}
