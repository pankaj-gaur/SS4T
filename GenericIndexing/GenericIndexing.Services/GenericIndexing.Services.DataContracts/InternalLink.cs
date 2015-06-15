using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    [Serializable]
    public class InternalLink
    {
        [XmlAttribute("Id")]
        [DataMember]
        public string InternalLinkId { get; set; }

        [XmlAttribute("Path")]
        [DataMember]
        public string ImagePath { get; set; }

        [XmlElement("alttext")]
        [DataMember]
        public string ImageAltText { get; set; }
    }
}
