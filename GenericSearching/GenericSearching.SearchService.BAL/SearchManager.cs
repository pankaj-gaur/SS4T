using GenericIndexing.Services.DataContracts;
using GenericIndexing.Common.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSearching.SearchService.BAL
{
    public class SearchManager
    {
        private string searchEngine;

        public SearchManager(string searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        public string GetContentFromSearchEngine(SearchRequest searchRequest)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(searchEngine))
            {
                ISearching searchManager = null;
                //SOLR Search Engine
                if (searchEngine.ToLower() == SearchEngine.SOLR.ToString().ToLower())
                {
                    searchManager = new SolrSearchManager();
                    SearchResults<SolrContent> searchResult = searchManager.GetContentFromSearchEngine<SolrContent>(searchRequest);

                    if (searchResult != null)
                    {
                        result = searchResult.ToJSONText();
                    }
                }
            }
            return result;
        }

        public string GetSuggestionFromSearchEngine(SearchRequest searchRequest)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(searchEngine))
            {
                ISearching searchManager = null;
                //SOLR Search Engine
                if (searchEngine.ToLower() == SearchEngine.SOLR.ToString().ToLower())
                {
                    searchManager = new SolrSearchManager();
                    SearchResults<SolrContent> searchResult = searchManager.GetSuggestionsFromSearchEngine<SolrContent>(searchRequest);

                    if (searchResult != null)
                    {
                        result = searchResult.ToJSONText();
                    }
                }
            }
            return result;
        }
    }
}
