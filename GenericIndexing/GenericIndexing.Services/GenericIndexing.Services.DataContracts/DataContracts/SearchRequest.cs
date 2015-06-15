using System.Collections.Generic;
using System.Collections.Specialized;

namespace GenericIndexing.Services.DataContracts
{
    public class SearchRequest
    {
        public SearchRequest() { }

        public HybridDictionary Filters { get; set; }

        public List<string> Facets { get; set; }

        public string ContentType { get; set; }

        public bool SuggestionRequest { get; set; }

        public string SuggestionQuery { get; set; }

        public string SolrCore { get; set; }

        public string QueryType { get; set; }

        public int CurrentPage { get; set; }

        public int RecordSize { get; set; }

        public string SortField { get; set; }

        public int SortOrder { get; set; }
    }
}
