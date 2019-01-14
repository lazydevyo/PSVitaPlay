using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ClientScript : NetworkBehaviour {
    const int nudgeAmount = 33;
    public int[] Settings;
    public GameObject Canvas;
    public enum NudgeDir
    {
        quality,
        audio,
        enchance,
        sharp,
        retro
    }
    // Use this for initialization
    void Start () {
     
        Canvas.SetActive(false);
      

    }
	
	[ClientCallback]
	void Update () {
        if(isServer){
            return;
        }
            Canvas.SetActive(true);
        

    }
   
    public void Button(int id)
    {
        if (!isLocalPlayer)
            return;
        if (id == 0)
        {
            CmdSetN(NudgeDir.quality);
        }else if (id == 1)
        {
            CmdSetN(NudgeDir.audio);

        }
        else if (id == 2)
        {
            CmdSetN(NudgeDir.enchance);

        }
        else if (id == 3)
        {
            CmdSetN(NudgeDir.sharp);

        }
        else if (id == 4)
        {

            CmdSetN(NudgeDir.retro);
        }

    }
    [Command]
    public void CmdSetN(NudgeDir direction)
    {
        switch (direction)
        {
           

          

            case NudgeDir.quality:
                if (Settings[0] == 2) {
                    Settings[0] = 0;
                }
                else
                {
                    Settings[0] += 1;
                }
              
                NetworkSettingsVita.Quality = Settings[0];
             
                GameObject.Find("NetworkSettings").GetComponent<NetworkSettingsVita>().Reload(0);
                Debug.Log("Client");
                break;

            case NudgeDir.audio:
                if (Settings[1] == 1)
                {
                    Settings[1] = 0;
                }
                else
                {
                    Settings[1] = 1;
                }
                NetworkSettingsVita.Audio = Settings[1];

                GameObject.Find("NetworkSettings").GetComponent<NetworkSettingsVita>().Reload(0);
                break;

            case NudgeDir.enchance:
                if (Settings[2] == 1)
                {
                    Settings[2] = 0;
                }
                else
                {
                    Settings[2] = 1;
                }
                NetworkSettingsVita.Enchance = Settings[2];

                GameObject.Find("NetworkSettings").GetComponent<NetworkSettingsVita>().Reload(1);
                break;

            case NudgeDir.sharp:
                if (Settings[3] == 1)
                {
                    Settings[3] = 0;
                }
                else
                {
                    Settings[3] = 1;
                }
                NetworkSettingsVita.Sharp = Settings[3];

                GameObject.Find("NetworkSettings").GetComponent<NetworkSettingsVita>().Reload(2);
                break;
            case NudgeDir.retro:
                if (Settings[4] == 1)
                {
                    Settings[4] = 0;
                }
                else
                {
                    Settings[4] = 1;
                }
                NetworkSettingsVita.Retro = Settings[4];

                GameObject.Find("NetworkSettings").GetComponent<NetworkSettingsVita>().Reload(3);
                break;

        }
    }
}
