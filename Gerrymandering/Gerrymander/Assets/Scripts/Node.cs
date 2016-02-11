using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Node : MonoBehaviour {
    private List<Connector> connectors;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public List<Connector> GetConnectors() { return connectors; }
}
