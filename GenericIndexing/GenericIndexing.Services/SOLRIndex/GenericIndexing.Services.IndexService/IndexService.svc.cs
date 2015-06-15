/*
 * File Name    : IndexService
 * Author       : pagaur
 */
using System;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml;
using GenericIndexing.Common.Logging;
using GenericIndexing.Common.ExceptionManagement;
using GenericIndexing.Common.Services;
using GenericIndexing.Common.Services.DataContracts;
using GenericIndexing.Common.Services.Helper;
using GenericIndexing.IndexService.DataContracts;
using GenericIndexing.IndexService.BAL;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

namespace GenericIndexing.IndexService
{
    /// <summary>
    /// This class exposes the SOLR Index Service methods
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class IndexService : IIndexService
    {
        /// <summary>
        /// This method creates an index in solr 
        /// </summary>
        /// <param name="query">An object of IndexRequest need to be passed</param>
        /// <returns>Object of type IndexResponse is returned which has field Result as 1 for success and 0 for failure</returns>
        [WebInvoke(UriTemplate = "/AddDocument/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SS4TServiceResponse<IndexResponse> AddDocument(SS4TServiceRequest<IndexRequest> query)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering into method IndexService.AddDocument");
            SS4TServiceResponse<IndexResponse> serviceResponse = new SS4TServiceResponse<IndexResponse>();
            string result = string.Empty;
            string request = query != null ? query.ToJSONText() : "Request = NULL";
            try
            {
                SS4TLogger.WriteLog(ELogLevel.DEBUG, request);
                string language = query.ServicePayload.LanguageInRequest;

                IndexResponse resultValue;
                SolrIndexManager indexManager = new SolrIndexManager(language);
                resultValue = indexManager.AddDocument(query.ServicePayload);
                serviceResponse.ServicePayload = resultValue;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                logString = string.Concat(logString, string.Format("Service Request: {0}", request),
                                            Environment.NewLine, string.Format("{0}{1}", ex.Message, ex.StackTrace));
                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                CatchException<IndexResponse>(ex, serviceResponse);
            }
            SS4TLogger.WriteLog(ELogLevel.INFO, "Exiting from method IndexService.AddDocument");
            return serviceResponse;
        }

        /// <summary>
        /// This method removes an index from solr
        /// </summary>
        /// <param name="query">An object of IndexRequest need to be passed</param>
        /// <returns>Object of type IndexResponse is returned which has field Result as 1 for success and 0 for failure</returns>
        [WebInvoke(UriTemplate = "/RemoveDocument/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SS4TServiceResponse<IndexResponse> RemoveDocument(SS4TServiceRequest<IndexRequest> query)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering into method IndexService.RemoveDocument");
            SS4TServiceResponse<IndexResponse> serviceResponse = new SS4TServiceResponse<IndexResponse>();
            string result = string.Empty;

            try
            {
                IndexResponse resultValue;
                SolrIndexManager indexManager = new SolrIndexManager(query.ServicePayload.LanguageInRequest);
                resultValue = indexManager.RemoveDocument(query.ServicePayload);
                serviceResponse.ServicePayload = resultValue;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                string request = query != null ? query.ToJSONText() : "Request = NULL";
                logString = string.Concat(logString, string.Format("Service Request: {0}", request),
                                            Environment.NewLine, string.Format("{0}{1}", ex.Message, ex.StackTrace));
                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                CatchException<IndexResponse>(ex, serviceResponse);
            }
            SS4TLogger.WriteLog(ELogLevel.INFO, "Exiting from method IndexService.RemoveDocument");
            return serviceResponse;
        }


        /// <summary>
        /// This method re-indexes all content, based on the passed Publication ID
        /// </summary>
        /// <param name="query">Publication Id</param>
        /// <returns>Object of type IndexResponse is returned which has field Result as 1 for success and 0 for failure</returns>
        /*[WebInvoke(UriTemplate = "/ReIndex/", Method = "POST")]
        public SS4TServiceResponse<IndexResponse> ReIndex(SS4TServiceRequest<IndexRequest> query)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering into method IndexService.ReIndex");
            SS4TServiceResponse<IndexResponse> serviceResponse = new SS4TServiceResponse<IndexResponse>();
            string result = string.Empty;

            try
            {
                IndexResponse resultValue;
                SolrIndexManager indexManager = new SolrIndexManager(query.ServicePayload.LanguageInRequest);
                resultValue = indexManager.ReIndex(query.ServicePayload);
                serviceResponse.ServicePayload = resultValue;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                string request = query != null ? query.ToJSONText() : "Request = NULL";
                logString = string.Concat(logString, string.Format("Service Request: {0}", request),
                                            Environment.NewLine, string.Format("{0}{1}", ex.Message, ex.StackTrace));
                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                CatchException<IndexResponse>(ex, serviceResponse);
            }
            SS4TLogger.WriteLog(ELogLevel.INFO, "Exiting from method IndexService.ReIndex");
            return serviceResponse;
        }*/


        private void CatchException<T>(Exception ex, SS4TServiceResponse<T> serviceResponse)
        {
            SS4TServiceFault fault = new SS4TServiceFault();
            ExceptionHelper.HandleException(ex, out fault);
            serviceResponse.ResponseContext.FaultCollection.Add(fault);
        }

        private static List<XmlDocument> GetXmlDocumentListFromXmlNodeList(XmlNodeList xmlnodelist)
        {
            List<XmlDocument> returnList = new List<XmlDocument>();

            foreach (XmlNode xnode in xmlnodelist)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(xnode.OuterXml);
                returnList.Add(xdoc);
            }

            return returnList;
        }
    }
}