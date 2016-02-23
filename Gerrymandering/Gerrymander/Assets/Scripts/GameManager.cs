using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//awful coding practice.
//change colors for color-blind people
public enum Affiliation { Red = 0, Blue = 1, Green = 2, };
public class GameManager : MonoBehaviour {
    public GameObject uiCanvas;
    GameObject[] nodes;
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
    public float maxConnectorLength = 5.0f;


    private Node startNode = null;
    private Connector tempConnector = null;

	// Use this for initialization
	void Start () {
        nodes = GameObject.FindGameObjectsWithTag("Node");
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
        hit.point = new Vector3(hit.point.x, 0.0f, hit.point.z);
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
            bool validConnector = true;
            if (startNode != null) {
                UpdateConnector(tempConnector, startNode.transform.position, hit.point);
                if (tempConnector.transform.localScale.z > maxConnectorLength) //too long
                {
                    tempConnector.GetComponent<Renderer>().material.color = Color.magenta;
                    validConnector = false;
                } else if (tempConnector.IsColliding()) //collides with other stuff
                {
                    tempConnector.GetComponent<Renderer>().material.color = Color.red;
                    validConnector = false;
                }
                else {
                    tempConnector.GetComponent<Renderer>().material.color = Color.green;
                }
            }
            // working click through
            for (Node endNode = objectHit.GetComponent<Node>(); startNode != null && endNode != null && startNode != endNode && validConnector; ) {
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
        if(connectors.Count > 2 && districts.Count < 10)
        {
            Dictionary<Node, List<Node>> map = CreateAdjMap();
            foreach (GameObject n in nodes)
            {
                List<Node> path = GetPath(map, n.GetComponent<Node>());
                if (path != null)
                {
                    //Debug.Log(n.name + ": path found - length: " + path.Count);
                    string str = "";
                    foreach (Node m in path)
                    {
                        str += "\t->" + m.name;
                    }
                    //Debug.Log(str);
                    Node[] nodeArray = path.ToArray();
                    //TODO SARAH: LINK WITH DISTRICT CODE
                    //nodeArray is an array of Nodes. 
                }
                else
                {
                    //Debug.Log("no path found for node: " + n.name);f
                }
                break;
                //Debug.Log("Number of Districts: " + districts.Count);
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Connectors: " + connectors.Count);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Nodes: " + nodes.Length);
        }
        //do not make connectors if there is no valid district made

        //update GUI
        #region Setting Text
        int unitsInDistricts = 0;
        uiManager.SetText(uiManager.Pop, unitsInDistricts + "/" + units.Count + " per\nDistrict");
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
        uiManager.SetText(uiManager.Goal, goalDistricts + " Districts\n" + winner + " Win" );
        uiManager.SetText(uiManager.District, currentDistricts + "/" + goalDistricts + "\nDistricts");
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
    /// http://stackoverflow.com/questions/526331/cycles-in-an-undirected-graph
    /// hard vs soft visit
    /// soft visit until you hit a cycle or run out of neighbors, then mark as hard.
    /// For unlinked branches, check for nodes not contained in the hard visit graph.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="startNode"></param>
    /// <returns></returns>
    private List<Node> GetPath(Dictionary<Node, List<Node>> map, Node startNode)
    {
        //checkForDistricts(n.GetComponent<Node>());
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> visited = new List<Node>();
        //Queue<Node> queue = new Queue<Node>();
        Stack<Node> stack = new Stack<Node>();
        stack.Push(startNode);
        prev[startNode] = null;
        //start breadth-first
        while (stack.Count > 0)
        {
            Node curr = stack.Pop(); // get current node
            visited.Add(curr);  //add to visited
            foreach (Node neighbor in map[curr]) //check currents neighbors
            {
                if (neighbor == startNode && prev[curr] != startNode) //if a neighbor is my start node and I've seen enough nodes to maybe have a path
                {
                    List<Node> path = new List<Node>();
                    Node temp = curr;
                    while (temp != startNode)
                    {
                        path.Add(temp);
                        temp = prev[temp];
                    }
                    if (path.Count < 2) //too short
                    {
                        continue;
                    }
                    path.Add(startNode);
                    return path;
                }
                if (!visited.Contains(neighbor)) //if i've already been to the neighbor, skip
                {
                    prev[neighbor] = curr; //set the neighbors previous node to be the current node
                    stack.Push(neighbor); //add the neighbor to the queue
                }
            }
        }
        return null; //found no path, return null
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
        c.transform.localScale = new Vector3(0.2f, 0.2f, 0.95f * (initPoint - endPoint).magnitude-1.0f);
    }



    public void ClearConnections()
    {

        while (connectors.Count > 0)
        {
            Destroy(connectors[connectors.Count - 1].gameObject);
            connectors.RemoveAt(connectors.Count - 1);
        }
    }

    #region deprecated code
    /*void checkForDistricts(Node first, Node curr)
    {
        //int[,] matrix = createAdjMatrix(connectors);
        if (curr == first)
        {
            districts.Add(new District());
            return;
        }
        for (int i = 0; i < curr.GetConnectors().Count; i++)
    }*/
    //void checkForDistricts(Node first, Node curr)
    //{        
    //    //int[,] matrix = createAdjMatrix(connectors);
    //    if(curr == first)
    //    {
    //        districts.Add(new District());
    //        return;
    //    }
    //    for(int i = 0; i < curr.GetConnectors().Count; i++)
    //    {
    //        if (!curr.GetConnectors()[i].isVisited)
    //        {
    //            if (curr.GetConnectors()[i].A != curr)
    //            {
    //                curr.GetConnectors()[i].isVisited = true;
    //                checkForDistricts(first, curr.GetConnectors()[i].A);
    //                curr.GetConnectors()[i].isVisited = false;
    //            }
    //            else
    //            {
    //                curr.GetConnectors()[i].isVisited = true;
    //                checkForDistricts(first, curr.GetConnectors()[i].B);
    //                curr.GetConnectors()[i].isVisited = false;
    //            }
    //        }
    //    }               
    //}

    void checkForDistricts(Node n)
    {
        Debug.Log(n);
        Queue<Node> active = new Queue<Node>();
        Node first = n;
        active.Enqueue(n);
        List<Connector> loop = new List<Connector>();
        while(active.Count != 0)
        {
            Node temp = active.Dequeue();
            if(temp == first)
            {
                districts.Add(new District());
            }
            foreach(Connector c in temp.GetConnectors())
            {
                if(!c.isVisited)
                {
                    if(c.A != temp)
                    {
                        loop.Add(c);
                        active.Enqueue(c.A);
                        c.isVisited = true;
                    }
                    else
                    {
                        loop.Add(c);
                        active.Enqueue(c.B);
                        c.isVisited = true;
                    }
                }
            }
        }
        foreach(Connector c in connectors)
        {
            c.isVisited = false;
        }
    }
#endregion

    Dictionary<Node, List<Node>> CreateAdjMap()
    {
        Dictionary<Node, List<Node>> map = new Dictionary<Node, List<Node>>();
        foreach (GameObject go in nodes)
        {
            map[go.GetComponent<Node>()] = new List<Node>();
        }
        foreach (Connector c in connectors)
        {
            if (!map.ContainsKey(c.A))
            {
                map[c.A] = new List<Node>();
            }
            map[c.A].Add(c.B);

            if (!map.ContainsKey(c.B))
            {
                map[c.B] = new List<Node>();
            }
            map[c.B].Add(c.A);
        }

        return map;
    }    
}

