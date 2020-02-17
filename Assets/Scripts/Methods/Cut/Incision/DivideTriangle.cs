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
        newTriangles[_triangleIdx + 2] = AdjacencyList.Instance.edgeList[_edgeIdx].vtx2;

        int _triLen = triangles.Length;
        newTriangles[_triLen++] = _centerIdx;
        newTriangles[_triLen++] = AdjacencyList.Instance.edgeList[_edgeIdx].vtx1;
        newTriangles[_triLen++] = vertices.Length;

        MeshManager.Instance.mesh.vertices = newVertices;
        MeshManager.Instance.mesh.triangles = newTriangles;
    }

    public void DivideTrianglesStart(Vector3 _center, Vector3 _new, int _triangleIdx, int _edgeIdx, int _vtxIdx, int _isInner)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 9];
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 3];

        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Transform objTransform = ObjManager.Instance.objTransform;

        

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        if (_isInner == 0)
        {
            // outer
            IncisionManager.Instance.leftSide.Add(vertices.Length + 1);
            IncisionManager.Instance.rightSide.Add(vertices.Length + 2);

            IncisionManager.Instance.leftSide.Add(AdjacencyList.Instance.edgeList[_edgeIdx].vtx1);
            IncisionManager.Instance.rightSide.Add(AdjacencyList.Instance.edgeList[_edgeIdx].vtx2);

        }
        else
        {
            // inner
            IncisionManager.Instance.rightSide.Add(vertices.Length + 1);
            IncisionManager.Instance.leftSide.Add(vertices.Length + 2);

            IncisionManager.Instance.rightSide.Add(AdjacencyList.Instance.edgeList[_edgeIdx].vtx1);
            IncisionManager.Instance.leftSide.Add(AdjacencyList.Instance.edgeList[_edgeIdx].vtx2);
        }

        newVertices[vertices.Length] = objTransform.InverseTransformPoint(new Vector3(_center.x, _center.y, _center.z));
        // left side vtx
        newVertices[vertices.Length + 1] = objTransform.InverseTransformPoint(_new);
        // right side vtx
        newVertices[vertices.Length + 2] = objTransform.InverseTransformPoint(_new);

        // 이 triangle은 지금 left side
        newTriangles[_triangleIdx] = vertices.Length;
        newTriangles[_triangleIdx + 1] = edgeList[_edgeIdx].vtx1;
        newTriangles[_triangleIdx + 2] = vertices.Length + 1;

        int _triLen = triangles.Length;
        // 이 triangle은 지금 right side
        newTriangles[_triLen++] = vertices.Length;
        newTriangles[_triLen++] = vertices.Length + 2;
        newTriangles[_triLen++] = edgeList[_edgeIdx].vtx2;

        // 밑에 두개 triangle은 무시 한다. 
        newTriangles[_triLen++] = vertices.Length;
        newTriangles[_triLen++] = edgeList[_edgeIdx].vtx2;
        newTriangles[_triLen++] = _vtxIdx;

        newTriangles[_triLen++] = vertices.Length;
        newTriangles[_triLen++] = _vtxIdx;
        newTriangles[_triLen] = edgeList[_edgeIdx].vtx1;

        MeshManager.Instance.mesh.vertices = newVertices;
        MeshManager.Instance.mesh.triangles = newTriangles;
    }

    public void DivideTrianglesEnd(Vector3 _newCenter, int _triangleIdx, int _edgeIdx, int side, int _isInner, int check)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 9];
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 1];

        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Transform objTransform = ObjManager.Instance.objTransform;

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        newVertices[vertices.Length] = objTransform.InverseTransformPoint(_newCenter);

        int newEdgeIdx = -1, newVtxIdx = -1;

        for (int i = 0; i < 3; i++)
        {
            if (triangles[_triangleIdx + i] != edgeList[_edgeIdx].vtx2 && triangles[_triangleIdx + i] != edgeList[_edgeIdx].vtx1)
                newVtxIdx = triangles[_triangleIdx + i];
            if (edgeList[_triangleIdx + i].vtx1 == edgeList[_edgeIdx].vtx2 && edgeList[_triangleIdx + i].vtx2 == edgeList[_edgeIdx].vtx1)
                newEdgeIdx = _triangleIdx + i;
        }

        int _leftVtxIdx = vertices.Length - (2 + check);
        int _rightVtxIdx = vertices.Length - (1 + check);
        if (_isInner == 0)
        {
            // outer
            // boundaryOuterEndVtx = vertices.Length;
            IncisionManager.Instance.leftSide.Add(edgeList[newEdgeIdx].vtx2);
            IncisionManager.Instance.rightSide.Add(edgeList[newEdgeIdx].vtx1);
        }
        else
        {
            // inner
            // boundaryInnerEndVtx = vertices.Length;
            IncisionManager.Instance.rightSide.Add(edgeList[newEdgeIdx].vtx2);
            IncisionManager.Instance.leftSide.Add(edgeList[newEdgeIdx].vtx1);
        }

        // left side
        newTriangles[_triangleIdx] = vertices.Length;
        newTriangles[_triangleIdx + 1] = _leftVtxIdx;
        newTriangles[_triangleIdx + 2] = edgeList[newEdgeIdx].vtx2;

        int _triLen = triangles.Length;

        newTriangles[_triLen++] = vertices.Length;
        newTriangles[_triLen++] = edgeList[newEdgeIdx].vtx2;
        newTriangles[_triLen++] = newVtxIdx;

        newTriangles[_triLen++] = vertices.Length;
        newTriangles[_triLen++] = newVtxIdx;
        newTriangles[_triLen++] = edgeList[newEdgeIdx].vtx1;

        // right side
        newTriangles[_triLen++] = vertices.Length;
        newTriangles[_triLen++] = edgeList[newEdgeIdx].vtx1;
        newTriangles[_triLen] = _rightVtxIdx;

        MeshManager.Instance.mesh.vertices = newVertices;
        MeshManager.Instance.mesh.triangles = newTriangles;
    }

    public void DivideTrianglesClockWise(Vector3 _new, int _triangleIdx, int _intersectEdgeIdx, int _leftVtxIdx, int _rightVtxIdx, int _isInner)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 6];
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 2];
        int nextLength = -2;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Transform objTransform = ObjManager.Instance.objTransform;
        if (_intersectEdgeIdx - _triangleIdx != 2)
            nextLength = 1;

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        if (_isInner == 0)
        {
            IncisionManager.Instance.leftSide.Add(vertices.Length);
            IncisionManager.Instance.rightSide.Add(vertices.Length + 1);

            IncisionManager.Instance.leftSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            IncisionManager.Instance.rightSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            IncisionManager.Instance.rightSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx2);

        }
        else
        {
            IncisionManager.Instance.rightSide.Add(vertices.Length);
            IncisionManager.Instance.leftSide.Add(vertices.Length + 1);

            IncisionManager.Instance.rightSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            IncisionManager.Instance.leftSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            IncisionManager.Instance.leftSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx2);

        }
        // left
        newVertices[vertices.Length] = objTransform.InverseTransformPoint(_new);

        // right
        newVertices[vertices.Length + 1] = objTransform.InverseTransformPoint(_new);


        // triangle 만드는 과정을 제대로 해야됨
        // 이 triangle은 지금 left side
        // intersectEdgeIdx는 지금 old edge임.
        newTriangles[_triangleIdx] = _leftVtxIdx;
        newTriangles[_triangleIdx + 1] = edgeList[_intersectEdgeIdx].vtx1;
        newTriangles[_triangleIdx + 2] = vertices.Length;
        int _triLen = triangles.Length;

        // 이 triangles는 right side
        newTriangles[_triLen++] = _rightVtxIdx;
        newTriangles[_triLen++] = vertices.Length + 1;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx].vtx2;
        newTriangles[_triLen++] = _rightVtxIdx;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx1;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx2;

        MeshManager.Instance.mesh.vertices = newVertices;
        MeshManager.Instance.mesh.triangles = newTriangles;
    }

    public void DivideTrianglesCounterClockWise(Vector3 _new, int _triangleIdx, int _intersectEdgeIdx, int _leftVtxIdx, int _rightVtxIdx, int _isInner)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 6];
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 2];
        int nextLength = 2;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Transform objTransform = ObjManager.Instance.objTransform;

        if (_intersectEdgeIdx - _triangleIdx != 0)
            nextLength = -1;

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        // left
        newVertices[vertices.Length] = objTransform.InverseTransformPoint(_new);
        if (_isInner == 0)
        {
            IncisionManager.Instance.leftSide.Add(vertices.Length);
            IncisionManager.Instance.rightSide.Add(vertices.Length + 1);

            IncisionManager.Instance.rightSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            IncisionManager.Instance.leftSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            IncisionManager.Instance.leftSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx1);

        }
        else
        {
            IncisionManager.Instance.rightSide.Add(vertices.Length);
            IncisionManager.Instance.leftSide.Add(vertices.Length + 1);

            IncisionManager.Instance.leftSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            IncisionManager.Instance.rightSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            IncisionManager.Instance.rightSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx1);

        }
        // right
        newVertices[vertices.Length + 1] = objTransform.InverseTransformPoint(_new);


        // right side
        newTriangles[_triangleIdx] = _rightVtxIdx;
        newTriangles[_triangleIdx + 1] = vertices.Length + 1;
        newTriangles[_triangleIdx + 2] = edgeList[_intersectEdgeIdx].vtx2;

        int _triLen = triangles.Length;

        // left side
        newTriangles[_triLen++] = _leftVtxIdx;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx].vtx1;
        newTriangles[_triLen++] = vertices.Length;

        newTriangles[_triLen++] = _leftVtxIdx;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx1;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx2;

        MeshManager.Instance.mesh.vertices = newVertices;
        MeshManager.Instance.mesh.triangles = newTriangles;
    }
}
