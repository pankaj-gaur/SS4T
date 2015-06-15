using SolrNet.Attributes;
using GenericIndexing.Common.Services.Helper;
using GenericIndexing.Services.DataContracts;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    public class SolrContent
    {
        [SolrUniqueKey(SolrServicesConstants.tcmURI)]
        [DataMember(EmitDefaultValue = false)]
        public string TcmUri { get; set; }

        [SolrField(SolrServicesConstants.ContentType)]
        public string ContentType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [SolrUniqueKey(SolrServicesConstants.Title)]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [SolrField(SolrServicesConstants.ShortDescription)]
        public string ShortDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [SolrField(SolrServicesConstants.LongDescription)]
        public string LongDescription { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [SolrField(SolrServicesConstants.SmallLogo)]
        public string SmallImage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [SolrField(SolrServicesConstants.LargeLogo)]
        public string LargeImage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [SolrField(SolrServicesConstants.PublishDate)]
        public string PublishDate { get; set; }
    }
}
