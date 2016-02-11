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

    public Connector connectorPrefab;

	private Node startNode = null;
    private Connector tempConnector = null;

	// Use this for initialization
	void Start () {
        nodes = new List<Node>();
        connectors = new List<Connector>();
        units = new List<Unit>();
        //partyDistricts[(int)Affiliation.Red]++;	
        
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
            }
		} else if (Input.GetMouseButton(0)) { // left click drag    
            if (startNode != null) {
                tempConnector.transform.position = 0.5f * (startNode.transform.position + hit.point);
                tempConnector.transform.forward = startNode.transform.position - hit.point;
                tempConnector.transform.localScale = new Vector3(1.0f, 1.0f, 0.7f * (startNode.transform.position - hit.point).magnitude);
                tempConnector.transform.SetParent(this.transform);
                tempConnector.name = "dragged connector";
            }
            // working click through
            for (Node endNode = objectHit.GetComponent<Node>(); startNode != null && endNode != null && startNode != endNode; ) {
                Connector c = (Connector)Instantiate(connectorPrefab);
                c.A = startNode;
                c.B = endNode;
                if (connectors.Contains(c)) {
                    //Debug.Log("Nodes already connected");
                    Destroy(c.gameObject);
                }
                else {
                    c.transform.position = 0.5f * (startNode.transform.position + endNode.transform.position);
                    c.transform.forward = startNode.transform.position - endNode.transform.position;
                    c.transform.localScale = new Vector3(1.0f, 1.0f, 0.7f * (startNode.transform.position - endNode.transform.position).magnitude);
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
        //check for win condition
	}
}
