using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//awful coding practice.
//change colors for color-blind people
enum Affiliation { Red = 0, Blue = 1, Green = 2, };
public class GameManager : MonoBehaviour {
    List<Node> nodes;
    List<Connector> connectors;
    List<Unit> units;
    int[] partyDistricts = new int[3];
    //Dictionary<Affiliation, int> partyDistricts;

    
    
	// Use this for initialization
	void Start () {
        //partyDistricts[(int)Affiliation.Red]++;	
	}
	
	// Update is called once per frame
	void Update () {
	    //mouse / touch input (raycasts)
            //after input calculate districts 
            //do not make connectors if there is no valid district made
        //update GUI
        //check for win condition
	}
}
