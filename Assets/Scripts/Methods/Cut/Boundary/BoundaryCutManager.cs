using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoundaryCutManager : Singleton<BoundaryCutManager>
{
    public Dictionary<int, Vector3> newVertices;
    public Dictionary<int, int> newTriangles;

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
    private int startTriangleIndex;
    private int endTriangleIndex;

    public bool isStartFromVtx;
    public bool isEndToVtx;

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
        // 여기에 라인렌더러 넣는걸
        // 두번 중복되어있음...
        endScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectionManager.Instance.GetIntersectedValues(endScreenRay, ref endVertexPosition, ref endTriangleIndex);
        isEndToVtx = false;
    }

    public void SetDividingList()
    {
        int side = 0;
        int edgeIdx = -1;

        Vector3 edgePoint = Vector3.zero;
        int triangleIndex = startTriangleIndex;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;

        // 100줄따리에서 문제점을 못찾다니 말이나 되는소린가 진짜
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
            // 두개이상 겹칠 수 있구나... 그게 문제가 됨.
            foreach (var item in connectedTriangles[startVertexIndex])
            {
                //item : triangle index
                bool checkIntersection = false;
                bool checkIntersection1 = false;
                bool checkIntersection2 = false;
                int count = 0;
                //여기서 그럼 두개이상 겹치는게 있는지 판단을 해볼까 ?
                if (edgeList[item].vtx1 != startVertexIndex && edgeList[item].vtx2 != startVertexIndex)
                {
                    checkIntersection = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], ref intersectionTemp);
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
                    checkIntersection1 = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item+1].vtx1].x, worldVertices[edgeList[item+1].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item+1].vtx1].x, worldVertices[edgeList[item+1].vtx2].x)))
                    {
                        Debug.Log("ray triangle intersection");
                    }
                    else
                        checkIntersection1 = false;
                    //edgeIdx = item + 1;
                }
                else if (edgeList[item + 2].vtx1 != startVertexIndex && edgeList[item + 2].vtx2 != startVertexIndex)
                {
                    checkIntersection2 = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], ref intersectionTemp);
                    if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[item+2].vtx1].x, worldVertices[edgeList[item+2].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[item+2].vtx1].x, worldVertices[edgeList[item + 2].vtx2].x)))
                    {
                        Debug.Log("ray triangle intersection");
                    }
                    else
                        checkIntersection2 = false;
                    //edgeIdx = item + 2;
                }

                //if (checkIntersection = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], ref intersectionTemp))
                //{
                //    count++;
                //    edgeIdx = item;
                //}
                //if (checkIntersection1 = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], ref intersectionTemp))
                //{
                //    count++;
                //    edgeIdx = item + 1;
                //}
                //if (checkIntersection2 = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], ref intersectionTemp))
                //{
                //    count++;
                //    edgeIdx = item + 2;
                //}

                if (checkIntersection || checkIntersection1 || checkIntersection2)
                {
                    intersectionCount++;
                    if (count >= 2)
                    {
                        Debug.Log("몇개나 겹치는지 보자");
                        Debug.Log(count);
                        Debug.Break();
                    }
                    if(checkIntersection)
                        edgeIdx = item;
                    else if (checkIntersection1)
                        edgeIdx = item + 1;
                    else if (checkIntersection2)
                        edgeIdx = item + 2;
                    edgePoint = intersectionTemp;
                    triangleIndex = item;

                    GameObject v_test = new GameObject();
                    v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    v_test.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    v_test.transform.position = intersectionTemp;

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
                Debug.Break();
        }
        else
            side = IntersectionManager.Instance.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay);

        if (side == -1)
        {
            Debug.Log("Boundary 시작이 잘못되었다.");
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

            _dividingMethods.DivideTrianglesStart(startVertexPosition, edgePoint, startTriangleIndex, notEdgeVertex, edgeIdx, ref triangleCount, false);
        }
        int asdf = 0;
        while (true)
        {
            asdf++;
            if (isStartFromVtx)
                Debug.Log("start from vtx로 들어옴");

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
                    _dividingMethods.DivideTrianglesEnd(endVertexPosition, endTriangleIndex, ref triangleCount, edgeIdx, true);
                break;
            }

            // 여기서 처음 start from vtx로 들어옴에서 

            side = IntersectionManager.Instance.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay);
            // 여기서 계속 side를 정하지 못해서 에러가 생기는데 문제네 문제

            if (side == 0 || side == -1)
                Debug.Break();
            if (side == 2)
                _dividingMethods.DivideTrianglesClockWise(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else if (side == 1)
                _dividingMethods.DivideTrianglesCounterClockWise(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else
            {
                Debug.Break();
                ChatManager.Instance.GenerateMessage("Edge와 라인이 intersect 되지 않았습니다.");
                break;
            }
        }

        endVertexIndex = MeshManager.Instance.mesh.vertexCount + newVertices.Count -1;

    }

    public void ExecuteDividing()
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

    public void BoundaryCutUpdate()
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

        newVertices.Clear();
        newTriangles.Clear();
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
        // screen point도 저장해야됨.
        isStartFromVtx = false;
        startScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        firstScreenRay = startScreenRay;
        IntersectionManager.Instance.GetIntersectedValues(startScreenRay, ref startVertexPosition, ref startTriangleIndex);
        firstVertexPosition = startVertexPosition;
        firstTriangleIndex = startTriangleIndex;
    }

    public void SetEndVerticesDF()
    {
        // 여기에 라인렌더러 넣는걸
        endScreenRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectionManager.Instance.GetIntersectedValues(endScreenRay, ref endVertexPosition, ref endTriangleIndex);
        isEndToVtx = false;
    }

    public void SetDividingListDF()
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
                    checkIntersection = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item].vtx1], worldVertices[edgeList[item].vtx2] - worldVertices[edgeList[item].vtx1], ref intersectionTemp);
                    edgeIdx = item;
                }
                else if (edgeList[item + 1].vtx1 != startVertexIndex && edgeList[item + 1].vtx2 != startVertexIndex)
                {
                    checkIntersection = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 1].vtx1], worldVertices[edgeList[item + 1].vtx2] - worldVertices[edgeList[item + 1].vtx1], ref intersectionTemp);
                    edgeIdx = item + 1;
                }
                else if (edgeList[item + 2].vtx1 != startVertexIndex && edgeList[item + 2].vtx2 != startVertexIndex)
                {
                    checkIntersection = IntersectionManager.Instance.RayTriangleIntersection(screenMiddlePoint, startVertexPosition + startScreenRay.direction * 10, endVertexPosition + endScreenRay.direction * 10, worldVertices[edgeList[item + 2].vtx1], worldVertices[edgeList[item + 2].vtx2] - worldVertices[edgeList[item + 2].vtx1], ref intersectionTemp);
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
            side = IntersectionManager.Instance.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay);

        if (side == -1)
        {
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
            _dividingMethods.DivideTrianglesStart(startVertexPosition, edgePoint, startTriangleIndex, notEdgeVertex, edgeIdx, ref triangleCount, false);
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
                    _dividingMethods.DivideTrianglesEnd(endVertexPosition, endTriangleIndex, ref triangleCount, edgeIdx, true);
                break;
            }

            side = IntersectionManager.Instance.TriangleEdgeIntersection(ref edgeIdx, ref edgePoint, startVertexPosition, endVertexPosition, ref triangleIndex, startScreenRay, endScreenRay);

            if (side == 2)
                _dividingMethods.DivideTrianglesClockWise(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else if (side == 1)
                _dividingMethods.DivideTrianglesCounterClockWise(edgePoint, edgeList[edgeIdx].tri1, ref triangleCount, edgeIdx, false);
            else
            {
                ChatManager.Instance.GenerateMessage("Edge와 라인이 intersect 되지 않았습니다.");
                break;
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

    public void BoundaryCutUpdateDF()
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

        newVertices.Clear();
        newTriangles.Clear();
    }
    // elements가 10만개 넘으면 reinitializing이 효과적이고 밑이면 그냥 clear 쓰는게 이득.
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

        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }

}
