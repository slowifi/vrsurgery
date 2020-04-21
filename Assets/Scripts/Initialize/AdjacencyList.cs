using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyList : Singleton<AdjacencyList>
{
    /// <summary>
    /// 이거 싱글톤에서 해제하고 각 메쉬마다 생성이 되도록 해야됨.
    /// </summary>


    public Dictionary<int, HashSet<int>> connectedVertices;
    public Dictionary<int, HashSet<int>> connectedTriangles;
    public List<Edge> edgeList;
    public List<Vector3> worldPositionVertices;

    public void ListUpdate()
    {
        MeshManager.Instance.MeshUpdate();
        connectedVertices = new Dictionary<int, HashSet<int>>();
        connectedTriangles = new Dictionary<int, HashSet<int>>();
        edgeList = new List<Edge>();

        int vertexCount = MeshManager.Instance.mesh.vertexCount;
        int[] triangles = MeshManager.Instance.mesh.triangles;

        worldPositionVertices = LocalToWorldPosition(MeshManager.Instance.mesh);
        ConnectedVerticesAndTriangles(vertexCount, triangles);
        GenerateEdgeList(vertexCount, triangles);
    }

    public void ListUpdateOnlyTriangles()
    {
        MeshManager.Instance.MeshUpdate();
        connectedTriangles = new Dictionary<int, HashSet<int>>();
        //edgeList = new List<Edge>();

        int vertexCount = MeshManager.Instance.mesh.vertexCount;
        int[] triangles = MeshManager.Instance.mesh.triangles;

        worldPositionVertices = LocalToWorldPosition(MeshManager.Instance.mesh);
        ConnectedTrianglesAndEdges(vertexCount, triangles);
        //GenerateEdgeList(vertexCount, triangles);
    }

    public void WorldPositionUpdate()
    {
        worldPositionVertices = LocalToWorldPosition(MeshManager.Instance.mesh);
    }

    public List<Vector3> LocalToWorldPosition(Mesh mesh)
    {
        List<Vector3> worldPosition = new List<Vector3>(mesh.vertices);
        Transform objTransform = MeshManager.Instance.objTransform;
        for (int i = 0; i < worldPosition.Count; i++)
        {
            worldPosition[i] = objTransform.TransformPoint(worldPosition[i]);
        }

        return worldPosition;
    }

    private void ConnectedTrianglesAndEdges(int verticesLength, int[] triangles)
    {
        for (int j = 0; j < verticesLength; j++)
        {
            connectedTriangles.Add(j, new HashSet<int>());
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            edgeList.Add(new Edge(triangles[i], triangles[i + 1], i, -1));
            edgeList.Add(new Edge(triangles[i + 1], triangles[i + 2], i, -1));
            edgeList.Add(new Edge(triangles[i + 2], triangles[i], i, -1));

            connectedTriangles[triangles[i]].Add(i);
            connectedTriangles[triangles[i + 1]].Add(i);
            connectedTriangles[triangles[i + 2]].Add(i);
        }

        for (int i = 0; i < verticesLength; i++)
        {
            foreach (int item in connectedTriangles[i])
            {
                foreach (int insideItem in connectedTriangles[i])
                {
                    if (edgeList[item].vtx1 == edgeList[insideItem].vtx2 && edgeList[item].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item].tri2 = insideItem;
                        edgeList[insideItem].tri2 = item;
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item + 1].tri2 = insideItem;
                        edgeList[insideItem].tri2 = item;
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item + 2].tri2 = insideItem;
                        edgeList[insideItem].tri2 = item;
                    }

                    else if (edgeList[item].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item].tri2 = insideItem;
                        edgeList[insideItem + 1].tri2 = item;
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item + 1].tri2 = insideItem;
                        edgeList[insideItem + 1].tri2 = item;
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item + 2].tri2 = insideItem;
                        edgeList[insideItem + 1].tri2 = item;
                    }

                    else if (edgeList[item].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item].tri2 = insideItem;
                        edgeList[insideItem + 2].tri2 = item;
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item + 1].tri2 = insideItem;
                        edgeList[insideItem + 2].tri2 = item;
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item + 2].tri2 = insideItem;
                        edgeList[insideItem + 2].tri2 = item;
                    }
                }
            }
        }
    }

    private void ConnectedVerticesAndTriangles(int verticesLength, int[] triangles)
    {
        for (int j = 0; j < verticesLength; j++)
        {
            connectedVertices.Add(j, new HashSet<int>());
            connectedTriangles.Add(j, new HashSet<int>());
        }
        
        for (int i = 0; i < triangles.Length; i += 3)
        {
            connectedVertices[triangles[i]].Add(triangles[i + 1]);
            connectedVertices[triangles[i]].Add(triangles[i + 2]);

            connectedVertices[triangles[i + 1]].Add(triangles[i]);
            connectedVertices[triangles[i + 1]].Add(triangles[i + 2]);

            connectedVertices[triangles[i + 2]].Add(triangles[i]);
            connectedVertices[triangles[i + 2]].Add(triangles[i + 1]);

            connectedTriangles[triangles[i]].Add(i);
            connectedTriangles[triangles[i + 1]].Add(i);
            connectedTriangles[triangles[i + 2]].Add(i);
        }
    }

    private void GenerateEdgeList(int verticesLength, int[] triangles)
    {
        for (int i = 0; i < triangles.Length; i+=3)
        {
            edgeList.Add(new Edge(triangles[i], triangles[i + 1], i, -1));
            edgeList.Add(new Edge(triangles[i + 1], triangles[i + 2], i, -1));
            edgeList.Add(new Edge(triangles[i + 2], triangles[i], i, -1));
        }

        for (int i = 0; i < verticesLength; i++)
        {
            foreach (int item in connectedTriangles[i])
            {
                foreach (int insideItem in connectedTriangles[i])
                {
                    if (edgeList[item].vtx1 == edgeList[insideItem].vtx2 && edgeList[item].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item].tri2 = insideItem;
                        edgeList[insideItem].tri2 = item;
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item + 1].tri2 = insideItem;
                        edgeList[insideItem].tri2 = item;
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item + 2].tri2 = insideItem;
                        edgeList[insideItem].tri2 = item;
                    }

                    else if (edgeList[item].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item].tri2 = insideItem;
                        edgeList[insideItem + 1].tri2 = item;
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item + 1].tri2 = insideItem;
                        edgeList[insideItem + 1].tri2 = item;
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item + 2].tri2 = insideItem;
                        edgeList[insideItem + 1].tri2 = item;
                    }

                    else if (edgeList[item].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item].tri2 = insideItem;
                        edgeList[insideItem + 2].tri2 = item;
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item + 1].tri2 = insideItem;
                        edgeList[insideItem + 2].tri2 = item;
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item + 2].tri2 = insideItem;
                        edgeList[insideItem + 2].tri2 = item;
                    }
                }
            }
        }
    }

    protected override void InitializeChild()
    {
        ListUpdate();
    }
}
