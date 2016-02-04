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

    public Connector connectorPrefab;

	private Node startNode = null;

	// Use this for initialization
	void Start () {
        nodes = new List<Node>();
        connectors = new List<Connector>();
        units = new List<Unit>();
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
