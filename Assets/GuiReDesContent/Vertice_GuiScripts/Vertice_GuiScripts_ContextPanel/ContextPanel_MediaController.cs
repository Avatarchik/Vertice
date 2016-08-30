﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ContextPanel_MediaController : MonoBehaviour {

	public string artefactId = "DeerMan"; //TODO ID is currently updated via the Panel_TestController script - not ideal
	public Toggle imagesToggle;
	public Toggle audioToggle;
	public Toggle videoToggle;

	public Transform contentParent;
	public Object imagePrefab; //TODO something is wrong with this prefab setup - revise rectTransforms
	public Object audioPrefab;
	public Object videoPrefab;


	void Start()
	{
		LoadMedia();
	}

//	TODO this current process isn't ideal the ID is referenced from the TestController script
	//this could later be a 'Panel Controller', however not sure if this is the best approach
	//(i.e. relying on a public var too unstable?)

	public void LoadMedia() 
	{
		DublinCoreReader.LoadXml("file://" + Application.dataPath + "/Scripts/Metadata/TestAssets/Metapipe_ObjArchive_Subset_As_DublinCore.xml");

		Debug.Log("artefactId: " + artefactId);
		ResetPanel();

		if (imagesToggle.isOn)
		{
			InstantMedia(artefactId, "Image");
		}
		else if (audioToggle.isOn)
		{
			InstantMedia(artefactId, "Audio");
		}
		else if (videoToggle.isOn)
		{
			InstantMedia(artefactId, "Video");
		}
	}


	private void ResetPanel()
	{
		for (int i = 0; i < contentParent.childCount; i++) 
		{
			GameObject contentChild = contentParent.GetChild(i).gameObject;	
			Destroy(contentChild);
		}
	}


	private void InstantMedia(string identifier, string mediaType)
	{
		Object mediaPrefab = new Object();
		if (mediaType == "Image")
		{
			mediaPrefab = imagePrefab;
		} 
		else if (mediaType == "Audio")
		{
			mediaPrefab = audioPrefab;
		}
		else if (mediaType == "Video")
		{
			mediaPrefab = videoPrefab;
		}

		try {
			Dictionary<string, string>[] media = DublinCoreReader.GetContextualMediaArtefactWithIdentifierAndType(identifier, mediaType); 

			for (int i = 0; i < media.Length; i++) 
			{
				string mediaName = media[i]["MediaName"];
				string mediaLocation = media[i]["MediaLocation"];
				Debug.Log(mediaName);
				Debug.Log(mediaLocation);

				GameObject mediaInstant = Object.Instantiate(mediaPrefab, contentParent) as GameObject;

				Text mediaText = mediaInstant.transform.GetChild(1).gameObject.GetComponent<Text>(); //updates the prefab title
				mediaText.text = mediaName;

				if (mediaType == "Image")
				{
					BrowseImpContextImg imgImpScript = mediaInstant.GetComponentInChildren<BrowseImpContextImg>();
//					StartCoroutine(imgImpScript.ContextImgImp(mediaLocation)); //TODO this coroutine is not working properly due to path issues
				} 
//				else if (mediaType == "Audio")
//				{
//					mediaPrefab = audioPrefab;
//				}
//				else if (mediaType == "Video")
//				{
//					mediaPrefab = videoPrefab;
//				}
			}
		}
		catch(System.Exception ex)
		{
			Debug.Log("No Contextual Media for this artefact");
			//TODO create feedback prefab for when not media to view
		}
	}
}