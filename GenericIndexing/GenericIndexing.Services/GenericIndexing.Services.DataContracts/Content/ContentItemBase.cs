using System.Runtime.Serialization;
using System.Xml.Serialization;
using System;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    [Serializable]
    public abstract class ContentItemBase
    {
        protected ContentItemBase() { }

        [XmlAttribute("Title")]
        [DataMember]
        public string Name { get; set; }
        [XmlElement("publication")]
        public Publication PublicationDetails { get; set; }
        [XmlAttribute("Id")]
        [DataMember]
        public string URI { get; set; }
    }

    [DataContract]
    [Serializable]
    public class Publication
    {
        public Publication() { }

        [XmlAttribute("Id")]
        [DataMember]
        public string Id { get; set; }
        [XmlAttribute("Title")]
        [DataMember]
        public string Title { get; set; }
    }
}
