/*
 * 
 * File Name    : IIndexService
 * Author       : pagaur
  
 */
using System.ServiceModel;
using GenericIndexing.Common.Services.DataContracts;
using GenericIndexing.IndexService.DataContracts;

namespace GenericIndexing.IndexService
{    
    [ServiceContract]
    public interface IIndexService
    {
        /// <summary>
        /// operation contract to create index
        /// </summary>
        /// <param name="query">An object of type IndexRequest need to be passed </param>
        /// <returns>IndexResponse object which will have Result as 0 for failure and 1 for success</returns>       
        [OperationContract]
        SS4TServiceResponse<IndexResponse> AddDocument(SS4TServiceRequest<IndexRequest> query);
        
        /// <summary>
        /// operation contract to delete index
        /// </summary>
        /// <param name="query">An object of type IndexRequest need to be passed  </param>
        /// <returns>IndexResponse object which will have Result as 0 for failure and 1 for success</returns>
        [OperationContract]
        SS4TServiceResponse<IndexResponse> RemoveDocument(SS4TServiceRequest<IndexRequest> query);

        /// <summary>
        /// operation contract to ReIndex All the Published Tridion Content
        /// </summary>
        /// <param name="query">An int need to be passed, this is the publication ID of the Publication whose published content need to be indexed </param>
        /// <returns>IndexResponse object which will have Result as 0 for failure and 1 for success</returns>       
        /*[OperationContract]
        SS4TServiceResponse<IndexResponse> ReIndex(SS4TServiceRequest<IndexRequest> query);*/


    }   
}
