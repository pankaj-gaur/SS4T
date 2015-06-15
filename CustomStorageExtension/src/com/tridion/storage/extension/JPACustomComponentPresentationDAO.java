/**
 * @(#) JPACustomComponentPresentationDAO.java
 * @author PankajGaur
 */

package com.tridion.storage.extension;

import java.io.File;
import java.io.UnsupportedEncodingException;
import java.net.URISyntaxException;
import java.security.CodeSource;
import java.util.Collection;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;

import org.codehaus.jettison.json.JSONObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Component;

import com.tridion.broker.StorageException;
import com.tridion.dcp.ComponentPresentationFactory;
import com.tridion.storage.ComponentPresentation;
import com.tridion.storage.dao.ComponentPresentationDAO;
import com.tridion.storage.persistence.JPAComponentPresentationDAO;
import com.tridion.storage.util.ComponentPresentationTypeEnum;

@Component("JPACustomComponentPresentationDAO")
@Scope("prototype")
/**
 * This class extends the default behavior of Storage Layer for the Component Presentations.
 * Apart from default behavior, this class also stores the Component Presentations with in the SOLR by
 * invoking the REST base SOLR Index service and passing the Component Presentation to it.
 * @author PankajGaur
 * @author 
 */
public class JPACustomComponentPresentationDAO extends
		JPAComponentPresentationDAO implements ComponentPresentationDAO {

	private String metaFieldName = null;
	private String serviceEndPoint = null;
	private String componentPresentation = null;
	private Logger log;
	private String metaFieldValue = null;
	private String languageMetaFieldValue = null;
	private String errorMessage = "";
	private String addDocumentMethodName = "";
	private String addDocumentQueryString = "";
	private String configFilePath = "";
	private String configFileRelativePath = "\\config\\CustomStorageConfig.xml";

	public Utility utility = null;

	/**
	 * Constructor - 1
	 * 
	 * @param storageId
	 * @param entityManagerFactory
	 * @param storageType
	 */
	public JPACustomComponentPresentationDAO(String storageId,
			EntityManagerFactory entityManagerFactory, String storageType) {

		super(storageId, entityManagerFactory, storageType);
		log = LoggerFactory.getLogger(JPACustomComponentPresentationDAO.class);
		log.debug("Entering Constructor 1: JPACustomComponentPresentationDAO("
				+ storageId + "," + entityManagerFactory.toString() + ","
				+ storageType + ")");
	}

	/**
	 * Constructor - 2
	 * 
	 * @param storageId
	 * @param entityManagerFactory
	 * @param entityManager
	 * @param storageType
	 */
	public JPACustomComponentPresentationDAO(String storageId,
			EntityManagerFactory entityManagerFactory,
			EntityManager entityManager, String storageType) {
		super(storageId, entityManagerFactory, entityManager, storageType);
		log = LoggerFactory.getLogger(JPACustomComponentPresentationDAO.class);
		log.debug("Entering Constructor 2: JPACustomComponentPresentationDAO("
				+ storageId + "," + entityManagerFactory.toString() + ","
				+ entityManager.toString() + "," + storageType + ")");
	}

	private String GetConfigFilePath() {
		String _configFilePath;

		// code to pick config file from appropriate path
		CodeSource codeSource = JPACustomComponentPresentationDAO.class
				.getProtectionDomain().getCodeSource();

		File jarFile = null;

		try {
			jarFile = new File(codeSource.getLocation().toURI().getPath());
		} catch (URISyntaxException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}

		String configFileDir = jarFile.getParentFile().getParentFile()
				.getPath();
		log.debug("Configuration file directory is: " + configFileDir);

		_configFilePath = configFileDir + configFileRelativePath;
		log.info("ConfigFilePath: " + _configFilePath);
		return _configFilePath;
	}

	/**
	 * Overridden create method : This method will get called in case of a
	 * Publish action
	 * 
	 * @param itemToCreate
	 * @param componentPresentationType
	 * @throws StorageException
	 */
	public void create(ComponentPresentation itemToCreate,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		String errorMessage = "";
		log.info("Entering Method: JPACustomComponentPresentationDAO.Create");

		configFilePath = GetConfigFilePath();

		super.create(itemToCreate);
		int templateID = itemToCreate.getTemplateId();
		log.info("Template ID: " + Integer.toString(templateID)
				+ " | Config Path: " + configFilePath);
		utility = new Utility(configFilePath);
		String templateToIndex = utility
				.GetNodeValueFromXML("TemplateIdToIndex");
		log.info("Template ID from Config: " + templateToIndex);
		if (templateToIndex.trim().equals(Integer.toString(templateID))) {
			log.info("Component Presentation Type: "
					+ componentPresentationType.name());

			byte[] dcpBytes = itemToCreate.getContent();

			try {
				componentPresentation = new String(dcpBytes, "UTF-8");
				log.info("Component Presentation After Encoding"
						+ componentPresentation);
			} catch (UnsupportedEncodingException e) {
				log.info("Error: Encoding Error from byte[] to string");
			}

			log.info("DCP: " + componentPresentation + " Legth: "
					+ componentPresentation.length());
			if (!componentPresentation.trim().isEmpty()) {
				int componentID = itemToCreate.getComponentId();

				try {
					validateMetadata(itemToCreate, componentID,
							componentPresentationType);
					if (metaFieldValue.toLowerCase().equals("true")) {
						String serviceResponse = IndexToSolr(
								languageMetaFieldValue, addDocumentMethodName,
								addDocumentQueryString);
						log.info("Index Service - Add Document Response: "
								+ serviceResponse);

						errorMessage = utility
								.GetNodeValueFromXML("Err_EN_IndexFailure");
						if (serviceResponse != null) {
							boolean isIndexingSuccessful = ValidateIndexing(serviceResponse);
							if (!isIndexingSuccessful) {
								// If Indexing is success but some issue
								// occurred at the Validation in
								// ValidateIndexing method
								// Then Remove the indexed component from SOLR
								/*
								 * String componentTCMURI = "tcm:" +
								 * Integer.toString
								 * (itemToCreate.getPublicationId()) + "-" +
								 * Integer.toString(componentID); String
								 * removeDocumentMethodName =
								 * utility.GetNodeValueFromXML
								 * ("RemoveDocumentMethodName"); String
								 * removeDocumentQueryString =
								 * utility.GetNodeValueFromXML
								 * ("RemoveDocumentQueryString");
								 * RemoveFromSolr(componentTCMURI,
								 * languageMetaFieldValue,
								 * removeDocumentMethodName,
								 * removeDocumentQueryString);
								 */
								throw new StorageException(errorMessage);
							}
						} else {
							log.debug("Error: Service Response is Null");
							throw new StorageException(errorMessage);
						}
					} else {
						log.info("Index MetaData is set to False, no need to index the component");
					}
				} catch (Exception ex) {
					errorMessage = utility
							.GetNodeValueFromXML("Err_EN_IndexFailure");
					throw new StorageException(errorMessage);
				}

			}
		}
	}

	public void validateMetadata(ComponentPresentation itemToCreate,
			int componentID,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		utility = new Utility(configFilePath);
		log.info("Component ID | Component Presentation Type | Component Presentation: "
				+ Integer.toString(componentID)
				+ "|"
				+ componentPresentationType.name()
				+ "|"
				+ componentPresentation);

		metaFieldName = utility.GetNodeValueFromXML("MetaFieldName");
		serviceEndPoint = utility.GetNodeValueFromXML("ServiceEndPoint");
		addDocumentMethodName = utility
				.GetNodeValueFromXML("AddDocumentMethodName");
		addDocumentQueryString = utility
				.GetNodeValueFromXML("AddDocumentQueryString");

		/*
		 * *****if(metaFieldName == null) { errorMessage =
		 * utility.GetNodeValueFromXML("Err_EN_MetaFieldConfiguration"); throw
		 * new StorageException(errorMessage); }
		 */

		if (serviceEndPoint == null) {
			errorMessage = utility
					.GetNodeValueFromXML("Err_EN_AddDocumentEndPointConfiguration");
			throw new StorageException(errorMessage);
		}

		metaFieldValue = "true";// utility.GetComponentMetaFieldValue(metaFieldName,
								// componentPresentation);
		languageMetaFieldValue = "en";// utility.GetPublicationMetaFieldValue(itemToCreate.getPublicationId());

		log.debug("Hardcoded Language Meta Field Value: "
				+ languageMetaFieldValue.trim());
		log.info(" Hardcoded Index Meta Field Value: "
				+ metaFieldValue.toString());

		if (metaFieldValue == null || metaFieldValue == "") {
			errorMessage = utility.GetNodeValueFromXML("Err_EN_MetaFieldValue");
			throw new StorageException(errorMessage);
		}

		if (languageMetaFieldValue == null || languageMetaFieldValue == "") {
			errorMessage = utility
					.GetNodeValueFromXML("Err_EN_PublicationLanguage");
			throw new StorageException(errorMessage);
		}

	}

	/**
	 * Overridden remove method : This method will get called in case of
	 * un-publish action
	 * 
	 * @param itemToRemove
	 * @param componentPresentationType
	 * @throws StorageException
	 */
	public void remove(ComponentPresentation itemToRemove,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		log.debug("JPACustomComponentPresentationDAO: Remove 1");

		int publicationID = itemToRemove.getPublicationId();
		log.debug("Publication ID: " + Integer.toString(publicationID));

		int componentID = itemToRemove.getComponentId();
		log.debug("Component ID: " + Integer.toString(componentID));

		super.remove(publicationID, componentID, itemToRemove.getTemplateId(),
				componentPresentationType);

		// RemoveComponent(publicationID, componentID, itemToRemove);
	}

	/**
	 * Overridden remove method : This method will get called in case of
	 * un-publish action
	 * 
	 * @param publicationId
	 * @param componentId
	 * @param componentTemplateId
	 * @param componentPresentationType
	 * @throws StorageException
	 */
	public void remove(int publicationId, int componentId,
			int componentTemplateId,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		log.debug("JPACustomComponentPresentationDAO: Remove 2");
		log.debug("Publication ID: " + publicationId + "\n Component ID: "
				+ componentId + "\n Template ID: " + componentTemplateId);
		ComponentPresentationFactory cpf = new ComponentPresentationFactory(
				publicationId);

		com.tridion.dcp.ComponentPresentation cp = cpf
				.getComponentPresentation(componentId, componentTemplateId);
		ComponentPresentation itemToRemove = new ComponentPresentation();

		if (cp != null) {
			log.debug("Setting component presentation in Remove 2 with Component Presentation: "
					+ cp.getContent()
					+ " And Component Template: "
					+ componentTemplateId);

			itemToRemove.setComponentId(componentId);
			itemToRemove.setPublicationId(publicationId);
			itemToRemove.setTemplateId(componentTemplateId);
			log.debug("Component Presentation After Setting from factory: "
					+ itemToRemove.getContent());
		} else {
			log.debug("CP from Factory is NULL");
		}

		super.remove(publicationId, componentId, componentTemplateId,
				componentPresentationType);
		RemoveComponent(publicationId, componentId, itemToRemove);
	}

	/**
	 * Overridden update Method
	 * 
	 * @param itemToUpdate
	 * @param componentPresentationType
	 * @throws StorageException
	 */
	public void update(ComponentPresentation itemToUpdate,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		log.debug("JPACustomComponentPresentationDAO: Update");
		super.update(itemToUpdate);
	}

	/**
	 * Overridden getComponentPresentation Method
	 * 
	 * @param publicationId
	 * @param componentId
	 * @param templateId
	 * @param componentPresentationType
	 * @throws StorageException
	 */
	public ComponentPresentation getComponentPresentation(int publicationId,
			int componentId, int templateId,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		log.debug("JPACustomComponentPresentationDAO: GetCP");
		return super.getComponentPresentation(publicationId, componentId,
				templateId, componentPresentationType);
	}

	/**
	 * Overridden findAll Method
	 * 
	 * @param publicationId
	 * @param componentId
	 * @param componentPresentationType
	 * @throws StorageException
	 */
	@SuppressWarnings({ "unchecked", "rawtypes" })
	public Collection findAll(int publicationId, int componentId,
			ComponentPresentationTypeEnum componentPresentationType)
			throws StorageException {
		log.debug("JPACustomComponentPresentationDAO: findALL");
		return super.findAll(publicationId, componentId,
				componentPresentationType);
	}

	/**
	 * This method manipulate the RESTFul service end point and invoke the Index
	 * Service to Add the Component Presentation to SOLR by calling the
	 * AddDocument method
	 * 
	 * @param serviceEndPoint
	 * @param languageMetaFieldValue
	 * @return Response of the AddDocument call to the Index Service
	 * @throws StorageException
	 */
	private String IndexToSolr(String languageMetaFieldValue,
			String addDocumentMethodName, String addDocumentQueryString)
			throws StorageException {
		String serviceResponse = null;
		String DCPQuoteReplace = componentPresentation.replace("\"", "'");
		String DCPNewLineReplace = DCPQuoteReplace.replace("\n", "\\n");
		String serviceEndPointLangReplace = addDocumentQueryString.replace(
				"LANGUAGE", languageMetaFieldValue.toLowerCase().trim());
		String finalQueryString = serviceEndPointLangReplace.replace(
				"COMPONENTPRESENTATION", DCPNewLineReplace);

		try {
			Utility utility = new Utility();
			serviceResponse = utility.InvokeServicePost(serviceEndPoint,
					finalQueryString, addDocumentMethodName);
		} catch (Exception e) {
			log.debug("Exception in Invoking the Index Service");
			throw new StorageException(e.getMessage());
		}
		return serviceResponse;
	}

	/**
	 * This method Validate whether the Indexing to SOLR (Addition/Removal of
	 * Document) is Passed or Failed
	 * 
	 * @param serviceResponse
	 * @return true if validated, false otherwise
	 */
	private boolean ValidateIndexing(String serviceResponse) {
		boolean isValidated = false;
		try {

			JSONObject responseObject_Payload = new JSONObject(serviceResponse)
					.getJSONObject("ServicePayload");
			if (responseObject_Payload != null
					&& responseObject_Payload.has("Result")) {
				int result = 1;
				result = responseObject_Payload.getInt("Result");
				log.debug("Response Result: " + Integer.toString(result));
				if (result == 0) {
					isValidated = true;
				}
			}
		} catch (Exception ex) {
			return false;
		}
		return isValidated;
	}

	/**
	 * This method manipulate the service end point for Removal of data from the
	 * SOLR
	 * 
	 * @param publicationID
	 * @param componentID
	 * @throws StorageException
	 */
	private void RemoveComponent(int publicationID, int componentID,
			ComponentPresentation itemToRemove) throws StorageException {
		log.info("Entered method Removecomponent with publicationID: "
				+ publicationID + " componentID: " + componentID);
		String errorMessage = "";
		String componentTCMURI = "tcm:" + Integer.toString(publicationID) + "-"
				+ Integer.toString(componentID);
		configFilePath = GetConfigFilePath();
		log.debug("ConfigFilePath: " + configFilePath);
		utility = new Utility(configFilePath);

		serviceEndPoint = utility.GetNodeValueFromXML("ServiceEndPoint");
		String removeDocumentMethodName = utility
				.GetNodeValueFromXML("RemoveDocumentMethodName");
		String removeDocumentQueryString = utility
				.GetNodeValueFromXML("RemoveDocumentQueryString");

		if (serviceEndPoint == null) {
			errorMessage = utility
					.GetNodeValueFromXML("Err_EN_RemoveDocumentEndPointConfiguration");
			throw new StorageException(errorMessage);
		}

		String languageMetaFieldValue = utility
				.GetPublicationMetaFieldValue(publicationID);
		String serviceResponse = RemoveFromSolr(componentTCMURI,
				languageMetaFieldValue, removeDocumentMethodName,
				removeDocumentQueryString);
		log.info("Index Service - Remove Document Response: " + serviceResponse);

		errorMessage = utility.GetNodeValueFromXML("Err_EN_RemoveFailure");
		if (serviceResponse != null) {
			boolean isIndexingSuccessful = ValidateIndexing(serviceResponse);
			if (!isIndexingSuccessful) {
				throw new StorageException(errorMessage);
			}
		} else {
			log.debug("Error: Service Response is Null");
			throw new StorageException(errorMessage);
		}
	}

	/**
	 * This method manipulate the RESTFul service end point and invoke the Index
	 * Service to Remove the Component Presentation from SOLR by calling the
	 * RemoveDocument method
	 * 
	 * @param serviceEndPoint
	 * @param componentTCMURI
	 * @param languageMetaFieldValue
	 * @return Response of the RemoveDocument call to the Index Service
	 * @throws StorageException
	 */
	private String RemoveFromSolr(String componentTCMURI,
			String languageMetaFieldValue, String removeDocumentMethodName,
			String removeDocumentQueryString) throws StorageException {
		String serviceResponse = null;
		String serviceEndPointLangReplace = removeDocumentQueryString.replace(
				"LANGUAGE", languageMetaFieldValue.toLowerCase().trim());
		String finalQueryString = serviceEndPointLangReplace.replace("TCMURI",
				componentTCMURI);
		try {
			log.debug("Query Parameter: " + finalQueryString);
			Utility utility = new Utility();
			serviceResponse = utility.InvokeServicePost(serviceEndPoint,
					finalQueryString, removeDocumentMethodName);
		} catch (Exception e) {
			log.debug("Exception in Invoking the Index Service");
			throw new StorageException(e.getMessage());
		}

		log.debug("Exiting RemoveFromSolr");
		return serviceResponse;
	}

	public String getBindingName() {
		return super.getBindingName();
	}

	public String getStorageId() {
		return super.getStorageId();
	}

	public String getStorageType() {
		return super.getStorageType();
	}

	public String getTypeMapping() {
		return null;
	}
}
