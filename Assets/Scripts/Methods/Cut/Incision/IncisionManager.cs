using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncisionManager : Singleton<IncisionManager>
{
    public Dictionary<int, Vector3> newVertices;
    public Dictionary<int, int> newTriangles;

    public List<int> leftSide;
    public List<int> rightSide;

    private Ray startScreenRay;
    private Ray endScreenRay;

    private Vector3 startOuterVertexPosition;
    private Vector3 startInnerVertexPosition;
    private Vector3 endOuterVertexPosition;
    private Vector3 endInnerVertexPosition;

    private int startOuterTriangleIndex;
    private int startInnerTriangleIndex;
    private int endOuterTriangleIndex;
    private int endInnerTriangleIndex;

    private int trianglesCount;

    private DivideTriangle _dividingMethods;
    
    // 여기서 버텍스 두개 입력 받고 start end points
    public void SetStartVertices()
    {
        // screen point도 저장해야됨.
        startScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectionManager.Instance.GetIntersectedValues(startScreenRay, ref startOuterVertexPosition, ref startInnerVertexPosition, ref startOuterTriangleIndex, ref startInnerTriangleIndex);
    }

    public void SetEndVertices()
    {
        // 여기에 라인렌더러 넣는걸
        endScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectionManager.Instance.GetIntersectedValues(endScreenRay, ref endOuterVertexPosition, ref endInnerVertexPosition, ref endOuterTriangleIndex, ref endInnerTriangleIndex);
    }

    public void SetDividingList()
    {
        int outerSide = 0, innerSide = 0;
        int outerEdgeIdx = -1, innerEdgeIdx = -1;

        Vector3 outerEdgePoint = Vector3.zero;
        Vector3 innerEdgePoint = Vector3.zero;
        int outerTriangleIndex = startOuterTriangleIndex;
        int innerTriangleIndex = startInnerTriangleIndex;

        // start
        outerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startOuterVertexPosition, endOuterVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay);
        innerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref innerEdgeIdx, ref innerEdgePoint, startInnerVertexPosition, endInnerVertexPosition, ref innerTriangleIndex, startScreenRay, endScreenRay);

        if (outerSide == -1)
        {
            Debug.Log("error");
            return;
        }
        else if (innerSide == -1)
        {
            Debug.Log("error");
            return;
        }

        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        // outer 부터 
        int outerTriangleCount = MeshManager.Instance.triangles.Length;
        int outerNotEdgeVertex = startOuterTriangleIndex;

        for (int i = 0; i < 3; i++)
        {
            if (edgeList[outerEdgeIdx].vtx1 == edgeList[outerNotEdgeVertex + i].vtx2)
            {
                outerNotEdgeVertex = edgeList[outerNotEdgeVertex + i].vtx1;
                break;
            }
            else if (edgeList[outerEdgeIdx].vtx2 == edgeList[outerNotEdgeVertex + i].vtx1)
            {
                outerNotEdgeVertex = edgeList[outerNotEdgeVertex + i].vtx2;
                break;
            }
        }

        _dividingMethods.DivideTrianglesStart(startOuterVertexPosition, outerEdgePoint, startOuterTriangleIndex, outerNotEdgeVertex, outerEdgeIdx, ref outerTriangleCount, false);

        while (true)
        {
            for (int i = 0; i < 3; i++)
                if (edgeList[outerEdgeIdx].vtx1 == edgeList[outerTriangleIndex + i].vtx2 && edgeList[outerEdgeIdx].vtx2 == edgeList[outerTriangleIndex + i].vtx1)
                    outerEdgeIdx = outerTriangleIndex + i;

            if (outerTriangleIndex == endOuterTriangleIndex)
            {
                _dividingMethods.DivideTrianglesEnd(endOuterVertexPosition, endOuterTriangleIndex, ref outerTriangleCount, outerEdgeIdx);
                break;
            }

            outerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startOuterVertexPosition, endOuterVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay);
            
            if (outerSide == 1)
                _dividingMethods.DivideTrianglesClockWise(outerEdgePoint, outerTriangleIndex, ref outerTriangleCount, outerEdgeIdx, false);
            else if (outerSide == 2)
                _dividingMethods.DivideTrianglesCounterClockWise(outerEdgePoint, outerTriangleIndex, ref outerTriangleCount, outerEdgeIdx, false);
            else
            {
                Debug.Log("error");
                break;
            }
        }

        int innerTriangleCount = outerTriangleCount;
        int innerNotEdgeVertex = startInnerTriangleIndex;

        for (int i = 0; i < 3; i++)
        {
            if (edgeList[innerEdgeIdx].vtx1 == edgeList[innerNotEdgeVertex + i].vtx2)
            {
                innerNotEdgeVertex = edgeList[innerNotEdgeVertex + i].vtx1;
                break;
            }
            else if (edgeList[innerEdgeIdx].vtx2 == edgeList[innerNotEdgeVertex + i].vtx1)
            {
                innerNotEdgeVertex = edgeList[innerNotEdgeVertex + i].vtx2;
                break;
            }
        }

        //inner
        _dividingMethods.DivideTrianglesStart(startInnerVertexPosition, innerEdgePoint, startInnerTriangleIndex, innerNotEdgeVertex, innerEdgeIdx, ref innerTriangleCount, false);

        while (true)
        {
            for (int i = 0; i < 3; i++)
                if (edgeList[innerEdgeIdx].vtx1 == edgeList[innerTriangleIndex + i].vtx2 && edgeList[innerEdgeIdx].vtx2 == edgeList[innerTriangleIndex + i].vtx1)
                    innerEdgeIdx = innerTriangleIndex + i;

            if (innerTriangleIndex == endInnerTriangleIndex)
            {
                _dividingMethods.DivideTrianglesEnd(endInnerVertexPosition, endInnerTriangleIndex, ref innerTriangleCount, innerEdgeIdx);
                break;
            }

            innerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref innerEdgeIdx, ref innerEdgePoint, startInnerVertexPosition, endInnerVertexPosition, ref innerTriangleIndex, startScreenRay, endScreenRay);

            if (innerSide == 1)
                _dividingMethods.DivideTrianglesClockWise(innerEdgePoint, innerTriangleIndex, ref innerTriangleCount, innerEdgeIdx, false);
            else if (innerSide == 2)
                _dividingMethods.DivideTrianglesCounterClockWise(innerEdgePoint, innerTriangleIndex, ref innerTriangleCount, innerEdgeIdx, false);
            else
            {
                Debug.Log("error");
                break;
            }
        }
        Debug.Log("Complete");
        trianglesCount = innerTriangleCount;
    }

    public void ExecuteDividing()
    {
        // 메쉬의 변동이 있으면 mesh update 해야됨.
        Mesh mesh = MeshManager.Instance.mesh;
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;

        Vector3[] vertices = new Vector3[mesh.vertexCount + newVertices.Count];
        int[] triangles = new int[mesh.triangles.Length + trianglesCount];

        for (int i = 0; i < oldVertices.Length; i++)
            vertices[i] = oldVertices[i];
        for (int i = 0; i < oldTriangles.Length; i++)
            triangles[i] = oldTriangles[i];

        foreach (var item in newVertices)
            vertices[item.Key] = item.Value;

        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    public void Extending()
    {





    }

    public void IncisionUpdate()
    {
        startOuterVertexPosition = Vector3.zero;
        startInnerVertexPosition = Vector3.zero;
        endOuterVertexPosition = Vector3.zero;
        endInnerVertexPosition = Vector3.zero;

        startOuterTriangleIndex = -1;
        startInnerTriangleIndex = -1;
        endOuterTriangleIndex = -1;
        endInnerTriangleIndex = -1;
        trianglesCount = 0;

        startScreenRay = new Ray();
        endScreenRay = new Ray();

        leftSide.Clear();
        rightSide.Clear();
        newVertices.Clear();
        newTriangles.Clear();
    }

    // elements가 10만개 넘으면 reinitializing이 효과적이고 밑이면 그냥 clear 쓰는게 이득.
    protected override void InitializeChild()
    {
        startOuterVertexPosition = Vector3.zero;
        startInnerVertexPosition = Vector3.zero;
        endOuterVertexPosition = Vector3.zero;
        endInnerVertexPosition = Vector3.zero;

        startOuterTriangleIndex = -1;
        startInnerTriangleIndex = -1;
        endOuterTriangleIndex = -1;
        endInnerTriangleIndex = -1;
        trianglesCount = 0;

        startScreenRay = new Ray();
        endScreenRay = new Ray();

        leftSide = new List<int>();
        rightSide = new List<int>();
        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }
}
