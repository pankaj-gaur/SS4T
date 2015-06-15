using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    [Serializable]
    public class LinkImage
    {
        [XmlElement("alttext")]
        [DataMember]
        public string AltText { get; set; }

        [XmlAttribute("Path")]
        [DataMember]
        public string Path { get; set; }

        [XmlAttribute("Id")]
        [DataMember]
        public string ImageURI { get; set; }
    }
}
