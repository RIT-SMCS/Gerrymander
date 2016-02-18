using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Unit : MonoBehaviour {
    public Affiliation affiliation;
    // Use this for initialization
    void Start()
    {
    }
	
	// Update is called once per frame
	void Update () {
        switch (affiliation)
        {
            case Affiliation.Red:
                this.GetComponent<Renderer>().sharedMaterial.color = Color.red;
                break;
            case Affiliation.Green:
                this.GetComponent<Renderer>().sharedMaterial.color = Color.green;
                break;
            case Affiliation.Blue:
                this.GetComponent<Renderer>().sharedMaterial.color = Color.cyan;
                break;
        }
            
	}
}

