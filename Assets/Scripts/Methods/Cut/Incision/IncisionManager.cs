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

        int[] triangles = MeshManager.Instance.triangles;
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;

        int outerTriangleCount = triangles.Length;
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

        // outer 부터 
        _dividingMethods.DivideTrianglesStart(startOuterVertexPosition, outerEdgePoint, startOuterTriangleIndex, outerNotEdgeVertex, outerEdgeIdx, ref outerTriangleCount, false);
        while (true)
        {
            


        }


        


















        // outer edge Idx
        for (int i = 0; i < 3; i++)
            if (triangles[incisionOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx1 && triangles[incisionOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx2)
                _outerVtxIdx = triangles[incisionOuterStartPointIdx + i];

        // inner edge Idx
        for (int i = 0; i < 3; i++)
            if (triangles[incisionInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx1 && triangles[incisionInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx2)
                _innerVtxIdx = triangles[incisionInnerStartPointIdx + i];





















        // EdgeLineIntersection(ref int edgeIdx, ref Vector3 EdgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, int incisionPointIdx) : 반환되는 값은 intersection 된 edge 개수
        // edgeIdx : 겹치는 old edge index
        // EdgePoint : intersection된 point 위치
        // incisionOuterStartPoint, incisionOuterEndPoint : start end point 위치
        if (outerIntersectionCount == 0 || innerIntersectionCount == 0)
        {
            Debug.Log("error 띄워야됨. intersect 되지 않음. ");
        }
        else if (outerIntersectionCount == 1 && innerIntersectionCount == 1)
        {
            // _vtxIdx : intersection된 edge가 아닌 vtx index
            int _outerVtxIdx = -1, _innerVtxIdx = -1;

            // outer
            for (int i = 0; i < 3; i++)
                if (triangles[incisionOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx1 && triangles[incisionOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx2)
                    _outerVtxIdx = triangles[incisionOuterStartPointIdx + i];

            // inner
            for (int i = 0; i < 3; i++)
                if (triangles[incisionInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx1 && triangles[incisionInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx2)
                    _innerVtxIdx = triangles[incisionInnerStartPointIdx + i];

            // start
            // outer
            _dividingMethods.DivideTrianglesStart(incisionOuterStartPoint, outerEdgePoint, incisionOuterStartPointIdx, outerEdgeIdx, _outerVtxIdx, 0);
            // inner
            _dividingMethods.DivideTrianglesStart(incisionInnerStartPoint, innerEdgePoint, incisionInnerStartPointIdx, innerEdgeIdx, _innerVtxIdx, 1);

            int start = 0;
            bool outerEnd = false, innerEnd = false;
            while (true)
            {
                Vector3 _outerNewEdgePoint = Vector3.zero;
                Vector3 _innerNewEdgePoint = Vector3.zero;

                int vtxLength = MeshManager.Instance.vertexCount;

                start++;
                // 여기서 기존의  edge가 아닌 새로운 edge를 찾고나서 그 index를 반환해야됨.
                // 지금 여기서 new triangles edge x line intersection 임.

                outerIntersectionCount = 0;
                innerIntersectionCount = 0;

                int outerTemp = 0;
                int innerTemp = 0;

                // 여기에 end triangle 쪼개는 부분.
                if (edgeList[outerEdgeIdx].tri2 == incisionOuterEndPointIdx && !outerEnd)
                {
                    if (innerEnd)
                        _dividingMethods.DivideTrianglesEnd(incisionOuterEndPoint, incisionOuterEndPointIdx, outerEdgeIdx, outerSide, 0, 0);
                    else
                        _dividingMethods.DivideTrianglesEnd(incisionOuterEndPoint, incisionOuterEndPointIdx, outerEdgeIdx, outerSide, 0, 2);
                    outerEnd = true;
                    outerTemp++;
                }
                else if (!outerEnd)
                    outerIntersectionCount = TriangleEdgeIntersection(ref outerSide, ref outerEdgeIdx, ref _outerNewEdgePoint, incisionOuterStartPoint, incisionOuterEndPoint, edgeList[outerEdgeIdx].tri2);

                if (edgeList[innerEdgeIdx].tri2 == incisionInnerEndPointIdx && !innerEnd)
                {
                    _dividingMethods.DivideTrianglesEnd(incisionInnerEndPoint, incisionInnerEndPointIdx, innerEdgeIdx, innerSide, 1, outerTemp);
                    innerEnd = true;
                    innerTemp++;
                }
                else if (!innerEnd)
                    innerIntersectionCount = TriangleEdgeIntersection(ref innerSide, ref innerEdgeIdx, ref _innerNewEdgePoint, incisionInnerStartPoint, incisionInnerEndPoint, edgeList[innerEdgeIdx].tri2);

                if (start >= 50 || (outerEnd && innerEnd && innerTemp == 0 && outerTemp == 0))
                    break;

                // bfs에 들어갈 vtx 선택을 각각에 대해서 잘 해야되는데 
                if (start == 4)
                {
                    // 지금 이 경우에 하나가 뒤집혀 있는거 같은데
                    outerLeftVtxIdx = edgeList[outerEdgeIdx].vtx1;
                    outerRightVtxIdx = edgeList[outerEdgeIdx].vtx2;
                    innerLeftVtxIdx = edgeList[innerEdgeIdx].vtx1;
                    innerRightVtxIdx = edgeList[innerEdgeIdx].vtx2;
                }


                int numTemp = outerTemp + innerTemp;
                if (outerIntersectionCount == 1)
                {
                    if (start == 1)
                    {
                        // 만약에 inner side가 일찍 end point에 도달한다면? 달라지겠지
                        if (outerSide == 1)
                            _dividingMethods.DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (5 + numTemp), vtxLength - (4 + numTemp), 0);
                        else if (outerSide == 2)
                            _dividingMethods.DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (5 + numTemp), vtxLength - (4 + numTemp), 0);
                    }
                    else
                    {
                        if (outerSide == 1 && innerEnd && innerTemp == 0)
                            _dividingMethods.DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - 2, vtxLength - 1, 0);
                        else if (outerSide == 2 && innerEnd && innerTemp == 0)
                            _dividingMethods.DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - 2, vtxLength - 1, 0);
                        else if (outerSide == 1)
                            _dividingMethods.DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (4 + numTemp), vtxLength - (3 + numTemp), 0);
                        else if (outerSide == 2)
                            _dividingMethods.DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (4 + numTemp), vtxLength - (3 + numTemp), 0);
                    }
                }

                // 여기 위에서 두개가 생성되니까 하지만 그거랑 상관없이 기존의 vtx index는 유지해야되지않나?
                if (innerIntersectionCount == 1)
                {
                    if (innerSide == 1)
                        _dividingMethods.DivideTrianglesClockWise(_innerNewEdgePoint, edgeList[innerEdgeIdx].tri1, innerEdgeIdx, vtxLength - (2 + numTemp), vtxLength - (1 + numTemp), 1);
                    else if (innerSide == 2)
                        _dividingMethods.DivideTrianglesCounterClockWise(_innerNewEdgePoint, edgeList[innerEdgeIdx].tri1, innerEdgeIdx, vtxLength - (2 + numTemp), vtxLength - (1 + numTemp), 1);
                }
            }
            Debug.Log("end the loop");
        }
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

        startScreenRay = new Ray();
        endScreenRay = new Ray();

        leftSide = new List<int>();
        rightSide = new List<int>();
        newVertices = new Dictionary<int, Vector3>();
        newTriangles = new Dictionary<int, int>();

        _dividingMethods = gameObject.AddComponent<DivideTriangle>();
    }
}
