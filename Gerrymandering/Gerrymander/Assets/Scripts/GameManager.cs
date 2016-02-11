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
    private Connector tempConnector = null;

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
        //partyDistricts[(int)Affiliation.Red]++;
        //Create a background collider for raycast checks	
        GameObject backgroundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        backgroundPlane.transform.position = Vector3.zero;
        backgroundPlane.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
        for (MeshRenderer mr = backgroundPlane.GetComponent<MeshRenderer>(); mr != null; mr = null){
            mr.enabled = false;
        }
        backgroundPlane.name = "Background Plane";
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
                tempConnector = (Connector)Instantiate(connectorPrefab);
                tempConnector.transform.localScale = Vector3.zero;
                tempConnector.transform.SetParent(this.transform);
                tempConnector.name = "dragged connector";
                tempConnector.GetComponent<Renderer>().material.color = Color.green;
            }
		} else if (Input.GetMouseButton(0)) { // left click drag    
            if (startNode != null) {
                UpdateConnector(tempConnector, startNode.transform.position, hit.point);
                tempConnector.GetComponent<Renderer>().material.color = Color.green;
            }
            // working click through
            for (Node endNode = objectHit.GetComponent<Node>(); startNode != null && endNode != null && startNode != endNode; ) {
                Connector c = (Connector)Instantiate(connectorPrefab);
                c.A = startNode;
                c.B = endNode;
                if (connectors.Contains(c)) {
                    //Debug.Log("Nodes already connected");
                    Destroy(c.gameObject);
                    tempConnector.GetComponent<Renderer>().material.color = Color.red;
                }
                else {
                    UpdateConnector(c, startNode.transform.position, endNode.transform.position);
                    c.GetComponent<Renderer>().material.color = Color.blue;
                    c.transform.SetParent(this.transform);
                    connectors.Add(c);
                    startNode = endNode;
                }
                break;
            } 
        }
        else if (Input.GetMouseButtonUp(0)) {//left click up
			Debug.Log("Release");
            if (tempConnector != null) {
                Destroy(tempConnector.gameObject);
                tempConnector = null;
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
    /// <summary>
    /// Updates the position and shape of a Connector to fit between the two given points
    /// </summary>
    /// <param name="c"></param>
    /// <param name="initPoint"></param>
    /// <param name="endPoint"></param>
    private void UpdateConnector(Connector c, Vector3 initPoint, Vector3 endPoint) {
        c.transform.position = 0.5f * (initPoint + endPoint);
        c.transform.forward = initPoint - endPoint;
        c.transform.localScale = new Vector3(0.5f, 0.5f, 0.9f * (initPoint - endPoint).magnitude);
    }
}
