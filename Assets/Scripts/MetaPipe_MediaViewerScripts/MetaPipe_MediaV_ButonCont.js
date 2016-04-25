﻿#pragma strict

import UnityEngine.UI;

var mvActivateScript : MetaPipe_MediaV_Activate;
var audSrce : AudioSource;

var playButton : Button;
var stopButton : Button;
var replayButton : Button;

var curMediaType : String;
var curVid : MovieTexture;


function setMedia( mediaType : String)
{
	curMediaType = mediaType;
	
	if (mediaType == "Video")
	{	
		curVid = mvActivateScript.vidTex;
		audSrce = mvActivateScript.audSrce;
	}
	if (mediaType == "Audio")
	{
		audSrce = mvActivateScript.audSrce;
	}
}


function playMedia()
{
	if (curMediaType == "Video")
	{
		curVid.Play();
		audSrce.Play();
	}
	else if (curMediaType == "Audio")
		audSrce.Play();	
}

function pauseMedia()
{
	if (curMediaType == "Video")
	{
		curVid.Pause();
		audSrce.Pause();
	} 
	else if (curMediaType == "Audio")
		audSrce.Pause();
}

function replayMedia()
{
	if (curMediaType == "Video")
	{
		curVid.Stop();
		audSrce.Stop();
		
		curVid.Play();
		audSrce.Play();
	}
	else if (curMediaType == "Audio")
	{
		audSrce.Stop();
		audSrce.Play();
	}
}