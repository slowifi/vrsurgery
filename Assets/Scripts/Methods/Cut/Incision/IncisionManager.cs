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

    public List<List<Vector3>> fixedLeftSideWorldPos;
    public List<List<Vector3>> fixedRightSideWorldPos;

    public List<List<float>> leftSideWeight;
    public List<List<float>> rightSideWeight;

    private List<Vector3> leftVector;
    private List<Vector3> rightVector;

    private List<int> startPointIndices;
    private List<int> endPointIndices;

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

    private GameObject normalVectorObject;


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

        fixedLeftSideWorldPos.Add(new List<Vector3>());
        fixedRightSideWorldPos.Add(new List<Vector3>());

        BFS.Instance.BFS_Circle(leftSide[currentIndex][leftSide[currentIndex].Count - 1], startOuterVertexPosition, endOuterVertexPosition, true);
        BFS.Instance.BFS_Circle(rightSide[currentIndex][rightSide[currentIndex].Count - 1], startOuterVertexPosition, endOuterVertexPosition, false);

        // 이건 start랑 end point에 대해 index만 가지고 있으면 될 일이긴 함.
        startPointIndices.Add(newTriangles.Values.First());
        endPointIndices.Add(vertices.Length - 1);

        float weightCheck = 1000000;
        
        //normalVectorObject = new GameObject("Normal");
        normalVectorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        normalVectorObject.transform.position = worldPosition[startPointIndices[currentIndex]] + MeshManager.Instance.mesh.normals[vertices.Length - 1];
        normalVectorObject.transform.SetParent(ObjManager.Instance.pivotTransform);
        // 처음에 벡터를 잘 설정해야됨. 그래야지 회전등을 하였을때 잘 할 수 있음...
        //Vector3 normalTemp = Vector3.Cross(worldPosition[endPointIndices[currentIndex]] - worldPosition[startPointIndices[currentIndex]], worldPosition[highWeightIndex] - worldPosition[startPointIndices[currentIndex]]);
        //Vector3 leftVector = Vector3.Cross(worldPosition[endPointIndices[currentIndex]] - worldPosition[startPointIndices[currentIndex]], normalTemp);
        //Vector3 rightVector = Vector3.Cross(worldPosition[startPointIndices[currentIndex]] - worldPosition[endPointIndices[currentIndex]], normalTemp);
        Transform objTransform = ObjManager.Instance.pivotTransform;
        Vector3 leftVector = Vector3.Cross(worldPosition[endPointIndices[currentIndex]] - worldPosition[startPointIndices[currentIndex]], objTransform.TransformPoint(normalVectorObject.transform.position) - worldPosition[startPointIndices[currentIndex]]);
        Vector3 rightVector = Vector3.Cross(worldPosition[startPointIndices[currentIndex]] - worldPosition[endPointIndices[currentIndex]], objTransform.TransformPoint(normalVectorObject.transform.position) - worldPosition[startPointIndices[currentIndex]]);
        Debug.Log(objTransform.TransformPoint(normalVectorObject.transform.position) - worldPosition[startPointIndices[currentIndex]]);


        Vector3 center = Vector3.Lerp(worldPosition[startPointIndices[currentIndex]], worldPosition[endPointIndices[currentIndex]], 0.5f);
        double radius = Vector2.Distance(center, worldPosition[endPointIndices[currentIndex]]);
        foreach (int item in leftSide[currentIndex])
        {
            double t = AlgorithmsManager.Instance.QuadraticEquation(worldPosition[item].x - center.x, worldPosition[item].y - center.y, leftVector.x, leftVector.y, radius);
            float temp = Convert.ToSingle(t);
            // temp 값을 리스트 화 시켜야되는데
            leftSideWeight[currentIndex].Add(temp);
            fixedLeftSideWorldPos[currentIndex].Add(worldPosition[item]);
            //vertices[item] = objTransform.InverseTransformPoint(new Vector3(1f * temp * leftVector.x + worldPosition[item].x, 1f * temp * leftVector.y + worldPosition[item].y, 1f * temp * leftVector.z + worldPosition[item].z));
        }
        foreach (int item in rightSide[currentIndex])
        {
            double t = AlgorithmsManager.Instance.QuadraticEquation(worldPosition[item].x - center.x, worldPosition[item].y - center.y, rightVector.x, rightVector.y, radius);
            float temp = Convert.ToSingle(t);
            // temp 값을 리스트 화 시켜야되는데
            rightSideWeight[currentIndex].Add(temp);
            fixedRightSideWorldPos[currentIndex].Add(worldPosition[item]);
            //vertices[item] = objTransform.InverseTransformPoint(new Vector3(1f * temp * rightVector.x + worldPosition[item].x, 1f * temp * rightVector.y + worldPosition[item].y, 1f * temp * rightVector.z + worldPosition[item].z));
        }
        MeshManager.Instance.mesh.vertices = vertices;
    }

    public void Extending(int incisionIndex, float currentExtendValue, float oldExtendValue)
    {
        AdjacencyList.Instance.WorldPositionUpdate();

        int[] triangles = MeshManager.Instance.mesh.triangles;
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        
        //local한 위치를 가지고 있으면 이게 원활한데 그게 또 아니니까 기존의 값을 가지고 한다는게 어렵네;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        
        
        Transform objTransform = ObjManager.Instance.objTransform;
        Transform pivotTransform = ObjManager.Instance.pivotTransform;
        // 처음에 한번만 하면되는것 다음에 leftside rightside리스트와 숫자가 같은 t를(weight) 저장할 수 있는 리스트를 만들어서 넣어주고 벡터값만 바뀌면 그 값에 따른 변화를 적용시키면됨.
        // 한번만 할 것들.
        //Vector3 normalTemp = Vector3.Cross(worldPosition[endPointIndices[incisionIndex]] - worldPosition[startPointIndices[incisionIndex]], worldPosition[highWeightIndex] - worldPosition[startPointIndices[incisionIndex]]);
        //Vector3 leftVector = Vector3.Cross(worldPosition[endPointIndices[incisionIndex]] - worldPosition[startPointIndices[incisionIndex]], normalTemp);
        //Vector3 rightVector = Vector3.Cross(worldPosition[startPointIndices[incisionIndex]] - worldPosition[endPointIndices[incisionIndex]], normalTemp);
        Vector3 leftVector = Vector3.Cross(worldPosition[endPointIndices[incisionIndex]] - worldPosition[startPointIndices[incisionIndex]], pivotTransform.TransformPoint(normalVectorObject.transform.position) - worldPosition[startPointIndices[incisionIndex]]);
        Vector3 rightVector = Vector3.Cross(worldPosition[startPointIndices[incisionIndex]] - worldPosition[endPointIndices[incisionIndex]], pivotTransform.TransformPoint(normalVectorObject.transform.position) - worldPosition[startPointIndices[incisionIndex]]);


        //이런식으로 고정된 값을 가져다가 붙이면 안되고, 얼만큼 변했는가를 나눠주는식으로?
        int count = 0;
        foreach (int item in leftSide[incisionIndex])
        {
            //Vector3 newWorldPosition = objTransform.TransformPoint(vertices[item]);
            //vertices[item] = objTransform.InverseTransformPoint(new Vector3(extendValue * leftSideWeight[incisionIndex][count] * leftVector[incisionIndex].x + worldPosition[item].x, extendValue * leftSideWeight[incisionIndex][count] * leftVector[incisionIndex].y + worldPosition[item].y, extendValue * leftSideWeight[incisionIndex][count] * leftVector[incisionIndex].z + worldPosition[item].z));

            Vector3 temp = (currentExtendValue - oldExtendValue) * leftSideWeight[incisionIndex][count] * leftVector;
            //worldPosition[item] += temp;
            vertices[item] = objTransform.InverseTransformPoint(worldPosition[item] + temp);
            //vertices[item] = objTransform.InverseTransformPoint(new Vector3(leftVector[incisionIndex].x * weight + worldPosition[item].x, leftVector[incisionIndex].y * weight + worldPosition[item].y, leftVector[incisionIndex].z * weight + worldPosition[item].z));
            count++;
        }
        count = 0;
        foreach (int item in rightSide[incisionIndex])
        {
            //Vector3 newWorldPosition = objTransform.TransformPoint(vertices[item]);
            //vertices[item] = objTransform.InverseTransformPoint(new Vector3(extendValue * rightSideWeight[incisionIndex][count] * rightVector[incisionIndex].x + worldPosition[item].x, extendValue * rightSideWeight[incisionIndex][count] * rightVector[incisionIndex].y + worldPosition[item].y, extendValue * rightSideWeight[incisionIndex][count] * rightVector[incisionIndex].z + worldPosition[item].z));
            //worldPosition[item] /= oldExtendValue;
            //float weight = rightSideWeight[incisionIndex][count] * currentExtendValue;
            //vertices[item] = objTransform.InverseTransformPoint(new Vector3(weight * rightVector.x + worldPosition[item].x, weight * rightVector.y + worldPosition[item].y, weight * rightVector.z + worldPosition[item].z));
            //count++;
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

        startPointIndices = new List<int>();
        endPointIndices = new List<int>();

        leftSide = new List<List<int>>();
        rightSide = new List<List<int>>();

        fixedLeftSideWorldPos = new List<List<Vector3>>();
        fixedRightSideWorldPos = new List<List<Vector3>>();

        leftSideWeight = new List<List<float>>();
        rightSideWeight = new List<List<float>>();

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }
}
