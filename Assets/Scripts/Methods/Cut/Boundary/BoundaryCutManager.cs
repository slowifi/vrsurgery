using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoundaryCutManager : Singleton<BoundaryCutManager>
{
    public Dictionary<int, Vector3> newVertices;
    public Dictionary<int, int> newTriangles;

    public List<int> removeBoundaryVertices;

    public List<Ray> rays;
    public List<Vector3> intersectedPosition;

    public int firstVertexIndex;
    public int startVertexIndex;
    public int endVertexIndex;

    private Ray firstScreenRay;
    private Ray startScreenRay;
    private Ray endScreenRay;

    public Vector3 firstVertexPosition;
    private Vector3 startVertexPosition;
    private Vector3 endVertexPosition;

    private int firstTriangleIndex;
    public int startTriangleIndex;
    private int endTriangleIndex;

    public bool isStartFromVtx;
    public bool isEndToVtx;

    private int edgeVertexIndex;

    private int triangleCount;

    private DivideTriangle _dividingMethods;

    public void ResetIndex()
    {
        isStartFromVtx = true;
        startScreenRay = endScreenRay;
        startVertexIndex = endVertexIndex;
        startTriangleIndex = endTriangleIndex;
        startVertexPosition = endVertexPosition;
    }

    public void SetEndVtxToStartVtx()
    {
        endVertexPosition = firstVertexPosition;
        endTriangleIndex = firstTriangleIndex;
        endVertexIndex = firstVertexIndex;
        endScreenRay = firstScreenRay;
        isEndToVtx = true;
    }

    public void SetEndVtxToStartVtxModified()
    {
        endVertexPosition = firstVertexPosition;
        endTriangleIndex = firstTriangleIndex;
        endVertexIndex = firstVertexIndex;
        endScreenRay = firstScreenRay;
        isEndToVtx = true;
    }

    public void SetStartVertices(Ray startRay, Vector3 vertexPosition, int triangleIndex)
    {
        // screen point도 저장해야됨.
        isStartFromVtx = false;

        startScreenRay = startRay;
        startVertexPosition = vertexPosition;
        startTriangleIndex = triangleIndex;

        firstScreenRay = startScreenRay;
        firstVertexPosition = startVertexPosition;
        firstTriangleIndex = startTriangleIndex;
    }

    public void SetEndVertices()
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        // 여기에 라인렌더러 넣는걸
        // 두번 중복되어있음...
        endScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        Intersections.GetIntersectedValues(endScreenRay, ref endVertexPosition, ref endTriangleIndex, triangles, worldPosition);
        isEndToVtx = false;
    }

    public void SetEndVertices(Ray endRay)
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        // 여기에 라인렌더러 넣는걸
        // 두번 중복되어있음...
        endScreenRay = endRay;
        Intersections.GetIntersectedValues(endScreenRay, ref endVertexPosition, ref endTriangleIndex, triangles, worldPosition);
        isEndToVtx = false;
    }

    public void SetDividingList(ref bool checkBool)
    {
        int side = 0;
        int edgeIdx = -1;
        Vector3 edgePoint = Vector3.zero;
        int triangleIndex = startTriangleIndex;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;

        // start
        // 여기서 이제 edge index랑 찾기를 해야됨.
        int intersectionCount = 0;
        if (isStartFromVtx)
        {
            Vector3 intersectionTemp = Vector3.zero;
            var connectedTriangles = AdjacencyList.Instance.connectedTriangles;
            Vector3 screenMiddlePoint = Vector3.Lerp(startScreenRay.origin, endScreenRay.origin, 0.5f);
            // 찾을때 어떻게 찾는지 보고 그 다음 거의 다 왔음.
            Debug.Log(connectedTriangles[startVertexIndex].Count);

            // 여기 수정 해야됨.
            foreach (var item in connectedTriangles[startVertexIndex])
            {
                //item : triangle index
                bool checkIntersection = false;
                bool checkIntersection1 = false;
                bool checkIntersection2 = false;
                int count = 0;

                if (edgeList[item].vtx1 != startVertexIndex && edgeList[item].vtx2 != startVertexIndex)
                {
                    checkIntersection = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item].vtx1].x, worldVertices[edgeList[item].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item].vtx1].x, worldVertices[edgeList[item].vtx2].x)))
                    {
                        Debug.Log("ray triangle intersection");
                    }
                    else
                        checkIntersection = false;
                    //edgeIdx = item;
                }
                else if (edgeList[item + 1].vtx1 != startVertexIndex && edgeList[item + 1].vtx2 != startVertexIndex)
                {
                    checkIntersection1 = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item + 1].vtx1].x, worldVertices[edgeList[item + 1].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item + 1].vtx1].x, worldVertices[edgeList[item + 1].vtx2].x)))
                    {
                        Debug.Log("ray triangle intersection");
                    }
                    else
                        checkIntersection1 = false;
                    //edgeIdx = item + 1;
                }
                else if (edgeList[item + 2].vtx1 != startVertexIndex && edgeList[item + 2].vtx2 != startVertexIndex)
                {
                    checkIntersection2 = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item + 2].vtx1].x, worldVertices[edgeList[item + 2].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item + 2].vtx1].x, worldVertices[edgeList[item + 2].vtx2].x)))
                    {
                        Debug.Log("ray triangle intersection");
                    }
                    else
                        checkIntersection2 = false;
                    //edgeIdx = item + 2;
                }

                if (checkIntersection || checkIntersection1 || checkIntersection2)
                {
                    intersectionCount++;
                    if (count >= 2)
                    {
                        Debug.Log("몇개나 겹치는지 보자");
                        Debug.Log(count);
                        //Debug.Break();
                    }
                    if (checkIntersection)
                        edgeIdx = item;
                    else if (checkIntersection1)
                        edgeIdx = item + 1;
                    else if (checkIntersection2)
                        edgeIdx = item + 2;
                    edgePoint = intersectionTemp;
                    triangleIndex = item;

                    //GameObject v_test = new GameObject();
                    //v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //v_test.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    //v_test.transform.position = intersectionTemp;

                    //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //v_test.transform.position = MeshManager.Instance.mesh.vertices[MeshManager.Instance.mesh.triangles[item]];

                    //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //v_test.transform.position = MeshManager.Instance.mesh.vertices[MeshManager.Instance.mesh.triangles[item+1]];

                    //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //v_test.transform.position = MeshManager.Instance.mesh.vertices[MeshManager.Instance.mesh.triangles[item+2]];
                    // break;
                }
            }
            Debug.Log(intersectionCount);
            if (intersectionCount >= 2)
            {

                //Debug.Break();
            }
        }
        //Debug.Log(startVertexIndex);
        //if (isStartFromVtx)
        //{
        //    Vector3 intersectionTemp = Vector3.zero;
        //    var connectedTriangles = AdjacencyList.Instance.connectedTriangles;
        //    Vector3 screenMiddlePoint = Vector3.Lerp(startScreenRay.origin, endScreenRay.origin, 0.5f);
        //    Vector3 planeNormal = AlgorithmsManager.Instance.GetPlaneNormal(screenMiddlePoint, startVertexPosition, endVertexPosition);
        //    // 찾을때 어떻게 찾는지 보고 그 다음 거의 다 왔음.
        //    Debug.Log(connectedTriangles[startVertexIndex].Count);
        //    float distance = 1000000;
        //    // 여기 수정 해야됨.
        //    foreach (var item in connectedTriangles[startVertexIndex])
        //    {
        //        //item : triangle index
        //        bool checkIntersection = false;
        //        bool checkIntersection1 = false;
        //        bool checkIntersection2 = false;
        //        int count = 0;
        //        //Vector3 intersectionTemp = Vector3.zero;
        //        if (edgeList[item].vtx1 != startVertexIndex && edgeList[item].vtx2 != startVertexIndex)
        //        {
        //            if (IntersectionManager.Instance.LinePlaneIntersectionModified(ref intersectionTemp, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], planeNormal, startVertexPosition))
        //                checkIntersection = true;
        //            else
        //                checkIntersection = false;
        //        }
        //        else if (edgeList[item + 1].vtx1 != startVertexIndex && edgeList[item + 1].vtx2 != startVertexIndex)
        //        {
        //            if (IntersectionManager.Instance.LinePlaneIntersectionModified(ref intersectionTemp, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], planeNormal, startVertexPosition))
        //                checkIntersection1 = true;
        //            else
        //                checkIntersection1 = false;
        //        }
        //        else if (edgeList[item + 2].vtx1 != startVertexIndex && edgeList[item + 2].vtx2 != startVertexIndex)
        //        {
        //            if (IntersectionManager.Instance.LinePlaneIntersectionModified(ref intersectionTemp, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], planeNormal, startVertexPosition))
        //                checkIntersection2 = true;
        //            else
        //                checkIntersection2 = false;
        //        }

        //        if (checkIntersection || checkIntersection1 || checkIntersection2)
        //        {
        //            intersectionCount++;

        //            float curDistance = Vector2.Distance(intersectionTemp, endVertexPosition);
        //            if (curDistance < distance)
        //            {
        //                distance = curDistance;
        //                if (checkIntersection)
        //                    edgeIdx = item;
        //                else if (checkIntersection1)
        //                    edgeIdx = item + 1;
        //                else if (checkIntersection2)
        //                    edgeIdx = item + 2;
        //                edgePoint = intersectionTemp;
        //                triangleIndex = item;
        //            }
        //            else
        //                continue;

        //            if (count >= 2)
        //            {
        //                Debug.Log("몇개나 겹치는지 보자");
        //                Debug.Log(count);
        //                Debug.Break();
        //            }

        //            GameObject v_test = new GameObject();
        //            v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //            v_test.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //            v_test.transform.position = intersectionTemp;
        //        }
        //    }
        //    Debug.Log(intersectionCount);
        //    //if (intersectionCount >= 2)
        //    //    Debug.Break();
        //}
        else
            side = Intersections.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices);
        // 시작이 문제인거같은데 어뒤지.
        //side = IntersectionManager.Instance.PlaneEdgeIntersectionStart(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay);


        if (side == -1 || edgeIdx == -1)
        {
            ChatManager.Instance.GenerateMessage(" Boundary가 적절하지 않습니다.");
            Debug.Log("Boundary 시작이 잘못되었다.");
            checkBool = true;
            return;
        }

        triangleCount = MeshManager.Instance.mesh.triangles.Length;


        if (isStartFromVtx)
        {
            Debug.Log(startVertexIndex);
            Debug.Log(edgePoint);
            Debug.Log(triangleIndex);
            Debug.Log(edgeIdx);
            _dividingMethods.DivideTrianglesStartFromVtx(startVertexIndex, edgePoint, triangleIndex, edgeIdx, ref triangleCount);
            triangleIndex = edgeList[edgeIdx].tri2;
        }
        else
        {
            int notEdgeVertex = startTriangleIndex;

            for (int i = 0; i < 3; i++)
            {
                if (edgeList[edgeIdx].vtx1 == edgeList[notEdgeVertex + i].vtx2)
                {
                    notEdgeVertex = edgeList[notEdgeVertex + i].vtx1;
                    break;
                }
                else if (edgeList[edgeIdx].vtx2 == edgeList[notEdgeVertex + i].vtx1)
                {
                    notEdgeVertex = edgeList[notEdgeVertex + i].vtx2;
                    break;
                }
            }

            _dividingMethods.DivideTrianglesStartBoundary(startVertexPosition, edgePoint, startTriangleIndex, notEdgeVertex, edgeIdx, ref triangleCount, false);
        }

        if (isEndToVtx)
        {
            //endTriangleIndex의 계산이 필요함.
            Vector3 intersectionTemp = Vector3.zero;
            var connectedTriangles = AdjacencyList.Instance.connectedTriangles;
            Vector3 screenMiddlePoint = Vector3.Lerp(startScreenRay.origin, endScreenRay.origin, 0.5f);
            foreach (var item in connectedTriangles[endVertexIndex])
            {
                //item : triangle index
                bool checkIntersection = false;
                bool checkIntersection1 = false;
                bool checkIntersection2 = false;
                int count = 0;
                //여기서 그럼 두개이상 겹치는게 있는지 판단을 해볼까 ?
                if (edgeList[item].vtx1 != endVertexIndex && edgeList[item].vtx2 != endVertexIndex)
                {
                    checkIntersection = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item].vtx1].x, worldVertices[edgeList[item].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item].vtx1].x, worldVertices[edgeList[item].vtx2].x)))
                        Debug.Log("ray triangle intersection");
                    else
                        checkIntersection = false;
                }
                else if (edgeList[item + 1].vtx1 != endVertexIndex && edgeList[item + 1].vtx2 != endVertexIndex)
                {
                    checkIntersection1 = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item + 1].vtx1].x, worldVertices[edgeList[item + 1].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item + 1].vtx1].x, worldVertices[edgeList[item + 1].vtx2].x)))
                        Debug.Log("ray triangle intersection");
                    else
                        checkIntersection1 = false;
                }
                else if (edgeList[item + 2].vtx1 != endVertexIndex && edgeList[item + 2].vtx2 != endVertexIndex)
                {
                    checkIntersection2 = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item + 2].vtx1].x, worldVertices[edgeList[item + 2].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item + 2].vtx1].x, worldVertices[edgeList[item + 2].vtx2].x)))
                        Debug.Log("ray triangle intersection");
                    else
                        checkIntersection2 = false;
                }

                if (checkIntersection || checkIntersection1 || checkIntersection2)
                {
                    intersectionCount++;
                    endTriangleIndex = item;
                    break;
                }
            }
        }
        int asdf = 0;
        while (true)
        {
            asdf++;
            if (asdf == 2000)
            {
                ChatManager.Instance.GenerateMessage(" Boundary가 너무 깁니다.");
                checkBool = true;
                return;
            }
            else if (triangleIndex == -1)
            {
                ChatManager.Instance.GenerateMessage(" Boundary가 잘못 그려졌습니다.");
                checkBool = true;
                return;
            }

            for (int i = 0; i < 3; i++)
                if (edgeList[edgeIdx].vtx1 == edgeList[triangleIndex + i].vtx2 && edgeList[edgeIdx].vtx2 == edgeList[triangleIndex + i].vtx1)
                {
                    edgeIdx = triangleIndex + i;
                    break;
                }

            if (triangleIndex == endTriangleIndex)
            {
                Debug.Log("end로 들어옴");
                if (isEndToVtx)
                    _dividingMethods.DivideTrianglesEndToVtx(endVertexIndex, endTriangleIndex, ref triangleCount, edgeIdx);
                else
                    _dividingMethods.DivideTrianglesEndBoundary(endVertexPosition, endTriangleIndex, ref triangleCount, edgeIdx, true);
                break;
            }

            // 여기서 처음 start from vtx로 들어옴에서 

            side = Intersections.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices);
            // 여기서 계속 side를 정하지 못해서 에러가 생기는데 문제네 문제

            if (side == 0 || side == -1)
            {
                ChatManager.Instance.GenerateMessage("Edge와 라인이 intersect 되지 않았습니다.");
                checkBool = true;
                //Debug.Break();
            }
            if (side == 2)
                _dividingMethods.DivideTrianglesClockWiseBoundary(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else if (side == 1)
                _dividingMethods.DivideTrianglesCounterClockWiseBoundary(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else
            {
                ChatManager.Instance.GenerateMessage("Edge와 라인이 intersect 되지 않았습니다.");
                checkBool = true;
                return;
            }
        }

        endVertexIndex = MeshManager.Instance.mesh.vertexCount + newVertices.Count - 1;

    }

    public void ExecuteDividing()
    {
        Mesh mesh = MeshManager.Instance.mesh;
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;

        Vector3[] vertices = new Vector3[mesh.vertexCount + newVertices.Count];
        int[] triangles = new int[triangleCount];

        //boundary cut 자동을 위한 vertex index를 저장함.
        edgeVertexIndex = mesh.vertexCount;

        for (int i = 0; i < oldVertices.Length; i++)
            vertices[i] = oldVertices[i];
        for (int i = 0; i < oldTriangles.Length; i++)
            triangles[i] = oldTriangles[i];

        // local 값이 지금 들어가있는 상태에서 
        foreach (var item in newVertices)
            vertices[item.Key] = ObjManager.Instance.objTransform.InverseTransformPoint(item.Value);

        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        newVertices.Clear();
        newTriangles.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    public void BoundaryCutUpdate()
    {
        newVertices.Clear();
        newTriangles.Clear();

        removeBoundaryVertices.Clear();

        rays.Clear();
        intersectedPosition.Clear();

        firstVertexIndex = -1;
        startVertexIndex = -1;
        endVertexIndex = -1;

        firstScreenRay = new Ray();
        startScreenRay = new Ray();
        endScreenRay = new Ray();

        firstVertexPosition = Vector3.zero;
        startVertexPosition = Vector3.zero;
        endVertexPosition = Vector3.zero;

        firstTriangleIndex = -1;
        startTriangleIndex = -1;
        endTriangleIndex = -1;

        isStartFromVtx = false;
        isEndToVtx = false;

        triangleCount = 0;
    }

    public bool AutomaticallyRemoveTriangles()
    {
        //저 버텍스 두개를 index로 갖는 edge로 좌우 버텍스의 
        int firstIndex = removeBoundaryVertices[3];
        int secondIndex = removeBoundaryVertices[4];
        int tri1 = 0, tri2 = 0;
        AdjacencyList.Instance.ListUpdate();
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;
        int[] triangles = MeshManager.Instance.mesh.triangles;

        for (int i = 0; i < edgeList.Count; i += 3)
        {
            if ((edgeList[i].vtx1 == firstIndex && edgeList[i].vtx2 == secondIndex) || (edgeList[i].vtx2 == firstIndex && edgeList[i].vtx1 == secondIndex))
            {
                tri1 = edgeList[i].tri1;
                tri2 = edgeList[i].tri2;
                break;
            }
            else if ((edgeList[i + 1].vtx1 == firstIndex && edgeList[i + 1].vtx2 == secondIndex) || (edgeList[i + 1].vtx2 == firstIndex && edgeList[i + 1].vtx1 == secondIndex))
            {
                tri1 = edgeList[i + 1].tri1;
                tri2 = edgeList[i + 1].tri2;
                break;
            }
            else if ((edgeList[i + 2].vtx1 == firstIndex && edgeList[i + 2].vtx2 == secondIndex) || (edgeList[i + 2].vtx2 == firstIndex && edgeList[i + 2].vtx1 == secondIndex))
            {
                tri1 = edgeList[i + 2].tri1;
                tri2 = edgeList[i + 2].tri2;
                break;
            }
        }

        int bfsVtx1 = 0, bfsVtx2 = 0;
        for (int i = 0; i < 3; i++)
        {
            if (triangles[tri1 + i] != firstIndex && triangles[tri1 + i] != secondIndex)
            {
                bfsVtx1 = triangles[tri1 + i];
                break;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (triangles[tri2 + i] != firstIndex && triangles[tri2 + i] != secondIndex)
            {
                bfsVtx2 = triangles[tri2 + i];
                break;
            }
        }


        Debug.Log(triangles.Length);

        if (!BFS.Instance.BFS_Boundary(bfsVtx1, removeBoundaryVertices))
            if (!BFS.Instance.BFS_Boundary(bfsVtx2, removeBoundaryVertices))
                return false;
        return true;
    }

    public void RemoveBoundaryTriangles()
    {

    }




    public bool PostProcess()
    {
        // 여기에 문제가 있을 가능성이 농후
        bool checkBool = false;
        SetStartVertices(rays[0], intersectedPosition[0], startTriangleIndex);
        for (int i = 1; i < rays.Count; i++)
        {
            if (i == 1)
            {
                SetEndVertices(rays[i]);
                SetDividingList(ref checkBool);
                if (checkBool)
                {
                    MeshManager.Instance.LoadOldMesh();
                    BoundaryCutUpdate();
                    return false;
                }
                ExecuteDividing();
                AdjacencyList.Instance.ListUpdate();
            }
            else
            {
                ResetIndex();
                SetEndVertices(rays[i]);
                SetDividingList(ref checkBool);
                if (checkBool)
                {
                    MeshManager.Instance.LoadOldMesh();
                    BoundaryCutUpdate();
                    return false;
                }
                ExecuteDividing();

                AdjacencyList.Instance.ListUpdate();
            }
        }
        ResetIndex();
        SetEndVtxToStartVtx();
        SetDividingList(ref checkBool);
        if (checkBool)
        {
            MeshManager.Instance.LoadOldMesh();
            BoundaryCutUpdate();
            return false;
        }
        ExecuteDividing();

        return true;
    }

    public void ResetIndexDF()
    {
        isStartFromVtx = true;
        startScreenRay = endScreenRay;
        startVertexIndex = endVertexIndex;
        startTriangleIndex = endTriangleIndex;
        startVertexPosition = endVertexPosition;
    }

    public void SetEndVtxToStartVtxDF()
    {
        endVertexPosition = firstVertexPosition;
        endTriangleIndex = firstTriangleIndex;
        endVertexIndex = firstVertexIndex;
        endScreenRay = firstScreenRay;
        isEndToVtx = true;
    }

    public void SetStartVerticesDF()
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        // screen point도 저장해야됨.
        isStartFromVtx = false;
        startScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        firstScreenRay = startScreenRay;

        Intersections.GetIntersectedValues(endScreenRay, ref endVertexPosition, ref endTriangleIndex, triangles, worldPosition);
        firstVertexPosition = startVertexPosition;
        firstTriangleIndex = startTriangleIndex;
    }

    public void SetEndVerticesDF()
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        // 여기에 라인렌더러 넣는걸
        endScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        Intersections.GetIntersectedValues(endScreenRay, ref endVertexPosition, ref endTriangleIndex, triangles, worldPosition);
        isEndToVtx = false;
    }

    public void SetDividingListDF(ref bool checkEdge)
    {
        int side = 0;
        int edgeIdx = -1;

        Vector3 edgePoint = Vector3.zero;
        int triangleIndex = startTriangleIndex;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;


        // 도대체 문제점이 뭘까 알 수가 없네.

        // start
        // 여기서 이제 edge index랑 찾기를 해야됨.
        if (isStartFromVtx)
        {
            Vector3 intersectionTemp = Vector3.zero;
            var connectedTriangles = AdjacencyList.Instance.connectedTriangles;
            Vector3 screenMiddlePoint = Vector3.Lerp(startScreenRay.origin, endScreenRay.origin, 0.5f);
            Debug.Log(startVertexIndex);
            foreach (var item in connectedTriangles[startVertexIndex])
            {
                //item : triangle index
                bool checkIntersection = false;
                if (edgeList[item].vtx1 != startVertexIndex && edgeList[item].vtx2 != startVertexIndex)
                {
                    checkIntersection = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], ref intersectionTemp);
                    edgeIdx = item;
                }
                else if (edgeList[item + 1].vtx1 != startVertexIndex && edgeList[item + 1].vtx2 != startVertexIndex)
                {
                    checkIntersection = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], ref intersectionTemp);
                    edgeIdx = item + 1;
                }
                else if (edgeList[item + 2].vtx1 != startVertexIndex && edgeList[item + 2].vtx2 != startVertexIndex)
                {
                    checkIntersection = Intersections.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], ref intersectionTemp);
                    edgeIdx = item + 2;
                }

                if (checkIntersection)
                {
                    edgePoint = intersectionTemp;
                    triangleIndex = item;
                    break;
                }
            }
        }
        else
            side = Intersections.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices);

        if (side == -1)
        {
            ChatManager.Instance.GenerateMessage(" Edge가 잘못되었습니다.");
            checkEdge = true;
            Debug.Log("Boundary 시작이 잘못되었다.");
            return;
        }

        triangleCount = MeshManager.Instance.mesh.triangles.Length;

        // 이 부분 만져야됨.
        if (isStartFromVtx)
        {
            Debug.Log(startVertexIndex);
            Debug.Log(edgePoint);
            Debug.Log(triangleIndex);
            Debug.Log(edgeIdx);
            _dividingMethods.DivideTrianglesStartFromVtx(startVertexIndex, edgePoint, triangleIndex, edgeIdx, ref triangleCount);
        }
        else
        {
            int notEdgeVertex = startTriangleIndex;

            for (int i = 0; i < 3; i++)
            {
                if (edgeList[edgeIdx].vtx1 == edgeList[notEdgeVertex + i].vtx2)
                {
                    notEdgeVertex = edgeList[notEdgeVertex + i].vtx1;
                    break;
                }
                else if (edgeList[edgeIdx].vtx2 == edgeList[notEdgeVertex + i].vtx1)
                {
                    notEdgeVertex = edgeList[notEdgeVertex + i].vtx2;
                    break;
                }
            }
            _dividingMethods.DivideTrianglesStartBoundary(startVertexPosition, edgePoint, startTriangleIndex, notEdgeVertex, edgeIdx, ref triangleCount, false);
        }

        while (true)
        {
            for (int i = 0; i < 3; i++)
                if (edgeList[edgeIdx].vtx1 == edgeList[triangleIndex + i].vtx2 && edgeList[edgeIdx].vtx2 == edgeList[triangleIndex + i].vtx1)
                    edgeIdx = triangleIndex + i;

            if (triangleIndex == endTriangleIndex)
            {
                Debug.Log("end로 들어옴");
                if (isEndToVtx)
                    _dividingMethods.DivideTrianglesEndToVtx(endVertexIndex, endTriangleIndex, ref triangleCount, edgeIdx);
                else
                    _dividingMethods.DivideTrianglesEndBoundary(endVertexPosition, endTriangleIndex, ref triangleCount, edgeIdx, true);
                break;
            }
            side = Intersections.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay, edgeList, worldVertices);

            if (side == 2)
                _dividingMethods.DivideTrianglesClockWiseBoundary(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else if (side == 1)
                _dividingMethods.DivideTrianglesCounterClockWiseBoundary(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else
            {
                ChatManager.Instance.GenerateMessage("Edge와 라인이 intersect 되지 않았습니다.");
                checkEdge = true;
                return;
            }
        }

        endVertexIndex = MeshManager.Instance.mesh.vertexCount + newVertices.Count - 1;

    }

    public void ExecuteDividingDF()
    {
        Mesh mesh = MeshManager.Instance.mesh;
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;

        Vector3[] vertices = new Vector3[mesh.vertexCount + newVertices.Count];
        int[] triangles = new int[triangleCount];

        for (int i = 0; i < oldVertices.Length; i++)
            vertices[i] = oldVertices[i];
        for (int i = 0; i < oldTriangles.Length; i++)
            triangles[i] = oldTriangles[i];

        // local 값이 지금 들어가있는 상태에서 
        foreach (var item in newVertices)
            vertices[item.Key] = ObjManager.Instance.objTransform.InverseTransformPoint(item.Value);

        foreach (var item in newTriangles)
            triangles[item.Key] = item.Value;

        newVertices.Clear();
        newTriangles.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    // elements가 10만개 넘으면 reinitializing이 효과적이고 밑이면 그냥 clear 쓰는게 이득.
    public void Reinitialize()
    {
        startVertexPosition = Vector3.zero;
        endVertexPosition = Vector3.zero;

        startTriangleIndex = -1;
        endTriangleIndex = -1;
        triangleCount = 0;

        isStartFromVtx = false;
        isEndToVtx = false;

        startScreenRay = new Ray();
        endScreenRay = new Ray();
        rays = new List<Ray>();
        intersectedPosition = new List<Vector3>();

        removeBoundaryVertices = new List<int>();

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }

    protected override void InitializeChild()
    {
        startVertexPosition = Vector3.zero;
        endVertexPosition = Vector3.zero;

        startTriangleIndex = -1;
        endTriangleIndex = -1;
        triangleCount = 0;

        isStartFromVtx = false;
        isEndToVtx = false;

        startScreenRay = new Ray();
        endScreenRay = new Ray();
        rays = new List<Ray>();
        intersectedPosition = new List<Vector3>();

        removeBoundaryVertices = new List<int>();

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }

}
