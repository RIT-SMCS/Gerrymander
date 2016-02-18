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
    private Connector tempConnector = null;

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

        //mouse / touch input (raycasts)
        //after input calculate districts 
        //do not make connectors if there is no valid district made

        //update GUI
        #region checkDistricts
        int unitsInDistricts = 0;
        uiManager.SetText(uiManager.Pop, unitsInDistricts + "/" + units.Count + " in district");
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
        uiManager.SetText(uiManager.Goal, goalDistricts + " Districts, " + winner + " Win" );
        uiManager.SetText(uiManager.District, currentDistricts + "/" + goalDistricts + " Districts");
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
                    uiManager.SetText(uiManager.GOP, currentRed + "/" + totalRed + " Rep");
                    break;
                case (int)Affiliation.Blue:
                    uiManager.SetText(uiManager.Dem, currentBlue + "/" + totalBlue + " Dem");
                    break;
                case (int)Affiliation.Green:
                    uiManager.SetText(uiManager.Ind, currentGreen + "/" + totalGreen + " Ind");
                    break;
                default:
                    break;
            }
        }
        #endregion
        //check for win condition

        bool goalMet = currentDistricts == goalDistricts && partyDistricts[(int)winningTeam] == Mathf.RoundToInt((goalDistricts / 2.0f) + 1);

        if(goalMet)
        {
            uiManager.ShowVictory();
        }
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


    public void ClearConnections()
    {

        while (connectors.Count > 0)
        {
            Destroy(connectors[connectors.Count - 1].gameObject);
            connectors.RemoveAt(connectors.Count - 1);
        }
    }

    void checkForDistricts(Node first, Node curr)
    {        
        //int[,] matrix = createAdjMatrix(connectors);
        if(curr == first)
        {
            districts.Add(new District());
            return;
        }
        for(int i = 0; i < curr.GetConnectors().Count; i++)
        {
            if (!curr.GetConnectors()[i].isVisited)
            {
                if (curr.GetConnectors()[i].A != curr)
                {
                    curr.GetConnectors()[i].isVisited = true;
                    checkForDistricts(first, curr.GetConnectors()[i].A);
                    curr.GetConnectors()[i].isVisited = false;
                }
                else
                {
                    curr.GetConnectors()[i].isVisited = true;
                    checkForDistricts(first, curr.GetConnectors()[i].B);
                    curr.GetConnectors()[i].isVisited = false;
                }
            }
        }
        //for (int i = 0; i < connectors.Count; i++)
        //{
        //    if(curr != connectors[i])
        //    {
        //        if(curr.B == connectors[i].A || curr.B == connectors[i].B)
        //        {
        //            next = connectors[i];
        //            if (connectors[i].A == first || connectors[i].B == first)
        //                districts.Add(new District());
        //            else
        //                checkForDistricts(first, next);
        //        }
        //    }       
        //}        
    }

    int[,] createAdjMatrix(List<Connector> _connectors)
    {
        int[,] m = new int[_connectors.Count, _connectors.Count];
        for (int i = 0; i < _connectors.Count; i++)
        {
            for (int j = 0; j < _connectors.Count; j++)
            {
                if (_connectors[i] != _connectors[j])
                {
                    if (_connectors[i].B == _connectors[j].A || _connectors[i].B == _connectors[j].B)
                        m[i, j] = 1;
                    else
                        m[i, j] = 0;
                }
            }
        }

        return m;
    }    
}

