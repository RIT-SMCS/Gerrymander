using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class District : MonoBehaviour {
    List<Connector> connectors;
    List<Unit> members;
    public Affiliation majority;

    int requiredSize = 0;
    
    
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void CalculateMajority() { }
    bool IsValid() { return members.Count == requiredSize; }
    public List<Unit> GetMembers()
    {
        return members;
    }
}
