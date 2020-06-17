using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class MultiMeshAdjacencyList : Singleton<MultiMeshAdjacencyList>
{
    public Dictionary<int, HashSet<int>>[] MultiMeshConnectedVertices;
    public Dictionary<int, HashSet<int>>[] MultiMeshConnectedTriangles;
    
    public List<Edge>[] MultiMeshEdgeList;
    public List<Vector3>[] MultiMeshWorldPositionVertices;
    public List<int[]> MultiMeshTriangles = new List<int[]>();

    public int[] MultiMeshVertexCount;

    public void ListsUpdate()
    {
        MultiMeshConnectedVertices = new Dictionary<int, HashSet<int>>[MultiMeshManager.Instance.Size];
        MultiMeshConnectedTriangles = new Dictionary<int, HashSet<int>>[MultiMeshManager.Instance.Size];
        MultiMeshEdgeList = new List<Edge>[MultiMeshManager.Instance.Size];
        MultiMeshWorldPositionVertices = new List<Vector3>[MultiMeshManager.Instance.Size];
        MultiMeshVertexCount = new int[MultiMeshManager.Instance.Size];

        MultiMeshManager.Instance.MeshesUpdate();
        for(int i=0;i<MultiMeshManager.Instance.Size;i++)
        {
            MultiMeshConnectedVertices[i] = new Dictionary<int, HashSet<int>>();
            MultiMeshConnectedTriangles[i] = new Dictionary<int, HashSet<int>>();
            MultiMeshEdgeList[i] = new List<Edge>();

            MultiMeshVertexCount[i] = MultiMeshManager.Instance.meshes[i].vertexCount;
            MultiMeshTriangles.Add(MultiMeshManager.Instance.meshes[i].triangles);
            
            MultiMeshWorldPositionVertices[i] = LocalToWorldPosition(MultiMeshManager.Instance.meshes[i], i);
            ConnectedVerticesAndTriangles(MultiMeshVertexCount[i], MultiMeshTriangles.ElementAt(i), i);
            GenerateEdgeList(MultiMeshVertexCount[i], MultiMeshTriangles.ElementAt(i), i);
        }
    }
    public void WorldPositionUpdate()
    {
        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            MultiMeshWorldPositionVertices[i] = LocalToWorldPosition(MultiMeshManager.Instance.meshes[i], i);
    }
    public List<Vector3> LocalToWorldPosition(Mesh mesh, int i)
    {
        List<Vector3> worldposition = new List<Vector3>(mesh.vertices);
        Transform objTransform = MultiMeshManager.Instance.objsTransform[i];
        for (int j = 0; j < worldposition.Count; j++)
            worldposition[j] = objTransform.TransformPoint(worldposition[j]);

        return worldposition;
    }
    private void ConnectedVerticesAndTriangles(int verticesLength, int[] triangles, int k)
    {
        for (int j = 0; j < verticesLength; j++)
        {
            MultiMeshConnectedVertices[k].Add(j, new HashSet<int>());
            MultiMeshConnectedTriangles[k].Add(j, new HashSet<int>());
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            MultiMeshConnectedVertices[k][triangles[i]].Add(triangles[i + 1]);
            MultiMeshConnectedVertices[k][triangles[i]].Add(triangles[i + 2]);

            MultiMeshConnectedVertices[k][triangles[i + 1]].Add(triangles[i]);
            MultiMeshConnectedVertices[k][triangles[i + 1]].Add(triangles[i + 2]);

            MultiMeshConnectedVertices[k][triangles[i + 2]].Add(triangles[i]);
            MultiMeshConnectedVertices[k][triangles[i + 2]].Add(triangles[i + 1]);

            MultiMeshConnectedTriangles[k][triangles[i]].Add(i);
            MultiMeshConnectedTriangles[k][triangles[i + 1]].Add(i);
            MultiMeshConnectedTriangles[k][triangles[i + 2]].Add(i);
        }
    }
    private void GenerateEdgeList(int verticesLength, int[] triangles, int k)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            MultiMeshEdgeList[k].Add(new Edge(triangles[i], triangles[i + 1], i, -1));
            MultiMeshEdgeList[k].Add(new Edge(triangles[i + 1], triangles[i + 2], i, -1));
            MultiMeshEdgeList[k].Add(new Edge(triangles[i + 2], triangles[i], i, -1));
        }

        for (int i = 0; i < verticesLength; i++)
        {
            foreach (int item in MultiMeshConnectedTriangles[k][i])
            {
                foreach (int insideItem in MultiMeshConnectedTriangles[k][i])
                {
                    if (MultiMeshEdgeList[k][item].vtx1 == MultiMeshEdgeList[k][insideItem].vtx2 && MultiMeshEdgeList[k][item].vtx2 == MultiMeshEdgeList[k][insideItem].vtx1)
                    {
                        MultiMeshEdgeList[k][item].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem].tri2 = item;
                    }
                    else if (MultiMeshEdgeList[k][item + 1].vtx1 == MultiMeshEdgeList[k][insideItem].vtx2 && MultiMeshEdgeList[k][item + 1].vtx2 == MultiMeshEdgeList[k][insideItem].vtx1)
                    {
                        MultiMeshEdgeList[k][item + 1].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem].tri2 = item;
                    }
                    else if (MultiMeshEdgeList[k][item + 2].vtx1 == MultiMeshEdgeList[k][insideItem].vtx2 && MultiMeshEdgeList[k][item + 2].vtx2 == MultiMeshEdgeList[k][insideItem].vtx1)
                    {
                        MultiMeshEdgeList[k][item + 2].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem].tri2 = item;
                    }

                    else if (MultiMeshEdgeList[k][item].vtx1 == MultiMeshEdgeList[k][insideItem + 1].vtx2 && MultiMeshEdgeList[k][item].vtx2 == MultiMeshEdgeList[k][insideItem + 1].vtx1)
                    {
                        MultiMeshEdgeList[k][item].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem + 1].tri2 = item;
                    }
                    else if (MultiMeshEdgeList[k][item + 1].vtx1 == MultiMeshEdgeList[k][insideItem + 1].vtx2 && MultiMeshEdgeList[k][item + 1].vtx2 == MultiMeshEdgeList[k][insideItem + 1].vtx1)
                    {
                        MultiMeshEdgeList[k][item + 1].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem + 1].tri2 = item;
                    }
                    else if (MultiMeshEdgeList[k][item + 2].vtx1 == MultiMeshEdgeList[k][insideItem + 1].vtx2 && MultiMeshEdgeList[k][item + 2].vtx2 == MultiMeshEdgeList[k][insideItem + 1].vtx1)
                    {
                        MultiMeshEdgeList[k][item + 2].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem + 1].tri2 = item;
                    }

                    else if (MultiMeshEdgeList[k][item].vtx1 == MultiMeshEdgeList[k][insideItem + 2].vtx2 && MultiMeshEdgeList[k][item + 2].vtx2 == MultiMeshEdgeList[k][insideItem + 2].vtx1)
                    {
                        MultiMeshEdgeList[k][item].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem + 2].tri2 = item;
                    }
                    else if (MultiMeshEdgeList[k][item + 1].vtx1 == MultiMeshEdgeList[k][insideItem + 2].vtx2 && MultiMeshEdgeList[k][item + 1].vtx2 == MultiMeshEdgeList[k][insideItem + 2].vtx1)
                    {
                        MultiMeshEdgeList[k][item + 1].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem + 2].tri2 = item;
                    }
                    else if (MultiMeshEdgeList[k][item + 2].vtx1 == MultiMeshEdgeList[k][insideItem + 2].vtx2 && MultiMeshEdgeList[k][item + 2].vtx2 == MultiMeshEdgeList[k][insideItem + 2].vtx1)
                    {
                        MultiMeshEdgeList[k][item + 2].tri2 = insideItem;
                        MultiMeshEdgeList[k][insideItem + 2].tri2 = item;
                    }
                }
            }
        }
    }
    protected override void InitializeChild()
    {
        ListsUpdate();
    }
}