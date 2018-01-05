﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Node : MonoBehaviour
{
    private List<Connector> connectors;
    public static int GLOBAL_ID = 0;
    private int id = -1;
    public Vector2 gridPosition = new Vector2();

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

    public List<Connector> GetConnectors() { return connectors; }
}
