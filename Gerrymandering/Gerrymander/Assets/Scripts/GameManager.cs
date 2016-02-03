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

	public Camera camera;

	private Node activeNode = null;
	private bool mouseDown = false;
	// Use this for initialization
	void Start () {
        //partyDistricts[(int)Affiliation.Red]++;	
	}

	Transform RayCastHelper(out RaycastHit hit) {
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			return hit.transform;
		} else {
			return null;
		}
	}
	// Update is called once per frame
	void Update () {
	    //mouse / touch input (raycasts)
		RaycastHit hit;
		Transform objectHit = null;
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			objectHit = hit.transform;
		}

		//click/drag
		if (Input.GetMouseButtonDown (0)) { //left click down
			Debug.Log("mouseDown");
			//click
			if (!mouseDown) {
				Debug.Log ("click");
				//after input calculate districts 
				//do not make connectors if there is no valid district made
				if ((activeNode = objectHit.GetComponent<Node> ()) != null) {
					activeNode.transform.Translate (Vector3.forward * Time.deltaTime);
					//objectHit.Translate(Vector3.forward * Time.deltaTime);
				}
			} else { //drag

			}
		} else {//left click up
			//release
			if (mouseDown) {
				Debug.Log("Release");
				Node node2;
				if (activeNode != null && (node2 = objectHit.GetComponent<Node> ()) != null) {
					node2.transform.Translate (Vector3.forward * Time.deltaTime * 10.0f);
					//objectHit.Translate(Vector3.forward * Time.deltaTime);
				}
			}
			//nothing
		}
		//end raycast

		
        //update GUI
        //check for win condition

		mouseDown = Input.GetMouseButtonDown (0);
	}
}
