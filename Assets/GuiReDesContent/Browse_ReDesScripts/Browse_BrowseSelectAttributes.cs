﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Browse_BrowseSelectAttributes : MonoBehaviour {

	//enables select attribute panel for user to refine their
	//browse query, then sends to browse control

	public Transform attributeParent; //transform for prefabs to be instantiated under
	public UnityEngine.Object attributePrefab;
	public Browse_BrowseControl BrowseCont;

	private string browseMode; //user defined browse mode



	/// <summary>
	/// Gets all attributes related to a user browse query type. Executes DCReader function.
	/// </summary>
	/// <param name="browseType">Type of browse user wants to view</param>
	public void GetAttributes(string browseType)
	{
//		Debug.Log("GetAttr: " + browseType);
		#if UNITY_WEBGL
		StartCoroutine (GetAttributesAsync (browseType));

		#elif UNITY_STANDALONE
//		Debug.Log ("Populating DublinCoreReader");
		DublinCoreReader.LoadXmlFromFile(Paths.ArtefactMetadata);

		BrowseMode(browseType);
		#endif
	}


	/// <summary>
	/// Provides a backing for GetAttributes(string browseType) that can load data in to the DublinCoreReader asynchronously in the case 
	/// where the DCReader has yet to be populated with data
	/// </summary>
	/// <param name="browseType">The field to browse on</param>
	private IEnumerator GetAttributesAsync(string browseType) {

		// If the DublinCoreReader has not been populated with data by some preceding operation, populate it now
		if (!DublinCoreReader.HasXml()) {
			
			Debug.Log ("Populating DublinCoreReader");
			UnityWebRequest www = UnityWebRequest.Get(Paths.ArtefactMetadata); //Paths.Remote + "/Metadata/Vertice_ArtefactInformation.xml"

			yield return www.Send ();

			if (www.isError) {
				Debug.Log ("There was an error downloading artefact information: " + www.error);
			} else {
				DublinCoreReader.LoadXmlFromText (www.downloadHandler.text);
			}
		}
		BrowseMode(browseType);
	}
		

	private void BrowseMode(string browseType)
	{
		string[] browseAttributes;

		switch (browseType) {

		case "Creator" : 
			browseAttributes = DublinCoreReader.GetValuesForCreator();	
			break;

		case "Contributor" : 
			browseAttributes = DublinCoreReader.GetValuesForContributor();	
			break;

		case "Date" : 
			browseAttributes = DublinCoreReader.GetAllYears();
			break;

		case "Subject" : 
			browseAttributes = DublinCoreReader.GetValuesForSubject();
			break;

		default:
			browseAttributes = null;
			break;
		}

		Array.Sort(browseAttributes);

		browseMode = browseType;
		InstantAttributes(browseAttributes);
	}


	/// <summary>
	/// Instantiates browse attribute prefabs for user to select
	/// </summary>
	/// <param name="InstantAttributes">attributes in XML related to user query</param>
	private void InstantAttributes( string[] browseAttributes)
	{
		ResetAttributes ();
		for (int i = 0; i < browseAttributes.Length; i++) {

			GameObject curBrowseAtt = UnityEngine.Object.Instantiate (attributePrefab, attributeParent) as GameObject;
			curBrowseAtt.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			curBrowseAtt.GetComponentInChildren<Text>().text = browseAttributes[i];
		}
	}


	/// <summary>
	/// Resets the attribute panel
	/// </summary>
	private void ResetAttributes()
	{
		for (int i = 0; i < attributeParent.childCount; i++) 
		{
			GameObject curAttr = attributeParent.GetChild (i).gameObject;
			Destroy(curAttr);
		}
	}


	/// <summary>
	/// Executed once user has finished their selection of relevant attributes
	/// </summary>
	public void DoneAttributeSelect()
	{
		List<string> activeAttributes = new List<string> ();

		for (int i = 0; i < attributeParent.childCount; i++) {

			Toggle curToggle = attributeParent.GetChild (i).GetComponent<Toggle>();
			if (curToggle.isOn) 
			{
				activeAttributes.Add (attributeParent.GetChild (i).GetComponentInChildren<Text>().text);
			}
		}

		string[] activeAttrArray = activeAttributes.ToArray(); //need to convert to array to account for DCReader, need list cause don't know how many attrs will be active
		string[] attributeIdentifiers;

		switch (browseMode)
		{
			case "Creator" : 
				attributeIdentifiers = DublinCoreReader.GetIdentifiersForCreators(activeAttrArray);
				break;

			case "Contributor" : 
				attributeIdentifiers = DublinCoreReader.GetIdentifiersForContributors(activeAttrArray);
				break;

			case "Date" : 
				attributeIdentifiers = DublinCoreReader.GetIdentifiersForYears(activeAttrArray);
				break;

			case "Subject" : 	
				attributeIdentifiers = DublinCoreReader.GetIdentifiersForSubjects(activeAttrArray);
				break;

			default :
				attributeIdentifiers = null;
				break;
		}
		if (attributeIdentifiers.Length > 0)
		{
			BrowseCont.ImportArtefacts(attributeIdentifiers);
		} 
		gameObject.SetActive(false); //turns panel off once query has been committed
	}
}
