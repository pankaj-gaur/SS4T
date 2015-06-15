using System;
using System.Collections.Generic;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// This class stores the mapped content. 
    /// </summary>
    public class MappedContent
    {
        public List<Dictionary<string, object>> MappedDocuments { get; set; }

        public List<KeyValuePair<string, object>> DeleteCriteria { get; set; }
            
        public MappedContent(List<Dictionary<string, object>> docs, List<KeyValuePair<string, object>> dels)
        {
            this.MappedDocuments = docs;
            this.DeleteCriteria = dels;
        }
    }
}
