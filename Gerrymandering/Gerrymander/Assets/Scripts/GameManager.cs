using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Unity.Collections;
using Unity.Jobs;
using System.Threading.Tasks;
using Voxel;


//awful coding practice.
//change colors for color-blind people
public enum Affiliation { Yellow = 0, Magenta = 1, Green = 2, None = -1};
public class GameManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public GameObject districtPrefab;
    public GameObject nodePrefab;
    public GameObject transitionPrefab;
    GameObject[] nodes;
    List<Connector> connectors;
    List<Unit> units;
    List<DistrictCollider2> districts;
    List<GameObject[]> dist; //THIS IS THE LIST OF CYCLES AS NODES. USE THIS TO MAKE DISTRICTS
    Graph graph = new Graph();
    /// </summary>
    List<List<Node>> cycles;
    //number of districts each party controls
    int[] partyDistricts = new int[3];
    UIManager uiManager;
    public Affiliation winningTeam = Affiliation.Magenta;
    public int goalDistricts = 3;
    int totalRed, totalBlue, totalGreen = 0;
    int currentRed, currentBlue, currentGreen = 0;
    //Dictionary<Affiliation, int> partyDistricts;
    public Connector connectorPrefab;
    public float maxConnectorLength = 5.0f;


    private Node startNode = null;
    private Connector tempConnector = null;
    int nodeStride = -1;
    bool split = false;

    private JobHandle handle;
    private CycleSortJob cycleSort;


    // Use this for initialization
    void Start()
    {
        cycles = new List<List<Node>>();
        Node.GLOBAL_ID = 0;
        nodes = GameObject.FindGameObjectsWithTag("Node");
        nodes = nodes.OrderByDescending(node => node.transform.position.x * 100 + node.transform.position.z).ToArray();
        for (int i = 1; i <= nodes.Count(); i++)  
        {
            nodes[i - 1].GetComponent<Node>().ID = i;
            if (i > 1 && nodeStride == -1 && (int)nodes[i - 1].transform.position.x != (int)nodes[0].transform.position.x) {
                nodeStride = i - 1;
            }
        }

        foreach (GameObject nodeObj in nodes) 
        {
            Node node = nodeObj.GetComponent<Node>();
            
            GridCoordinate nodePos = new GridCoordinate();
            nodePos.x = (node.ID - 1) % nodeStride;
            nodePos.y = (node.ID - 1) / nodeStride;
            node.GridPosition = nodePos;
            //print(nodeObj.name + ": " + node.gridPosition); 
        }

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
                case Affiliation.Magenta:
                    totalBlue += 1;
                    break;
                case Affiliation.Green:
                    totalGreen += 1;
                    break;
                case Affiliation.Yellow:
                    totalRed += 1;
                    break;
                default:
                    break;

            }
        }


        if (uiCanvas != null)
        {
            uiManager = uiCanvas.GetComponent<UIManager>();
        }
        foreach (GameObject node in nodes)
        {
            graph.AddVertex(node.GetComponent<Node>());
        }


        //Create ckground collider for raycast checks	
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
            //Debug.Log("click");
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
                if (!graph.EdgeExists(startNode, endNode))
                {
                    graph.addEdge(new Edge(graph.IndexOfVertex(startNode), graph.IndexOfVertex(endNode)));
                }
                Connector c = (Connector)Instantiate(connectorPrefab);
                c.A = startNode;
                c.B = endNode;
                if (connectors.Contains(c))
                {
                    //Debug.Log("Nodes already connected");
                    Destroy(c.gameObject);                    tempConnector.GetComponent<Renderer>().material.color = Color.red;
                }
                else {
                    UpdateConnector(c, startNode.transform.position, endNode.transform.position);
                    c.GetComponent<Renderer>().material.color = Color.black;
                    c.transform.SetParent(this.transform);
                    c.transform.localScale = new Vector3(1.5f, c.transform.localScale.y, c.transform.localScale.z);
                    c.name = "Connector_" + c.A.ID + "_" + c.B.ID;
                    connectors.Add(c);
                    startNode = endNode;
                    split = true;
                }

                break;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {//left click up
            //Debug.Log("Release");
            if (tempConnector != null)
            {
                Destroy(tempConnector.gameObject);
                tempConnector = null;
            }
            if (split)
                CheckCycles();

        }
        #endregion
        #region right click
        if (Input.GetMouseButtonDown(1))
        { //right click down
            //Debug.Log("click");
            for (Connector ctr = objectHit.GetComponent<Connector>(); ctr != null;)
            {
                graph.RemoveAllEdges(graph.IndexOfVertex(ctr.A), graph.IndexOfVertex(ctr.B));
                //print("Graph: " + graph.ToString());
                //Debug.Log("Removing conenector");
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


        //update GUI
        #region Setting Text

        string winner = "blah";
        Color winColor = Color.white;
        if (winningTeam == Affiliation.Magenta)
        {
            winner = "Magenta";
            winColor = new Color(234.0f / 255.0f, 100.0f / 255.0f, 222.0f / 255.0f);
        }
        else if (winningTeam == Affiliation.Yellow)
        {
            winner = "Yellow";
            winColor = new Color(243.0f / 255.0f, 201.0f / 255.0f, 105.0f / 255.0f);
        }
        else
        {
            winner = "Green";
            winColor = new Color(35.0f / 255.0f, 150.0f / 255.0f, 127.0f / 255.0f);
        }
        uiManager.SetText(uiManager.Goal, goalDistricts + " Districts\n" + winner + " Wins");
        uiManager.SetColor(uiManager.Goal, winColor);
        uiManager.SetText(uiManager.District, districts.Count() + "/" + goalDistricts + "\nDistricts");

        currentBlue = currentGreen = currentRed = 0;
        foreach (DistrictCollider2 dist in districts)
        {
            switch (dist.winner)
            {
                case Affiliation.Yellow:
                    partyDistricts[0] += 1;
                    break;
                case Affiliation.Magenta:
                    partyDistricts[1] += 1;
                    break;
                case Affiliation.Green:
                    partyDistricts[2] += 1;
                    break;
            }
            currentRed += dist.rgb[0];
            currentGreen += dist.rgb[1];
            currentBlue += dist.rgb[2];

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
        bool rightDistrictPop = true;

        foreach (DistrictCollider2 district in districts)
        {
            if (district.NumUnits != units.Count / goalDistricts)
            {
                rightDistrictPop = false;
                break;
            }
        }
        bool goalMet = rightDistrictPop && (districts.Count == goalDistricts) && (partyDistricts[(int)winningTeam] >= Mathf.RoundToInt(((goalDistricts + 1) / 3) + 1)) && allDistricted;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Goal Met: " + goalMet);
            Debug.Log("Correct num districts: " + (districts.Count == goalDistricts));
            Debug.Log("All units Districted: " + (allDistricted));
            Debug.Log("winning team has enough: " + (partyDistricts[(int)winningTeam] >= Mathf.RoundToInt(((goalDistricts + 1) / 3) + 1)));
        }

        if (goalMet)
        {
            uiManager.ShowVictory();
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void NextLevel()
    {
        string name = SceneManager.GetActiveScene().name;
        string[] splitName = name.Split('_');
        int num;
        if (int.TryParse(splitName[splitName.Length - 1], out num))
        {
            PlayerPrefs.SetInt(num.ToString(), 1);
            num += 1;
            string nextLevelString = "Lvl_" + num;
            //Application.LoadLevel("Scenes/Levels/" + nextLevelString);
            ClearConnections();
            SceneManager.LoadScene(nextLevelString, LoadSceneMode.Single);


        }

    }

    private void LateUpdate()
    {
        if (!handle.IsCompleted || cycleSort.resultCycles.Length == 0) return;

        handle.Complete();

        List<List<Node>> newCycles = new List<List<Node>>();
        newCycles = ConvertCycles(cycleSort.resultCycles);
        cycleSort.cyclesList.Dispose();
        cycleSort.resultCycles.Dispose();
       

        newCycles = newCycles.OrderBy(cycle => cycle.Count()).ToList();
        //print("Cycles: ");
        //foreach (List<Node> cycle in newCycles)
        //{
        //print(cycle.AsEnumerable().Select(node => node.name).Aggregate((total, next) => total += " -> " + next));
        //}
        List<GameObject[]> districtList = new List<GameObject[]>();
        foreach (List<Node> c in newCycles)
        {
            districtList.Add(c.Select(node => node.gameObject).ToArray());
        }
        dist = districtList;

        for (int k = 0; k < newCycles.Count; ++k)
        {
            if (k < dist.Count)
            {
                GameObject[] c = dist[k];
                if (c.Length > 0)
                {
                    Node node = c[0].GetComponent<Node>();
                    string str = "" + c[0].GetComponent<Node>().ID;
                    for (int i = 1; i < c.Length; ++i)
                    {
                        str += "," + c[i].GetComponent<Node>().ID;
                    }
                }

                GameObject newDistrict = Instantiate(districtPrefab) as GameObject;
                newDistrict.GetComponent<DistrictCollider2>().SetCollider(c);
                districts.Add(newDistrict.GetComponent<DistrictCollider2>());
                newDistrict.name = "district_" + districts.Count;
                newDistrict.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    struct cycleCheckResult
    {
        public bool contains;
        public List<Node> smallCycle;
        public List<Node> largeCycle;
    }

    private bool CompareCycles(List<Node> smaller, List<Node> larger)
    {
        foreach(Node node in smaller)
        {
            if(!larger.Contains(node))
            {
                return false;
            }
        }
        return true;
    }

    private cycleCheckResult CycleContains(List<Node> first, List<Node> second)
    {
        if (first.Count >= second.Count)
        {
            cycleCheckResult result = new cycleCheckResult();
            result.contains = CompareCycles(second, first);
            result.smallCycle = first;
            result.largeCycle = second;
            return result;
        } else
        {
            cycleCheckResult result = new cycleCheckResult();
            result.contains = CompareCycles(first, second);
            result.smallCycle = second;
            result.largeCycle = first;
            return result;
        }
    }

    private void CycleSearch()
    {
        List<List<Node>> newCycles = new List<List<Node>>();
        
        //get the list of all cycles, with a minimum length of 3 to prevent line cycles
        newCycles = graph.detectCycles(aboveLength: 3);

        //create an empty list of cycles
        List<NativeList<Node.NodeData>> temp = new List<NativeList<Node.NodeData>>();

        //enumerate by 2 to compare each Cycle
        //for (int i = 0; i < newCycles.Count - 1; i += 2)
        //{
        //    //if cycles are the same size, and compare cycles returns true, then the lists describe the same cycle, so filter them
        //    if(newCycles[i].Count == newCycles[i + 1].Count && CompareCycles(smaller: newCycles[i], larger: newCycles[i + 1]))
        //    {
        //        temp.Add(newCycles[i]);
        //    }
        //}
        ////newCycles becomes the temp array, and temp is cleared
        //newCycles = new List<List<Node>>(temp);
        //temp.Clear();

        Lookup<Node, List<Node>> cycleGroup = (Lookup<Node, List<Node>>)newCycles.ToLookup(cycle => cycle.First());
        NativeList<BlitableArray<Cycle>> groupedCycleData = new NativeList<BlitableArray<Cycle>>(Allocator.Persistent);
        List<List<List<Node>>> groupedCycles = new List<List<List<Node>>>();
        foreach (GameObject obj in nodes)
        {
            Node node = obj.GetComponent<Node>();
            Predicate<List<Node>> startsWithNode = (List<Node> cycle) => { return cycle.First() == node; };
            groupedCycles.Add(newCycles.FindAll(startsWithNode));
        }

        //Parallel.ForEach(groupedCycles, cycleList =>
        for(int x = 0; x < groupedCycles.Count; x++)
        {
            List<List<Node>> cycleList = groupedCycles[x];
            Cycle[] tempList = new Cycle[cycleList.Count];
            //Parallel.ForEach(cycleList, cycle =>
            for(int y = 0; y< cycleList.Count; y++)
            {
                List<Node> cycle = cycleList[y];
                Node.NodeData[] tempCycle = new Node.NodeData[cycle.Count];
                Cycle cycleData = new Cycle();
                for(int z = 0; z < cycle.Count; z++)
                {
                    Node tempNode = cycle[z];
                    tempCycle[z] = tempNode.data;
                    
                }
                cycleData.nodes.Allocate(tempCycle.ToArray(), Allocator.Persistent);
                tempList[y] = cycleData;
            }
            //);
            BlitableArray<Cycle> tempBlitable = new BlitableArray<Cycle>();
            tempBlitable.Allocate(tempList.ToArray(), Allocator.Persistent);
            groupedCycleData.Add(tempBlitable);
            /*tempBlitable.Dispose();
            tempList.Dispose();*/
        }
        //);

        NativeArray<Cycle> sortedCycleData = new NativeArray<Cycle>(groupedCycleData.Length, Allocator.Persistent);
        cycleSort = new CycleSortJob();
        cycleSort.cyclesList = groupedCycleData;
        cycleSort.resultCycles = sortedCycleData;
        handle = cycleSort.Schedule(groupedCycleData.Length, 1);
        //cycleSort.Run(groupedCycleData.Length);

        /*sortedCycleData.Dispose();
        groupedCycleData.Dispose();*/
        temp.Clear();
    }

    private List<List<Node>> ConvertCycles(NativeArray<Cycle> cyclesData)
    {
        List<List<Node>> convertedCycleList = new List<List<Node>>();
        for (int j = 0; j < cyclesData.Length; j++)
        {
            Node.NodeData[] cycleData = cyclesData[j].nodes.ToArray();
            List<Node> convertedCycle = new List<Node>();
            for (int i = 0; i < cycleData.Length; i++)
            {
                Node.NodeData data = cycleData[i];
                convertedCycle.Add(nodes[data.index].GetComponent<Node>());
            }
            if (convertedCycle.Count > 0) { 
                convertedCycleList.Add(convertedCycle);
            }
        }
        return convertedCycleList;
    }


    /// <summary>
    /// Updates the position and shape of a Connector to fit between the two given points
    /// </summary>
    /// <param name="c"></param>
    /// <param name="initPoint"></param>
    /// <param name="endPoint"></param>
    private void UpdateConnector(Connector c, Vector3 initPoint, Vector3 endPoint)
    {
        c.transform.position = .5f * (initPoint + endPoint) + new Vector3(0.0f, 0.0f, 0.0f);
        c.transform.forward = initPoint - endPoint;
		float displacement = nodePrefab.GetComponent<CapsuleCollider> ().radius * 2;
        c.transform.localScale = new Vector3(0.5f, 0.5f, 0.95f * (initPoint - endPoint).magnitude - displacement);
    }

    /// <summary>
    /// Checks for Cycles to create districts with
    /// </summary>
    private void CheckCycles()
    {
        
        if (connectors.Count > 2)
        {
            for (int i = 0; i < districts.Count; ++i)
            {
                Destroy(districts[i].gameObject);
            }
            districts.Clear();
            CycleSearch();
            

        }
        split = false;
    }

    string printArray(int[] arr)
    {
        string str = "" + arr[0];
        for (int i = 1; i < arr.Length; ++i)
        {
            str += "," + arr[i];
        }
        return str;
    }

    public void ClearConnections()
    {
        //print(graph.edgeCount());;
        graph.Clear();
        //print(graph.edgeCount()); ;

        while (connectors.Count > 0)
        {
            Destroy(connectors[connectors.Count - 1].gameObject);
            connectors.RemoveAt(connectors.Count - 1);
        }

        for (int i = 0; i < districts.Count; ++i)
        {
            Destroy(districts[i].gameObject);
        }
        districts.Clear();
    }
}



public struct CycleSortJob: IJobParallelFor
{
    [ReadOnly]
    public NativeList<BlitableArray<Cycle>> cyclesList;
    public NativeArray<Cycle> resultCycles;

    public void Execute(int index)
    {
        Cycle[] cycles = new Cycle[cyclesList[index].Length];
        cycles = cyclesList[index].ToArray();
        int smallestArea = int.MaxValue;
        if(cycles.Length > 0)
        {
            Cycle smallestCycle = cycles[0];
            for (int cycleIndex = 0; cycleIndex < cycles.Length; cycleIndex++)
            {
                Cycle cycle = cycles[cycleIndex];
                //print(cycle.AsEnumerable().Select(node => node.name).Aggregate((total, next) => total += " -> " + next));
                //print(cycle.Count());

                //int stride = 0;

                Node.NodeData previousNode = cycle.nodes[0];
                Node.NodeData currentNode = cycle.nodes[1];
                List<GridCoordinate> Corners = new List<GridCoordinate>();
                GridCoordinate corner = new GridCoordinate();
                corner.x = previousNode.x;
                corner.y = previousNode.y;
                Corners.Add(corner);

                for (int i = 1; i < cycle.nodes.Length - 1; i++)
                {

                    Node.NodeData nextNode = cycle.nodes[i + 1];
                    if (previousNode != currentNode)
                    {
                        int prevNodeIndex = previousNode.index - 1;
                        int nodeIndex = currentNode.index - 1;
                        int nextNodeIndex = nextNode.index - 1;

                        int prevStride = nodeIndex - prevNodeIndex;
                        int nextStride = nextNodeIndex - nodeIndex;
                        if (nextStride != prevStride)
                        {
                            corner.x = currentNode.x;
                            corner.y = currentNode.y;
                            Corners.Add(corner);
                        }

                    }
                    previousNode = currentNode;
                    currentNode = nextNode;
                }
                //foreach (Vector2 corner in Corners)
                //{
                //print(corner); 
                //}

                int area = (int)districtArea(Corners);
                //print(area); 
                if (area < smallestArea)
                {
                    smallestArea = area;
                    smallestCycle = cycle;
                }

            }
            if (smallestArea != 0)
            {
                resultCycles[index] = smallestCycle;
            }
        }
        
    }

    float districtArea(List<GridCoordinate> Corners)
    {
        float area = 0.0f;
        GridCoordinate p1 = new GridCoordinate();
        GridCoordinate p2 = new GridCoordinate();
        for (int i = 0; i < Corners.Count(); i++)
        {
            p1 = Corners[i];
            p2 = Corners[(i + 1) % Corners.Count()];

            float avgHeight = (p1.y + p2.y) / 2;
            float width = p1.x - p2.x;
            float snapArea = width * avgHeight;
            area += snapArea;
        }
        return Math.Abs(area);
    }
}

