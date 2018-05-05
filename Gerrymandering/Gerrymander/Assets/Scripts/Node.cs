using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public struct NodeData: IComponentData
{
    public Vector2 gridPosition = new Vector2();
}

public class Node : MonoBehaviour
{
    public static int GLOBAL_ID = 0;
    private int id = -1;
    

    public int ID
    {
        get { return id; }
        set { 
            id = value;
            this.gameObject.name = "Node " + id;
        }
    }
    // Use this for initialization
    void Awake()
    {
        id = ++GLOBAL_ID;
        this.gameObject.name = "Node " + id;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
