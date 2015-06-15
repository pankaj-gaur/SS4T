/*
 * File Name    : TridionDataHelper
 * Author       : pgaur
 */


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using GenericIndexing.Common.Logging;
using GenericIndexing.Common.ExceptionManagement;
using Com.Tridion.Util;
using Tridion.ContentDelivery.DynamicContent;
using Tridion.ContentDelivery.DynamicContent.Query;
using Tridion.ContentDelivery.Taxonomies;
using Tridion.ContentDelivery.Web.Linking;

namespace GenericIndexing.IndexService.DataHelper
{
	/// <summary>
	/// This class contains helper function to retrieve the data from tridion
	/// </summary>
	public static class TridionDataHelper
	{
		#region Constants
		private const string TCM_URI_FORMAT = "tcm:{0}-{1}-{2}";
	    private const string TCM_URI_FORMAT_COMPONENT = "tcm:{0}-{1}";
        private const string TCM_URI_INVALID_VALUE = "Invalid URI Value";
        private const string TCM_URI_INVALID_CODE = "CO0001";
		private const int TRIDION_CATEGORYTYPE = 512;
		private const int TRIDION_ITEMTYPE = 16;
		private const int TRIDION_KEYWORDTYPE = 1024;
        private const string TCM_URI_PATTERN = @"^tcm:[0-9][0-9]*-[0-9][0-9]*$";
        private const string TCM_URI_COMPONENT_PATTERN = @"^tcm:[0-9][0-9]*\-[0-9][0-9]*\-[0-9][0-9]*$";
		#endregion

		# region Static Methods

		private static List<XmlDocument> BuildAndExecute(int contentRepositoryId, Criteria[] criteria, int nbrOfRecords)
		{
			Query query;
			List<XmlDocument> results;
			PublicationCriteria publicationCriteria = new PublicationCriteria(contentRepositoryId);
			Criteria searchCriteria = CriteriaFactory.And(criteria);
			searchCriteria = CriteriaFactory.And(searchCriteria, publicationCriteria);
			query = new Query { Criteria = searchCriteria };
			SortParameter sortParameter = new SortParameter(SortParameter.ItemTitle, SortParameter.Ascending);
			LimitFilter limitFilter = new LimitFilter(nbrOfRecords);
			results = ExecuteQuery(query, limitFilter, sortParameter);
			return results;
		}

		/// <summary>
		/// this method shall be used to return the component based on a categories and keywords
		/// </summary>
		/// <param name="category"></param>
		/// <param name="keywords"></param>
		/// <param name="contentRepositoryId"></param>
		/// <returns></returns>
		public static List<XmlDocument> GetComponentsByKeywords(string[] category, string[] keywords, int contentRepositoryId, int nbrOfRecords)
		{
			List<XmlDocument> results;
		    Criteria criteria = null;
			KeywordCriteria keywordCriteria ;
            ItemTypeCriteria typeCriteria = new ItemTypeCriteria(TRIDION_ITEMTYPE);
		    for(int i=0;i< keywords.Length;i++)
		    {

                keywordCriteria = new KeywordCriteria(category[i],keywords[i]);
		        if (i==0)
		        {
		            criteria = CriteriaFactory.And(keywordCriteria, typeCriteria);
		        }
		        else
		        {
                    criteria = CriteriaFactory.And(keywordCriteria,criteria);
		        }
		        
		    }
            
			
            results = BuildAndExecute(contentRepositoryId, new Criteria[] { criteria}, nbrOfRecords);
			return results;
		}
		/// <summary>
		/// this method shall be used to return the component based on a category
		/// </summary>
		/// <param name="category"></param>
		/// <param name="contentRepositoryId"></param>
		/// <param name="nbrOfRecords"></param>
		/// <returns></returns>
		public static List<XmlDocument> GetComponentsByCategory(string category, int contentRepositoryId, int nbrOfRecords)
		{
			List<XmlDocument> results;
			CategoryCriteria categoryCriteria = new CategoryCriteria(category);
			ItemTypeCriteria typeCriteria = new ItemTypeCriteria(TRIDION_ITEMTYPE);
			results = BuildAndExecute(contentRepositoryId, new Criteria[] { categoryCriteria, typeCriteria }, nbrOfRecords);
			return results;
		}
		/// <summary>
		/// GetContentRepositoryId shall return the Publication Id from TCM URI
		/// </summary>
		/// <param name="URI"></param>
		/// <returns></returns>
		public static int GetContentRepositoryId(string URI)
		{
			int contentRepositoryId;
			if (!string.IsNullOrEmpty(URI))
			{
                // Todo: optimize regex expression
                Regex tcmURIPattern = new Regex(TCM_URI_PATTERN);
                Regex tcmURIComponentPattern = new Regex(TCM_URI_COMPONENT_PATTERN);
                if (!(tcmURIPattern.IsMatch(URI)) && !(tcmURIComponentPattern.IsMatch(URI)))
                {
                    string logString = "An error has occurred. The request trace is: " + Environment.NewLine;
                    logString = string.Concat(logString, string.Format("Invalid Item URI:{0}", URI));
                    SS4TLogger.WriteLog(ELogLevel.WARN, logString);
                    throw new GenericIndexingException(TCM_URI_INVALID_CODE, TCM_URI_INVALID_VALUE);
                }
				TCMURI uri = new TCMURI(URI);
				contentRepositoryId = uri.GetPublicationId();
			}
			else
			{
				contentRepositoryId = 0;
			}
			return contentRepositoryId;
		}

		/// <summary>
		/// Execute Query will execute the query and will return the list of Xml if the component
		/// </summary>
		/// <param name="query"></param>
		/// <param name="limitFilter"></param>
		/// <param name="sortParameter"></param>
		/// <returns></returns>
		public static List<XmlDocument> ExecuteQuery(Query query, LimitFilter limitFilter, SortParameter sortParameter)
		{
			List<XmlDocument> results = null;
			query.AddSorting(sortParameter);
			query.AddLimitFilter(limitFilter);
			string[] compURIList = query.ExecuteQuery();
			if (compURIList != null && compURIList.Length > 0)
			{
				results = new List<XmlDocument>();
				for (int componentCount = 0; componentCount < compURIList.Length; componentCount++)
				{
					results.Add(GetComponent(compURIList[componentCount]));
				}
			}
			return results;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="schemaId"></param>
		/// <param name="contentRepositoryId"></param>
		/// <param name="noOfRecords"></param>
		/// <returns></returns>
		public static List<XmlDocument> GetContentByType(int schemaId, int contentRepositoryId, int noOfRecords)
		{
			List<XmlDocument> results;
			ItemSchemaCriteria schemaCriteria = new ItemSchemaCriteria(schemaId);
			ItemTypeCriteria typeCriteria = new ItemTypeCriteria(TRIDION_ITEMTYPE);
			results = BuildAndExecute(contentRepositoryId, new Criteria[]{schemaCriteria, typeCriteria}, noOfRecords);
			return results;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="URI"></param>
		/// <returns></returns>
		public static XmlDocument GetComponent(string URI)
		 {            
             ComponentPresentationFactory cpf;
             XmlDocument result = null;
             int contentRepositoryId = GetContentRepositoryId(URI);
             cpf = new ComponentPresentationFactory(contentRepositoryId);
             ComponentPresentation cp = cpf.GetComponentPresentationWithHighestPriority(URI);

             if (cp != null && (!string.IsNullOrEmpty(cp.Content)))
             {
                 result = new XmlDocument();
                 result.LoadXml(cp.Content);
             }
             else
             {
                 result = null;
             }

			return result;
		}

        /// <summary>
        /// Return the keywords based on Taxonomy 
        /// </summary>
        public static Keyword GetKeywords(int contentRepositoryId, int contentId,int itemType)
        {
            TaxonomyFactory taxonomyFactory = new TaxonomyFactory();

          
            string URI = string.Format(TCM_URI_FORMAT, contentRepositoryId, contentId, itemType);
            Keyword keyWord ;
            if (itemType == TRIDION_KEYWORDTYPE)
            {
                keyWord = taxonomyFactory.GetTaxonomyKeyword(URI);
            }
            else
            {
                keyWord = taxonomyFactory.GetTaxonomyKeywords(URI);
            }
            return keyWord;
            
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uriList"></param>
		/// <returns></returns>
        public static List<XmlDocument> GetComponents(string[] uriList)
        {
            List<XmlDocument> componentList = null;
            if (uriList != null && uriList.Length > 0)
            {
                componentList = new List<XmlDocument>();
                foreach (string uri in uriList)
                {
                    try
                    {
                        XmlDocument doc = GetComponent(uri);
                        if (doc != null)
                        {
                            componentList.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log Warning Here and just move
                        SS4TLogger.WriteLog(ELogLevel.WARN, "Error in getting component list: " + ex.Message + ex.StackTrace);
                    }
                }
                return componentList;
            }
            else
            {
                return componentList;
            }
        }
        /// <summary>
        /// Creates a new Link instance and returnes this as a string
        /// </summary>
        /// <param name="title">Attribute to include in the link as string</param>                        
        /// //example<a href="..">title</a>
        /// <param name="tcmuri">tcmUri of the Component as string</param>
        /// <returns>url string</returns>
        public static string ComponentLinkMethod(string tcmuri)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering method TridionDataHelper.ComponentLinkMethod");
            ComponentLink componentLink = null;
            Link link = null;
            int publicationID;
            String linkUrl = string.Empty;
            try
            {
                publicationID = TridionDataHelper.GetContentRepositoryId(tcmuri);
                SS4TLogger.WriteLog(ELogLevel.DEBUG, "Publication ID: "+publicationID);
                componentLink = new ComponentLink(publicationID);
                link = componentLink.GetLink(tcmuri);
                SS4TLogger.WriteLog(ELogLevel.DEBUG, "ComponentLink: "+link);
                if (link.IsResolved)
                {
                    linkUrl = link.Url;
                }
                SS4TLogger.WriteLog(ELogLevel.INFO, "Link URL : " + linkUrl);

            }
            catch (Exception ex)
            {
                SS4TLogger.WriteLog(ELogLevel.ERROR, ex.Message + ex.StackTrace);
            }
            finally
            {
                componentLink.Dispose();
                componentLink = null;
                link = null;
            }

            SS4TLogger.WriteLog(ELogLevel.INFO, "Exiting method TridionDataHelper.ComponentLinkMethod");
            return linkUrl;
        }
        
		#endregion
	}
}