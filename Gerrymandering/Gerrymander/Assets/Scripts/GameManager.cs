using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//awful coding practice.
//change colors for color-blind people
enum Affiliation { Red = 0, Blue = 1, Green = 2, };
public class GameManager : MonoBehaviour
{
    List<Node> nodes;
    List<Connector> connectors;
    List<Unit> units;
    List<District> districts;
    int[] partyDistricts = new int[3];
    //Dictionary<Affiliation, int> partyDistricts;



    // Use this for initialization
    void Start()
    {
        //partyDistricts[(int)Affiliation.Red]++;	
    }

    // Update is called once per frame
    void Update()
    {
        //mouse / touch input (raycasts)
        //after input calculate districts 
        //do not make connectors if there is no valid district made
        //update GUI
        //check for win condition
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

