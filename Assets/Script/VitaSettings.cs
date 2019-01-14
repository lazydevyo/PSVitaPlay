using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class VitaSettings : MonoBehaviour {
	public Text Ip_Label;
	public Text Quality_Label;
	public Text Audio_Label;
	public Text Fullscreen_Label;
	public Text AA_Effect_Label;
	public Text Color_Effect_Label;
	public Text Retro_Effect_Llabel;
	public GameObject VitaCapture;
	// Use this for initialization
	void Start () {
		Ip_Label.text = IPManager.GetIP (ADDRESSFAM.IPv4);
		Effect_Retro_Button ();
		Effect_AA_Button ();
		Effect_Color_Button ();
	}
	
	// Update is called once per frame
	void Update () {
		Quality_Label.text = VitaCapture.GetComponent<VitaCaptureU> ().QualityLevelSignLabel.text;
		if (VitaCapture.GetComponent<VitaCaptureU> ().AudioOutput == 0) {
			Audio_Label.text = "PC";

		} else {
			Audio_Label.text = "Console";
		}
		if (VitaCapture.GetComponent<VitaCaptureU> ().FullScreen == 0) {
			Fullscreen_Label.text = "No";
		} else {
			Fullscreen_Label.text = "Yes";
		}
	}
	public void Effect_AA_Button(){
		if(VitaCapture.GetComponent<VitaCaptureU> ().Effects_AA_int==1){
			AA_Effect_Label.text = "Yes";
		}else{
			AA_Effect_Label.text = "No";
		}
	}
	public void Effect_Color_Button(){
		if(VitaCapture.GetComponent<VitaCaptureU> ().ColorEn_int==1){
			Color_Effect_Label.text = "Yes";
		}else{
			Color_Effect_Label.text = "No";
		}
	}
	public void Effect_Retro_Button(){
		if(VitaCapture.GetComponent<VitaCaptureU> ().Retro_int==1){
			Retro_Effect_Llabel.text = "Yes";
		}else{
			Retro_Effect_Llabel.text = "No";
		}
	}
	public void AudioButton(){
		if (VitaCapture.GetComponent<VitaCaptureU> ().AudioOutput == 0) {

			VitaCapture.GetComponent<VitaCaptureU> ().AudioOutput = 1;
			Audio_Label.text = "Console";
		} else {

			VitaCapture.GetComponent<VitaCaptureU> ().AudioOutput = 0;
			Audio_Label.text = "PC";
		}
	}

	public void FullScreenButton(){
		if (VitaCapture.GetComponent<VitaCaptureU> ().FullScreen == 0) {
			VitaCapture.GetComponent<VitaCaptureU> ().FullScreen = 1;
			VitaCapture.GetComponent<VitaCaptureU> ().ChangeFullScreen(true);
		} else {
			VitaCapture.GetComponent<VitaCaptureU> ().FullScreen = 0;
			VitaCapture.GetComponent<VitaCaptureU> ().ChangeFullScreen(false);
		}
	}
	public void QualityButton(){

		if (VitaCapture.GetComponent<VitaCaptureU> ().QualityLevel == 0) {
			VitaCapture.GetComponent<VitaCaptureU> ().QualityLevel = 1;
			VitaCapture.GetComponent<VitaCaptureU> ().ChangeQuality (1);
			VitaCapture.GetComponent<VitaCaptureU> ().QualityLevelSignLabel.text= "Good";
		}else if(VitaCapture.GetComponent<VitaCaptureU> ().QualityLevel == 1){
			VitaCapture.GetComponent<VitaCaptureU> ().QualityLevel = 2;
			VitaCapture.GetComponent<VitaCaptureU> ().ChangeQuality (2);
			VitaCapture.GetComponent<VitaCaptureU> ().QualityLevelSignLabel.text= "HQ";
		}else if(VitaCapture.GetComponent<VitaCaptureU> ().QualityLevel == 2){
			VitaCapture.GetComponent<VitaCaptureU> ().QualityLevel = 0;
			VitaCapture.GetComponent<VitaCaptureU> ().ChangeQuality (0);
			VitaCapture.GetComponent<VitaCaptureU> ().QualityLevelSignLabel.text= "Smooth";
		}

		Quality_Label.text = VitaCapture.GetComponent<VitaCaptureU> ().QualityLevelSignLabel.text;

	}
}
