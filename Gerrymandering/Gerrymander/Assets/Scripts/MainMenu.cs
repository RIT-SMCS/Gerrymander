using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLatestLevel ()
    {
        //TODO: get latest incomplete level scene, and open it.
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("Lvl_Select", LoadSceneMode.Single);
    }
}
