using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiMeshAdjacencyList : Singleton<MultiMeshAdjacencyList>
{
    public Dictionary<int, HashSet<int>>[] ConnectedVertices;
    public Dictionary<int, HashSet<int>>[] ConnectedTriangles;

    public List<Edge>[] EdgeList;
    public List<Vector3>[] WorldPositionVertices;
    public List<int[]> Triangles;

    public int[] VertexCount;

    public int Size;

    protected override void InitializeChild()
    {
        ListUpdate();
    }
    public void InitSize()
    {
        Size = GameObject.Find("ImportButton").GetComponent<ImportMesh>().Length;

        ConnectedVertices = new Dictionary<int, HashSet<int>>[Size];
        ConnectedTriangles = new Dictionary<int, HashSet<int>>[Size];
        WorldPositionVertices = new List<Vector3>[Size];
        EdgeList = new List<Edge>[Size];
        VertexCount = new int[Size];
        Triangles = new List<int[]>();
    }
    public void ListUpdate()
    {
        InitSize();

        MultiMeshManager.Instance.MeshUpdate();
        for (int i = 0; i < Size; i++)
        {
            ConnectedVertices[i] = new Dictionary<int, HashSet<int>>();
            ConnectedTriangles[i] = new Dictionary<int, HashSet<int>>();

            EdgeList[i] = new List<Edge>();
            VertexCount[i] = InstanceMesh(i).vertexCount;
            Triangles.Add(InstanceMesh(i).triangles);

            WorldPositionVertices[i] = LocalToWorldPosition(InstanceMesh(i), i);
            ConnectedVerticesAndTriangles(VertexCount[i], Triangles[i], i);
            GenerateEdgeList(VertexCount[i], Triangles.ElementAt(i), i);
        }
    }
    public void WorldPositionUpdate()
    {
        for (int i = 0; i < Size; i++)
            WorldPositionVertices[i] = LocalToWorldPosition(InstanceMesh(i), i);
    }
    public List<Vector3> LocalToWorldPosition(Mesh mesh, int i)
    {
        List<Vector3> Worldposition = new List<Vector3>(mesh.vertices);
        Transform PartTransform = MultiMeshManager.Instance.Transforms[i];

        for (int j = 0; j < Worldposition.Count; j++)
            Worldposition[j] = PartTransform.TransformPoint(Worldposition[j]);

        return Worldposition;
    }
    private void ConnectedVerticesAndTriangles(int VerticesLength, int[] triangles, int k)
    {
        for (int j = 0; j < VerticesLength; j++)
        {
            ConnectedVertices[k].Add(j, new HashSet<int>());
            ConnectedTriangles[k].Add(j, new HashSet<int>());
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            ConnectedVertices[k][triangles[i]].Add(triangles[i + 1]);
            ConnectedVertices[k][triangles[i]].Add(triangles[i + 2]);

            ConnectedVertices[k][triangles[i + 1]].Add(triangles[i]);
            ConnectedVertices[k][triangles[i + 1]].Add(triangles[i + 2]);

            ConnectedVertices[k][triangles[i + 2]].Add(triangles[i]);
            ConnectedVertices[k][triangles[i + 2]].Add(triangles[i + 1]);

            ConnectedTriangles[k][triangles[i]].Add(i);
            ConnectedTriangles[k][triangles[i + 1]].Add(i);
            ConnectedTriangles[k][triangles[i + 2]].Add(i);
        }
    }
    private void GenerateEdgeList(int VerticesLength, int[] triangles, int k)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            EdgeList[k].Add(new Edge(triangles[i], triangles[i + 1], i, -1));
            EdgeList[k].Add(new Edge(triangles[i + 1], triangles[i + 2], i, -1));
            EdgeList[k].Add(new Edge(triangles[i + 2], triangles[i], i, -1));
        }

        for (int i = 0; i < VerticesLength; i++)
        {
            foreach (int item in ConnectedTriangles[k][i])
            {
                foreach (int insideItem in ConnectedTriangles[k][i])
                {
                    if (EdgeList[k][item].vtx1 == EdgeList[k][insideItem].vtx2 && EdgeList[k][item].vtx2 == EdgeList[k][insideItem].vtx1)
                    {
                        EdgeList[k][item].tri2 = insideItem;
                        EdgeList[k][insideItem].tri2 = item;
                    }
                    else if (EdgeList[k][item + 1].vtx1 == EdgeList[k][insideItem].vtx2 && EdgeList[k][item + 1].vtx2 == EdgeList[k][insideItem].vtx1)
                    {
                        EdgeList[k][item + 1].tri2 = insideItem;
                        EdgeList[k][insideItem].tri2 = item;
                    }
                    else if (EdgeList[k][item + 2].vtx1 == EdgeList[k][insideItem].vtx2 && EdgeList[k][item + 2].vtx2 == EdgeList[k][insideItem].vtx1)
                    {
                        EdgeList[k][item + 2].tri2 = insideItem;
                        EdgeList[k][insideItem].tri2 = item;
                    }

                    else if (EdgeList[k][item].vtx1 == EdgeList[k][insideItem + 1].vtx2 && EdgeList[k][item].vtx2 == EdgeList[k][insideItem + 1].vtx1)
                    {
                        EdgeList[k][item].tri2 = insideItem;
                        EdgeList[k][insideItem + 1].tri2 = item;
                    }
                    else if (EdgeList[k][item + 1].vtx1 == EdgeList[k][insideItem + 1].vtx2 && EdgeList[k][item + 1].vtx2 == EdgeList[k][insideItem + 1].vtx1)
                    {
                        EdgeList[k][item + 1].tri2 = insideItem;
                        EdgeList[k][insideItem + 1].tri2 = item;
                    }
                    else if (EdgeList[k][item + 2].vtx1 == EdgeList[k][insideItem + 1].vtx2 && EdgeList[k][item + 2].vtx2 == EdgeList[k][insideItem + 1].vtx1)
                    {
                        EdgeList[k][item + 2].tri2 = insideItem;
                        EdgeList[k][insideItem + 1].tri2 = item;
                    }

                    else if (EdgeList[k][item].vtx1 == EdgeList[k][insideItem + 2].vtx2 && EdgeList[k][item + 2].vtx2 == EdgeList[k][insideItem + 2].vtx1)
                    {
                        EdgeList[k][item].tri2 = insideItem;
                        EdgeList[k][insideItem + 2].tri2 = item;
                    }
                    else if (EdgeList[k][item + 1].vtx1 == EdgeList[k][insideItem + 2].vtx2 && EdgeList[k][item + 1].vtx2 == EdgeList[k][insideItem + 2].vtx1)
                    {
                        EdgeList[k][item + 1].tri2 = insideItem;
                        EdgeList[k][insideItem + 2].tri2 = item;
                    }
                    else if (EdgeList[k][item + 2].vtx1 == EdgeList[k][insideItem + 2].vtx2 && EdgeList[k][item + 2].vtx2 == EdgeList[k][insideItem + 2].vtx1)
                    {
                        EdgeList[k][item + 2].tri2 = insideItem;
                        EdgeList[k][insideItem + 2].tri2 = item;
                    }
                }
            }
        }
    }
    public Mesh InstanceMesh(int i)
    {
        return MultiMeshManager.Instance.Meshes[i];
    }
}