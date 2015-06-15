/**
 * @(#)Utility.java
 * @author PankajGaur
 */

package com.tridion.storage.extension;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.net.URL;
import java.net.URLConnection;
import java.sql.Connection;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.w3c.dom.*;

import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.UriBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;
import org.xml.sax.InputSource;

import com.tridion.broker.StorageException;
import com.tridion.storage.Publication;
import com.tridion.storage.StorageManagerFactory;
import com.tridion.storage.StorageTypeMapping;
import com.tridion.storage.dao.PublicationDAO;
import com.sun.jersey.api.client.Client;
import com.sun.jersey.api.client.WebResource;
import com.sun.jersey.api.client.config.DefaultClientConfig;


/**
 * This is the Utility class containing various commonly used functions like:
 * parsing an xml, invoking a RestFul client, Retrieving a meta data field etc.
 * @author PankajGaur
 *
 */
public class Utility 
{
	private String xmlFilePath = null;
	private Logger log;
	public static Connection con;
	
	/**
	 * Default Constructor
	 */
	public Utility()
	{
		log = LoggerFactory.getLogger(Utility.class);
	}
	
	/**
	 * Constructor taking XMLFile path as parameter for XML parsing 
	 * @param xmlPath
	 */
	public Utility(String xmlPath)
	{
		log = LoggerFactory.getLogger(Utility.class);
		log.info("Constructor: Setting Utility Class");
		xmlFilePath = xmlPath;
		log.info("Constructor: Created Utility Class");
	}
	
	/**
	 * Get value of a node specified by the nodeID of an XML file specified by the xmlFilePath
	 * @param nodeID
	 * @return Value of the XMLNode specified by the nodeID
	 */
	public String GetNodeValueFromXML(String nodeID)
	{
		log.info("Entering Utility.GetNodeValueFromXML with XML File Path: " + xmlFilePath + " And Node ID: "+ nodeID);
		
		String returnValue = null;
		DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
        DocumentBuilder docBuilder = null;
        Document doc = null;
		try 
		{
			docBuilder = docBuilderFactory.newDocumentBuilder();
        	doc = docBuilder.parse(new File(xmlFilePath));
        	NodeList nodeList = doc.getElementsByTagName(nodeID);
        	if(nodeList != null && nodeList.getLength() > 0)
        	{
        		returnValue = nodeList.item(0).getTextContent();
        	}
		}
		catch (SAXException e) 
		{
			log.debug("SAXException:" + e.getLocalizedMessage());
		} 
		catch (ParserConfigurationException e) 
		{
			log.debug("ParserConfigurationException:" + e.getLocalizedMessage());
		}
		catch (IOException e) 
		{
			log.debug("IOException:" + e.getLocalizedMessage());
		}
		finally
		{
			doc= null;
			docBuilder = null;
			docBuilderFactory = null;
		}
		log.info("Exiting Utility.GetNodeValueFromXML"); 
		return returnValue;
	}
	
	/**
	 * Get a component meta field specified by the metaFieldName from the Component Presentation
	 * @param metaFieldName
	 * @param componentPresentation
	 * @return Value of the meta data field
	 * @throws StorageException
	 */
	public String GetComponentMetaFieldValue(String metaFieldName, String componentPresentation) throws StorageException
	{
		log.info("Entering Utility.GetComponentMetaFieldValue");
		String componentMetaFieldValue = null;
		DocumentBuilderFactory xmlDocBuilderFactory = DocumentBuilderFactory.newInstance();
        DocumentBuilder xmlDocBuilder = null;
        Document doc = null;
        InputSource xmlInputSource = new InputSource();
		try 
		{
			xmlDocBuilder = xmlDocBuilderFactory.newDocumentBuilder();
			String dcpXMLNSReplaced = componentPresentation.replace("xmlns:", "");
			log.debug("DCP After xmlns replaced :" + dcpXMLNSReplaced);
			xmlInputSource.setCharacterStream(new StringReader(dcpXMLNSReplaced));

			doc = xmlDocBuilder.parse(xmlInputSource);
        	NodeList nodeList = doc.getElementsByTagName(metaFieldName);
        	if(nodeList != null && nodeList.getLength() > 0)
        	{
        		componentMetaFieldValue = nodeList.item(nodeList.getLength()-1).getTextContent();
        	}
		} 
		catch(Exception ex)
		{
			log.debug("Exception: " + ex.getMessage());
			throw new StorageException(GetNodeValueFromXML("Err_EN_MetaFieldRetrieval") + ex.getLocalizedMessage());
		}
		finally
		{
			doc= null;
			xmlDocBuilder = null;
			xmlDocBuilderFactory = null;
			xmlInputSource = null;
		}
		
		log.debug("Exiting Utility.GetComponentMetaFieldValue");
		return componentMetaFieldValue;
	}
	
	/**
	 * Get meta value specified by the Publication Key attribute of the Publication and separated by last instance of "-"
	 * For example: <publication key> - <meta value>
	 * This function for above example will return <meta value>
	 * @param publicationID
	 * @return Publication Language
	 * @throws StorageException
	 */
	public String GetPublicationMetaFieldValue(int publicationID) throws StorageException
	{
		log.info("Entering Utility.GetPublicationMetaFieldValue");
		String publicationMetaFieldValue = null;
		PublicationDAO pubDAO = null;
		Publication pub = null;
		try
		{
			pubDAO =  (PublicationDAO)StorageManagerFactory.getDAO(publicationID, StorageTypeMapping.PUBLICATION);
			pub = pubDAO.findById(publicationID);
			
			if(pub.getKey().lastIndexOf("-") > 0)
			{
				publicationMetaFieldValue = pub.getKey().substring(pub.getKey().lastIndexOf("-") + 1);
			}
		}
		catch(StorageException ex)
		{
			throw new StorageException(GetNodeValueFromXML("Err_EN_LanguageMetaFieldRetrieval") + ex.getLocalizedMessage());		
		}
		catch(Exception ex)
		{
			throw new StorageException(GetNodeValueFromXML("Err_EN_LanguageMetaFieldRetrieval") + ex.getLocalizedMessage());
		}
		finally
		{
			pub = null;
			pubDAO = null;
		}
		log.info("Exiting Utility.GetPublicationMetaFieldValue");
		return publicationMetaFieldValue;
	}
	
	/**
	 * This method invokes a method specified by the methodName parameter of a RestFul service over HTTP POST 
	 * @param url
	 * @param request
	 * @param methodName
	 * @return Response of the method invoke
	 * @throws IOException
	 */
	public String InvokeServicePost(String url, String request, String methodName) throws IOException
	{
		log.info("Entering Utility.InvokeServicePost with URL: "+ url + "Request: "+ request + "Method Name: " + methodName);
		String serviceResponse = null;
		try
		{
			Client client = Client.create(new DefaultClientConfig());
			WebResource service = client.resource(UriBuilder.fromUri(url).build());
			serviceResponse = service.path(methodName).type(MediaType.APPLICATION_JSON).post(String.class, request);
		}
		catch(Exception ex)
		{
			log.error("Exception in calling HTTP Post Method. Error: "+ex.getMessage());
			throw new IOException(ex.getMessage());
		}
		log.info("Exiting Utility.InvokeServicePost");
		return serviceResponse;
	}

	/**
	 * This method invokes a GET based RESTful service
	 * @param url
	 * @return response
	 * @throws IOException
	 */
	public String InvokeService(String url) throws  IOException 
	{
		log.info("Entered utility.InvokeService with url: "+url);
		URL uri =null;
		URLConnection urlConnection = null;  
		BufferedReader inStream = null;
		
		String inputLine;
		String response = "";
		try 
		{
			uri= new URL(url);
			urlConnection= uri.openConnection();
			inStream =new BufferedReader(
	                new InputStreamReader(
	                		urlConnection.getInputStream()));
			while ((inputLine = inStream.readLine()) != null)
			{
				response += inputLine;
			}		
		} catch (IOException e) 
		{
			throw new IOException("Fatal transport error: " + e.getMessage());
		} 
		finally 
		{
			// close input stream
			inStream.close();
		}
		log.info("Exiting utility.InvokeService");
		return response;
	}
}
