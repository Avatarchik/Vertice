﻿#pragma strict

#if UNITY_WEBGL

function Awake(){
	Debug.LogError("ImpContextVid.Awake not implemented in WebGL");
	Debug.Break();
}


function FixedUpdate()
{	
	Debug.LogError("ImpContextVid.FixedUpdate not implemented in WebGL");
	Debug.Break();
}


public function OpenVidAsset(){ 
	Debug.LogError("ImpContextVid.OpenVidAsset not implemented in WebGL");
	Debug.Break();

}

public function VidAssetFile(pathToTex : String){
	Debug.LogError("ImpContextVid.VidAssetFile not implemented in WebGL");
	Debug.Break();

}


public function ContextVidImp(){
	Debug.LogError("ImpContextVid.ContextVidImp not implemented in WebGL");
	Debug.Break();

}

function VidNameSplit(){
	Debug.LogError("ImpContextVid.VidNameSplit not implemented in WebGL");
	Debug.Break();

}

function sendActive()
{
	Debug.LogError("ImpContextVid.sendActive not implemented in WebGL");
	Debug.Break();
}


#else


//script for importing assets to be associated with imported objects

import UnityEngine.UI;

public var infoCont : ObjInfoControl;
public var contextMediaType : String = "Video"; //new - used to send to media viewer
public var texLocation : String;

public var contextVid : GameObject;
private var contVid : RawImage;
public var contVidTitle : GameObject;
private var vidTex : MovieTexture;
public var contVidTex : MovieTexture;
public var vidAudio : AudioSource; 
private var assetFromFile : boolean;

public var playTime : float = 0f;

var vidFeedback : FeedbackScript; //***NEW***

function Awake(){

	contVid = contextVid.GetComponent(RawImage);
	
	var gameControl = GameObject.FindGameObjectWithTag("GameController");
	infoCont = gameControl.GetComponent(ObjInfoControl);
}


function FixedUpdate()
{	
	playTime += Time.deltaTime;
	
	if (playTime >= 3.0f && contVidTex != null) 
	{	
		//Debug.Log("Stopping Audio");
		contVidTex.Stop();
	}
}


public function OpenVidAsset(){ 
	
	//Filter Files
	var fileExtensions : String[] = [".ogv"]; //***NEW*** ".ogg" taken out - could put back after user testing
	UniFileBrowser.use.SetFileExtensions(fileExtensions); //***NEW***
	
	UniFileBrowser.use.OpenFileWindow(VidAssetFile);
}

public function VidAssetFile(pathToTex : String){
	
	texLocation = pathToTex;
	ContextVidImp();
	assetFromFile = true;
}


public function ContextVidImp(){

	Debug.Log("Context Vid Located at: " + texLocation);
		
	var wwwDirectory = "file://" + texLocation; //this will probably need to change for other OS (PC = file:/ [I think?]) - **REVISE**

	var www : WWW = new WWW(wwwDirectory);
	var videoTex = www.movie;
	
	while(!www.isDone){
	
		yield www;
	
		Debug.Log("Done Downloading Texture");
		
		if (www.isDone){
			break; //if done downloading image break loop
		}
	}	
	contVid.texture = videoTex;
	contVidTex = contVid.texture as MovieTexture;
	
	//Video Audio assign
	vidAudio.clip = contVidTex.audioClip;
	
	
	//Debug.Log("Ready to play");
	contVidTex.Play();
	//vidAudio.Play();	

	
	if (assetFromFile){
		VidNameSplit(); //only execute namesplit if from file
		
		//execute add create node for the context media
		infoCont.CreateContextNode("Video");
		infoCont.Save();  //for autosave
		
		if (vidFeedback == null) //***NEW***
			vidFeedback = GameObject.Find("AddVidButton").GetComponent.<FeedbackScript>(); //***NEW***
		vidFeedback.Feedback(); //***NEW***
	}
}

function VidNameSplit(){

	var directorySplit : String[];
	directorySplit = texLocation.Split("/"[0]);
	var vidFileName = directorySplit[directorySplit.Length-1]; //original file name
	
	var vidNameSplit = vidFileName.Split("."[0]); 
	var vidName = vidNameSplit[vidNameSplit.Length-2]; //object name minus extension
	
	infoCont.control.cntxtMediaName = vidName;
	infoCont.control.cntxtMediaLocation = texLocation;
	var vidNameScript = contVidTitle.GetComponent(ContextVidTitle);
	vidNameScript.VidName();
	
	
	Debug.Log("Context Vid Name: " + vidName);
	
	assetFromFile = false; //reset after import from file
}

function sendActive() //used to activate media viewer
{
	var mediaViewer = GameObject.FindGameObjectWithTag("MediaViewer");
	var mediaActiveScript = mediaViewer.GetComponent.<MetaPipe_MediaV_Activate>();

	//mediaActiveScript.activeMediaViewer(contextMediaType, texLocation);
	var vidTitle = contVidTitle.GetComponent.<Text>().text;
	Debug.Log("vidTitle: " + vidTitle);
//	mediaActiveScript.activeMediaViewer(vidTitle, contextMediaType, texLocation);
}
#endif