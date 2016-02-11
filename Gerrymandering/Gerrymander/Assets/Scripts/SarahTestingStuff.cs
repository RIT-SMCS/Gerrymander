using UnityEngine;
using System.Collections;

public class SarahTestingStuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("Sarah's test is running..."); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Colliding"); 
	}
}
