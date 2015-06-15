using System.Runtime.Serialization;

namespace GenericIndexing.Services.DataContracts
{
    [DataContract]
    public class ServiceFault
    {
        private string _reason;

        /// <summary>
        /// Specifies fault reason
        /// </summary>

        [DataMember]
        public string Reason
        {
            get { return _reason; }

            set { _reason = value; }
        }
        
    }
}
