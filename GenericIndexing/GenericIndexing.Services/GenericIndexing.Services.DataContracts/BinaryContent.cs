using System.Xml.Serialization;

namespace GenericIndexing.Services.DataContracts
{
    [XmlRoot("binarycontent")]
    public class BinaryContent : ContentItemBase
    {

        
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("contenttype")]
        public string ContentType { get; set; }

        [XmlElement("binarytype")]
        public string BinaryType { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }

    }
}
