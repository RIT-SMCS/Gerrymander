using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//awful coding practice.
//change colors for color-blind people
enum Affiliation { Red = 0, Blue = 1, Green = 2, };
public class GameManager : MonoBehaviour {
    public GameObject uiCanvas;
    List<Node> nodes;
    List<Connector> connectors;
    List<Unit> units;
    int[] partyDistricts = new int[3];
    UIManager uiManager;
    
    //Dictionary<Affiliation, int> partyDistricts;

    
    
	// Use this for initialization
	void Start () {
        //partyDistricts[(int)Affiliation.Red]++;	
        if(uiCanvas != null)
        {
            uiManager = uiCanvas.GetComponent<UIManager>();
        }

        for (int i = 0; i < 3; i++)
        {
            partyDistricts[i] = i;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    //mouse / touch input (raycasts)
            //after input calculate districts 
            //do not make connectors if there is no valid district made
        //update GUI

        // TODO: Let these functionCalls not error out by having a way to get these strings a good value
        //uiManager.setText(uiManager.Pop, unitsInDistricts "/" + units.Count);
        //uiManager.setText(uiManager.Goal, goalParameters);
        //uiManager.setText(uiManager.Districts, [NEED A VARIABLE FOR THIS])

        foreach(int party in partyDistricts)
        {
            switch (party){
                case (int)Affiliation.Red:
                    uiManager.setText(uiManager.GOP, party + "/5 Rep");
                    break;
                case (int)Affiliation.Blue:
                    uiManager.setText(uiManager.Dem, party + "/6 Dem");
                    break;
                case (int)Affiliation.Green:
                    uiManager.setText(uiManager.Ind, party + "/5 Ind");
                    break;
                default:
                    break;
            }
        }
        //check for win condition
	}
}
