using GenericIndexing.Common.Logging;
using System;
using System.Collections.Generic;
using StructureMap;
using StructureMap.SolrNetIntegration;
using StructureMap.SolrNetIntegration.Config;
using SolrNet.Impl;
using SolrNet;
using GenericIndexing.Services.DataContracts;
using System.Configuration;
using GenericIndexing.Common.Services.Helper;
using System.Collections;
using SolrNet.Commands.Parameters;
using System.Xml;
using System.IO;
using System.Text;

namespace GenericSearching.SearchService.BAL
{
    /// <summary>
    /// Description of SolrManager.
    /// </summary>
    public class SolrSearchManager : ISearching
    {
        public SolrSearchManager()
        {
        }

        static SolrSearchManager()
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "SolrSearchManager Constructor");
            Initialize();
        }

        static void Initialize()
        {
            try
            {
                var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
                ObjectFactory.Initialize(c =>
                {
                    c.Scan(s =>
                    {
                        s.Assembly(typeof(SolrNetRegistry).Assembly);
                        s.Assembly(typeof(SolrConnection).Assembly);
                        s.WithDefaultConventions();
                    });
                    c.AddRegistry(new SolrNetRegistry(solrConfig.SolrServers));
                });
            }
            catch (Exception ex)
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, "Error Initializing SOLR Instance. StackTrace: " + ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public static ISolrOperations<SolrContent> GetSolrOperations(string corename)
        {
            try
            {
                SS4TLogger.WriteLog(ELogLevel.DEBUG, "SolrManager.GetSolrOperations(" + corename + ")");
                return ObjectFactory.Container.GetInstance<ISolrOperations<SolrContent>>(corename);
            }
            catch (Exception ex)
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, "Error Getting Solr Instance. StackTrace: " + ex.Message + ex.StackTrace);
                return null;
            }
        }

        public SearchResults<T> GetSuggestionsFromSearchEngine<T>(SearchRequest request) where T : class
        {
            SearchResults<T> result = new SearchResults<T>();
            if (request.SuggestionRequest)
            {
                ISolrOperations<SolrContent> solr = GetSolrOperations(request.SolrCore);
                SpellCheckingParameters spellCheckOption = GetSolrQuerySpellCheckOption(request.SuggestionQuery);
                QueryOptions queryOptions = new QueryOptions();
                queryOptions.SpellCheck = spellCheckOption;

                var solrResult = solr.Query(request.SuggestionQuery, queryOptions);

                if (solrResult != null && solrResult.SpellChecking != null && solrResult.SpellChecking.Count > 0)
                {
                    var spellCheckResult = solrResult.SpellChecking.GetEnumerator();
                    spellCheckResult.MoveNext();


                    result.suggestions = spellCheckResult.Current.Suggestions;
                }
            }
            return result;
        }

        public SearchResults<T> GetContentFromSearchEngine<T>(SearchRequest request) where T : class
        {
            SearchResults<T> result = new SearchResults<T>();
            try
            {
                SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrSearchManager.GetContentFromSolr");

                ISolrOperations<SolrContent> solr = GetSolrOperations(request.SolrCore);

                AbstractSolrQuery solrQuery = GetSolrQuery(request);
                QueryOptions queryOption = null;

                queryOption = new QueryOptions();
                if (request.Facets != null && request.Facets.Count > 0)
                {
                    queryOption = GetSolrQueryFacetOption(request.Facets);
                }

                queryOption.Rows = request.RecordSize;
                queryOption.Start = request.CurrentPage * request.RecordSize;
                
                if (!string.IsNullOrEmpty(request.SortField))
                {
                    List<SortOrder> sortFields = new List<SortOrder>();
                    string[] srtFields = request.SortField.Split(',');
                    if (srtFields != null && srtFields.Length > 0)
                    {
                        Order sortOrder = (Order)request.SortOrder;
                        
                        foreach (string field in srtFields)
                        {
                            SortOrder so = new SortOrder(field, sortOrder);
                            sortFields.Add(so);
                        }
                        queryOption.OrderBy = sortFields;
                    }
                }

                var solrResult = queryOption != null ? solr.Query(solrQuery, queryOption) : solr.Query(solrQuery);

                if (solrResult != null && solrResult.Count > 0)
                {
                    result.RecordCount = solrResult.NumFound;
                    result.CurrentPage = request.CurrentPage;
                    result.MaxPages = (int)Math.Ceiling((double)result.RecordCount / request.RecordSize);

                    result.ResultPayload = new List<SolrContent>();
                    foreach (SolrContent content in solrResult)
                    {
                        result.ResultPayload.Add(content);
                    }
                    result.Facets = solrResult.FacetFields;
                }
                SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.AddToSolr");

            }
            catch (Exception ex)
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, "Error Retrieving Content from SOLR. StackTrace: " + ex.Message + ex.StackTrace);
            }
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrSearchManager.GetContentFromSolr");
            return result;
        }

        private SpellCheckingParameters GetSolrQuerySpellCheckOption(string searchQuery)
        {
            SpellCheckingParameters returnParam = new SpellCheckingParameters() { OnlyMorePopular = false, Query = searchQuery, Collate = true };
            return returnParam;
        }

        private QueryOptions GetSolrQueryFacetOption(List<string> facets)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrIndexManager.GetSolrQueryOption");
            QueryOptions queryOptions = new QueryOptions();
            queryOptions.Facet = new FacetParameters();
            queryOptions.Facet.MinCount = 1;
            queryOptions.Facet.Queries = new List<ISolrFacetQuery>();
            foreach (string facet in facets)
            {
                SolrFacetFieldQuery fq = new SolrFacetFieldQuery(facet);
                queryOptions.Facet.Queries.Add(fq);
            }
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.GetSolrQueryOption");
            return queryOptions;
        }

        private AbstractSolrQuery GetSolrQuery(SearchRequest request)
        {
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Entering SolrIndexManager.GetSolrQuery");
            AbstractSolrQuery solrQuery = null;
            solrQuery = new SolrQueryByField(SolrServicesConstants.ContentType, request.ContentType.ToString());

            if (request.Filters != null && request.Filters.Count > 0)
            {
                if (request.QueryType == "AND")
                {
                    foreach (DictionaryEntry filter in request.Filters)
                    {
                        if (!string.IsNullOrEmpty(filter.Value.ToString()))
                        {
                            solrQuery = solrQuery && new SolrQueryByField(filter.Key.ToString(), filter.Value.ToString());
                        }
                    }
                }
                else
                {
                    foreach (DictionaryEntry filter in request.Filters)
                    {
                        if (!string.IsNullOrEmpty(filter.Value.ToString()))
                        {
                            solrQuery = solrQuery || new SolrQueryByField(filter.Key.ToString(), filter.Value.ToString());
                        }
                    }
                }
            }
            SS4TLogger.WriteLog(ELogLevel.DEBUG, "Exiting SolrIndexManager.GetSolrQuery");
            return solrQuery;
        }
    }
}