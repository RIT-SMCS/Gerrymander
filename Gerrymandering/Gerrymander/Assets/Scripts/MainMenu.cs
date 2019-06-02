using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    int latestLevel = 0;

	// Use this for initialization
	void Start () {
        bool foundLast = false;
        while (foundLast == false)
        {
            latestLevel += 1;
            foundLast = PlayerPrefs.GetInt((latestLevel).ToString(), defaultValue: 0) <= 0;

        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLatestLevel ()
    {
        SceneManager.LoadScene("Lvl_" + latestLevel, LoadSceneMode.Single);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("Lvl_Select", LoadSceneMode.Single);
    }
}
