using GenericIndexing.Services.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSearching.SearchService.BAL
{
    public interface ISearching
    {
        SearchResults<T> GetContentFromSearchEngine<T>(SearchRequest request) where T : class;
        SearchResults<T> GetSuggestionsFromSearchEngine<T>(SearchRequest request) where T : class;
    }
}
