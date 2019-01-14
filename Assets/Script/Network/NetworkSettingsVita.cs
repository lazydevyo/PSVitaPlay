using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class NetworkSettingsVita : NetworkBehaviour {


    public static int Quality;
    public static int Audio;
    public static int Enchance;
    public static int Sharp;
    public static int Retro;

	void Start(){
		LoadSettings();
	}
    // Use this for initialization
   
	
	// Update is called once per frame
	void Update () {
		
	}


    public void Reload(int id)
    {

        Debug.Log(Quality);
        SaveSettings();
		//VitaCaptureU.reloadsettingsID = id;
       // VitaCaptureU.reloadsettings = true;

    }
   public  void SaveSettings()
    {
        PlayerPrefs.SetInt("QualityLevel", Quality);
   
        PlayerPrefs.SetInt("Effects_AA_int", Sharp);
        PlayerPrefs.SetInt("ColorEn_int", Enchance);
        PlayerPrefs.SetInt("Retro_int", Retro);
        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        if (!PlayerPrefs.HasKey("QualityLevel"))
        {
            SaveSettings();
        }
        else
        {
            Quality = PlayerPrefs.GetInt("QualityLevel");
        }

        
        if (!PlayerPrefs.HasKey("Effects_AA_int"))
        {
            SaveSettings();
        }
        else
        {
            Sharp = PlayerPrefs.GetInt("Effects_AA_int");
        }
        if (!PlayerPrefs.HasKey("ColorEn_int"))
        {
            SaveSettings();
        }
        else
        {
            Enchance = PlayerPrefs.GetInt("ColorEn_int");
        }
        if (!PlayerPrefs.HasKey("Retro_int"))
        {
            SaveSettings();
        }
        else
        {
            Retro = PlayerPrefs.GetInt("Retro_int");
        }
    }
}
