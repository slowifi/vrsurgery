using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MultiMeshIncisionManager : MonoBehaviour
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

    public int firstVertexIndex;
    public int lastVertexIndex;

    private int startOuterTriangleIndex;
    private int endOuterTriangleIndex;

    private int startVertexIndex;
    private int endVertexIndex;
    private int middleIndextest;

    private List<GameObject> leftVectorObject;
    private List<GameObject> rightVectorObject;

    public int currentIndex;
    private int trianglesCount;

    public MultiMeshIncisionManager()
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

    }
    // 메쉬에 변동이 있으면 업데이트 해야함.
    public void ExecuteDividing(int MeshIndex)
    {
        Mesh mesh = MultiMeshManager.Instance.Meshes[MeshIndex];
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;
        Vector3[] vertices = new Vector3[mesh.vertexCount + newVertices.Count];
        int[] triangles = new int[trianglesCount];

        for (int i = 0; i < oldVertices.Length; i++)
            vertices[i] = oldVertices[i];
        for (int i = 0; i < oldTriangles.Length; i++)
            triangles[i] = oldTriangles[i];

        foreach (var item in newVertices)
            vertices[item.Key] = MultiMeshManager.Instance.Transforms[MeshIndex].InverseTransformPoint(item.Value);
        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
    public void GenerateIncisionList(int MeshIndex)
    {
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();

        Vector3[] vertices = MultiMeshManager.Instance.Meshes[MeshIndex].vertices;
        List<Vector3> worldPosition = MultiMeshAdjacencyList.Instance.WorldPositionVertices[MeshIndex];

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

        middleIndextest = leftSide[currentIndex][leftSide[currentIndex].Count / 2];

        zMax += MultiMeshManager.Instance.PivotTransform.lossyScale.z;
        zMin -= MultiMeshManager.Instance.PivotTransform.lossyScale.z;

        List<int> leftItems = BFS.Circle(leftSide[currentIndex][leftSide[currentIndex].Count - 1], startVertexPosition, endVertexPosition, zMin, zMax, currentIndex, firstVertexIndex, lastVertexIndex, MeshIndex);
        List<int> rightItems = BFS.Circle(rightSide[currentIndex][rightSide[currentIndex].Count - 1], startVertexPosition, endVertexPosition, zMin, zMax, currentIndex, firstVertexIndex, lastVertexIndex, MeshIndex);

        if (leftItems.Count > 0 && rightItems.Count > 0)
        {
            foreach (int item in leftItems)
                leftSide[currentIndex].Add(item);
            foreach (int item in rightItems)
                rightSide[currentIndex].Add(item);
        }

        startPointIndices.Add(newTriangles.Values.First());
        endPointIndices.Add(worldPosition.Count - 1);

        Transform objTransform = MultiMeshManager.Instance.PivotTransform;

        // 안쪽, 바깥쪽 판단 필요함.
        Vector2 rightVector = Vector2.zero;
        Vector2 leftVector = Vector2.zero;

        if (worldPosition[endPointIndices[currentIndex]].z - MultiMeshManager.Instance.Transforms[MeshIndex].TransformPoint(MultiMeshManager.Instance.Meshes[MeshIndex].normals[endPointIndices[currentIndex]] + MultiMeshManager.Instance.Meshes[MeshIndex].vertices[endPointIndices[currentIndex]]).z < 0)
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

        leftVectorObject[currentIndex].transform.SetParent(MultiMeshManager.Instance.PivotTransform);
        rightVectorObject[currentIndex].transform.SetParent(MultiMeshManager.Instance.PivotTransform);

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

        MultiMeshManager.Instance.Meshes[MeshIndex].vertices = vertices;
    }
    public void Extending(int incisionIndex, float currentExtendValue, float oldExtendValue, int MeshIndex)
    {
        int count = 0;
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();

        int[] triangles = MultiMeshManager.Instance.Meshes[MeshIndex].triangles;
        Vector3[] vertices = MultiMeshManager.Instance.Meshes[MeshIndex].vertices;

        List<Vector3> worldPosition = MultiMeshAdjacencyList.Instance.WorldPositionVertices[MeshIndex];

        Transform objTransform = MultiMeshManager.Instance.Transforms[MeshIndex];
        Transform pivotTransform = MultiMeshManager.Instance.PivotTransform;

        Vector3 leftVector = leftVectorObject[incisionIndex].transform.position - worldPosition[startPointIndices[incisionIndex]];
        Vector3 rightVector = rightVectorObject[incisionIndex].transform.position - worldPosition[startPointIndices[incisionIndex]];

        foreach (int item in leftSide[incisionIndex])
        {
            Vector3 temp = (currentExtendValue - oldExtendValue) * leftSideWeight[incisionIndex][count] * leftVector; // original
            //Vector3 temp = (0.1f) * leftSideWeight[incisionIndex][count] * leftVector;
            vertices[item] = objTransform.InverseTransformPoint(worldPosition[item] + temp);
            count++;
        }
        count = 0;
        foreach (int item in rightSide[incisionIndex])
        {
            Vector3 temp = (currentExtendValue - oldExtendValue) * rightSideWeight[incisionIndex][count] * rightVector; // original
            //Vector3 temp = (0.1f) * rightSideWeight[incisionIndex][count] * rightVector;
            vertices[item] = objTransform.InverseTransformPoint(worldPosition[item] + temp);
            count++;
        }

        MultiMeshManager.Instance.Meshes[MeshIndex].vertices = vertices;
    }
    public void SetStartVerticesDF(int MeshIndex)
    {
        startScreenRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(MeshIndex);
        startVertexPosition = intersectedValues.IntersectedPosition;
        startOuterTriangleIndex = intersectedValues.TriangleIndex;
    }
    public void SetEndVerticesDF(int MeshIndex)
    {
        endScreenRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(MeshIndex);
        endVertexPosition = intersectedValues.IntersectedPosition;
        endOuterTriangleIndex = intersectedValues.TriangleIndex;
    }
    public void SetDividingListDF(ref bool edgeCheck, int MeshIndex)
    {
        int Side = 0;
        int EdgeIdx = -1;

        Vector3 EdgePoint = Vector3.zero;
        int TriangleIndex = startOuterTriangleIndex;

        leftSide.Add(new List<int>());
        rightSide.Add(new List<int>());

        List<Edge> edgeList = MultiMeshAdjacencyList.Instance.EdgeList[MeshIndex];
        List<Vector3> worldVertices = MultiMeshAdjacencyList.Instance.WorldPositionVertices[MeshIndex];

        startVertexIndex = worldVertices.Count;

        Side = Intersections.MultiMeshTriangleEdgeIntersection(ref EdgeIdx, ref EdgePoint, startVertexPosition, endVertexPosition, ref TriangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices, MeshIndex);

        if (Side == -1)
        {
            ChatManager.Instance.GenerateMessage(" 자를 수 없는 Edge 입니다.");
            edgeCheck = true;
            return;
        }

        int TriangleCount = MultiMeshManager.Instance.Meshes[MeshIndex].triangles.Length;
        int NotEdgeVertex = startOuterTriangleIndex;

        for (int i = 0; i < 3; i++)
        {
            if (edgeList[EdgeIdx].vtx1 == edgeList[NotEdgeVertex + i].vtx2)
            {
                NotEdgeVertex = edgeList[NotEdgeVertex + i].vtx1;
                break;
            }
            else if (edgeList[EdgeIdx].vtx2 == edgeList[NotEdgeVertex + i].vtx1)
            {
                NotEdgeVertex = edgeList[NotEdgeVertex + i].vtx2;
                break;
            }
        }

        DivideTrianglesStartIncision(startVertexPosition, EdgePoint, startOuterTriangleIndex, NotEdgeVertex, EdgeIdx, ref TriangleCount, false, MeshIndex);

        while (true)
        {
            if (TriangleIndex == -1)
            {
                ChatManager.Instance.GenerateMessage(" 자를 수 없는 Edge 입니다.");
                edgeCheck = true;
                Debug.Log("error");
                return;
            }

            for (int i = 0; i < 3; i++)
                if (edgeList[EdgeIdx].vtx1 == edgeList[TriangleIndex + i].vtx2 && edgeList[EdgeIdx].vtx2 == edgeList[TriangleIndex + i].vtx1)
                    EdgeIdx = TriangleIndex + i;

            if (TriangleIndex == endOuterTriangleIndex)
            {
                DivideTrianglesEndIncision(endVertexPosition, endOuterTriangleIndex, ref TriangleCount, EdgeIdx, false, MeshIndex);
                endVertexIndex = startVertexIndex + newVertices.Count - 1;
                break;
            }

            Side = Intersections.MultiMeshTriangleEdgeIntersection(ref EdgeIdx, ref EdgePoint, startVertexPosition, endVertexPosition, ref TriangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices, MeshIndex);

            if (Side == 2)
                DivideTrianglesClockWiseIncision(EdgePoint, edgeList[EdgeIdx].tri1, ref TriangleCount, EdgeIdx, false, MeshIndex);
            else if (Side == 1)
                DivideTrianglesCounterClockWiseIncision(EdgePoint, edgeList[EdgeIdx].tri1, ref TriangleCount, EdgeIdx, false, MeshIndex);
            else
            {
                ChatManager.Instance.GenerateMessage(" 자를 수 없는 Edge 입니다.");
                edgeCheck = true;
                return;
            }
        }

        trianglesCount = TriangleCount;
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
    //incision
    private void DivideTrianglesStartIncision(Vector3 centerPosition, Vector3 newEdgeVertexPosition, int triangleIdx, int vertexIndex, int edgeIdx, ref int triangleCount, bool isInner, int MeshIndex)
    {
        int newTriangleLength = MultiMeshManager.Instance.Meshes[MeshIndex].triangles.Length + newTriangles.Count;
        int newVertexLength = MultiMeshManager.Instance.Meshes[MeshIndex].vertexCount + newVertices.Count;
        int currentIncisionIndex = currentIndex;

        List<Edge> edgeList = MultiMeshAdjacencyList.Instance.EdgeList[MeshIndex];

        if (!isInner)
        {
            firstVertexIndex = newVertexLength;
            leftSide[currentIncisionIndex].Add(newVertexLength + 1);
            rightSide[currentIncisionIndex].Add(newVertexLength + 2);
        }

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

        // 이걸 반환해줘야됨.
        newVertices.Add(newVertexLength++, centerPosition);
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
        newVertices.Add(newVertexLength++, newEdgeVertexPosition);
    }
    private void DivideTrianglesEndIncision(Vector3 centerPosition, int triangleIdx, ref int triangleCount, int edgeIdx, bool isInner, int MeshIndex)
    {
        int newTriangleLength;
        int newVertexLength;
        int notEdgeVertex = -1;

        // 지금 이부분에 문제가 있는데 뭐가 문젠지 모르겠다 어떻게 뚫리게된거지?
        newTriangleLength = MultiMeshManager.Instance.Meshes[MeshIndex].triangles.Length + newTriangles.Count;
        newVertexLength = MultiMeshManager.Instance.Meshes[MeshIndex].vertexCount + newVertices.Count;

        List<Edge> edgeList = MultiMeshAdjacencyList.Instance.EdgeList[MeshIndex];

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

        if (!isInner)
            lastVertexIndex = newVertexLength;

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
    private void DivideTrianglesClockWiseIncision(Vector3 newEdgeVertexPosition, int triangleIdx, ref int triangleCount, int intersectEdgeIdx, bool isInner, int MeshIndex)
    {
        int newTriangleLength = MultiMeshManager.Instance.Meshes[MeshIndex].triangles.Length + newTriangles.Count;
        int newVertexLength = MultiMeshManager.Instance.Meshes[MeshIndex].vertexCount + newVertices.Count;
        int currentIncisionIndex = currentIndex;
        List<Edge> edgeList = MultiMeshAdjacencyList.Instance.EdgeList[MeshIndex];

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
            leftSide[currentIncisionIndex].Add(newVertexLength);
            rightSide[currentIncisionIndex].Add(newVertexLength + 1);
        }

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
    private void DivideTrianglesCounterClockWiseIncision(Vector3 newEdgeVertexPosition, int triangleIdx, ref int triangleCount, int intersectEdgeIdx, bool isInner, int MeshIndex)
    {
        int newTriangleLength = MultiMeshManager.Instance.Meshes[MeshIndex].triangles.Length + newTriangles.Count;
        int newVertexLength = MultiMeshManager.Instance.Meshes[MeshIndex].vertexCount + newVertices.Count;
        int currentIncisionIndex = currentIndex;
        List<Edge> edgeList = MultiMeshAdjacencyList.Instance.EdgeList[MeshIndex];

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
            leftSide[currentIncisionIndex].Add(newVertexLength);
            rightSide[currentIncisionIndex].Add(newVertexLength + 1);
        }

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