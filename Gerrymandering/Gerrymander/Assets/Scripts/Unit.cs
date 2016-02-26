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

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("District")) 
		{
			DistrictCollider2 dist = other.gameObject.GetComponent<DistrictCollider2>(); 
			if(affiliation == Affiliation.Red)
				dist.AddUnit(0); 
			else if(affiliation == Affiliation.Green)
				dist.AddUnit(1); 
			else
				dist.AddUnit(2); 
		}
	}
}

