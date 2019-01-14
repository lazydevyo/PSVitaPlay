using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Documentation : MonoBehaviour {
	public Image[] Pages;
	public int Page_Int;
	void Start(){
		ChangePage ();
	}
	public void ChangePage (){
		int i = 0;

		foreach (Image pag in Pages) {
		
			i++;
			Pages[(i-1)].gameObject.SetActive(false);
		}

		Pages [Page_Int].gameObject.SetActive (true);
	}

	public void NextPage(){
		if (Page_Int == Pages.Length-1) {
			//Page_Int = 0;
		} else {
			Page_Int++;
		}

		ChangePage ();
	}

	public void PrewPage(){
		if (Page_Int == 0) {
			//Page_Int = Pages.Length-1;
		} else {
			Page_Int--;
		}
		ChangePage ();
	}


}
