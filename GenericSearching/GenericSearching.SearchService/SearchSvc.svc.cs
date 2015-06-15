using GenericIndexing.Common.Services.DataContracts;
using GenericIndexing.Services.DataContracts;
using GenericIndexing.Common.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using GenericSearching.SearchService.BAL;
using System.Configuration;
using System.IO;

namespace GenericSearching.SearchService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class SearchSvc : ISearchSvc
    {
        [WebInvoke(UriTemplate = "/GetContentFromSolr/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Stream GetContentFromSolr(SS4TServiceRequest<SearchRequest> request)
        {
            string resultJSON = string.Empty;
            if (request != null && request.ServicePayload != null && !string.IsNullOrEmpty(request.ServicePayload.SolrCore))
            {
                string searchEngine = ConfigurationManager.AppSettings["SearchEngine"];
                string contentType = request.ServicePayload.ContentType;

                if (request.ServicePayload.RecordSize == 0)
                {
                    string recSize = ConfigurationManager.AppSettings[contentType + "_RecordSize"];
                    if (!string.IsNullOrEmpty(recSize) && recSize != "0")
                    {
                        request.ServicePayload.RecordSize = Convert.ToInt16(recSize);
                    }
                    else
                    {
                        recSize = ConfigurationManager.AppSettings["default_RecordSize"];
                        if (!string.IsNullOrEmpty(recSize) && recSize != "0")
                        {
                            request.ServicePayload.RecordSize = Convert.ToInt16(recSize);
                        }
                        else
                        {
                            request.ServicePayload.RecordSize = int.MaxValue;
                        }
                    }
                }
                else
                {
                    request.ServicePayload.RecordSize = request.ServicePayload.RecordSize;
                }

                SearchManager manager = new SearchManager(searchEngine);
                resultJSON = manager.GetContentFromSearchEngine(request.ServicePayload);
            }
            return new MemoryStream(Encoding.UTF8.GetBytes(resultJSON));
        }

        [WebInvoke(UriTemplate = "/GetSuggestionsFromSolr/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public string GetSuggestionsFromSolr(SS4TServiceRequest<SearchRequest> request)
        {
            string resultJSON = string.Empty;
            if (request != null && request.ServicePayload != null && !string.IsNullOrEmpty(request.ServicePayload.SolrCore))
            {
                string searchEngine = ConfigurationManager.AppSettings["SearchEngine"];
                SearchManager manager = new SearchManager(searchEngine);
                resultJSON = manager.GetSuggestionFromSearchEngine(request.ServicePayload);
            }
            return resultJSON;
        }
    }
}
