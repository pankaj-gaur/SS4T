/*
 * User: Pankaj Gaur
 * Date: 5/12/2012
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GenericIndexing.Common.Logging;
using GenericIndexing.Common.Configuration;
using GenericIndexing.Common.Configuration.Interface;
using GenericIndexing.Common.Services;
using GenericIndexing.Common.Services.Helper;
using GenericIndexing.IndexService.DataHelper;

using GenericIndexing.IndexService.DataContracts;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using GenericIndexing.Services.DataContracts;
using System.Collections;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// Solr Index Manager provides acts as a glue between Solr instance and CMS content
    /// Contains the business logic to map CMS content to Solr
    /// </summary>
    public class SolrIndexManager
    {
        private IPropertyConfiguration propertyConfiguration;
        private static IPropertyConfiguration propConfiguration;
        private static object containerLock;

        /// <summary>
        /// Singleton SoleIndexManager static constructor
        /// </summary>
        static SolrIndexManager()
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering static SolrIndexManager.SolrIndexManager()");
            try
            {
                string solrIndexConfigPath = Utility.GetConfigurationValue("SearchIndexServiceConfig");

                SS4TLogger.WriteLog(ELogLevel.DEBUG, "Config Path: " + solrIndexConfigPath);
                propConfiguration = ConfigurationManager.GetInstance().GetConfiguration(solrIndexConfigPath)
                    as IPropertyConfiguration;
                containerLock = new object();
            }
            catch (Exception ex)
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, ex.Message + ex.StackTrace);
                throw ex;
            }
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.SolrIndexManager()");
        }

        /// <summary>
        /// Instantiate SolrIndexManager for a core
        /// </summary>
        /// <param name="language"></param>
        public SolrIndexManager(string solr_core)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering SolrIndexManager(core) with core:" +
                                   solr_core);
            try
            {
                Startup.Container.Clear();
                Startup.InitContainer();

                string solrURL = propConfiguration.GetString(SolrServicesConstants.SOLR_URL);
                solrURL = string.Concat(solrURL, SolrServicesConstants.DELIMITER_SLASH, solr_core);

                SS4TLogger.WriteLog(ELogLevel.INFO, "SOLR URL: " + solrURL);

                Boolean instanceExistsAlready = InstanceExists();
                lock (containerLock)
                {
                    if (!string.IsNullOrEmpty(solrURL))
                    {
                        if (!instanceExistsAlready)
                        {
                            try
                            {
                                Startup.Init<Dictionary<string, object>>(solrURL);
                            }
                            catch (Exception ex)
                            {
                                // Work Around: Need to use mutex here to resolve it properly
                                Startup.Container.Clear();
                                Startup.InitContainer();
                                Startup.Init<Dictionary<string, object>>(solrURL);
                                throw ex;
                            }
                        }
                    }
                }

                SS4TLogger.WriteLog(ELogLevel.INFO, "Instance exists: " +
                                       instanceExistsAlready.ToString());
            }
            catch (Exception ex)
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, ex.Message + ex.StackTrace);
                throw ex;
            }
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering SolrIndexManager.SolrIndexManager(core)");
        }

        /// <summary>
        /// Returns True if the Solr client instance exists
        /// </summary>
        /// <returns></returns>
        private ISolrOperations<T> GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<ISolrOperations<T>>();
        }

        private Boolean InstanceExists()
        {
            IEnumerable<ISolrOperations<Dictionary<string, object>>> instances;
            instances = ServiceLocator.Current.GetAllInstances<ISolrOperations<Dictionary<string, object>>>();
            Boolean indexExists = instances.Count() > 0;
            return indexExists;
        }

        /// <summary>
        /// This method indexes a Component presentation in Solr 
        /// </summary>
        /// <param name="query">IndexRequest containing the component presentation</param>
        /// <returns>IndexResponse indicating success or failure</returns>
        public IndexResponse AddDocument(IndexRequest query)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO,
                       "Entering SolrIndexManager.AddDocument for TCM URI: " +
                       query.ItemURI);

            IndexResponse response = new IndexResponse();

            OperationResult result = OperationResult.Failure;

            try
            {
                XmlDocument dcpXML = new XmlDocument();

                if (!string.IsNullOrEmpty(query.DCP))
                {
                    dcpXML.LoadXml(query.DCP);
                }

                AddDocumentXML(dcpXML);
                CommitToSOLR();
                result = OperationResult.Success;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;

                logString = string.Concat(logString,
                                          Environment.NewLine,
                                          string.Format("{0}{1}", ex.Message, ex.StackTrace));

                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                result = OperationResult.Failure;
            }

            response.Result = (int)result;

            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Exiting SolrIndexManager.AddDocument, Result: " +
                                   result.ToString());

            return response;
        }

        /// <summary>
        /// This method removes an index from Solr 
        /// </summary>
        /// <param name="query">IndexRequest containing delete criteria</param>
        /// <returns>IndexResponse indicating success or failure</returns>
        public IndexResponse RemoveDocument(IndexRequest query)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering SolrIndexManager.RemoveDocument for TCM URI: " +
                                  query.ItemURI);

            IndexResponse response = new IndexResponse();

            OperationResult result = OperationResult.Failure;

            try
            {
                // To Remove element and child elements from the SOLR
                //AbstractSolrQuery deleteQuery = new SolrQueryByField(SolrServicesConstants.BaseParent, query.ItemURI);
                AbstractSolrQuery deleteQuery = new SolrQueryByField(SolrServicesConstants.tcmURI, query.ItemURI);
                RemoveFromSOLR(deleteQuery);
                CommitToSOLR();
                result = OperationResult.Success;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                logString = string.Concat(logString,
                                          string.Format("Item URI : {0}", query.ItemURI),
                                          Environment.NewLine, string.Format("{0}{1}", ex.Message, ex.StackTrace));
                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                result = OperationResult.Failure;
            }

            response.Result = (int)result;

            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Exiting SolrIndexManager.RemoveDocument, Result: " +
                                   result.ToString());

            return response;
        }

        /// <summary>
        /// Reindexes all content 
        /// </summary>
        /// <param name="query">IndexRequest containing publication information</param>
        /// <returns>IndexResponse indicating success or failure</returns>
        /*public IndexResponse ReIndex(IndexRequest request)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering SolrIndexManager.ReIndex for TCM URI: " + 
                                  request.ItemURI);
            
            OperationResult result = OperationResult.Failure;
            string[] schemas;
            int publicationID = Convert.ToInt32(request.ItemURI);
            
            IndexResponse response = new IndexResponse();
            try
            {
                propertyConfiguration = ConfigurationManager.GetInstance().GetConfiguration(SolrServicesConstants.SCHEMA_ID_MAPPING_CONFIG) 
                    as IPropertyConfiguration;
                
                schemas = propertyConfiguration.GetPropertyArray("Schema");
                
                ReIndex(schemas, publicationID);
                CommitToSOLR();
                result = OperationResult.Success;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                string itemURI = request != null ? request.ItemURI : "Request Query = NULL";
                logString = string.Concat(logString, string.Format("Item URI : {0}", itemURI),
                                            Environment.NewLine, string.Format("{0}{1}", ex.Message, ex.StackTrace));
                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                result = OperationResult.Failure;
            }
            response.Result = (int) result;
            
            SS4TLogger.WriteLog(ELogLevel.INFO, 
                                   "Exiting SolrIndexManager.ReIndex, Result: " + 
                                   result.ToString());
            
            return response;
        }*/

        /// <summary>
        /// Adds a list of component presentation XML Documents to Solr
        /// </summary>
        /// <param name="productDocuments">List of component presentation XMLs</param>
        /// <returns>IndexResponse</returns>
        public IndexResponse AddMultipleDocumentXML(List<XmlDocument> documents)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Entering SolrIndexManager.UpdateProductDocuments: "
                                   + documents.Count
                                   + " documents");

            OperationResult result = OperationResult.Failure;

            IndexResponse response = new IndexResponse();

            try
            {
                documents.ForEach(document => AddDocumentXML(document));
                CommitToSOLR();
                result = OperationResult.Success;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                logString = string.Concat(logString, Environment.NewLine,
                                          string.Format("{0}{1}", ex.Message, ex.StackTrace));

                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                result = OperationResult.Failure;
            }

            response.Result = (int)result;

            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Exiting SolrIndexManager.AddProductDocuments, Result: " +
                                   result.ToString());
            return response;
        }

        /// <summary>
        /// This method removes all documents of the given type from a specific Solr Core
        /// </summary>
        /// <returns>IndexResponse</returns>
        public IndexResponse RemoveAllDocumentsOfType(string content_type)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Entering SolrIndexManager.RemoveAllDocumentsOfType, Content Type: " +
                                   content_type);

            IndexResponse response = new IndexResponse();
            OperationResult result = OperationResult.Failure;

            try
            {
                AbstractSolrQuery query = new SolrQueryByField(SolrServicesConstants.ContentType, content_type);
                RemoveFromSOLR(query);
                CommitToSOLR();
                result = OperationResult.Success;
            }
            catch (Exception ex)
            {
                string logString = ServiceConstants.LOG_MESSAGE + Environment.NewLine;
                logString = string.Concat(logString, string.Format("{0}{1}", ex.Message, ex.StackTrace));
                SS4TLogger.WriteLog(ELogLevel.ERROR, logString);
                result = OperationResult.Failure;
            }

            response.Result = (int)result;
            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Exiting SolrIndexManager.RemoveAllDocumentsOfType, Result: " +
                                   result.ToString());
            return response;
        }

        /// <summary>
        /// This method takes Publication ID and Schema Array  as input and inserts the index in Solr. 
        /// This is used to ReIndex all the document type which are the candidates for indexing.
        /// </summary>
        /// <param name="schemaArray">Schema Array</param>
        /// <param name="contentRepositoryId">Content Repository ID</param>
        private void ReIndex(string[] schemaArray, int contentRepositoryId)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrIndexManager.ReIndex");

            RemoveALLFromSOLR(contentRepositoryId);

            foreach (string schemaID in schemaArray)
            {
                ReIndexSchema(schemaID, contentRepositoryId);
            }

            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.ReIndex");
        }

        /// <summary>
        /// Re Index all documents of a Content Type identified by Schema ID
        /// </summary>
        /// <param name="schemaID">Schema ID</param>
        /// <param name="contentRepositoryId">Content Repository ID</param>
        private void ReIndexSchema(string schemaID, int contentRepositoryId)
        {
            List<XmlDocument> componentXMLs = TridionDataHelper.GetContentByType(Convert.ToInt32(schemaID),
                                                                                contentRepositoryId,
                                                                                Int32.MaxValue);

            if (componentXMLs == null) return;

            // Do not process null value component presentations
            componentXMLs = componentXMLs.Where(xdoc => xdoc != null).ToList();

            componentXMLs.ForEach(xdoc => AddDocumentXML(xdoc));
        }

        /// <summary>
        /// Adds XML document to Solr. 
        /// Maps the content to a list of Solr documents and adds them.
        /// </summary>
        /// <param name="componentXML">XML Document containing Component Presentation</param>
        private void AddDocumentXML(XmlDocument componentXML)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrIndexManager.AddDocumentXML");

            string compXML = componentXML != null ? componentXML.InnerXml : " NULL";
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Component XML: " + compXML);

            MappedContent mappedContent = ContentMapper.GetMappedContent(componentXML);
            AddToSolr(mappedContent.MappedDocuments, mappedContent.DeleteCriteria);

            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.AddDocumentXML");
        }

        /// <summary>
        /// Adds passed documents and deletes existing documents matching delete criteria
        /// </summary>
        /// <param name="docs">List of documents to add</param>
        /// <param name="dels">List of delete crileria as key-value pairs</param>
        private void AddToSolr(List<Dictionary<string, object>> documentsToAdd,
                               List<KeyValuePair<string, object>> deleteCriteria)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrIndexManager.AddToSolr");
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();

            List<Dictionary<string, object>> deldocs = new List<Dictionary<string, object>>();

            List<SolrQueryByField> deleteQueries = deleteCriteria.Select(item =>
                                                               new SolrQueryByField(item.Key, (string)item.Value)).ToList();

            if (deleteQueries.Count > 0)
            {
                AbstractSolrQuery deleteQuery = deleteQueries[0];
                foreach (SolrQueryByField dq in deleteQueries)
                {
                    deleteQuery |= dq;
                }
                solr.Delete(deleteQuery);
            }
            solr.Add(documentsToAdd);
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.AddToSolr");
        }

        /// <summary>
        /// Removes Solr Indexes based on the query
        /// </summary>
        /// <param name="solrQuery">Solr Query</param>
        private void RemoveFromSOLR(AbstractSolrQuery solrQuery)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrIndexManager.RemoveFromSOLR");
            var solr = GetInstance<Dictionary<string, object>>();
            solr.Delete(solrQuery);
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.RemoveFromSOLR");
        }

        /// <summary>
        /// Commits changes till now to Solr
        /// </summary>
        /// <param name="rebuild">Oprimize Solr and Rebuild SpellCheck Dictoinary</param>
        private void CommitToSOLR(Boolean rebuild = false)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG,
                                   "Entering SolrIndexManager.CommitToSOLR, Rebuild: " + rebuild.ToString());
            var solr = GetInstance<Dictionary<string, object>>();
            solr.Commit();
            if (rebuild)
            {
                solr.Optimize();
                solr.BuildSpellCheckDictionary();
            }
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.CommitToSOLR");
        }

        /// <summary>
        /// Removes all content for the given Publication ID
        /// </summary>
        /// <param name="publicationID">Publication ID</param>
        private void RemoveALLFromSOLR(int publicationID)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG,
                                   "Entering SolrIndexManager.RemoveALLFromSOLR for Publication ID: " +
                                   publicationID.ToString());

            string publication = "tcm:{0}-*";
            publication = string.Format(publication, publicationID.ToString());

            AbstractSolrQuery query = new SolrQueryByField(SolrServicesConstants.tcmURI, publication);
            RemoveFromSOLR(query);

            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.RemoveALLFromSOLR");
        }
    }
}
