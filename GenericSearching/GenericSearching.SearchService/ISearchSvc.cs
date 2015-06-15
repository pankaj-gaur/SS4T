using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using GenericIndexing.Services.DataContracts;
using GenericIndexing.Common.Services.DataContracts;
using System.IO;
namespace GenericSearching.SearchService
{
    [ServiceContract]
    public interface ISearchSvc
    {

        [OperationContract]
        Stream GetContentFromSolr(SS4TServiceRequest<SearchRequest> request);

        [OperationContract]
        string GetSuggestionsFromSolr(SS4TServiceRequest<SearchRequest> request);
    }
}
