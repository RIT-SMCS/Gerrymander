using UnityEngine;
using System.Collections;

public class Transition : MonoBehaviour {

	public bool transitioning;
	Color color;
	float transitionValue = 1.0f; 
	public bool startVisible = true; 
	bool ending = false;
	public bool lastLevel = false;
    public bool readyForNext = false;

	// Use this for initialization
	void Start () {
		transitioning = true; 
		color = GetComponent<Renderer>().material.color; 
		if(startVisible)
			color.a = 1.0f; 
		else
			color.a = 0.0f; 
		GetComponent<Renderer>().material.color = color; 
	}
	
	// Update is called once per frame
	void Update () {
		if(transitioning)
		{
			color.a -= (transitionValue * Time.deltaTime); 
			GetComponent<Renderer>().material.color = color; 
			if(color.a < 0.0f || color.a > 1.0f)
				transitioning = false; 
		}
        if (ending && !transitioning && !lastLevel)
            readyForNext = true;
	}
	
	public void FadeOut()
	{
		transitioning = true; 
		transitionValue = 1.0f;
	}
	
	public void FadeIn()
	{
		ending = true; 
		transitioning = true; 
		transitionValue = -1.0f;
		Debug.Log ("fadein called"); 
	}
}
