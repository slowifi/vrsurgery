using UnityEngine;
using System.Collections.Generic;

public class DivideTriangle : MonoBehaviour
{
    /// <summary>
    ///  지금 divide되는 triangle들은 버텍스가 두개씩 존재함.
    /// </summary>
    /// <param name="_centerIdx"> 시작 vtx index </param>
    /// <param name="_new"> edge 위에 있는 vtx 좌표 </param>
    /// <param name="_triangleIdx">  </param>
    /// <param name="_edgeIdx"></param>
    
    public void DivideTrianglesStartFromVtx(int _centerIdx, Vector3 _new, int _triangleIdx, int _edgeIdx)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 3];
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 1];
        
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        // world to local position
        newVertices[vertices.Length] = ObjManager.Instance.objTransform.InverseTransformPoint(_new);

        newTriangles[_triangleIdx] = _centerIdx;
        newTriangles[_triangleIdx + 1] = vertices.Length;
        newTriangles[_triangleIdx + 2] = edgeList[_edgeIdx].vtx2;

        int _triLen = triangles.Length;
        newTriangles[_triLen++] = _centerIdx;
        newTriangles[_triLen++] = edgeList[_edgeIdx].vtx1;
        newTriangles[_triLen++] = vertices.Length;

        MeshManager.Instance.mesh.vertices = newVertices;
        MeshManager.Instance.mesh.triangles = newTriangles;
    }

    public void DivideTrianglesStart(Vector3 centerPosition, Vector3 newEdgeVertexPosition, int triangleIdx, int vertexIndex, int edgeIdx, ref int triangleCount, bool isInner)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int newTriangleLength = triangles.Length + IncisionManager.Instance.newTriangles.Count;
        int newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;
        
        Transform objTransform = ObjManager.Instance.objTransform;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

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
        newTriangles.Add(triangleIdx+1, edgeList[edgeIdx].vtx1);
        newTriangles.Add(triangleIdx+2, newVertexLength + 1);

        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, newVertexLength + 2);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);

        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);
        newTriangles.Add(triangleCount++, vertexIndex);

        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, vertexIndex);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);

        // 버텍스 추가를 가장 나중에
        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(centerPosition));
        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(newEdgeVertexPosition));
        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(newEdgeVertexPosition));
        newVertices.Add(newVertexLength++, centerPosition);
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
    }

    public void DivideTrianglesEnd(Vector3 centerPosition, int triangleIdx, ref int triangleCount, int edgeIdx)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int newTriangleLength = triangles.Length + IncisionManager.Instance.newTriangles.Count;
        int newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;

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

        Dictionary<int, Vector3> newVertices = IncisionManager.Instance.newVertices;
        Dictionary<int, int> newTriangles = IncisionManager.Instance.newTriangles;

        //newVertices[vertices.Length] = objTransform.InverseTransformPoint(newCenter);

        //// left side
        //newTriangles[triangleIdx] = vertices.Length;
        //newTriangles[triangleIdx + 1] = _leftVtxIdx;
        //newTriangles[triangleIdx + 2] = edgeList[newEdgeIdx].vtx2;

        //int _triLen = triangles.Length;

        //newTriangles[_triLen++] = vertices.Length;
        //newTriangles[_triLen++] = edgeList[newEdgeIdx].vtx2;
        //newTriangles[_triLen++] = newVtxIdx;

        //newTriangles[_triLen++] = vertices.Length;
        //newTriangles[_triLen++] = newVtxIdx;
        //newTriangles[_triLen++] = edgeList[newEdgeIdx].vtx1;

        //// right side
        //newTriangles[_triLen++] = vertices.Length;
        //newTriangles[_triLen++] = edgeList[newEdgeIdx].vtx1;
        //newTriangles[_triLen] = _rightVtxIdx;

        //left
        newTriangles.Add(triangleIdx, newVertexLength);
        newTriangles.Add(triangleIdx+1, newVertexLength-2);
        newTriangles.Add(triangleIdx+2, edgeList[edgeIdx].vtx2);
        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx2);
        newTriangles.Add(triangleCount++, notEdgeVertex);

        //right
        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, notEdgeVertex);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);
        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, edgeList[edgeIdx].vtx1);
        newTriangles.Add(triangleCount++, newVertexLength-1);

        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(centerPosition));
        newVertices.Add(newVertexLength++, centerPosition);
    }

    public void DivideTrianglesClockWise(Vector3 newEdgeVertexPosition, int triangleIdx, ref int triangleCount, int intersectEdgeIdx, bool isInner)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int newTriangleLength = triangles.Length + IncisionManager.Instance.newTriangles.Count;
        int newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;

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

        //newTriangles[triangleIdx] = leftVtxIdx;
        //newTriangles[triangleIdx + 1] = edgeList[intersectEdgeIdx].vtx1;
        //newTriangles[triangleIdx + 2] = vertices.Length;
        //int _triLen = triangles.Length;

        //// 이 triangles는 right side
        //newTriangles[_triLen++] = rightVtxIdx;
        //newTriangles[_triLen++] = vertices.Length + 1;
        //newTriangles[_triLen++] = edgeList[intersectEdgeIdx].vtx2;
        //newTriangles[_triLen++] = rightVtxIdx;
        //newTriangles[_triLen++] = edgeList[intersectEdgeIdx + nextLength].vtx1;
        //newTriangles[_triLen++] = edgeList[intersectEdgeIdx + nextLength].vtx2;

        //left
        newTriangles.Add(triangleIdx, newVertexLength-2);
        newTriangles.Add(triangleIdx + 1, edgeList[intersectEdgeIdx].vtx1);
        newTriangles.Add(triangleIdx + 2, newVertexLength);

        //right
        newTriangles.Add(triangleCount++, newVertexLength - 1);
        newTriangles.Add(triangleCount++, newVertexLength+1);
        newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx2);
        newTriangles.Add(triangleCount++, newVertexLength - 1);
        newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx2);
        newTriangles.Add(triangleCount++, notEdgeVertex);

        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(newEdgeVertexPosition));
        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(newEdgeVertexPosition));
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
    }

    public void DivideTrianglesCounterClockWise(Vector3 newEdgeVertexPosition, int triangleIdx, ref int triangleCount, int intersectEdgeIdx, bool isInner)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int newTriangleLength = triangles.Length + IncisionManager.Instance.newTriangles.Count;
        int newVertexLength = MeshManager.Instance.vertexCount + IncisionManager.Instance.newVertices.Count;

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
        newTriangles.Add(triangleIdx, newVertexLength-1);
        newTriangles.Add(triangleIdx+1, newVertexLength+1);
        newTriangles.Add(triangleIdx+2, edgeList[intersectEdgeIdx].vtx2);

        //left
        newTriangles.Add(triangleCount++, newVertexLength-2);
        newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx1);
        newTriangles.Add(triangleCount++, newVertexLength);
        newTriangles.Add(triangleCount++, newVertexLength-2);
        newTriangles.Add(triangleCount++, notEdgeVertex);
        newTriangles.Add(triangleCount++, edgeList[intersectEdgeIdx].vtx1);
        //// left
        //newVertices[vertices.Length] = objTransform.InverseTransformPoint(newEdgeVertexPosition);
        //// right
        //newVertices[vertices.Length + 1] = objTransform.InverseTransformPoint(newEdgeVertexPosition);


        //// right side
        //newTriangles[_triangleIdx] = rightVtxIdx;
        //newTriangles[_triangleIdx + 1] = vertices.Length + 1;
        //newTriangles[_triangleIdx + 2] = edgeList[intersectEdgeIdx].vtx2;

        //int _triLen = triangles.Length;

        //// left side
        //newTriangles[_triLen++] = leftVtxIdx;
        //newTriangles[_triLen++] = edgeList[intersectEdgeIdx].vtx1;
        //newTriangles[_triLen++] = vertices.Length;

        //newTriangles[_triLen++] = leftVtxIdx;
        //newTriangles[_triLen++] = edgeList[intersectEdgeIdx + nextLength].vtx1;
        //newTriangles[_triLen++] = edgeList[intersectEdgeIdx + nextLength].vtx2;

        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(newEdgeVertexPosition));
        //newVertices.Add(newVertexLength++, objTransform.InverseTransformPoint(newEdgeVertexPosition));
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
    }
}
