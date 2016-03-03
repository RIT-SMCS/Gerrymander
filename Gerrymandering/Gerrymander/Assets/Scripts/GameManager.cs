﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//awful coding practice.
//change colors for color-blind people
public enum Affiliation { Red = 0, Blue = 1, Green = 2, None = -1};
public class GameManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public GameObject districtPrefab; 
    GameObject[] nodes;
    List<Connector> connectors;
    List<Unit> units;
    List<DistrictCollider2> districts;
    List<GameObject[]> dist; //THIS IS THE LIST OF CYCLES AS NODES. USE THIS TO MAKE DISTRICTS
    /// </summary>
    List<int[]> cycles;
    //number of districts each party controls
    int[] partyDistricts = new int[3];
    UIManager uiManager;
    public Affiliation winningTeam = Affiliation.Blue;
    public int goalDistricts = 3;
    int currentDistricts = 0;
    int totalRed, totalBlue, totalGreen = 0;
    int currentRed, currentBlue, currentGreen = 0;    

    //Dictionary<Affiliation, int> partyDistricts;
    public Connector connectorPrefab;
    public float maxConnectorLength = 5.0f;


    private Node startNode = null;
    private Connector tempConnector = null;


    // Use this for initialization
    void Start()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
        connectors = new List<Connector>();
        units = new List<Unit>();
        districts = new List<DistrictCollider2>();
        dist = new List<GameObject[]>();
        GameObject[] unitPrefabs = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject obj in unitPrefabs)
        {
            units.Add(obj.GetComponent<Unit>());
            switch (units[units.Count - 1].affiliation)
            {
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
        if (uiCanvas != null)
        {
            uiManager = uiCanvas.GetComponent<UIManager>();
        }

        
        //partyDistricts[(int)Affiliation.Red]++;
        //Create a background collider for raycast checks	
        GameObject backgroundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        backgroundPlane.transform.position = Vector3.zero;
        backgroundPlane.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
        for (MeshRenderer mr = backgroundPlane.GetComponent<MeshRenderer>(); mr != null; mr = null)
        {
            mr.enabled = false;
        }
        backgroundPlane.name = "Background Plane";
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < partyDistricts.Length; i++)
        {
            partyDistricts[i] = 0;
        }
        #region mouse raycast
        RaycastHit hit;
        Transform objectHit = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            objectHit = hit.transform;
        }
        else {
            objectHit = this.transform;
        }
        hit.point = new Vector3(hit.point.x, 0.0f, hit.point.z);
        #endregion
        #region left click
        if (Input.GetMouseButtonDown(0))
        { //left click down
            Debug.Log("click");
            if ((startNode = objectHit.GetComponent<Node>()) != null)
            {
                tempConnector = (Connector)Instantiate(connectorPrefab);
                tempConnector.transform.localScale = Vector3.zero;
                tempConnector.transform.SetParent(this.transform);
                tempConnector.name = "dragged connector";
                tempConnector.GetComponent<Renderer>().material.color = Color.green;
            }
        }
        else if (Input.GetMouseButton(0))
        { // left click drag    
            bool validConnector = true;
            if (startNode != null)
            {
                UpdateConnector(tempConnector, startNode.transform.position, hit.point);
                if (tempConnector.transform.localScale.z > maxConnectorLength) //too long
                {
                    tempConnector.GetComponent<Renderer>().material.color = Color.magenta;
                    validConnector = false;
                }
                else if (tempConnector.IsColliding()) //collides with other stuff
                {
                    tempConnector.GetComponent<Renderer>().material.color = Color.red;
                    validConnector = false;
                }
                else {
                    tempConnector.GetComponent<Renderer>().material.color = Color.green;
                }
            }
            // working click through
            for (Node endNode = objectHit.GetComponent<Node>(); startNode != null && endNode != null && startNode != endNode && validConnector;)
            {
                Connector c = (Connector)Instantiate(connectorPrefab);
                c.A = startNode;
                c.B = endNode;
                if (connectors.Contains(c))
                {
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
        else if (Input.GetMouseButtonUp(0))
        {//left click up
            Debug.Log("Release");
            if (tempConnector != null)
            {
                Destroy(tempConnector.gameObject);
                tempConnector = null;
            }
            CheckCycles();
        }
        #endregion
        #region right click
        if (Input.GetMouseButtonDown(1))
        { //right click down
            Debug.Log("click");
            for (Connector ctr = objectHit.GetComponent<Connector>(); ctr != null;)
            {
                Debug.Log("Removing conenector");
                connectors.Remove(ctr);
                Destroy(ctr.gameObject);
                CheckCycles();
                break;
            }
        }
        if (Input.GetMouseButton(1))
        { // right click drag    
        }
        else if (Input.GetMouseButtonUp(1))
        {//right click up
        }
        #endregion
        //end raycast

        //mouse / touch input (raycasts)
        //after input calculate districts 
        //if (connectors.Count > 2 && districts.Count < 10)
        //{
        //    Dictionary<Node, List<Node>> map = CreateAdjMap();
        //    foreach (GameObject n in nodes)
        //    {
        //        List<Node> path = GetPath(map, n.GetComponent<Node>());
        //        if (path != null)
        //        {
        //            //Debug.Log(n.name + ": path found - length: " + path.Count);
        //            string str = "";
        //            foreach (Node m in path)
        //            {
        //                str += "\t->" + m.name;
        //            }
        //            //Debug.Log(str);
        //            Node[] nodeArray = path.ToArray();
        //            //TODO SARAH: LINK WITH DISTRICT CODE
        //            //nodeArray is an array of Nodes. 
        //        }
        //        else
        //        {
        //            //Debug.Log("no path found for node: " + n.name);f
        //        }
        //        break;
        //        //Debug.Log("Number of Districts: " + districts.Count);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Connectors: " + connectors.Count);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Nodes: " + nodes.Length);
            foreach (GameObject n in nodes)
            {
                Debug.Log(n.GetComponent<Node>().ID);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextLevel();
        }

        //do not make connectors if there is no valid district made

        //update GUI
        #region Setting Text

        string winner = "blah";
        Color winColor = Color.white;
        if (winningTeam == Affiliation.Blue)
        {
            winner = "Dems";
            winColor = new Color(74.0f / 255.0f, 94.0f / 255.0f, 232.0f / 255.0f);
        }
        else if (winningTeam == Affiliation.Red)
        {
            winner = "Reps";
            winColor = new Color(255.0f / 255.0f, 81.0f / 255.0f, 98.0f / 255.0f);
        }
        else
        {
            winner = "Inds";
            winColor = new Color(94.0f / 255.0f, 255.0f / 255.0f, 134.0f / 255.0f);
        }
        uiManager.SetText(uiManager.Goal, goalDistricts + " Districts\n" + winner + " Win");
        uiManager.SetColor(uiManager.Goal, winColor);
        uiManager.SetText(uiManager.District, districts.Count + "/" + goalDistricts + "\nDistricts");

        currentBlue = currentGreen = currentRed = 0;
        foreach (DistrictCollider2 dist in districts)
        {
            switch (dist.winner)
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
            currentRed += dist.rgb[0];
            currentGreen += dist.rgb[1];
            currentBlue += dist.rgb[2];

            
            //foreach (int v in dist.rgb)
            //{
            //    Unit voter = v.GetComponent<Unit>();
            //    switch (voter.affiliation)
            //    {
            //        case Affiliation.Red:
            //            currentRed += 1;
            //            break;
            //        case Affiliation.Blue:
            //            currentBlue += 1;
            //            break;
            //        case Affiliation.Green:
            //            currentGreen += 1;
            //            break;
            //    }
            //}
        }

        int unitsInDistricts = currentRed + currentGreen + currentBlue;
        uiManager.SetText(uiManager.Pop, unitsInDistricts + "/" + units.Count + " in\nDistricts");

        GameObject panel;
        uiManager.SetText(uiManager.GOP, currentRed + "/" + totalRed);
        panel = uiManager.GOP.transform.parent.gameObject;
        if (currentRed == totalRed)
        {
            panel.GetComponent<Image>().fillCenter = true;
            uiManager.SetColor(uiManager.GOP, Color.black);
        }
        else
        {
            panel.GetComponent<Image>().fillCenter = false;
            uiManager.SetColor(uiManager.GOP, Color.white);
        }

        uiManager.SetText(uiManager.Dem, currentBlue + "/" + totalBlue);
        panel = uiManager.Dem.transform.parent.gameObject;
        if (currentBlue == totalBlue)
        {
            panel.GetComponent<Image>().fillCenter = true;
            uiManager.SetColor(uiManager.Dem, Color.black);
        }
        else
        {
            panel.GetComponent<Image>().fillCenter = false;
            uiManager.SetColor(uiManager.Dem, Color.white);
        }

        uiManager.SetText(uiManager.Ind, currentGreen + "/" + totalGreen);
        panel = uiManager.Ind.transform.parent.gameObject;
        if (currentGreen == totalGreen)
        {
            panel.GetComponent<Image>().fillCenter = true;
            uiManager.SetColor(uiManager.Ind, Color.black);
        }
        else
        {
            panel.GetComponent<Image>().fillCenter = false;
            uiManager.SetColor(uiManager.Ind, Color.white);
        }



        #endregion
        //check for win condition
        bool allDistricted = (currentBlue == totalBlue) && (currentRed == totalRed) && (currentGreen == totalGreen);
        bool goalMet = (districts.Count == goalDistricts) && (partyDistricts[(int)winningTeam] >= Mathf.RoundToInt(((goalDistricts + 1) / 3) + 1)) && allDistricted;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Goal Met: " +goalMet);
            Debug.Log("Correct num districts: " +(districts.Count == goalDistricts));
            Debug.Log("All units Districted: " + (allDistricted));
            Debug.Log("winning team has enough: " + (partyDistricts[(int)winningTeam] >= Mathf.RoundToInt(((goalDistricts + 1) / 3) + 1)));
        }

        if (goalMet)
        {
            uiManager.ShowVictory();
        }
    }

    public void NextLevel()
    {
        string name = Application.loadedLevelName;
        string[] split = name.Split('_');
        int num;
        if( int.TryParse(split[split.Length - 1], out num))
        {
            num += 1;
            string nextLevelString = "Lvl_" + num;
            //Application.LoadLevel("Scenes/Levels/" + nextLevelString);
            ClearConnections();
            Application.LoadLevel(nextLevelString);
        }
    }

    /// <summary>
    /// http://stackoverflow.com/questions/526331/cycles-in-an-undirected-graph
    /// hard vs soft visit
    /// soft visit until you hit a cycle or run out of neighbors, then mark as hard.
    /// For unlinked branches, check for nodes not contained in the hard visit graph.
    /// 
    /// professional research paper on the subject
    /// http://arxiv.org/pdf/1205.2766.pdf
    /// naive implementation of paper alg in c# and java
    /// http://stackoverflow.com/questions/12367801/finding-all-cycles-in-undirected-graphs
    ///     
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
    private void UpdateConnector(Connector c, Vector3 initPoint, Vector3 endPoint)
    {
        c.transform.position = 0.5f * (initPoint + endPoint);
        c.transform.forward = initPoint - endPoint;
        c.transform.localScale = new Vector3(0.2f, 0.2f, 0.95f * (initPoint - endPoint).magnitude - 1.0f);
    }

    private void CheckCycles()
    {
        if (connectors.Count > 2)
        {
            districts.Clear();
            cycles = GetCycles();
            List<GameObject[]> temp = new List<GameObject[]>();
            foreach (int[] c in cycles)
            {
                //PAY NO ATTENTION TO THE TERRIBLE CODING
                //THIS IS REALLY BAD AND WILL BE CHANGED LATER
                GameObject[] d = new GameObject[c.Length]; //make temp array
                for (int i = 0; i < c.Length; ++i) //loop through each cycle...
                {
                    for (int j = 0; j < nodes.Length; ++j) //...and compare to each node
                    {
                        if (nodes[j].GetComponent<Node>().ID == c[i]) //fill temp array with corresponding nodes
                            d[i] = nodes[j];
                    }                    
                }
                temp.Add(d);                   
            }
            dist = temp;
            foreach (GameObject[] c in dist)
            {
                //SARAH: MAKE DISTRICT HERE
                string str = "" + c[0].GetComponent<Node>().ID;
                for (int i = 1; i < c.Length; ++i)
                {
                    str += "," + c[i].GetComponent<Node>().ID;
                }
                Debug.Log(str);
                GameObject newDistrict = Instantiate(districtPrefab) as GameObject;
                newDistrict.GetComponent<DistrictCollider2>().SetCollider(c);
                districts.Add(newDistrict.GetComponent<DistrictCollider2>());
            }
        }
    }   

    #region Find Cycles take 1

    public void ClearConnections()
    {

        while (connectors.Count > 0)
        {
            Destroy(connectors[connectors.Count - 1].gameObject);
            connectors.RemoveAt(connectors.Count - 1);
        }

        while (districts.Count > 0)
        {
            Destroy(districts[districts.Count - 1].gameObject);
            districts.RemoveAt(districts.Count - 1);
        }
    }

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
    #endregion

    #region Find Cycles take 2

    int[,] CreateAdjMatrix()
    {
        int[,] matrix = new int[nodes.Length, nodes.Length];
        foreach (Connector c in connectors)
        {
            matrix[c.A.ID - 1, c.B.ID - 1] = 1;
            matrix[c.B.ID - 1, c.A.ID - 1] = 1;
        }
        return matrix;
    }

    int[,] CreateAdjGraph()
    {
        int[,] graph = new int[connectors.Count, 2];
        for (int i = 0; i < connectors.Count; ++i)
        {
            graph[i, 0] = connectors[i].A.ID;
            graph[i, 1] = connectors[i].B.ID;

        }
        return graph;
    }

    List<int[]> GetCycles()
    {
        //int[,] adj = CreateAdjMatrix();
        int[,] graph = CreateAdjGraph();
        List<int[]> cycles = new List<int[]>();

        for (int i = 0; i < graph.GetLength(0); ++i)
        {
            for (int j = 0; j < graph.GetLength(1); ++j)
            {

                FindNewCycles(graph, cycles, new int[] { graph[i, j] });
            }
        }
        return cycles;
    }

    void FindNewCycles(int[,] graph, List<int[]> cycles, int[] path)
    {
        int n = path[0];
        int x;
        int[] subPath = new int[path.Length + 1];

        for (int i = 0; i < graph.GetLength(0); ++i)
        {
            for (int y = 0; y <= 1; ++y)
            {
                if (graph[i, y] == n) //current node
                {
                    x = graph[i, (y + 1) % 2];//???
                    if (!IsVisited(x, path)) //havent seen node yet
                    {
                        subPath[0] = x;
                        System.Array.Copy(path, 0, subPath, 1, path.Length);
                        FindNewCycles(graph, cycles, subPath);
                    } else if (path.Length > 2 && x == path[path.Length-1]){ //found cycle
                        int[] normalized = Normalize(path);//normalize
                        int[] inverted = Invert(path);//invert
                        if (IsNew(normalized, cycles) && IsNew(inverted, cycles)) // isnew normalized and isnew inverted
                        {
                            cycles.Add(normalized);
                        }

                    }
                }
            }
        }
    }

    int FindSmallest(int[] path)
    {
        int min = path[0];
        
        foreach (int p in path)
        {
            if (p < min) min = p;
        }

        return min;
    }

    int[] Normalize(int[] path)
    {
        int[] p = new int[path.Length];
        int x = FindSmallest(path);
        int n;

        System.Array.Copy(path, 0, p, 0, path.Length);

        while(p[0] != x)
        {
            n = p[0];
            System.Array.Copy(p, 1, p, 0, p.Length - 1);
            p[p.Length - 1] = n;
        }

        return p;
    }

    int[] Invert(int[] path)
    {
        int[] p = new int[path.Length];

        for(int i =0; i < path.Length; ++i)
        {
            p[i] = path[path.Length - 1 - i];
        }

        return Normalize(p);
    }

    bool Equals(int[] a, int[] b)
    {
        bool ret = (a[0] == b[0]) && (a.Length == b.Length);

        for(int i = 1; ret && (i < a.Length); ++i)
        {
            if(a[i] != b[i])
            {
                ret = false;
            }
        }

        return ret;
    }

    bool IsVisited(int n, int[] path)
    {
        foreach (int i in path)
        {
            if (i == n)
            {
                return true;
            }
        }
        return false;
    }

    bool IsNew(int[] path, List<int[]> cycles)
    {
        foreach(int[] p in cycles)
        {
            if(Equals(p, path))
            {
                return false;
            }
        }

        return true;
    }
    #endregion
}

