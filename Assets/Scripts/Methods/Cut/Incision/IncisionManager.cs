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

    private List<int> startPointIndices;
    private List<int> endPointIndices;

    private Ray startScreenRay;
    private Ray endScreenRay;

    private Vector3 startVertexPosition;

    private Vector3 endVertexPosition;

    public int firstOuterVertexIndex;
    public int lastOuterVertexIndex;

    private int startOuterTriangleIndex;

    private int endOuterTriangleIndex;

    private int startVertexIndex;
    private int endVertexIndex;

    private List<GameObject> leftVectorObject;
    private List<GameObject> rightVectorObject;


    public int currentIndex;
    private int trianglesCount;

    private DivideTriangle _dividingMethods;

    // 여기서 버텍스 두개 입력 받고 start end points
    // 기존것들은 유지한 상태에서 추가

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
            vertices[item.Key] = MeshManager.Instance.objTransform.InverseTransformPoint(item.Value);

        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    public void GenerateIncisionList()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        //MeshManager.Instance.mesh.vertices = AdjacencyList.Instance.worldPositionVertices.ToArray();
        leftSideWeight.Add(new List<float>());
        rightSideWeight.Add(new List<float>());
        
        float zMin = 1000000;
        float zMax = -1000000;

        for (int i = 0; i < leftSide[currentIndex].Count; i++)
        {
            float tempZ = worldPosition[leftSide[currentIndex][i]].z;
            if (tempZ < zMin)
                zMin = tempZ;
            if (tempZ > zMax)
                zMax = tempZ;
        }
        
        zMax += MeshManager.Instance.pivotTransform.lossyScale.z;
        zMin -= MeshManager.Instance.pivotTransform.lossyScale.z;
        //Debug.Log(startVertexIndex);
        //Debug.Log(endVertexIndex);
        //Debug.Log(leftSide[currentIndex][leftSide[currentIndex].Count - 1]);
        //Debug.Log(rightSide[currentIndex][rightSide[currentIndex].Count - 1]);

        //int[] leftSideTemptest = CGAL.ExtractCircleByBFS_Test(startVertexIndex, endVertexIndex, leftSide[currentIndex][leftSide[currentIndex].Count - 3]);
        //int[] rightSideTemptest = CGAL.ExtractCircleByBFS_Test(startVertexIndex, endVertexIndex, rightSide[currentIndex][rightSide[currentIndex].Count - 3]);

        //foreach (var item in rightSideTemptest)
        //{
        //    GameObject v_test1 = new GameObject();
        //    v_test1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    v_test1.transform.position = worldPosition[item];
        //}

        //foreach (var item in leftSideTemptest)
        //{
        //    GameObject v_test1 = new GameObject();
        //    v_test1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //    v_test1.transform.position = worldPosition[item];
        //}

        //int[] leftSideTemp = CGAL.ExtractCircleByBFS(startVertexIndex, endVertexIndex, leftSide[currentIndex][leftSide[currentIndex].Count - 1]);
        //int[] rightSideTemp = CGAL.ExtractCircleByBFS(startVertexIndex, endVertexIndex, rightSide[currentIndex][rightSide[currentIndex].Count - 1]);


        //leftSide[currentIndex].Clear();
        //rightSide[currentIndex].Clear();

        //leftSide[currentIndex] = leftSideTemp.ToList();
        //rightSide[currentIndex] = rightSideTemp.ToList();

        BFS.Circle(leftSide[currentIndex][leftSide[currentIndex].Count - 1], startVertexPosition, endVertexPosition, true, zMin, zMax);
        BFS.Circle(rightSide[currentIndex][rightSide[currentIndex].Count - 1], startVertexPosition, endVertexPosition, false, zMin, zMax);

        startPointIndices.Add(newTriangles.Values.First());
        endPointIndices.Add(vertices.Length - 1);

        //Vector3 normalVector = worldPosition[startPointIndices[currentIndex]] + MeshManager.Instance.mesh.normals[newTriangles.Values.First()];

        Transform objTransform = MeshManager.Instance.pivotTransform;

        //여기에다가 안쪽면인지 바깥면인지를 판단 필요.
        Vector2 rightVector = Vector2.zero;
        Vector2 leftVector = Vector2.zero;

        if (worldPosition[endPointIndices[currentIndex]].z - MeshManager.Instance.objTransform.TransformPoint(MeshManager.Instance.mesh.normals[endPointIndices[currentIndex]] + MeshManager.Instance.mesh.vertices[endPointIndices[currentIndex]]).z < 0)
        {
            rightVector = Vector2.Perpendicular(worldPosition[endPointIndices[currentIndex]] - worldPosition[startPointIndices[currentIndex]]);
            leftVector = Vector2.Perpendicular(worldPosition[startPointIndices[currentIndex]] - worldPosition[endPointIndices[currentIndex]]);
        }
        else
        {
            leftVector = Vector2.Perpendicular(worldPosition[endPointIndices[currentIndex]] - worldPosition[startPointIndices[currentIndex]]);
            rightVector = Vector2.Perpendicular(worldPosition[startPointIndices[currentIndex]] - worldPosition[endPointIndices[currentIndex]]);
        }


        leftVectorObject.Add(new GameObject("Left Vector" + currentIndex + 1));
        rightVectorObject.Add(new GameObject("Right Vector" + currentIndex + 1));

        leftVectorObject[currentIndex].transform.position = new Vector3(leftVector.x + worldPosition[startPointIndices[currentIndex]].x, leftVector.y + worldPosition[startPointIndices[currentIndex]].y, worldPosition[startPointIndices[currentIndex]].z);
        rightVectorObject[currentIndex].transform.position = new Vector3(rightVector.x + worldPosition[startPointIndices[currentIndex]].x, rightVector.y + worldPosition[startPointIndices[currentIndex]].y, worldPosition[startPointIndices[currentIndex]].z);

        leftVectorObject[currentIndex].transform.SetParent(MeshManager.Instance.pivotTransform);
        rightVectorObject[currentIndex].transform.SetParent(MeshManager.Instance.pivotTransform);

        Vector3 center = Vector3.Lerp(worldPosition[startPointIndices[currentIndex]], worldPosition[endPointIndices[currentIndex]], 0.5f);
        double radius = Vector2.Distance(center, worldPosition[endPointIndices[currentIndex]]);
        foreach (int item in leftSide[currentIndex])
        {
            double t = Algorithms.QuadraticEquation(worldPosition[item].x - center.x, worldPosition[item].y - center.y, leftVector.x, leftVector.y, radius);
            float temp = Convert.ToSingle(t);
            leftSideWeight[currentIndex].Add(temp);
        }
        foreach (int item in rightSide[currentIndex])
        {
            double t = Algorithms.QuadraticEquation(worldPosition[item].x - center.x, worldPosition[item].y - center.y, rightVector.x, rightVector.y, radius);
            float temp = Convert.ToSingle(t);
            rightSideWeight[currentIndex].Add(temp);
        }
        MeshManager.Instance.mesh.vertices = vertices;
    }

    public void Extending(int incisionIndex, float currentExtendValue, float oldExtendValue)
    {
        AdjacencyList.Instance.WorldPositionUpdate();

        int[] triangles = MeshManager.Instance.mesh.triangles;
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;

        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        Transform objTransform = MeshManager.Instance.objTransform;
        Transform pivotTransform = MeshManager.Instance.pivotTransform;

        Vector3 leftVector = leftVectorObject[incisionIndex].transform.position - worldPosition[startPointIndices[incisionIndex]];
        Vector3 rightVector = rightVectorObject[incisionIndex].transform.position - worldPosition[startPointIndices[incisionIndex]];
        int count = 0;
        foreach (int item in leftSide[incisionIndex])
        {
            Vector3 temp = (currentExtendValue - oldExtendValue) * leftSideWeight[incisionIndex][count] * leftVector;
            vertices[item] = objTransform.InverseTransformPoint(worldPosition[item] + temp);
            count++;
        }
        count = 0;
        foreach (int item in rightSide[incisionIndex])
        {
            Vector3 temp = (currentExtendValue - oldExtendValue) * rightSideWeight[incisionIndex][count] * rightVector;
            vertices[item] = objTransform.InverseTransformPoint(worldPosition[item] + temp);
            count++;
        }
        MeshManager.Instance.mesh.vertices = vertices;
    }

    // 양면메쉬 전용 알고리즘
    public void SetStartVerticesDF()
    {
        startScreenRay = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);

        IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
        startVertexPosition = intersectedValues.IntersectedPosition;
        startOuterTriangleIndex = intersectedValues.TriangleIndex;
    }

    public void SetEndVerticesDF()
    {
        endScreenRay = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);

        IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
        endVertexPosition = intersectedValues.IntersectedPosition;
        endOuterTriangleIndex = intersectedValues.TriangleIndex;
    }

    public void SetDividingListDF(ref bool edgeCheck)
    {
        int outerSide = 0;
        int outerEdgeIdx = -1;

        Vector3 outerEdgePoint = Vector3.zero;
        int outerTriangleIndex = startOuterTriangleIndex;

        leftSide.Add(new List<int>());
        rightSide.Add(new List<int>());

        List<Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;
        startVertexIndex = worldVertices.Count;
        // start
        outerSide = Intersections.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startVertexPosition, endVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices);

        if (outerSide == -1)
        {
            ChatManager.Instance.GenerateMessage(" 자를 수 없는 Edge 입니다.");
            edgeCheck = true;
            Debug.Log("error");
            return;
        }


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

        _dividingMethods.DivideTrianglesStartIncision(startVertexPosition, outerEdgePoint, startOuterTriangleIndex, outerNotEdgeVertex, outerEdgeIdx, ref outerTriangleCount, false);

        while (true)
        {
            if (outerTriangleIndex == -1)
            {
                ChatManager.Instance.GenerateMessage(" 자를 수 없는 Edge 입니다.");
                edgeCheck = true;
                Debug.Log("error");
                return;
            }
            for (int i = 0; i < 3; i++)
                if (edgeList[outerEdgeIdx].vtx1 == edgeList[outerTriangleIndex + i].vtx2 && edgeList[outerEdgeIdx].vtx2 == edgeList[outerTriangleIndex + i].vtx1)
                    outerEdgeIdx = outerTriangleIndex + i;

            if (outerTriangleIndex == endOuterTriangleIndex)
            {
                _dividingMethods.DivideTrianglesEndIncision(endVertexPosition, endOuterTriangleIndex, ref outerTriangleCount, outerEdgeIdx, false);
                endVertexIndex = startVertexIndex + newVertices.Count - 1;
                break;
            }

            outerSide = Intersections.TriangleEdgeIntersection(ref outerEdgeIdx, ref outerEdgePoint, startVertexPosition, endVertexPosition, ref outerTriangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices);

            if (outerSide == 2)
                _dividingMethods.DivideTrianglesClockWiseIncision(outerEdgePoint, edgeList[outerEdgeIdx].tri1, ref outerTriangleCount, outerEdgeIdx, false);
            else if (outerSide == 1)
                _dividingMethods.DivideTrianglesCounterClockWiseIncision(outerEdgePoint, edgeList[outerEdgeIdx].tri1, ref outerTriangleCount, outerEdgeIdx, false);
            else
            {
                ChatManager.Instance.GenerateMessage(" 자를 수 없는 Edge 입니다.");
                edgeCheck = true;
                Debug.Log("error");
                return;
            }
        }
        trianglesCount = outerTriangleCount;
    }

    public void IncisionUpdate()
    {
        startVertexPosition = Vector3.zero;

        endVertexPosition = Vector3.zero;

        startVertexIndex = -1;
        endVertexIndex = -1;

        startOuterTriangleIndex = -1;

        endOuterTriangleIndex = -1;

        trianglesCount = 0;

        startScreenRay = new Ray();
        endScreenRay = new Ray();

        newVertices.Clear();
        newTriangles.Clear();
    }

    public void Reinitialize()
    {
        startVertexPosition = Vector3.zero;
        endVertexPosition = Vector3.zero;

        startOuterTriangleIndex = -1;
        endOuterTriangleIndex = -1;
        trianglesCount = 0;
        currentIndex = 0;
        startVertexIndex = -1;
        endVertexIndex = -1;
        startScreenRay = new Ray();
        endScreenRay = new Ray();

        startPointIndices = new List<int>();
        endPointIndices = new List<int>();

        leftSide = new List<List<int>>();
        rightSide = new List<List<int>>();

        leftVectorObject = new List<GameObject>();
        rightVectorObject = new List<GameObject>();

        leftSideWeight = new List<List<float>>();
        rightSideWeight = new List<List<float>>();

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = new DivideTriangle();
    }

    // elements가 10만개 넘으면 reinitializing이 효과적이고 밑이면 그냥 clear 쓰는게 이득.
    protected override void InitializeChild()
    {
        startVertexPosition = Vector3.zero;
        //startInnerVertexPosition = Vector3.zero;
        endVertexPosition = Vector3.zero;
        //endInnerVertexPosition = Vector3.zero;

        startOuterTriangleIndex = -1;
        //startInnerTriangleIndex = -1;
        endOuterTriangleIndex = -1;
        //endInnerTriangleIndex = -1;
        trianglesCount = 0;
        currentIndex = 0;

        startVertexIndex = -1;
        endVertexIndex = -1;

        startScreenRay = new Ray();
        endScreenRay = new Ray();

        startPointIndices = new List<int>();
        endPointIndices = new List<int>();

        leftSide = new List<List<int>>();
        rightSide = new List<List<int>>();

        leftVectorObject = new List<GameObject>();
        rightVectorObject = new List<GameObject>();

        leftSideWeight = new List<List<float>>();
        rightSideWeight = new List<List<float>>();

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = new DivideTriangle();
    }
}
