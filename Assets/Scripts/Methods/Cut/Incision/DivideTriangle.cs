﻿using UnityEngine;
using System.Collections.Generic;

public class DivideTriangle : MonoBehaviour
{
    public bool isBoundary = true;

    /// <summary>
    ///  지금 divide되는 triangle들은 버텍스가 두개씩 존재함.
    /// </summary>
    /// <param name="centerIdx"> 시작 vtx index </param>
    /// <param name="newVtx"> edge 위에 있는 vtx 좌표 </param>
    /// <param name="triangleIdx">  </param>
    /// <param name="edgeIdx"></param>

    // 밑에 두개는 기존의 boundary cut 외곽 vtx 저장하는것.
    //public void DivideTrianglesStartFromVtx(int centerIdx, Vector3 newVtx, int triangleIdx, int edgeIdx, ref int vtxIdxBeforeDivide, ref int verticesLength, ref int triangleCount, int isInner)
    //{
    //    int vertexCount = verticesLength;
    //    vtxIdxBeforeDivide = vertexCount;

    //    if (isInner == 0)
    //        outerBoundaryCutVertices.Add(vertexCount);
    //    else
    //        innerBoundaryCutVertices.Add(vertexCount);

    //    boundaryNewVerticesList.Add(verticesLength++, obj.transform.InverseTransformPoint(newVtx));

    //    if (!boundaryNewTrianglesList.ContainsKey(triangleIdx))
    //    {
    //        boundaryNewTrianglesList.Add(triangleIdx, centerIdx);
    //        boundaryNewTrianglesList.Add(triangleIdx + 1, vertexCount);
    //        boundaryNewTrianglesList.Add(triangleIdx + 2, edgeList[edgeIdx].vtx2);
    //    }
    //    else
    //    {
    //        boundaryNewTrianglesList[triangleIdx] = centerIdx;
    //        boundaryNewTrianglesList[triangleIdx + 1] = vertexCount;
    //        boundaryNewTrianglesList[triangleIdx + 2] = edgeList[edgeIdx].vtx2;
    //    }
    //    boundaryNewTrianglesList.Add(triangleCount++, centerIdx);
    //    boundaryNewTrianglesList.Add(triangleCount++, edgeList[edgeIdx].vtx1);
    //    boundaryNewTrianglesList.Add(triangleCount++, vertexCount);
    //}

    //public void DivideTrianglesEndToVtx(int endVtxIdx, int triangleIdx, int edgeIdx, int edgeVtxIdx, ref int triangleCount, int isInner)
    //{
    //    if (!boundaryNewTrianglesList.ContainsKey(triangleIdx))
    //    {
    //        boundaryNewTrianglesList.Add(triangleIdx, edgeVtxIdx);
    //        boundaryNewTrianglesList.Add(triangleIdx + 1, edgeList[edgeIdx].vtx2);
    //        boundaryNewTrianglesList.Add(triangleIdx + 2, endVtxIdx);
    //    }
    //    else
    //    {
    //        boundaryNewTrianglesList[triangleIdx] = edgeVtxIdx;
    //        boundaryNewTrianglesList[triangleIdx + 1] = edgeList[edgeIdx].vtx2;
    //        boundaryNewTrianglesList[triangleIdx + 2] = endVtxIdx;
    //    }
    //    boundaryNewTrianglesList.Add(triangleCount++, edgeVtxIdx);
    //    boundaryNewTrianglesList.Add(triangleCount++, endVtxIdx);
    //    boundaryNewTrianglesList.Add(triangleCount++, edgeList[edgeIdx].vtx1);
    //}

    public void DivideTrianglesStartFromVtx(int startVtxIndex, Vector3 newEdgeVertexPosition, int triangleIdx, int edgeIdx, ref int triangleCount)
    {
        int newTriangleLength = MeshManager.Instance.mesh.triangles.Length + IncisionManager.Instance.newTriangles.Count;
        int newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;

        Transform objTransform = ObjManager.Instance.objTransform;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        Dictionary<int, Vector3> newVertices = BoundaryCutManager.Instance.newVertices;
        Dictionary<int, int> newTriangles = BoundaryCutManager.Instance.newTriangles;
        if (newTriangles.ContainsKey(triangleIdx))
        {
            newTriangles[triangleIdx] = startVtxIndex;
            newTriangles[triangleIdx+1] = edgeList[edgeIdx].vtx1;
            newTriangles[triangleIdx+2] = newVertexLength;
        }
        else
        {
            newTriangles.Add(triangleIdx, startVtxIndex);
            newTriangles.Add(triangleIdx + 1, edgeList[edgeIdx].vtx1);
            newTriangles.Add(triangleIdx + 2, newVertexLength);
        }
        newTriangles.Add(triangleCount++, startVtxIndex);
        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);

        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
    }

    public void DivideTrianglesEndToVtx(int endVtxIndex, int triangleIdx, ref int triangleCount, int edgeIdx)
    {
        int newTriangleLength = MeshManager.Instance.mesh.triangles.Length + IncisionManager.Instance.newTriangles.Count;
        int newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;

        Transform objTransform = ObjManager.Instance.objTransform;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        Dictionary<int, Vector3> newVertices = BoundaryCutManager.Instance.newVertices;
        Dictionary<int, int> newTriangles = BoundaryCutManager.Instance.newTriangles;
        if (newTriangles.ContainsKey(triangleIdx))
        {
            newTriangles[triangleIdx] = endVtxIndex;
            newTriangles[triangleIdx + 1] = newVertexLength - 1;
            newTriangles[triangleIdx + 2] = edgeList[edgeIdx].vtx2;
        }
        else
        {
            newTriangles.Add(triangleIdx, endVtxIndex);
            newTriangles.Add(triangleIdx + 1, newVertexLength - 1);
            newTriangles.Add(triangleIdx + 2, edgeList[edgeIdx].vtx2);
        }

        newTriangles.Add(triangleCount++, endVtxIndex);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);
        newTriangles.Add(triangleCount++, newVertexLength-1);
    }

    public void DivideTrianglesStart(Vector3 centerPosition, Vector3 newEdgeVertexPosition, int triangleIdx, int vertexIndex, int edgeIdx, ref int triangleCount, bool isInner)
    {
        int newTriangleLength;
        int newVertexLength;
        if (isBoundary)
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + BoundaryCutManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + BoundaryCutManager.Instance.newVertices.Count;
        }
        else
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + IncisionManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;
        }
        Transform objTransform = ObjManager.Instance.objTransform;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        if(isBoundary)
        {
            Dictionary<int, Vector3> newVertices = BoundaryCutManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = BoundaryCutManager.Instance.newTriangles;
            BoundaryCutManager.Instance.startVertexIndex = newVertexLength;

            newTriangles.Add(triangleIdx, newVertexLength);
            newTriangles.Add(triangleIdx + 1, edgeList[edgeIdx].vtx1);
            newTriangles.Add(triangleIdx + 2, newVertexLength + 1);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, newVertexLength + 1);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);
            newTriangles.Add(triangleCount++, vertexIndex);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, vertexIndex);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);

            newVertices.Add(newVertexLength++, centerPosition);
            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        }
        else
        {
            if (!isInner)
            {
                // outer 딱 가운데 점들만 모으고 나서 BFS할 때 쓰일 vtx는 따로 찾는걸로.
                IncisionManager.Instance.leftSide.Add(AdjacencyList.Instance.edgeList[edgeIdx].vtx1);
                IncisionManager.Instance.rightSide.Add(AdjacencyList.Instance.edgeList[edgeIdx].vtx2);
            }
            else
            {
                // inner
                IncisionManager.Instance.rightSide.Add(AdjacencyList.Instance.edgeList[edgeIdx].vtx1);
                IncisionManager.Instance.leftSide.Add(AdjacencyList.Instance.edgeList[edgeIdx].vtx2);
            }

            Dictionary<int, Vector3> newVertices = IncisionManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = IncisionManager.Instance.newTriangles;

            newTriangles.Add(triangleIdx, newVertexLength);
            newTriangles.Add(triangleIdx + 1, edgeList[edgeIdx].vtx1);
            newTriangles.Add(triangleIdx + 2, newVertexLength + 1);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, newVertexLength + 2);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);
            newTriangles.Add(triangleCount++, vertexIndex);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, vertexIndex);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);

            newVertices.Add(newVertexLength++, centerPosition);
            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        }
    }

    public void DivideTrianglesEnd(Vector3 centerPosition, int triangleIdx, ref int triangleCount, int edgeIdx)
    {
        int newTriangleLength;
        int newVertexLength;
        if (isBoundary)
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + BoundaryCutManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + BoundaryCutManager.Instance.newVertices.Count;
        }
        else
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + IncisionManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;
        }
        Transform objTransform = ObjManager.Instance.objTransform;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        int notEdgeVertex = -1;
        for (int i = 0; i < 3; i++)
        {
            if (edgeList[edgeIdx].vtx1 == edgeList[triangleIdx + i].vtx2)
            {
                notEdgeVertex = edgeList[triangleIdx + i].vtx1;
                break;
            }
            else if (edgeList[edgeIdx].vtx2 == edgeList[triangleIdx + i].vtx1)
            {
                notEdgeVertex = edgeList[triangleIdx + i].vtx2;
                break;
            }
        }

        if(isBoundary)
        {
            Dictionary<int, Vector3> newVertices = BoundaryCutManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = BoundaryCutManager.Instance.newTriangles;

            newTriangles.Add(triangleIdx, newVertexLength);
            newTriangles.Add(triangleIdx + 1, newVertexLength - 1);
            newTriangles.Add(triangleIdx + 2, edgeList[edgeIdx].vtx2);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);
            newTriangles.Add(triangleCount++, notEdgeVertex);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, notEdgeVertex);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);

            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);
            newTriangles.Add(triangleCount++, newVertexLength - 1);
            Debug.Log(newVertexLength);
            newVertices.Add(newVertexLength++, centerPosition);
        }
        else
        {
            Dictionary<int, Vector3> newVertices = IncisionManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = IncisionManager.Instance.newTriangles;

            //left
            newTriangles.Add(triangleIdx, newVertexLength);
            newTriangles.Add(triangleIdx + 1, newVertexLength - 2);
            newTriangles.Add(triangleIdx + 2, edgeList[edgeIdx].vtx2);
            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);
            newTriangles.Add(triangleCount++, notEdgeVertex);

            //right
            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, notEdgeVertex);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);
            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);
            newTriangles.Add(triangleCount++, newVertexLength - 1);

            newVertices.Add(newVertexLength++, centerPosition);
        }
    }

    public void DivideTrianglesClockWise(Vector3 newEdgeVertexPosition, int triangleIdx, ref int triangleCount, int intersectEdgeIdx, bool isInner)
    {
        int newTriangleLength;
        int newVertexLength;
        if (isBoundary)
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + BoundaryCutManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + BoundaryCutManager.Instance.newVertices.Count;
        }
        else
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + IncisionManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;
        }
        

        Transform objTransform = ObjManager.Instance.objTransform;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        int notEdgeVertex = -1;
        for (int i = 0; i < 3; i++)
        {
            if (edgeList[intersectEdgeIdx].vtx1 == edgeList[triangleIdx + i].vtx2)
            {
                notEdgeVertex = edgeList[triangleIdx + i].vtx1;
                break;
            }
            else if (edgeList[intersectEdgeIdx].vtx2 == edgeList[triangleIdx + i].vtx1)
            {
                notEdgeVertex = edgeList[triangleIdx + i].vtx2;
                break;
            }
        }

        if(isBoundary)
        {

            Dictionary<int, Vector3> newVertices = BoundaryCutManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = BoundaryCutManager.Instance.newTriangles;
            if (newTriangles.ContainsKey(triangleIdx))
            {
                newTriangles[triangleIdx] = newVertexLength - 1;
                newTriangles[triangleIdx + 1] = edgeList[intersectEdgeIdx].vtx1;
                newTriangles[triangleIdx + 2] = newVertexLength;
            }
            else
            {
                newTriangles.Add(triangleIdx, newVertexLength - 1);
                newTriangles.Add(triangleIdx + 1, edgeList[intersectEdgeIdx].vtx1);
                newTriangles.Add(triangleIdx + 2, newVertexLength);
            }
            newTriangles.Add(triangleCount++, newVertexLength - 1);
            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx2);
            newTriangles.Add(triangleCount++, newVertexLength - 1);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx2);
            newTriangles.Add(triangleCount++, notEdgeVertex);

            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        }
        else
        {
            if (!isInner)
            {
                IncisionManager.Instance.leftSide.Add(newVertexLength);
                IncisionManager.Instance.rightSide.Add(newVertexLength + 1);
            }
            else
            {
                IncisionManager.Instance.rightSide.Add(newVertexLength);
                IncisionManager.Instance.leftSide.Add(newVertexLength + 1);
            }

            Dictionary<int, Vector3> newVertices = IncisionManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = IncisionManager.Instance.newTriangles;

            //left
            newTriangles.Add(triangleIdx, newVertexLength - 2);
            newTriangles.Add(triangleIdx + 1, edgeList[intersectEdgeIdx].vtx1);
            newTriangles.Add(triangleIdx + 2, newVertexLength);

            //right
            newTriangles.Add(triangleCount++, newVertexLength - 1);
            newTriangles.Add(triangleCount++, newVertexLength + 1);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx2);
            newTriangles.Add(triangleCount++, newVertexLength - 1);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx2);
            newTriangles.Add(triangleCount++, notEdgeVertex);

            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        }
        
    }

    public void DivideTrianglesCounterClockWise(Vector3 newEdgeVertexPosition, int triangleIdx, ref int triangleCount, int intersectEdgeIdx, bool isInner)
    {
        int newTriangleLength;
        int newVertexLength;
        if (isBoundary)
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + BoundaryCutManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + BoundaryCutManager.Instance.newVertices.Count;
        }
        else
        {
            newTriangleLength = MeshManager.Instance.mesh.triangles.Length + IncisionManager.Instance.newTriangles.Count;
            newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;
        }

        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Transform objTransform = ObjManager.Instance.objTransform;

        int notEdgeVertex = -1;
        for (int i = 0; i < 3; i++)
        {
            if (edgeList[intersectEdgeIdx].vtx1 == edgeList[triangleIdx + i].vtx2)
            {
                notEdgeVertex = edgeList[triangleIdx + i].vtx1;
                break;
            }
            else if (edgeList[intersectEdgeIdx].vtx2 == edgeList[triangleIdx + i].vtx1)
            {
                notEdgeVertex = edgeList[triangleIdx + i].vtx2;
                break;
            }
        }

        if(isBoundary)
        {
            Dictionary<int, Vector3> newVertices = BoundaryCutManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = BoundaryCutManager.Instance.newTriangles;
            if (newTriangles.ContainsKey(triangleIdx))
            {
                newTriangles[triangleIdx] = newVertexLength - 1;
                newTriangles[triangleIdx + 1] = newVertexLength;
                newTriangles[triangleIdx + 2] = edgeList[intersectEdgeIdx].vtx2;
            }
            else
            {
                newTriangles.Add(triangleIdx, newVertexLength - 1);
                newTriangles.Add(triangleIdx + 1, newVertexLength);
                newTriangles.Add(triangleIdx + 2, edgeList[intersectEdgeIdx].vtx2);
            }
            newTriangles.Add(triangleCount++, newVertexLength - 1);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx1);
            newTriangles.Add(triangleCount++, newVertexLength);

            newTriangles.Add(triangleCount++, newVertexLength - 1);
            newTriangles.Add(triangleCount++, notEdgeVertex);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx1);

            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        }
        else
        {
            if (!isInner)
            {
                IncisionManager.Instance.leftSide.Add(newVertexLength);
                IncisionManager.Instance.rightSide.Add(newVertexLength + 1);
            }
            else
            {
                IncisionManager.Instance.rightSide.Add(newVertexLength);
                IncisionManager.Instance.leftSide.Add(newVertexLength + 1);
            }

            Dictionary<int, Vector3> newVertices = IncisionManager.Instance.newVertices;
            Dictionary<int, int> newTriangles = IncisionManager.Instance.newTriangles;

            //right
            newTriangles.Add(triangleIdx, newVertexLength - 1);
            newTriangles.Add(triangleIdx + 1, newVertexLength + 1);
            newTriangles.Add(triangleIdx + 2, edgeList[intersectEdgeIdx].vtx2);

            //left
            newTriangles.Add(triangleCount++, newVertexLength - 2);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx1);
            newTriangles.Add(triangleCount++, newVertexLength);
            newTriangles.Add(triangleCount++, newVertexLength - 2);
            newTriangles.Add(triangleCount++, notEdgeVertex);
            newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx1);

            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
            newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        }
        
    }
}
