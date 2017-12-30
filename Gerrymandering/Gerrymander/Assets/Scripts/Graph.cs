using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;



public class Graph: System.Collections.IEnumerable {
    //All vertices in the graph
    List<Node> vertices = new List<Node>();
    //2D array of edges between vertices
    List<List<Edge>> edges = new List<List<Edge>>();

    public Graph()
    {

    }

    public Graph(List<Node> vertices)
    {
        this.vertices = vertices;
        foreach (Node vertex in vertices)
        {
            edges.Add(new List<Edge>());
        }
    }

    public void Clear()
    {
        this.edges = new List<List<Edge>>();
        foreach(Node vertex in vertices)
        {
            edges.Add(new List<Edge>());
        }
    }

    public int VertexCount()
    {
        return vertices.Count;
    }

    public int edgeCount()
    {
        int total = 0; 
        edges.ForEach(delegate(List<Edge> list)
        {
            total += list.Count;
        });
        return total;
    }

    public Node VertexAtIndex(int index)
    {
        return vertices[index];
    }

    public int IndexOfVertex(Node vertex)
    {
        return vertices.IndexOf(vertex);
    }

    public List<Node> NeighborsForIndex(int index)
    {
        return edges[index].AsEnumerable().Select(edge => this.vertices[edge.V]).ToList<Node>();
    }

    public List<Node> NeighborsForVertex(Node vertex)
    {
        int i = vertices.IndexOf(vertex);
        if (i != -1 && i < edges.Count())
        {
            return NeighborsForIndex(i);
        }

        return new List<Node>();
    }

    public List<Edge> EdgesForIndex(int index)
    {
        return edges[index];
    }

    public List<Edge> EdgesForVertex(Node vertex)
    {
        int i = vertices.IndexOf(vertex);
        if( i!= 1)
        {
            return EdgesForIndex(i);
        }

        return new List<Edge>();
    }

    public bool EdgeExists(int from, int to)
    {
        return edges[from].Select(edge => edge.V).Contains(to);
    }

    public bool EdgeExists(Node from, Node to)
    {
        int u = IndexOfVertex(from);
        if(u != -1 && u < edges.Count())
        {
            int v = IndexOfVertex(to);
            if(v != -1 && v < edges.Count())
            {
                return EdgeExists(from: u, to: v);
            }
        }
        return false;
    }

    public bool VertexInGraph(Node vertex)
    {
        return (vertices.IndexOf(vertex) != -1);
    }

    public int AddVertex(Node vertex)
    {
        vertices.Add(vertex);
        edges.Add(new List<Edge>());
        return vertices.Count - 1;
    }

    public void addEdge(int from, int to)
    {
        addEdge(new Edge(u: from, v: to));
    }

    public void addEdge(Edge edge)
    {
         
        edges[edge.U].Add(edge);
        edges[edge.V].Add(edge.Reversed());
    }

    public void RemoveAllEdges(int from, int to)
    {
        //var removingEdges = edges[from].Select((edge, i) => new { edge, i }).Reverse().Where(obj => obj.edge.V == from);
        foreach (var obj in edges[from].AsEnumerable().Select((edge, i) => new { edge, i }).Reverse().Where(obj => obj.edge.V == to))
        {
            int i = obj.i;
            edges[from].RemoveAt(i);
        }

        //removingEdges = edges[to].Select((edge, i) => new { edge, i }).Reverse().Where(edge => edge.v == to);
        foreach (var obj in edges[to].AsEnumerable().Select((edge, i) => new { edge, i }).Reverse().Where(obj => obj.edge.V == from))
        {
            int i = obj.i;
            edges[to].RemoveAt(i);
        }
    }

    public void RemoveVertexAtIndex(int index)
    {
        var range = Enumerable.Range(0, index);
        foreach (int j in range)
        {
            List<int> toRemove = new List<int>();
            var edgeRange = Enumerable.Range(0, edges[j].Count);
            foreach (int l in edgeRange)
            {
                if (edges[j][l].V == index)
                {
                    toRemove.Add(l);
                    continue;
                }
                if (edges[j][l].V > index)
                {
                    edges[j][l].V -= 1;
                }
            }
            foreach (int f in toRemove.AsEnumerable().Reverse())
            {
                edges[j].RemoveAt(f);
            }
        }
        edges.RemoveAt(index);
        vertices.RemoveAt(index);
    }

    public void RemoveVertex(Node vertex)
    {
        int i = IndexOfVertex(vertex);
        if (i != -1)
        {
            RemoveVertexAtIndex(i);
        }
    }
    
    public List<List<Node>> detectCycles(int upToLength = int.MaxValue, int aboveLength = 0)
    {
        //Empty list that will contain cycles, which are a list of nodes that  are connected
        List<List<Node>> cycles = new List<List<Node>>();
        //Creates all possible paths by putting all vertices into their own list of noedes
        List<List<Node>> openPaths = vertices.AsEnumerable().Select(vertex => new List<Node> { vertex }).ToList<List<Node>>();

        //While the list of possible paths is not empty
        while (openPaths.Count > 0)
        {

            //pick the frontmost path, remove it from the list
            List<Node> openPath = openPaths.First();
            openPaths.RemoveAt(0);


            //If the path is longer than the max length, return the current list of cycles
            if (openPath.Count > upToLength) { return cycles; }

            //grab the end of the current path and the head of the path
            Node tail = openPath.Last(), head = openPath.First();

            //if neither is null
            if (tail != null && head != null)
            {
                //grab all of the vertices adjacent to the tail, and for each neighbor
                List<Node> neighbors = NeighborsForVertex(tail);
                foreach(Node neighbor in neighbors)
                {
                    //if that neighbor is ALSO the head node, and the path is at least as long as the minimum cycle length
                    if (neighbor == head && openPath.Count > aboveLength)
                    {
                        //copy the open path, add the neighbor node, and then add the new cycle to the list of cycles
                        List<Node> copy = new List<Node>(openPath);
                        copy.Add(neighbor);
                        cycles.Add(copy);
                    //otherwise, if the openPath doesn't contain the neighbor already, and the neighbor vertex has already been explored
                    } else if(!openPath.Contains(neighbor) && IndexOfVertex(neighbor) > IndexOfVertex(head))
                    {
                        //copy the path, add the neighbor node, and then add the path back to the list of open Paths
                        List<Node> copy = new List<Node>(openPath);
                        copy.Add(neighbor);
                        openPaths.Add(copy);
                    }
                }
            }
        }

        //return the cycles list
        return cycles;
    }

    override public string ToString()
    {
        string d = "";
        foreach(int i in Enumerable.Range(0, vertices.Count))
        {
            d += vertices[i] + " -> " + NeighborsForIndex(i).ToString() + "\n";
        }
        return d;
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
        for (int i = 0; i < VertexCount(); i++)
        {
            yield return VertexAtIndex(i);
        }
    }

    public Node this[int index]
    {
        get { return VertexAtIndex(index); }
    }

}

public class Edge
{
    private int u;
    public int U
    {
        get {
            return u;
        }
        set
        {
            u = value;
        }
    }

    private int v;
    public int V
    {
        get
        {
            return v;
        }
        set
        {
            v = value;
        }
    }

    public Edge(int u, int v)
    {
        this.u = u;
        this.v = v;
    }

    public string toString()
    {
        return (u + " <-> " + v);
    }

    public Edge Reversed()
    {
        return new Edge(u: v, v: u);
    }
}