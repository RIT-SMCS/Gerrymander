using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public int levelNumber = 1;
    public bool completed = false;
    public Text CompletionText;
    public Text LevelText;

	// Use this for initialization
	void Start () {
        LevelText.text = "" + levelNumber;
        if (completed)
        {
            CompletionText.text = "\uf058";
            CompletionText.color = new Color(48.0f/255, 171.0f/255, 12.0f/255);
        }
        else
        {
            CompletionText.text = "\uf057";
            CompletionText.color = new Color(171.0f /255 , 12.0f/255, 48.0f/255);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
