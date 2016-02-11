using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//awful coding practice.
//change colors for color-blind people
public enum Affiliation { Red = 0, Blue = 1, Green = 2, };
public class GameManager : MonoBehaviour {
    public GameObject uiCanvas;
    List<Node> nodes;
    List<Connector> connectors;
    List<Unit> units;
    List<District> districts;
    //number of districts each party controls
    int[] partyDistricts = new int[3];
    UIManager uiManager;
    Affiliation winningTeam = Affiliation.Red;
    int goalDistricts = 3;
    int currentDistricts = 0;
    int totalRed, totalBlue, totalGreen = 0;
    int currentRed, currentBlue, currentGreen = 0;

    //Dictionary<Affiliation, int> partyDistricts;

    public Connector connectorPrefab;

	private Node startNode = null;

	// Use this for initialization
	void Start () {
        nodes = new List<Node>();
        connectors = new List<Connector>();
        units = new List<Unit>();
        districts = new List<District>();
        GameObject[] unitPrefabs = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject obj in unitPrefabs) {
            units.Add(obj.GetComponent<Unit>());
            switch (units[units.Count - 1].affiliation) {
                case Affiliation.Blue:
                    totalBlue += 1;
                    break;
                case Affiliation.Green:
                    totalGreen += 1;
                    break;
                case Affiliation.Red:
                    totalRed += 1;
                    break;
                default:
                    break;
            
            }
        }
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
        #region mouse raycast
        RaycastHit hit;
		Transform objectHit = null;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            objectHit = hit.transform;
        }
        else {
            objectHit = this.transform;
        }
        #endregion
        #region left click
        if (Input.GetMouseButtonDown (0)) { //left click down
			Debug.Log ("click");
			if ((startNode = objectHit.GetComponent<Node> ()) != null) {
            }
		} if (Input.GetMouseButton(0)) { // left click drag    
        }
        else if (Input.GetMouseButtonUp(0)) {//left click up
			Debug.Log("Release");
            for (Node endNode = objectHit.GetComponent<Node>(); startNode != null && endNode != null && startNode != endNode; ) {
                Connector c = (Connector)Instantiate(connectorPrefab);
                c.A = startNode; 
                c.B = endNode;
                if (connectors.Contains(c)) {
                    Debug.Log("Nodes already connected");
                    Destroy(c.gameObject);
                } else {
                    c.transform.position = 0.5f * (startNode.transform.position+endNode.transform.position);
                    c.transform.forward = startNode.transform.position-endNode.transform.position;
                    c.transform.localScale = new Vector3(1.0f,1.0f,0.7f * (startNode.transform.position-endNode.transform.position).magnitude);
                    c.transform.SetParent(this.transform);
                    connectors.Add(c);
                }
                break;
            }
        }
        #endregion
        #region right click
        if (Input.GetMouseButtonDown(1)) { //right click down
            Debug.Log("click");
            for (Connector ctr = objectHit.GetComponent<Connector>(); ctr != null;) {
                Debug.Log("Removing conenector");
                connectors.Remove(ctr);
                Destroy(ctr.gameObject);
                break;
            }
        } if (Input.GetMouseButton(1)) { // right click drag    
        }
        else if (Input.GetMouseButtonUp(1)) {//right click up
        }
        #endregion
        //end raycast


        //update GUI

        int unitsInDistricts = 0;
        uiManager.setText(uiManager.Pop, unitsInDistricts + "/" + units.Count + " in district");
        string winner = "blah" ;
        if (winningTeam == Affiliation.Blue)
        {
            winner = "Dems";
        }
        else if (winningTeam == Affiliation.Red)
        {
            winner = "Reps";
        }
        else winner = "Inds";
        uiManager.setText(uiManager.Goal, goalDistricts + " Districts, " + winner + " Win" );
        uiManager.setText(uiManager.District, currentDistricts + "/" + goalDistricts + " Districts");
        currentBlue = currentGreen = currentRed = 0;
        foreach (District dist in districts)
        {
            switch (dist.majority)
            {
                case Affiliation.Red:
                    partyDistricts[0] += 1;
                    break;
                case Affiliation.Blue:
                    partyDistricts[1] += 1;
                    break;
                 case Affiliation.Green:
                    partyDistricts[2] += 1;
                    break;
            }

            foreach (Unit voter in dist.GetMembers())
            {
                switch(voter.affiliation)
                {
                    case Affiliation.Red:
                        currentRed += 1;
                        break;
                    case Affiliation.Blue:
                        currentBlue += 1;
                        break;
                    case Affiliation.Green:
                        currentGreen += 1;
                        break;
                }
            }
        }
        foreach(int party in partyDistricts)
        {
            switch (party){
                case (int)Affiliation.Red:
                    uiManager.setText(uiManager.GOP, currentRed + "/" + totalRed + " Rep");
                    break;
                case (int)Affiliation.Blue:
                    uiManager.setText(uiManager.Dem, currentBlue + "/" + totalBlue + " Dem");
                    break;
                case (int)Affiliation.Green:
                    uiManager.setText(uiManager.Ind, currentGreen + "/" + totalGreen + " Ind");
                    break;
                default:
                    break;
            }
        }
        //check for win condition
	}
}
