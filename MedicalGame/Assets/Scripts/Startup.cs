﻿using UnityEngine;
using Gamedonia.Backend;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class Startup : MonoBehaviour {

	public string sceneName;
	// Use this for initialization
	void Start () {
		RuntimeData.I.Load ();
        AudioManagerScript.I.Load();
		//ProductManager.I.Load();
        if(!PlayerManager.I.player.verificationComplete)
        {
            SceneManager.LoadScene("First_Login");
        }
        else
        {

            if (!PlayerManager.I.player.loggedIn)
            {
                SceneManager.LoadScene("Login");
            }
            else
            {
			    if (PlayerManager.I.player.createdProfile)
                {
                    if (!PlayerManager.I.player.completedIntro)
                    {
                        sceneName = "Introduction";
                    }
                    else
                    {
                        sceneName = "Home";
                    }
                }
                else
                {
                    sceneName = "Profile_Create";
                }
                GamedoniaUsers.LoginUserWithSessionToken (delegate (bool success) {
				    if (success) {
						ProductManager.I.RequestProducts();
						PlayerManager.I.LoadFriends ();
						if(sceneName == "Home") {
							Loader.I.enableLoader();
							StartCoroutine(waitBeforeInformationIsProcessed());
						} else {
							SceneManager.LoadScene (sceneName);
						}
				    } else {
						
					    SceneManager.LoadScene ("Login");
				    }
			    });
		    }
	    }
    }//end start
	
	private IEnumerator waitBeforeInformationIsProcessed() {
		while(PlayerManager.I.startUpDone) {
			yield return new WaitForSeconds(1f);
		}
		SceneManager.LoadScene (sceneName);
	}
}// end class