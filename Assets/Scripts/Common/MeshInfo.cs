using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Edge
{
    public int vtx1 { get; set; }
    public int vtx2 { get; set; }
    public int tri1 { get; set; }
    public int tri2 { get; set; }

    public Edge(int v1, int v2, int t1, int t2)
    {
        vtx1 = v1;
        vtx2 = v2;
        tri1 = t1;
        tri2 = t2;
    }
}
