using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Unit : MonoBehaviour {
    public Affiliation affiliation;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        switch (affiliation)
        {
            case Affiliation.Red:
                this.GetComponent<Renderer>().material.color = Color.red;
                break;
            case Affiliation.Green:
                this.GetComponent<Renderer>().material.color = Color.green;
                break;
            case Affiliation.Blue:
                this.GetComponent<Renderer>().material.color = Color.cyan;
                break;
        }
	}
}
