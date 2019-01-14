using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RenderHeads.Media.AVProLiveCamera;
using UnityEngine.PostProcessing;


//using Aforge.Video;
public class VitaCaptureU : MonoBehaviour {
public static int reloadsettingsID=0;
    public static bool reloadsettings = false;
	public PostProcessingProfile PProfile;
	
	static public bool PSVitaConnected;
	static public bool DeviceFailed = false;
	public AVProLiveCamera _liveCamera;
//	public AVProLiveCameraDevice _liveCameraManager;
	public Text NoVitaText;
	public GameObject OutputSprite;
	public Image PSLogo;
	public Image Background;
	public Image AudioSign;
	public Image fullscreenSign;
	public Image screenshotUI;
	public Image screenshotUI_Image;
	public Image QualityLevelSign;
	public Text QualityLevelSignLabel;
	public GameObject SettingsPanel;
	private AudioSource VitaAudio;
	public Text ResDebug;
	float[] clipSampleData = new float[1024];
	public bool AudioSilent;

	public string DeviceName;
	public int QualityLevel=0;
	public int FullScreen = 0;
	public int AudioOutput=0; 
	private int ResScreen_w;
	private int ResScreen_h;

	public int Effects_AA_int=0;
	public int ColorEn_int=0;
	public int Retro_int=0;
	public AudioClip Clip_Click;
	public AudioClip Clip_FullScreen;

	// Use this for initialization
	void SaveSettings(){
		PlayerPrefs.SetInt ("QualityLevel", QualityLevel);
		PlayerPrefs.SetInt ("FullScreen", FullScreen);
		PlayerPrefs.SetInt ("Effects_AA_int", Effects_AA_int);
		PlayerPrefs.SetInt ("ColorEn_int", ColorEn_int);
		PlayerPrefs.SetInt ("Retro_int", Retro_int);
		PlayerPrefs.Save ();
	}
    public void Reload()
    {
        LoadSettings();
	if(reloadsettingsID==0){     
			QualityChangeFunct();
	}else if(reloadsettingsID==1){     
			Effects_Color(ColorEn_int);
	}else if(reloadsettingsID==2){ 
			Effects_AA(Effects_AA_int);
     
	}else if(reloadsettingsID==3){     
        Effects_Retro(Retro_int);
}

    }
	void LoadSettings(){
		if (!PlayerPrefs.HasKey ("QualityLevel")) {
			SaveSettings ();
		} else {
			QualityLevel = PlayerPrefs.GetInt("QualityLevel");
		}

		if (!PlayerPrefs.HasKey ("FullScreen")) {
			SaveSettings ();
		} else {
			FullScreen = PlayerPrefs.GetInt("FullScreen");
		}
		if (!PlayerPrefs.HasKey ("Effects_AA_int")) {
			SaveSettings ();
		} else {
			Effects_AA_int = PlayerPrefs.GetInt("Effects_AA_int");
		}
		if (!PlayerPrefs.HasKey ("ColorEn_int")) {
			SaveSettings ();
		} else {
			ColorEn_int = PlayerPrefs.GetInt("ColorEn_int");
		}
		if (!PlayerPrefs.HasKey ("Retro_int")) {
			SaveSettings ();
		} else {
			Retro_int = PlayerPrefs.GetInt("Retro_int");
		}
	}


	IEnumerable FlushMemory(){
		for (;;) {

			_liveCamera.GetComponent<AVProLiveCamera>().OnDestroy();

			yield return new WaitForSeconds(.2f);
			
			
		}
	}
	void Awake(){
	
		ResScreen_w = Screen.currentResolution.width;
		ResScreen_h = Screen.currentResolution.height;

		LoadSettings ();

		if (FullScreen == 1) {
			ChangeFullScreen(true);
		} else {
			ChangeFullScreen(false);	
		}
	}

	void Initalize(){

		
		VitaAudio = this.GetComponent<AudioSource> ();
		fullscreenSign.gameObject.SetActive (false);
		AudioSign.gameObject.SetActive (false);
	}

	void Start () {
	

		Application.HasUserAuthorization (UserAuthorization.Microphone);
		Application.HasUserAuthorization (UserAuthorization.WebCam);
		//QualityLevelSign.gameObject.SetActive (false);
		Initalize ();

			if (!PSVitaConnected) {
			StartCoroutine ("RefreshVita");


		} 

		if(QualityLevel==0){
			QualityLevelSignLabel.text = "Smooth";
			//QualityLevel=2;
		}else if(QualityLevel==1){
			QualityLevelSignLabel.text = "Medium";
			//QualityLevel=0;
		}else if(QualityLevel==2){
			QualityLevelSignLabel.text = "HQ";
			//QualityLevel=0;
		}

		ChangeQuality (QualityLevel);
		Effects_AA (Effects_AA_int); 
		Effects_Color (ColorEn_int);
		Effects_Retro (Retro_int);
		StartCoroutine ("FlushAudio");
		//StartCoroutine("FlushMemory");
	
	}

	IEnumerator RefreshVita(){
		for (;;) {
			_liveCamera._deviceSelection = AVProLiveCamera.SelectDeviceBy.Name;
			_liveCamera._desiredDeviceIndex = 0;
			_liveCamera.Begin ();


			//Audio

			checkAudioDevices ();
		
			if(DeviceName!=null){

				if(AudioOutput==0){
				StartAudio ();
				}
			
			}


			yield return new WaitForSeconds(2f);
		
		
	}
	}
	
	
	public void ChangeQuality(int i){
	
		//2 high res;
		//0 low fast
		_liveCamera._modeSelection = AVProLiveCamera.SelectModeBy.Index;
		_liveCamera._desiredModeIndex = i;
		_liveCamera.Begin();
		SaveSettings();
	}

	public void ChangeFullScreen(bool yes){
		//#if UNITY_STANDALONE_WIN
		float tempres_w = ResScreen_w;
		float tempres_h = ResScreen_h;
		if (yes == true) {
			//Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			FullScreen=1;
			Screen.SetResolution(ResScreen_w,ResScreen_h,true);
		} else {
			//Screen.fullScreenMode =  FullScreenMode.Windowed;
			FullScreen=0;
			//Screen.SetResolution(960,544,false);
			Screen.SetResolution(Mathf.RoundToInt(tempres_w/1.3f),Mathf.RoundToInt((tempres_w/1.3f)/1.777777777f),false);
		}
		SaveSettings();

//#endif
	}


	IEnumerator FlushAudio(){
		for (;;) {
			if (AudioOutput == 0) {
				
				AudioListener.volume=0;
					Destroy (VitaAudio.clip);
					Debug.Log ("Flushing Mic");
				

				StartAudio();


			}else{

				Destroy (VitaAudio.clip);

			}
			yield return new WaitForSeconds(60f);
		}

	}
	void StartAudio(){
	
		
	//if (VitaAudio.clip != null) {
			//Destroy (VitaAudio.clip);
			//Debug.Log ("Flushing Mic");
		
		//} else {

			if (AudioOutput == 0) {

				VitaAudio.clip = Microphone.Start (DeviceName, true, 1, 44100);
				//while (!(Microphone.GetPosition(DeviceName)>0)) {
				//while (!(Microphone.GetPosition(DeviceName)>0f)) {}
				VitaAudio.Play ();
			AudioListener.volume=1;
			//}
				
			

			//}
		}

	}

IEnumerator FixLatency(){
	
	
			if(AudioOutput==0){

				if(VitaAudio.clip !=null){
			while (!(Microphone.GetPosition(DeviceName)>0.2f)) {
				VitaAudio.Play ();
			}
			
			}
			}
			yield return new WaitForSeconds(15f);



	}
	private float minimumLevel = 0;
	// Update is called once per frame
	void CheckIfInputs(){

		VitaAudio.GetSpectrumData(clipSampleData, 0, FFTWindow.Rectangular);
		float currentAverageVolume = clipSampleData.Average()*10000;
		Debug.Log (currentAverageVolume);
		if(currentAverageVolume>minimumLevel){ 
			AudioSilent=false;
		} 
		else{
			AudioSilent=true;
			//volume below level, but user was speaking before. So user stopped speaking
		}
	}
	void checkAudioDevices(){


		foreach (var device in Microphone.devices)
		{
			//Debug.Log("Name: " + device);
			
			if(device.Contains ("PSVita")){
				DeviceName = device;
				AudioSign.gameObject.SetActive (false);
			}else{
				DeviceName=null;
				AudioSign.gameObject.SetActive (true);
			}
		}

	}

	void QualityChangeFunct(){

		this.GetComponent<AudioSource> ().PlayOneShot (Clip_Click);
		
		QualityLevelSign.gameObject.SetActive (false);
		if(QualityLevel==0){
			QualityLevelSignLabel.text = "Good";
			QualityLevel=1;
		} else if(QualityLevel==1){
			QualityLevelSignLabel.text = "HQ";
			QualityLevel=2;
		} else if(QualityLevel==2){
			QualityLevelSignLabel.text = "Smooth";
			QualityLevel=0;
		}
		AnimateQualityChange();
		QualityLevelSign.gameObject.SetActive (true);
	}
	void AnimateSettings(bool go){

		if (go == true) {
			if (!OutputSprite.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("OutPutSettingsAnim")) {
				SettingsPanel.SetActive(true);
		
				OutputSprite.GetComponent<Animator> ().Play ("OutPutSettingsAnim");
			
			}
		} else {
			if (!OutputSprite.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("OutPutSettingsBack")) {
				
				
				OutputSprite.GetComponent<Animator> ().Play ("OutPutSettingsBack");
				SettingsPanel.SetActive(false);
			}
		}
	}
	void AnimateQualityChange(){
		if (!OutputSprite.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("OutPutQuality")) {

			ChangeQuality(QualityLevel);
			OutputSprite.GetComponent<Animator> ().Play ("OutPutQuality");

		}
	}
	private float logotransparent;
	void Update () {
        if (reloadsettings == true)
        {
            Reload();
            reloadsettings = false;
        }
		ResDebug.text = ResScreen_w.ToString () + "x" + ResScreen_h.ToString ();
	if (AudioOutput == 1) {
			VitaAudio.Stop();
		}


		if (Input.GetKeyDown (KeyCode.A)) {

		}
	if (Input.GetKeyDown (KeyCode.Return)) {
			//RefreshVita();
			this.GetComponent<AudioSource> ().PlayOneShot (Clip_FullScreen);
			if(SettingsPanel.activeSelf){
				AnimateSettings(false);
			}else{
				AnimateSettings(true);
					}
					

		}
		if (Input.GetKeyDown(KeyCode.Escape)){
			this.GetComponent<AudioSource> ().PlayOneShot (Clip_FullScreen);
		
				if(FullScreen==1){
					ChangeFullScreen(false);
				}else{
					ChangeFullScreen(true);
				}
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			if(	_liveCamera.GetComponent<AVProLiveCamera>().SavePNG()){
				Debug.Log ("ScreenShot");
				string Path = _liveCamera.GetComponent<AVProLiveCamera>().ScreenshotPath;
				Texture2D tex = null;
				byte[] fileData;
				
				if (System.IO.File.Exists(Path))     {
					fileData = System.IO.File.ReadAllBytes(Path);
					tex = new Texture2D(2, 2);
					tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
					Sprite ScreenShotSprite = Sprite.Create (tex, new Rect(0.0f,0.0f,tex.width,tex.height),new Vector2(0.5f,0.5f),100.0f);
					screenshotUI_Image.sprite = ScreenShotSprite;
				}
				Debug.Log (Path);
				screenshotUI.GetComponent<Animator>().Play ("TopBubble");
			
			}
			
		}



		if (Input.GetKeyDown (KeyCode.Tab)) {
			QualityChangeFunct();

		}

		//CheckIfInputs ();


		if (!PSVitaConnected || DeviceFailed) {
		
			QualityLevelSign.gameObject.SetActive (false);

			logotransparent=0f;
			NoVitaText.gameObject.SetActive (true);

			if(DeviceFailed==true){
				NoVitaText.text = "PSVITA failed to start!";
			}else{
				NoVitaText.text = "PSVITA is not connected!";
			}
			if(!DeviceFailed){
			PSLogo.color = Color.Lerp (PSLogo.color, new Color32 (36, 36, 36, 255), Time.deltaTime);
			}else{
			PSLogo.color = Color.Lerp (PSLogo.color, new Color32 (210, 36, 36, 255), Time.deltaTime);
			}
			Background.color = Color.black;
			VitaAudio.volume = 0f;
		} else {
			StopCoroutine("RefreshVita");

			StartCoroutine ("FixLatency");
			
			
			if (Screen.fullScreen) {
				fullscreenSign.gameObject.SetActive (false);
			} else {
				fullscreenSign.gameObject.SetActive(true);
			}





			NoVitaText.gameObject.SetActive (false);


			logotransparent = Mathf.Lerp (logotransparent,1f,Time.deltaTime);
			if(logotransparent>0.9f){

			PSLogo.color = Color.Lerp (PSLogo.color, new Color32 (0, 0, 0, 0), Time.deltaTime*10);        
			Background.color = Color.Lerp (Background.color, new Color32 (255, 255, 255, 0), Time.deltaTime*10);
			VitaAudio.volume = Mathf.Lerp (VitaAudio.volume,1f,Time.deltaTime*2);
			}else{
		
			PSLogo.color = Color.Lerp (PSLogo.color, new Color32 (255, 255, 255, 255), Time.deltaTime);
			}
		}
	}


	public void Effect_AA_Button(){
		if (Effects_AA_int == 0) {
			Effects_AA_int = 1;
		} else {
			Effects_AA_int = 0;
		}
		Effects_AA (Effects_AA_int);
		SaveSettings ();
	}

	public void Color_Button(){
		if (ColorEn_int == 0) {
			ColorEn_int = 1;
		} else {
			ColorEn_int = 0;
		}
		Effects_Color (ColorEn_int);
		SaveSettings ();
	}
	public void Retro_Button(){
		if (Retro_int == 0) {
			Retro_int = 1;
		} else {
			Retro_int = 0;
		}
		Effects_Retro (Retro_int);
		SaveSettings ();
	}

	 void Effects_AA(int enabled){

		PostProcessingProfile tempant = PProfile;

		if (enabled == 1) {
			tempant.antialiasing.enabled = true;
		} else {
			tempant.antialiasing.enabled = false;
		}
		PProfile = tempant;
	}
	 void Effects_Color(int enabled){
		PostProcessingProfile tempant = PProfile;

		if (enabled == 1) {
			tempant.colorGrading.enabled = true;
		} else {
			tempant.colorGrading.enabled = false;
		}

		PProfile = tempant;

	}

		 void Effects_Retro(int enabled){
		PostProcessingProfile tempant = PProfile;
		
		if (enabled == 1) {
			tempant.grain.enabled = true;
			tempant.chromaticAberration.enabled = true;
		} else {
			tempant.grain.enabled = false;
			tempant.chromaticAberration.enabled = false;
		}
		
		PProfile = tempant;
		
	}

}
