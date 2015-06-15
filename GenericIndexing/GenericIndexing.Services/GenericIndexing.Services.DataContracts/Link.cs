using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    [XmlRoot("Link")]
    [Serializable]
    public class EmbeddedLink
    {

        [XmlElement("title")]
        [DataMember]
        public string LinkTitle { get; set; }

        [XmlElement("urltype")]
        [DataMember]
        public string UrlType { get; set; }

        [XmlElement("externallink")]
        [DataMember]
        public string ExternalLink { get; set; }

        [XmlElement("internallink")]
        [DataMember]
        public InternalLink InternalLink { get; set; }

        [XmlElement("openinwhichwindow")]
        [DataMember]
        public string OpenInWhichWindow { get; set; }

        [XmlElement("calltoaction")]
        [DataMember]
        public string CallToAction { get; set; }
                
    }
}
