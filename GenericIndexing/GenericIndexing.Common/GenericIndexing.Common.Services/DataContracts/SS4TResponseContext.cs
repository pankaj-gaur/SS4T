/*
 * File Name    : SS4TResponseContext
 * Author       : pgaur
 * Date Created : 10/31/2009 12:55:34 PM
 */



using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using GenericIndexing.Common.Services.DataContracts;

namespace GenericIndexing.Common.Services.DataContracts
{
    /// <summary>
    /// GenericIndexingReposneContext class holds the information about the service response context
    /// like faults(error) information and other useful information related to service response
    /// </summary>
    [DataContract]
    public class SS4TResponseContext
    {
        public SS4TResponseContext()
        {
            FaultCollection = new Collection<SS4TServiceFault>();
        }

        //Contains the Environment Context of the service response
        [DataMember]
        public SS4TEnvironmentContext  EnvironmentContext { get; set; }


        private Collection<SS4TServiceFault> faultCollection;

        /// <summary>
        /// Fault Collection, a collection of GenericIndexingServiceFault
        /// </summary>
        [DataMember]
        public Collection<SS4TServiceFault> FaultCollection
        {
            get { return faultCollection; }
            set { faultCollection = value; }
        }

        /// <summary>
        /// public to check if response got any fault
        /// </summary>
        public bool IsFault
        {
            get {return faultCollection.Count > 0;}
            set {}
        }
    }
}
