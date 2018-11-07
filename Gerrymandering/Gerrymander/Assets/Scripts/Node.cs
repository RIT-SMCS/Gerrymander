using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.Collections;
using Voxel;

public struct GridCoordinate
{
    public int x;
    public int y;
}

public class Node : MonoBehaviour
{
    public static int GLOBAL_ID = 0;
    private int id = -1;
    public GridCoordinate GridPosition {
        get
        {
            GridCoordinate position = new GridCoordinate();
            position.x = data.x;
            position.y = data.y;

            return position;
        }
        set
        {
            data.x = value.x;
            data.y = value.y;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct NodeData: IEquatable<Node.NodeData>
    {
        public int index;
        public int x;
        public int y;

        public static bool operator == (NodeData l, NodeData r)
        {
            return l.index == r.index && l.x == r.x && l.y == r.y;
        }
        
        public static bool operator != (NodeData l, NodeData r)
        {
            return !(l == r);
        }

        public override bool Equals(object o)
        {
            NodeData second = (NodeData)o;
            return second != null && index == second.index && x == second.x && y == second.y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(NodeData other)
        {
            return other.index == index && x == other.x && y == other.y;
        }
    }



    public NodeData data;

    public int ID
    {
        get { return id; }
        set { 
            id = value;
            this.gameObject.name = "Node " + id;
            data.index = value - 1;
        }
    }
    // Use this for initialization
    void Awake()
    {
        data = new NodeData();
        id = ++GLOBAL_ID;
        this.gameObject.name = "Node " + id;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Cycle//: IEquatable<Cycle>
{
    public BlitableArray<Node.NodeData> nodes;
    //public static bool operator ==(Cycle l, Cycle r)
    //{
    //    if (l.nodes.Length != r.nodes.Length)
    //    {
    //        return false;
    //    }
    //    for( int i = 0; i < l.nodes.Length; i++)
    //    {
    //        if (!r.nodes.Contains(l.nodes[i])) {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //public static bool operator !=(Cycle l, Cycle r)
    //{
    //    return !(l == r);
    //}

    //public override bool Equals(object o)
    //{
    //    Cycle second = (Cycle)o;
    //    if (nodes.Length != second.nodes.Length)
    //    {
    //        return false;
    //    }

    //    for (int i = 0; i < second.nodes.Length; i++)
    //    {
    //        if (!nodes.Contains(second.nodes[i]))
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //public bool Equals(Cycle other)
    //{
    //    if(nodes.Length != other.nodes.Length)
    //    {
    //        return false;
    //    }

    //    for (int i = 0; i < other.nodes.Length; i++)
    //    {
    //        if (!nodes.Contains(other.nodes[i]))
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //public override int GetHashCode()
    //{
    //    return base.GetHashCode();
    //}
}