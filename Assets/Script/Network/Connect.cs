using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class Connect : MonoBehaviour {
	private string IpAddress;
	private string Port = "5554";
	public InputField ipinput;
	public GameObject Canvas;
	public Button Focus;
	// Use this for initialization
	void Start () {
		Canvas.SetActive (false);
		Scene temp = SceneManager.GetActiveScene();
		if (temp.name == "PSVitaDock") {
			
			
			NetworkManager.singleton.networkPort = int.Parse (Port);
			NetworkManager.singleton.StartHost ();
			
		} else {
		
		if (PlayerPrefs.HasKey("IPA"))
		{
			IpAddress = PlayerPrefs.GetString("IPA");
			ipinput.text = IpAddress;
		}else{
			
			IpAddress = "";
			ipinput.text = IpAddress;
		}
	
	
			Canvas.SetActive (true);
			Focus.Select ();
		}
		
	}
	public void Update(){
		if (!GameObject.Find ("Cliobj (Clone)")) {
			//Canvas.SetActive (true);
		}
	}
	public void ConnectButton(){
		IpAddress = ipinput.text;
		PlayerPrefs.SetString("IPA", IpAddress);
		PlayerPrefs.Save();
		NetworkManager.singleton.networkAddress = IpAddress;
		NetworkManager.singleton.networkPort = int.Parse(Port);
		NetworkManager.singleton.StartClient();
	
		Canvas.SetActive (false);
	}

	public void ScreenButtons(int id){
		ipinput.text += id;
	}

	public void DotButton(){
		ipinput.text += ".";
	}
	public void DelButton(){
		ipinput.text = ipinput.text.Substring (0, ipinput.text.Length - 1);
	}
	// Update is called once per frame

}
