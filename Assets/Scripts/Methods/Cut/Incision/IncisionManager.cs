using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IncisionManager : Singleton<IncisionManager>
{
    public Dictionary<int, Vector3> newVertices;
    public Dictionary<int, int> newTriangles;

    public List<List<int>> leftSide;
    public List<List<int>> rightSide;

    public List<List<float>> leftSideWeight;
    public List<List<float>> rightSideWeight;

    private List<Vector3> leftVector;
    private List<Vector3> rightVector;

    private Ray startScreenRay;
    private Ray endScreenRay;

    private Vector3 startOuterVertexPosition;
    private Vector3 startInnerVertexPosition;
    private Vector3 endOuterVertexPosition;
    private Vector3 endInnerVertexPosition;

    public int firstOuterVertexIndex;
    public int lastOuterVertexIndex;
    public int firstInnerVertexIndex;
    public int lastInnerVertexIndex;

    private int startOuterTriangleIndex;
    private int startInnerTriangleIndex;
    private int endOuterTriangleIndex;
    private int endInnerTriangleIndex;

    public int currentIndex;
    private int trianglesCount;

    private DivideTriangle _dividingMethods;

    // 여기서 버텍스 두개 입력 받고 start end points
    // 기존것들은 유지한 상태에서 추가

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
        int outerTriangleCount = MeshManager.Instance.mesh.triangles.Length;
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

        _dividingMethods.DivideTrianglesStartIncision(startOuterVertexPosition, outerEdgePoint, startOuterTriangleIndex, outerNotEdgeVertex, outerEdgeIdx, ref outerTriangleCount, false);

        while (true)
        {
            for (int i = 0; i < 3; i++)
                if (edgeList[outerEdgeIdx].vtx1 == edgeList[outerTriangleIndex + i].vtx2 && edgeList[outerEdgeIdx].vtx2 == edgeList[outerTriangleIndex + i].vtx1)
                    outerEdgeIdx = outerTriangleIndex + i;

            if (outerTriangleIndex == endOuterTriangleIndex)
            {
                _dividingMethods.DivideTrianglesEndIncision(endOuterVertexPosition, endOuterTriangleIndex, ref outerTriangleCount, outerEdgeIdx, false);
                break;
            }

            outerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startOuterVertexPosition, endOuterVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay);

            if (outerSide == 2)
                _dividingMethods.DivideTrianglesClockWiseIncision(outerEdgePoint, edgeList[outerEdgeIdx].tri1, ref outerTriangleCount, outerEdgeIdx, false);
            else if (outerSide == 1)
                _dividingMethods.DivideTrianglesCounterClockWiseIncision(outerEdgePoint, edgeList[outerEdgeIdx].tri1, ref outerTriangleCount, outerEdgeIdx, false);
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
        _dividingMethods.DivideTrianglesStartIncision(startInnerVertexPosition, innerEdgePoint, startInnerTriangleIndex, innerNotEdgeVertex, innerEdgeIdx, ref innerTriangleCount, true);

        while (true)
        {
            for (int i = 0; i < 3; i++)
                if (edgeList[innerEdgeIdx].vtx1 == edgeList[innerTriangleIndex + i].vtx2 && edgeList[innerEdgeIdx].vtx2 == edgeList[innerTriangleIndex + i].vtx1)
                    innerEdgeIdx = innerTriangleIndex + i;

            if (innerTriangleIndex == endInnerTriangleIndex)
            {
                _dividingMethods.DivideTrianglesEndIncision(endInnerVertexPosition, endInnerTriangleIndex, ref innerTriangleCount, innerEdgeIdx, true);
                break;
            }

            innerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref innerEdgeIdx, ref innerEdgePoint, startInnerVertexPosition, endInnerVertexPosition, ref innerTriangleIndex, startScreenRay, endScreenRay);

            if (innerSide == 2)
                _dividingMethods.DivideTrianglesClockWiseIncision(innerEdgePoint, edgeList[innerEdgeIdx].tri1, ref innerTriangleCount, innerEdgeIdx, true);
            else if (innerSide == 1)
                _dividingMethods.DivideTrianglesCounterClockWiseIncision(innerEdgePoint, edgeList[innerEdgeIdx].tri1, ref innerTriangleCount, innerEdgeIdx, true);
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
        int[] triangles = new int[trianglesCount];

        for (int i = 0; i < oldVertices.Length; i++)
            vertices[i] = oldVertices[i];
        for (int i = 0; i < oldTriangles.Length; i++)
            triangles[i] = oldTriangles[i];

        // local 값이 지금 들어가있는 상태에서 
        foreach (var item in newVertices)
            vertices[item.Key] = ObjManager.Instance.objTransform.InverseTransformPoint(item.Value);

        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    public void GenerateIncisionList()
    {
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        leftSideWeight.Add(new List<float>());
        rightSideWeight.Add(new List<float>());

        BFS.Instance.BFS_Circle(leftSide[currentIndex][leftSide[currentIndex].Count - 1], startOuterVertexPosition, endOuterVertexPosition, true);
        BFS.Instance.BFS_Circle(rightSide[currentIndex][rightSide[currentIndex].Count - 1], startOuterVertexPosition, endOuterVertexPosition, false);

        // 이건 start랑 end point에 대해 index만 가지고 있으면 될 일이긴 함.
        Vector3 incisionStartPoint = worldPosition[newTriangles.Values.First()];
        Vector3 incisionEndPoint = worldPosition[vertices.Length - 1];

        leftVector.Add(Vector3.Cross(incisionEndPoint - incisionStartPoint, MeshManager.Instance.mesh.normals[vertices.Length - 1]));
        rightVector.Add(Vector3.Cross(incisionStartPoint - incisionEndPoint, MeshManager.Instance.mesh.normals[vertices.Length - 1]));

        Transform objTransform = ObjManager.Instance.objTransform;

        Vector3 center = Vector3.Lerp(incisionStartPoint, incisionEndPoint, 0.5f);
        double radius = Vector2.Distance(center, incisionEndPoint);
        foreach (int item in leftSide[currentIndex])
        {
            double t = AlgorithmsManager.Instance.QuadraticEquation(worldPosition[item].x - center.x, worldPosition[item].y - center.y, leftVector[currentIndex].x, leftVector[currentIndex].y, radius);
            float temp = Convert.ToSingle(t);
            // temp 값을 리스트 화 시켜야되는데
            leftSideWeight[currentIndex].Add(temp);
            vertices[item] = objTransform.InverseTransformPoint(new Vector3(0.1f * temp * leftVector[currentIndex].x + worldPosition[item].x, 0.1f * temp * leftVector[currentIndex].y + worldPosition[item].y, 0.1f * temp * leftVector[currentIndex].z + worldPosition[item].z));
        }
        foreach (int item in rightSide[currentIndex])
        {
            double t = AlgorithmsManager.Instance.QuadraticEquation(worldPosition[item].x - center.x, worldPosition[item].y - center.y, rightVector[currentIndex].x, rightVector[currentIndex].y, radius);
            float temp = Convert.ToSingle(t);
            // temp 값을 리스트 화 시켜야되는데
            rightSideWeight[currentIndex].Add(temp);
            vertices[item] = objTransform.InverseTransformPoint(new Vector3(0.1f * temp * rightVector[currentIndex].x + worldPosition[item].x, 0.1f * temp * rightVector[currentIndex].y + worldPosition[item].y, 0.1f * temp * rightVector[currentIndex].z + worldPosition[item].z));
        }
        MeshManager.Instance.mesh.vertices = vertices;
    }

    public void Extending(int incisionIndex)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
       
        Transform objTransform = ObjManager.Instance.objTransform;

        // 처음에 한번만 하면되는것 다음에 leftside rightside리스트와 숫자가 같은 t를(weight) 저장할 수 있는 리스트를 만들어서 넣어주고 벡터값만 바뀌면 그 값에 따른 변화를 적용시키면됨.
        // 한번만 할 것들.

        int count = 0;
        foreach (int item in leftSide[incisionIndex])
        {
            vertices[item] = objTransform.InverseTransformPoint(new Vector3(1f * leftSideWeight[incisionIndex][count] * leftVector[incisionIndex].x + worldPosition[item].x, 1f * leftSideWeight[incisionIndex][count] * leftVector[incisionIndex].y + worldPosition[item].y, 1f * leftSideWeight[incisionIndex][count] * leftVector[incisionIndex].z + worldPosition[item].z));
            count++;
        }
        count = 0;
        foreach (int item in rightSide[incisionIndex])
        {
            vertices[item] = objTransform.InverseTransformPoint(new Vector3(1f * rightSideWeight[incisionIndex][count] * rightVector[incisionIndex].x + worldPosition[item].x, 1f * rightSideWeight[incisionIndex][count] * rightVector[incisionIndex].y + worldPosition[item].y, 1f * rightSideWeight[incisionIndex][count] * rightVector[incisionIndex].z + worldPosition[item].z));
            count++;
        }
        
        MeshManager.Instance.mesh.vertices = vertices;
    }

    // 양면메쉬 전용 알고리즘
    public void SetStartVerticesDF()
    {
        // screen point도 저장해야됨.
        startScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectionManager.Instance.GetIntersectedValues(startScreenRay, ref startOuterVertexPosition, ref startOuterTriangleIndex);
    }

    public void SetEndVerticesDF()
    {
        // 여기에 라인렌더러 넣는걸
        endScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectionManager.Instance.GetIntersectedValues(endScreenRay, ref endOuterVertexPosition, ref endOuterTriangleIndex);
    }

    public void SetDividingListDF()
    {
        int outerSide = 0;
        int outerEdgeIdx = -1;

        Vector3 outerEdgePoint = Vector3.zero;
        int outerTriangleIndex = startOuterTriangleIndex;

        leftSide.Add(new List<int>());
        rightSide.Add(new List<int>());

        // start
        outerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startOuterVertexPosition, endOuterVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay);

        if (outerSide == -1)
        {
            Debug.Log("error");
            return;
        }
        
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        // outer 부터 
        int outerTriangleCount = MeshManager.Instance.mesh.triangles.Length;
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

        _dividingMethods.DivideTrianglesStartIncision(startOuterVertexPosition, outerEdgePoint, startOuterTriangleIndex, outerNotEdgeVertex, outerEdgeIdx, ref outerTriangleCount, false);

        while (true)
        {
            for (int i = 0; i < 3; i++)
                if (edgeList[outerEdgeIdx].vtx1 == edgeList[outerTriangleIndex + i].vtx2 && edgeList[outerEdgeIdx].vtx2 == edgeList[outerTriangleIndex + i].vtx1)
                    outerEdgeIdx = outerTriangleIndex + i;

            if (outerTriangleIndex == endOuterTriangleIndex)
            {
                _dividingMethods.DivideTrianglesEndIncision(endOuterVertexPosition, endOuterTriangleIndex, ref outerTriangleCount, outerEdgeIdx, false);
                break;
            }

            outerSide = IntersectionManager.Instance.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startOuterVertexPosition, endOuterVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay);

            if (outerSide == 2)
                _dividingMethods.DivideTrianglesClockWiseIncision(outerEdgePoint, edgeList[outerEdgeIdx].tri1, ref outerTriangleCount, outerEdgeIdx, false);
            else if (outerSide == 1)
                _dividingMethods.DivideTrianglesCounterClockWiseIncision(outerEdgePoint, edgeList[outerEdgeIdx].tri1, ref outerTriangleCount, outerEdgeIdx, false);
            else
            {
                Debug.Log("error");
                break;
            }
        }
        trianglesCount = outerTriangleCount;
    }

    public void ExecuteDividingDF()
    {
        // 메쉬의 변동이 있으면 mesh update 해야됨.
        Mesh mesh = MeshManager.Instance.mesh;
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;

        Vector3[] vertices = new Vector3[mesh.vertexCount + newVertices.Count];
        int[] triangles = new int[trianglesCount];

        for (int i = 0; i < oldVertices.Length; i++)
            vertices[i] = oldVertices[i];
        for (int i = 0; i < oldTriangles.Length; i++)
            triangles[i] = oldTriangles[i];

        // local 값이 지금 들어가있는 상태에서 
        foreach (var item in newVertices)
            vertices[item.Key] = ObjManager.Instance.objTransform.InverseTransformPoint(item.Value);

        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
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

        //leftSide.Clear();
        //rightSide.Clear();
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
        currentIndex = 0;

        startScreenRay = new Ray();
        endScreenRay = new Ray();

        leftVector = new List<Vector3>();
        rightVector = new List<Vector3>();

        leftSide = new List<List<int>>();
        rightSide = new List<List<int>>();

        leftSideWeight = new List<List<float>>();
        rightSideWeight = new List<List<float>>();

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }
}
