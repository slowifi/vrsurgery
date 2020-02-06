﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RayIntersection : MonoBehaviour
{

    public struct Edge
    {
        public int vtx1;// { get; set; }
        public int vtx2;// { get; set; }
        public int tri1;// { get; set; }
        public int tri2;// { get; set; }

        public Edge(int v1, int v2, int t1, int t2)
        {
            vtx1 = v1;
            vtx2 = v2;
            tri1 = t1;
            tri2 = t2;
        }
    }

    private Mesh newMesh;

    private int bef_outervtx1;
    private int bef_outervtx2;
    private int bef_outervtx3;

    private int bef_innervtx1;
    private int bef_innervtx2;
    private int bef_innervtx3;

    private bool first;

    private List<int> inner_ShortestPath;
    private List<int> outer_ShortestPath;


    public Scrollbar m_cuttingSizeBar;
    public Scrollbar m_bar;
    public Scrollbar m_Posbar;
    public Scrollbar m_Extend;

    public float patchWeight = 20.0f;
    private Vector3 avgNorm;
    private Vector3 weightCenterPos;
    private bool reCalculate;
    private int patchNumber;
    private int sphereNumber;
    private int distanceNumber;
    private float heartScale;
    public Text m_distance;
    public GameObject sp1;
    public GameObject sp2;


    private bool rayincision;
    private bool incision;

    private bool firstInit;
    // cut
    public int cutNum;
       
    public Dictionary<int, HashSet<int>> connectedVertices;
    public Dictionary<int, HashSet<int>> connectedTriangles;

    public List<Edge> edgeList;

    public List<int> boundaryVertices;
    public HashSet<int> hashBoundaryVertices;

    public List<int> innerBoundaryVertices;
    public HashSet<int> innerHashBoundaryVertices;

    public List<int> removedOuterTriangles;
    public List<int> removedInnerTriangles;

    public List<int> innerBoundaryTriangles;
    public List<int> outerBoundaryTriangles;

    public List<int> wholeTriangles;
    public HashSet<int> wholeTriangleIndexSet;

    public HashSet<int> innerBoundaryTriangleIndexSet;
    public HashSet<int> outerBoundaryTriangleIndexSet;

    public LinkedList<int> ll_innerContourVertices;
    public LinkedList<int> ll_outerContourVertices;

    // patch
    public float oldValue_curve;
    public float oldValue_height;
    public Vector3 patchCenterPos;
    public List<Vector3> patchVertices;
    List<Vector3>[] insidePatchVertices;

    public GameObject obj;

    private Camera cam;

    private Vector3[] test;
    private Vector3[] test_world;

    private Vector3 p1_vec;

    private bool cutmode;
    private bool check_function;
    private Color[] colors;

    // incision

    private int outerLeftVtxIdx;
    private int outerRightVtxIdx;
    private int innerLeftVtxIdx;
    private int innerRightVtxIdx;

    private bool _onlyOnce;
    private float oldExtendValue;
    private bool extending;
    
    public HashSet<int> leftSide;
    public HashSet<int> rightSide;

    // incision cut
    private Vector3 screenStartOrigin;
    private Vector3 screenEndOrigin;

    private Vector3 screenStartDirection;
    private Vector3 screenEndDirection;

    private Vector3 incisionOuterStartPoint;
    private Vector3 incisionOuterEndPoint;

    private int incisionOuterStartPointIdx;
    private int incisionOuterEndPointIdx;

    private Vector3 incisionInnerStartPoint;
    private Vector3 incisionInnerEndPoint;

    private int incisionInnerStartPointIdx;
    private int incisionInnerEndPointIdx;

    // boundary cut
    private Vector3 boundaryOuterFirstPoint;
    private Vector3 boundaryInnerFirstPoint;

    private Vector3 boundaryOuterStartPoint;
    private Vector3 boundaryOuterEndPoint;

    private Vector3 boundaryInnerStartPoint;
    private Vector3 boundaryInnerEndPoint;

    private int boundaryOuterEndVtx;
    private int boundaryInnerEndVtx;

    private int boundaryOuterStartPointIdx;
    private int boundaryOuterEndPointIdx;

    private int boundaryInnerStartPointIdx;
    private int boundaryInnerEndPointIdx;
    private bool firstBoundary;
    private bool boundaryCut;
    private bool firstRay;

    private Vector3 intersectionTemp;
    private Vector3 intersectionPoint;
    private Vector3 outerIntersectionPoint;
    private Vector3 innerIntersectionPoint;
    private Vector3 OldIntersectionPoint;

    private int patchedCount;
    private bool measured_check;
    private bool cutting;
    private bool measuring;
    private bool patching;

    private void Start()
    {
        // CombineMeshes();
        patchNumber = 0;
        measured_check = false;
        firstInit = true;
        cutting = false;
        measuring = false;
        patching = false;
        cutmode = false;
        rayincision = false;
        incision = false;
        boundaryCut = false;
        firstBoundary = true;
        firstRay = true;

        cam = GetComponent<Camera>();
        check_function = true;
        Mesh_Initialize();
        
        ConnectedVertices();
        ConnectedTriangles();
        
        StartCoroutine(CreateVerticesToObj());
        newMesh = obj.GetComponent<MeshFilter>().mesh;
        SetColor();
    }

    private void Connected_Initialize()
    {
        connectedTriangles = new Dictionary<int, HashSet<int>>();
        connectedVertices = new Dictionary<int, HashSet<int>>();
        return;
    }

    private void Mesh_Initialize()
    {
        first = true;
        patchedCount = 0;
        cutNum = 3;
        _onlyOnce = false;
        hashBoundaryVertices = new HashSet<int>();
        innerHashBoundaryVertices = new HashSet<int>();

        // cut
        wholeTriangleIndexSet = new HashSet<int>();
        wholeTriangles = new List<int>();
        innerBoundaryTriangleIndexSet = new HashSet<int>();
        outerBoundaryTriangleIndexSet = new HashSet<int>();

        innerBoundaryTriangles = new List<int>();
        outerBoundaryTriangles = new List<int>();

        intersectionTemp = Vector3.zero;
        intersectionPoint = Vector3.zero;
        outerIntersectionPoint = Vector3.zero;
        innerIntersectionPoint = Vector3.zero;
        OldIntersectionPoint = Vector3.zero;
        patchCenterPos = Vector3.zero;

        inner_ShortestPath = new List<int>();
        outer_ShortestPath = new List<int>();

        // incision
        extending = false;
        leftSide = new HashSet<int>();
        rightSide = new HashSet<int>();

        // boundary cut
        boundaryCut = false;
        firstBoundary = true;

        // patchVertices.Clear();
        patchVertices = new List<Vector3>();
        insidePatchVertices = new List<Vector3>[5];

        removedInnerTriangles = new List<int>();
        removedOuterTriangles = new List<int>();

        ll_innerContourVertices = new LinkedList<int>();
        ll_outerContourVertices = new LinkedList<int>();

        connectedVertices = new Dictionary<int, HashSet<int>>();
        connectedTriangles = new Dictionary<int, HashSet<int>>();
        edgeList = new List<Edge>();

        boundaryVertices = new List<int>();
        innerBoundaryVertices = new List<int>();

    }

    private void GenerateEdgeList()
    {
        // 일단 edge(vtx1, vtx2, tri1, ) 까지는 채워졌고, 나머지 tri2의 값을 각각 찾아서 채워줘야됨.
        int v_length = obj.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
        int t_length = obj.GetComponent<MeshFilter>().sharedMesh.triangles.Length;
        int[] m_triangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;

        for (int i = 0; i < v_length; i++)
        {
            foreach (int item in connectedTriangles[i])
            {
                foreach (int insideItem in connectedTriangles[i])
                {
                    if (edgeList[item].vtx1 == edgeList[insideItem].vtx2 && edgeList[item].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item] = new Edge(edgeList[item].vtx1, edgeList[item].vtx2, edgeList[item].tri1, insideItem);
                        edgeList[insideItem] = new Edge(edgeList[insideItem].vtx1, edgeList[insideItem].vtx2, edgeList[insideItem].tri1, item);
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item+1] = new Edge(edgeList[item + 1].vtx1, edgeList[item + 1].vtx2, edgeList[item + 1].tri1, insideItem);
                        edgeList[insideItem] = new Edge(edgeList[insideItem].vtx1, edgeList[insideItem].vtx2, edgeList[insideItem].tri1, item);
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem].vtx1)
                    {
                        edgeList[item+2] = new Edge(edgeList[item + 2].vtx1, edgeList[item + 2].vtx2, edgeList[item + 2].tri1, insideItem);
                        edgeList[insideItem] = new Edge(edgeList[insideItem].vtx1, edgeList[insideItem].vtx2, edgeList[insideItem].tri1, item);
                    }

                    else if (edgeList[item].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item] = new Edge(edgeList[item].vtx1, edgeList[item].vtx2, edgeList[item].tri1, insideItem);
                        edgeList[insideItem+1] = new Edge(edgeList[insideItem + 1].vtx1, edgeList[insideItem + 1].vtx2, edgeList[insideItem + 1].tri1, item);
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item + 1] = new Edge(edgeList[item + 1].vtx1, edgeList[item + 1].vtx2, edgeList[item + 1].tri1, insideItem);
                        edgeList[insideItem + 1] = new Edge(edgeList[insideItem + 1].vtx1, edgeList[insideItem + 1].vtx2, edgeList[insideItem + 1].tri1, item);
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem + 1].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 1].vtx1)
                    {
                        edgeList[item + 2] = new Edge(edgeList[item + 2].vtx1, edgeList[item + 2].vtx2, edgeList[item + 2].tri1, insideItem);
                        edgeList[insideItem + 1] = new Edge(edgeList[insideItem + 1].vtx1, edgeList[insideItem + 1].vtx2, edgeList[insideItem + 1].tri1, item);
                    }

                    else if (edgeList[item].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item] = new Edge(edgeList[item].vtx1, edgeList[item].vtx2, edgeList[item].tri1, insideItem);
                        edgeList[insideItem+2] = new Edge(edgeList[insideItem + 2].vtx1, edgeList[insideItem + 2].vtx2, edgeList[insideItem + 2].tri1, item);
                    }
                    else if (edgeList[item + 1].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 1].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item + 1] = new Edge(edgeList[item + 1].vtx1, edgeList[item + 1].vtx2, edgeList[item + 1].tri1, insideItem);
                        edgeList[insideItem + 2] = new Edge(edgeList[insideItem + 2].vtx1, edgeList[insideItem + 2].vtx2, edgeList[insideItem + 2].tri1, item);
                    }
                    else if (edgeList[item + 2].vtx1 == edgeList[insideItem + 2].vtx2 && edgeList[item + 2].vtx2 == edgeList[insideItem + 2].vtx1)
                    {
                        edgeList[item + 2] = new Edge(edgeList[item + 2].vtx1, edgeList[item + 2].vtx2, edgeList[item + 2].tri1, insideItem);
                        edgeList[insideItem + 2] = new Edge(edgeList[insideItem + 2].vtx1, edgeList[insideItem + 2].vtx2, edgeList[insideItem + 2].tri1, item);
                    }
                }
            }
        }
    }

    public void BoundaryCuttingOn()
    {

    }

    public void CuttingOn()
    {
        cutting = true;
        measuring = false;
        patching = false;
        reCalculate = false;
        measured_check = false;
        Destroy(GameObject.Find("/p1"));
        Destroy(GameObject.Find("distance"));
        MeshRecalculate();
        Mesh_Initialize();
        ConnectedVertices();
        ConnectedTriangles();
        GenerateEdgeList();
        sp1.active = false;
        sp2.active = false;
    }

    public void PatchingOn()
    {
        cutting = false;
        measuring = false;
        patching = true;
        reCalculate = false;
        measured_check = false;
        patchNumber++;
        MeshRecalculate();
        Mesh_Initialize();
        Destroy(GameObject.Find("/p1"));
        Destroy(GameObject.Find("distance"));
        sp1.active = false;
        sp2.active = false;
    }
    
    public void MeasuringOn()
    {
        cutting = false;
        measuring = true;
        patching = false;
        reCalculate = false;
        sp1.active = false;
        sp2.active = false;
        Destroy(GameObject.Find("/p1"));
        Destroy(GameObject.Find("distance"));
        Mesh_Initialize();
        MeshRecalculate();
    }
    
    public void Initializing()
    {
        cutting = false;
        measuring = false;
        patching = false;
        reCalculate = false;
        measured_check = false;
        rayincision = false;
        extending = false;
        Mesh_Initialize();
        Destroy(GameObject.Find("/p1"));
        Destroy(GameObject.Find("distance"));
        sp1.active = false;
        sp2.active = false;
        Destroy(GameObject.Find("Incision line"));
    }

    private void Incisioning()
    {
        cutting = false;
        measuring = false;
        patching = false;
        reCalculate = false;
        measured_check = false;
        Destroy(GameObject.Find("/p1"));
        Destroy(GameObject.Find("distance"));
        sp1.active = false;
        sp2.active = false;
        ConnectedVertices();
        ConnectedTriangles();
        GenerateEdgeList();
        // edgeTest();
        rayincision = true;
        MeshRecalculate();
    }

    public void RendererOverlapping()
    {
        MeshRenderer ren = GameObject.Find("Patch" + patchNumber).GetComponent<MeshRenderer>();
        ren.material.color = new Color32(115, 0, 0, 255);
        // Material yourMaterial = (Material)Resources.Load("2019_Heart", typeof(Material));
        // ren.material = yourMaterial;
    }


    public void Update()
    {
        if(firstInit)
        {
            firstInit = false;
            MeshRecalculate();
            Mesh_Initialize();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Cutting");
            CuttingOn();
            // boundaryVertices.Clear();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Measuring");
            MeasuringOn();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Patching");
            PatchingOn();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SetColor();
            MeshRecalculate();
        }


        if (check_function)
        {
            // update vertices
            MeshRecalculate();
            check_function = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // testInsideSet();
            // RendererOverlapping();
            // MeshRecalculate();
            // SetColor();
            ConnectedVertices();
            ConnectedTriangles();
            GenerateEdgeList();
            
            rayincision = true;
            MeshRecalculate();
        }

        if (extending)
        {
            if (oldExtendValue == m_Extend.value)
                return;
            else
                oldExtendValue = m_Extend.value;

            if (!_onlyOnce)
            {
                _onlyOnce = true;
                BFS_Circle(outerLeftVtxIdx, incisionOuterStartPoint, incisionOuterEndPoint, true);
                BFS_Circle(outerRightVtxIdx, incisionOuterStartPoint, incisionOuterEndPoint, false);
                BFS_Circle(innerLeftVtxIdx, incisionOuterStartPoint, incisionOuterEndPoint, false);
                BFS_Circle(innerRightVtxIdx, incisionOuterStartPoint, incisionOuterEndPoint, true);
            }
            // inner부분도 리스트에 넣어줘야됨.

            Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            
            Vector3 outerCenter = Vector3.Lerp(incisionOuterStartPoint, incisionOuterEndPoint, 0.5f);
            // Vector2 innerCenter = Vector3.Lerp(incisionInnerStartPoint, incisionInnerEndPoint, 0.5f);

            double radius = Vector2.Distance(incisionOuterStartPoint, incisionOuterEndPoint) / 2;

            Vector2 leftVec = Vector2.Perpendicular(incisionOuterEndPoint - incisionOuterStartPoint);
            Vector2 rightVec = Vector2.Perpendicular(incisionOuterStartPoint - incisionOuterEndPoint);

            // 그때그때 계산하지말고 이건 한번만 계산해도 되는거
            foreach (int item in leftSide)
            {
                double t = QuadraticEquation(test_world[item].x - outerCenter.x, test_world[item].y - outerCenter.y, leftVec.x, leftVec.y, radius);
                float temp = Convert.ToSingle(t);
                vertices[item] = obj.transform.InverseTransformPoint(new Vector3(m_Extend.value * temp * leftVec.x + test_world[item].x, m_Extend.value * temp * leftVec.y+ test_world[item].y, test_world[item].z));
            }
              
            foreach (int item in rightSide)
            {
                double t = QuadraticEquation(test_world[item].x - outerCenter.x, test_world[item].y - outerCenter.y, rightVec.x, rightVec.y, radius);
                float temp = Convert.ToSingle(t);
                vertices[item] = obj.transform.InverseTransformPoint(new Vector3(m_Extend.value * temp * rightVec.x+ test_world[item].x, m_Extend.value * temp * rightVec.y+ test_world[item].y, test_world[item].z));
            }
            mesh.vertices = vertices;
        }

        if(!rayincision && incision)
        {
            // 여기다가 이제 divide triangles
            // 첫 start triangle에서 각 edge마다 intersection을 검사한다.
            // 각 edge에 대해 검사한다.
            incision = false;
            int outerIntersectionCount = 0, innerIntersectionCount = 0;
            int outerSide = 0, innerSide = 0;
            // edge index
            int outerEdgeIdx = -1, innerEdgeIdx = -1;
            // DivideTest(incisionOuterStartPoint, incisionOuterStartPointIdx);

            Vector3 outerStartEdgePoint = Vector3.zero;
            Vector3 innerStartEdgePoint = Vector3.zero;
            
            // triangle / edge intersection
            outerIntersectionCount = TriangleEdgeIntersection(ref outerSide, ref outerEdgeIdx, ref outerStartEdgePoint, incisionOuterStartPoint, incisionOuterEndPoint, incisionOuterStartPointIdx);
            innerIntersectionCount = TriangleEdgeIntersection(ref innerSide, ref innerEdgeIdx, ref innerStartEdgePoint, incisionInnerStartPoint, incisionInnerEndPoint, incisionInnerStartPointIdx);

            // EdgeLineIntersection(ref int edgeIdx, ref Vector3 EdgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, int incisionPointIdx) : 반환되는 값은 intersection 된 edge 개수
            // edgeIdx : 겹치는 old edge index
            // EdgePoint : intersection된 point 위치
            // incisionOuterStartPoint, incisionOuterEndPoint : start end point 위치
            if (outerIntersectionCount == 0 || innerIntersectionCount == 0)
            {
                Debug.Log("error 띄워야됨. intersect 되지 않음. ");
            }
            else if(outerIntersectionCount == 1 && innerIntersectionCount == 1)
            {
                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;
                // _vtxIdx : intersection된 edge가 아닌 vtx index
                int _outerVtxIdx = -1, _innerVtxIdx = -1;

                // outer
                for (int i = 0; i < 3; i++)
                    if (temp[incisionOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx1 && temp[incisionOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx2)
                        _outerVtxIdx = temp[incisionOuterStartPointIdx + i];

                // inner
                for (int i = 0; i < 3; i++)
                    if (temp[incisionInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx1 && temp[incisionInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx2)
                        _innerVtxIdx = temp[incisionInnerStartPointIdx + i];
                                                                                                                                                                                                                                                                                                                                                
                // start
                // outer
                DivideTrianglesStart(incisionOuterStartPoint, outerStartEdgePoint, incisionOuterStartPointIdx, outerEdgeIdx, _outerVtxIdx, 0);
                // inner
                DivideTrianglesStart(incisionInnerStartPoint, innerStartEdgePoint, incisionInnerStartPointIdx, innerEdgeIdx, _innerVtxIdx, 1);

                int start = 0;
                bool outerEnd = false, innerEnd = false;
                while(true)
                {
                    // 여기에 이제 이어지는 edge들 계산 해야됨.
                    // 매번 vtx 업데이트해서 쓸 수 있도록 해야됨.
                    // vertices[vertices.Length-1]
                    Vector3 _outerNewEdgePoint = Vector3.zero;
                    Vector3 _innerNewEdgePoint = Vector3.zero;

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
                        if(innerEnd)
                            DivideTrianglesEnd(incisionOuterEndPoint, incisionOuterEndPointIdx, outerEdgeIdx, outerSide, 0, 0);
                        else
                            DivideTrianglesEnd(incisionOuterEndPoint, incisionOuterEndPointIdx, outerEdgeIdx, outerSide, 0, 2);
                        outerEnd = true;
                        outerTemp++;
                    }
                    else if(!outerEnd)
                        outerIntersectionCount = TriangleEdgeIntersection(ref outerSide, ref outerEdgeIdx, ref _outerNewEdgePoint, incisionOuterStartPoint, incisionOuterEndPoint, edgeList[outerEdgeIdx].tri2);

                    if(edgeList[innerEdgeIdx].tri2 == incisionInnerEndPointIdx && !innerEnd)
                    {
                        DivideTrianglesEnd(incisionInnerEndPoint, incisionInnerEndPointIdx, innerEdgeIdx, innerSide, 1, outerTemp);
                        innerEnd = true;
                        innerTemp++;
                    }
                    else if(!innerEnd)
                        innerIntersectionCount = TriangleEdgeIntersection(ref innerSide, ref innerEdgeIdx, ref _innerNewEdgePoint, incisionInnerStartPoint, incisionInnerEndPoint, edgeList[innerEdgeIdx].tri2);

                    if (start >= 50 || (outerEnd && innerEnd && innerTemp == 0 && outerTemp ==0))
                        break;

                    // 지금 bfs로 들어갈 vtx idx를 고르는게 문제임
                    if (start == 4)
                    {
                        // 지금 이 경우에 하나가 뒤집혀 있는거 같은데
                        outerLeftVtxIdx = edgeList[outerEdgeIdx].vtx1;
                        outerRightVtxIdx = edgeList[outerEdgeIdx].vtx2;
                        innerLeftVtxIdx = edgeList[innerEdgeIdx].vtx1;
                        innerRightVtxIdx = edgeList[innerEdgeIdx].vtx2;
                    }

                    int vtxLength = obj.GetComponent<MeshFilter>().mesh.vertices.Length;

                    int numTemp = outerTemp + innerTemp;
                    if(outerIntersectionCount == 1)
                    {
                        if (start == 1)
                        {
                            // 만약에 inner side가 일찍 end point에 도달한다면? 달라지겠지
                            if (outerSide == 1)
                                DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (5+numTemp), vtxLength - (4 + numTemp), 0);
                            else if (outerSide == 2)
                                DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (5 + numTemp), vtxLength - (4 + numTemp), 0);
                        }
                        else
                        {
                            if(outerSide == 1 && innerEnd && innerTemp == 0)
                                DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - 2, vtxLength - 1, 0);
                            else if(outerSide == 2 && innerEnd && innerTemp == 0)
                                DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - 2, vtxLength - 1, 0);
                            else if (outerSide == 1)
                                DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (4 + numTemp), vtxLength - (3 + numTemp), 0);
                            else if (outerSide == 2)
                                DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (4 + numTemp), vtxLength - (3 + numTemp), 0);
                        }
                    }

                    // 여기 위에서 두개가 생성되니까 하지만 그거랑 상관없이 기존의 vtx index는 유지해야되지않나?
                    if(innerIntersectionCount == 1)
                    {
                        if (innerSide == 1)
                            DivideTrianglesClockWise(_innerNewEdgePoint, edgeList[innerEdgeIdx].tri1, innerEdgeIdx, vtxLength - (2 + numTemp), vtxLength - (1 + numTemp), 1);
                        else if(innerSide == 2)
                            DivideTrianglesCounterClockWise(_innerNewEdgePoint, edgeList[innerEdgeIdx].tri1, innerEdgeIdx, vtxLength - (2 + numTemp), vtxLength - (1 + numTemp), 1);
                    }
                }
                Debug.Log("end the loop");
            }

            // 쪼개는것까지 한 프레임에 끝내고 BFS 돌리는건 다음 프레임부터 실행
            connectedVertices.Clear();
            ConnectedVertices();
            MeshRecalculate();
            SetColor();

            extending = true;
            incision = false;
        }

        if (rayincision)
        {
            // button down일때랑 up일때랑만 start end point만 필요함.
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            // if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                float dst_min = 10000000;
                float dst_2nd = 10000000;
                int triangleIndex_outer = -1;
                int triangleIndex_inner = -1;

                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;

                int vtx1 = 0, vtx2 = 0, vtx3 = 0;
                int vtx1_2nd = 0, vtx2_2nd = 0, vtx3_2nd = 0;
                Vector3 incisionInnerPoint = Vector3.zero;
                Vector3 incisionOuterPoint = Vector3.zero;
                
                for (int i = 0; i < temp.Length; i += 3)
                {
                    // ray triangle intersection 할 때 vertex 좌표들은 world 좌표로 받아야함.
                    if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.mousePosition)))
                    // if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.GetTouch(0).position)))
                    {
                        // float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.GetTouch(0).position).origin - intersectionTemp);
                        float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.mousePosition).origin - intersectionTemp);
                        if (dst_min > dst_temp)
                        {
                            if (dst_min != 10000000)
                            {
                                triangleIndex_inner = triangleIndex_outer;
                                incisionInnerPoint = incisionOuterPoint;
                                dst_2nd = dst_min;
                                vtx1_2nd = vtx1;
                                vtx2_2nd = vtx2;
                                vtx3_2nd = vtx3;
                            }
                            dst_min = dst_temp;
                            triangleIndex_outer = i;
                            incisionOuterPoint = intersectionTemp;
                            vtx1 = temp[i];
                            vtx2 = temp[i + 1];
                            vtx3 = temp[i + 2];
                            continue;
                        }
                        if (dst_2nd > dst_temp)
                        {
                            triangleIndex_inner = i;
                            incisionInnerPoint = intersectionTemp;
                            dst_2nd = dst_temp;
                            vtx1_2nd = temp[i];
                            vtx2_2nd = temp[i + 1];
                            vtx3_2nd = temp[i + 2];
                        }
                    }

                    /*
                    colors[vtx1] = Color.black;
                    colors[vtx2] = Color.black;
                    colors[vtx3] = Color.black;

                    colors[vtx1_2nd] = Color.black;
                    colors[vtx2_2nd] = Color.black;
                    colors[vtx3_2nd] = Color.black;
                    */
                    // mesh.colors = colors;
                }

                if (dst_min != 10000000)
                {
                    // 각 point에 대한 정보만 받아 놓고 실시간으로 ray만 뿌려주고 endpoint까지 찍은 후 나눈다.
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 여기다가 start point에 대한 정보 triangle 등
                        // 일단은 outer point에 대해서만 정의 해놓고 나중에 inner까지 추가시키는걸로 가자.
                        screenStartOrigin = cam.ScreenPointToRay(Input.mousePosition).origin;
                        screenStartDirection = cam.ScreenPointToRay(Input.mousePosition).direction;
                        incisionOuterStartPoint = incisionOuterPoint;
                        incisionInnerStartPoint = incisionInnerPoint;

                        incisionOuterStartPointIdx = triangleIndex_outer;
                        incisionInnerStartPointIdx = triangleIndex_inner;

                        // 여기서 line renderer 시작 
                        Debug.Log("incision start");
                        Debug.Log("start triangle idx : " + incisionOuterStartPointIdx);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    // if(Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        // 여기서 끊어진 boundary 이어주기.
                        // 중간중간에 해도 되지 않나
                        // ConnectBoundary();
                        screenEndOrigin = cam.ScreenPointToRay(Input.mousePosition).origin;
                        screenEndDirection = cam.ScreenPointToRay(Input.mousePosition).direction;
                        incisionOuterEndPoint = incisionOuterPoint;
                        incisionInnerEndPoint = incisionInnerPoint;

                        incisionOuterEndPointIdx = triangleIndex_outer;
                        incisionInnerEndPointIdx = triangleIndex_inner;
                        Debug.Log("incision end");
                        Debug.Log("end triangle idx : " + incisionOuterEndPointIdx );
                        // 여기서 incision false 값으로 바뀌고, triangle division 실행 
                        var boundaryLine = new GameObject("Incision line");
                        var lineRenderer = boundaryLine.AddComponent<LineRenderer>();
                        lineRenderer.material.color = Color.black;
                        lineRenderer.SetPositions(new Vector3[] { incisionOuterStartPoint-screenStartDirection, incisionOuterEndPoint - screenEndDirection });
                        rayincision = false;
                        incision = true;
                    }
                }
            }
        }

        if (cutmode)
        {
            cutmode = false;
            
            int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;

            foreach (int item in wholeTriangleIndexSet)
            {
                wholeTriangles.Add(item);
                // wholeTriangles.Add(triangles[item+1]);
                // wholeTriangles.Add(triangles[item+2]);
            }

            RemoveTriangles();
            // DisconnectedTriangles();
            Connected_Initialize(); // 초기화
            ConnectedVertices(); // connectedVertices 재계산
            ConnectedTriangles(); // connectedTriangles 재계산
            
            SetColor();
            MeshRecalculate();
            Initializing();
            GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = true;
            ColorBlock cb = GameObject.Find("Control button_").GetComponent<Button>().colors;
            cb.normalColor = new Color32(176, 48, 48, 255);
            GameObject.Find("Control button_").GetComponent<Button>().colors = cb;
            cb.normalColor = new Color32(137, 96, 96, 255);
            GameObject.Find("Cutting button_").GetComponent<Button>().colors = cb;
        }

        if (boundaryCut)
        {
            int verticesLength = obj.GetComponent<MeshFilter>().mesh.vertices.Length;
            int outerIntersectionCount = 0, innerIntersectionCount = 0;
            int outerSide = 0, innerSide = 0;
            int outerTriangleIdx = -1, innerTriangleIdx = -1;
            // edge index
            int outerEdgeIdx = -1, innerEdgeIdx = -1;
            // DivideTest(incisionOuterStartPoint, incisionOuterStartPointIdx);

            Vector3 outerStartEdgePoint = Vector3.zero;
            Vector3 innerStartEdgePoint = Vector3.zero;

            // 여기다가 조건을 넣어서 첫 스타트가 아닌 경우 end point를 start point에 넣어야됨.
            if (firstBoundary)
            {
                outerIntersectionCount = TriangleEdgeIntersection(ref outerSide, ref outerEdgeIdx, ref outerStartEdgePoint, boundaryOuterStartPoint, boundaryOuterEndPoint, boundaryOuterStartPointIdx);
                innerIntersectionCount = TriangleEdgeIntersection(ref innerSide, ref innerEdgeIdx, ref innerStartEdgePoint, boundaryInnerStartPoint, boundaryInnerEndPoint, boundaryInnerStartPointIdx);
            }
            else
            {
                // 지금 intersection 되지 않는 문제 있음.
                // 처음 start에서의 문제만 잡으면 나머지는 이상 없음.

                // outer
                int checkIntersectionCount = 2;
                foreach (int item in connectedTriangles[boundaryOuterEndVtx])
                {
                    checkIntersectionCount = 2;
                    // 여기서 각 edge에 대해 intersection 체크 필요.
                    outerTriangleIdx = item;
                    if (edgeList[item].vtx1 != boundaryOuterEndVtx && edgeList[item].vtx2 != boundaryOuterEndVtx)
                    {
                        if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, boundaryOuterStartPoint + screenStartDirection * 5, test_world[edgeList[item].vtx1], test_world[edgeList[item].vtx2] - test_world[edgeList[item].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                outerStartEdgePoint = intersectionTemp;
                                outerEdgeIdx = item;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;

                        if (RayTriangleIntersection(screenEndOrigin, boundaryOuterEndPoint + screenEndDirection * 5, boundaryOuterStartPoint + screenStartDirection * 5, test_world[edgeList[item].vtx1], test_world[edgeList[item].vtx2] - test_world[edgeList[item].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                outerStartEdgePoint = intersectionTemp;
                                outerEdgeIdx = item;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;
                    }
                    else if (edgeList[item + 1].vtx1 != boundaryOuterEndVtx && edgeList[item + 1].vtx2 != boundaryOuterEndVtx)
                    {
                        if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, boundaryOuterStartPoint + screenStartDirection * 5, test_world[edgeList[item + 1].vtx1], test_world[edgeList[item + 1].vtx2] - test_world[edgeList[item + 1].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                outerStartEdgePoint = intersectionTemp;
                                outerEdgeIdx = item+1;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;

                        if (RayTriangleIntersection(screenEndOrigin, boundaryOuterEndPoint + screenEndDirection * 5, boundaryOuterStartPoint + screenStartDirection * 5, test_world[edgeList[item + 1].vtx1], test_world[edgeList[item + 1].vtx2] - test_world[edgeList[item + 1].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                outerStartEdgePoint = intersectionTemp;
                                outerEdgeIdx = item+1;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;
                    }
                    else if (edgeList[item + 2].vtx1 != boundaryOuterEndVtx && edgeList[item + 2].vtx2 != boundaryOuterEndVtx)
                    {
                        if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, boundaryOuterStartPoint + screenStartDirection * 5, test_world[edgeList[item + 2].vtx1], test_world[edgeList[item + 2].vtx2] - test_world[edgeList[item + 2].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                outerStartEdgePoint = intersectionTemp;
                                outerEdgeIdx = item+2;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;

                        if (RayTriangleIntersection(screenEndOrigin, boundaryOuterEndPoint + screenEndDirection * 5, boundaryOuterStartPoint + screenStartDirection * 5, test_world[edgeList[item + 2].vtx1], test_world[edgeList[item + 2].vtx2] - test_world[edgeList[item + 2].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                outerStartEdgePoint = intersectionTemp;
                                outerEdgeIdx = item+2;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;
                    }
                }

                if (checkIntersectionCount != 0)
                {
                    outerIntersectionCount = 1;
                }
                else
                {
                    Debug.Log("intersection error");
                    // return;
                }

                // inner
                foreach (int item in connectedTriangles[boundaryInnerEndVtx])
                {
                    checkIntersectionCount = 2;
                    innerTriangleIdx = item;
                    // 여기서 각 edge에 대해 intersection 체크 필요.
                    if (edgeList[item].vtx1 != boundaryInnerEndVtx && edgeList[item].vtx2 != boundaryInnerEndVtx)
                    {
                        if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, boundaryInnerStartPoint + screenStartDirection * 5, test_world[edgeList[item].vtx1], test_world[edgeList[item].vtx2] - test_world[edgeList[item].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                innerStartEdgePoint = intersectionTemp;
                                innerEdgeIdx = item;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;

                        if (RayTriangleIntersection(screenEndOrigin, boundaryInnerEndPoint + screenEndDirection * 5, boundaryInnerStartPoint + screenStartDirection * 5, test_world[edgeList[item].vtx1], test_world[edgeList[item].vtx2] - test_world[edgeList[item].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item].vtx1].x, test_world[edgeList[item].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                innerStartEdgePoint = intersectionTemp;
                                innerEdgeIdx = item;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;
                    }
                    else if (edgeList[item + 1].vtx1 != boundaryInnerEndVtx && edgeList[item + 1].vtx2 != boundaryInnerEndVtx)
                    {
                        if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, boundaryInnerStartPoint + screenStartDirection * 5, test_world[edgeList[item + 1].vtx1], test_world[edgeList[item + 1].vtx2] - test_world[edgeList[item + 1].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                innerStartEdgePoint = intersectionTemp;
                                innerEdgeIdx = item+1;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;

                        if (RayTriangleIntersection(screenEndOrigin, boundaryInnerEndPoint + screenEndDirection * 5, boundaryInnerStartPoint + screenStartDirection * 5, test_world[edgeList[item + 1].vtx1], test_world[edgeList[item + 1].vtx2] - test_world[edgeList[item + 1].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 1].vtx1].x, test_world[edgeList[item + 1].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                innerStartEdgePoint = intersectionTemp;
                                innerEdgeIdx = item+1;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;
                    }
                    else if (edgeList[item + 1].vtx1 != boundaryInnerEndVtx && edgeList[item + 1].vtx2 != boundaryInnerEndVtx)
                    {
                        if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, boundaryInnerStartPoint + screenStartDirection * 5, test_world[edgeList[item + 2].vtx1], test_world[edgeList[item + 2].vtx2] - test_world[edgeList[item + 2].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                innerStartEdgePoint = intersectionTemp;
                                innerEdgeIdx = item+2;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;

                        if (RayTriangleIntersection(screenEndOrigin, boundaryInnerEndPoint + screenEndDirection * 5, boundaryInnerStartPoint + screenStartDirection * 5, test_world[edgeList[item + 2].vtx1], test_world[edgeList[item + 2].vtx2] - test_world[edgeList[item + 2].vtx1]))
                        {
                            if (intersectionTemp.x < Mathf.Min(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[item + 2].vtx1].x, test_world[edgeList[item + 2].vtx2].x))
                                checkIntersectionCount--;
                            else
                            {
                                innerStartEdgePoint = intersectionTemp;
                                innerEdgeIdx = item+2;
                                break;
                            }
                        }
                        else
                            checkIntersectionCount--;
                    }
                }

                if (checkIntersectionCount != 0)
                {
                    innerIntersectionCount = 1;
                }
                else
                {
                    Debug.Log("intersection error");
                    // return;
                }
            }

            // triangle / edge intersection
            if (outerIntersectionCount == 0 || innerIntersectionCount == 0)
            {
                Debug.Log("error 띄워야됨. intersect 되지 않음. ");
            }
            else if (outerIntersectionCount == 1 && innerIntersectionCount == 1)
            {
                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;
                int _outerVtxIdx = -1, _innerVtxIdx = -1;

                // 처음과 달라야됨
                if (firstBoundary)
                {
                    // outer
                    for (int i = 0; i < 3; i++)
                        if (temp[boundaryOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx1 && temp[boundaryOuterStartPointIdx + i] != edgeList[outerEdgeIdx].vtx2)
                            _outerVtxIdx = temp[boundaryOuterStartPointIdx + i];

                    // inner
                    for (int i = 0; i < 3; i++)
                        if (temp[boundaryInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx1 && temp[boundaryInnerStartPointIdx + i] != edgeList[innerEdgeIdx].vtx2)
                            _innerVtxIdx = temp[boundaryInnerStartPointIdx + i];

                    // start
                    // outer
                    DivideTrianglesStart(boundaryOuterStartPoint, outerStartEdgePoint, boundaryOuterStartPointIdx, outerEdgeIdx, _outerVtxIdx, 0);
                    // inner
                    DivideTrianglesStart(boundaryInnerStartPoint, innerStartEdgePoint, boundaryInnerStartPointIdx, innerEdgeIdx, _innerVtxIdx, 1);
                    firstBoundary = false;
                    Debug.Log("first boundary");
                }
                else
                {
                    // 처음이 아닌경우 start point가 정점에서 시작할 때
                    DivideTrianglesStartFromVtx(boundaryOuterEndVtx, outerStartEdgePoint, outerTriangleIdx, outerEdgeIdx);
                    DivideTrianglesStartFromVtx(boundaryInnerEndVtx, innerStartEdgePoint, innerTriangleIdx, innerEdgeIdx);
                }

                int start = 0;
                bool outerEnd = false, innerEnd = false;
                while (true)
                {
                    Vector3 _outerNewEdgePoint = Vector3.zero;
                    Vector3 _innerNewEdgePoint = Vector3.zero;

                    start++;

                    outerIntersectionCount = 0;
                    innerIntersectionCount = 0;

                    int outerTemp = 0;
                    int innerTemp = 0;

                    if (edgeList[outerEdgeIdx].tri2 == boundaryOuterEndPointIdx && !outerEnd)
                    {
                        if (innerEnd)
                            DivideTrianglesEnd(boundaryOuterEndPoint, boundaryOuterEndPointIdx, outerEdgeIdx, outerSide, 0, 0);
                        else
                            DivideTrianglesEnd(boundaryOuterEndPoint, boundaryOuterEndPointIdx, outerEdgeIdx, outerSide, 0, 2);
                        
                        outerEnd = true;
                        outerTemp++;
                    }
                    else if (!outerEnd)
                        outerIntersectionCount = TriangleEdgeIntersection(ref outerSide, ref outerEdgeIdx, ref _outerNewEdgePoint, boundaryOuterStartPoint, boundaryOuterEndPoint, edgeList[outerEdgeIdx].tri2);

                    if (edgeList[innerEdgeIdx].tri2 == boundaryInnerEndPointIdx && !innerEnd)
                    {
                        DivideTrianglesEnd(boundaryInnerEndPoint, boundaryInnerEndPointIdx, innerEdgeIdx, innerSide, 1, outerTemp);
                        innerEnd = true;
                        innerTemp++;
                    }
                    else if (!innerEnd)
                        innerIntersectionCount = TriangleEdgeIntersection(ref innerSide, ref innerEdgeIdx, ref _innerNewEdgePoint, boundaryInnerStartPoint, boundaryInnerEndPoint, edgeList[innerEdgeIdx].tri2);

                    if (start >= 50 || (outerEnd && innerEnd && innerTemp == 0 && outerTemp == 0))
                        break;

                    int vtxLength = obj.GetComponent<MeshFilter>().mesh.vertices.Length;

                    int numTemp = outerTemp + innerTemp;
                    if (outerIntersectionCount == 1)
                    {
                        if (start == 1)
                        {
                            // 만약에 inner side가 일찍 end point에 도달한다면? 달라지겠지
                            if (outerSide == 1)
                                DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (5 + numTemp), vtxLength - (4 + numTemp), 0);
                            else if (outerSide == 2)
                                DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (5 + numTemp), vtxLength - (4 + numTemp), 0);
                        }
                        else
                        {
                            if (outerSide == 1 && innerEnd && innerTemp == 0)
                                DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - 2, vtxLength - 1, 0);
                            else if (outerSide == 2 && innerEnd && innerTemp == 0)
                                DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - 2, vtxLength - 1, 0);
                            else if (outerSide == 1)
                                DivideTrianglesClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (4 + numTemp), vtxLength - (3 + numTemp), 0);
                            else if (outerSide == 2)
                                DivideTrianglesCounterClockWise(_outerNewEdgePoint, edgeList[outerEdgeIdx].tri1, outerEdgeIdx, vtxLength - (4 + numTemp), vtxLength - (3 + numTemp), 0);
                        }
                    }

                    // 여기 위에서 두개가 생성되니까 하지만 그거랑 상관없이 기존의 vtx index는 유지해야되지않나?
                    if (innerIntersectionCount == 1)
                    {
                        if (innerSide == 1)
                            DivideTrianglesClockWise(_innerNewEdgePoint, edgeList[innerEdgeIdx].tri1, innerEdgeIdx, vtxLength - (2 + numTemp), vtxLength - (1 + numTemp), 1);
                        else if (innerSide == 2)
                            DivideTrianglesCounterClockWise(_innerNewEdgePoint, edgeList[innerEdgeIdx].tri1, innerEdgeIdx, vtxLength - (2 + numTemp), vtxLength - (1 + numTemp), 1);
                    }
                }
                Debug.Log("end the loop");
            }

            // 이 부분이 문제인데 어떻게 해야 하지 나뭉린물이ㅜㄴ라ㅣㄴ위
            boundaryOuterStartPoint = boundaryOuterEndPoint;
            boundaryInnerStartPoint = boundaryInnerEndPoint;

            connectedTriangles.Clear();
            ConnectedTriangles();
            connectedVertices.Clear();
            ConnectedVertices();
            GenerateEdgeList();
            MeshRecalculate();
            screenStartOrigin = screenEndOrigin;
            screenStartDirection = screenEndDirection;
        }
    
        /*
        if (cutting && !cutmode)
        {
            // 조건 정리 해야됨.
            // 마우스 조건
            if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            // if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                // colors = new Color[mesh.vertexCount];
                float dst_min = 10000000;
                float dst_2nd = 10000000;
                int triangleIndex_outer = -1;
                int triangleIndex_inner = -1;

                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;

                int vtx1 = 0, vtx2 = 0, vtx3 = 0;
                int vtx1_2nd = 0, vtx2_2nd = 0, vtx3_2nd = 0;

                for (int i = 0; i < temp.Length; i += 3)
                {
                    // ray triangle intersection 할 때 vertex 좌표들은 world 좌표로 받아야함.
                    if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.mousePosition)))
                    // if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.GetTouch(0).position)))
                    {
                        // float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.GetTouch(0).position).origin - intersectionTemp);
                        float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.mousePosition).origin - intersectionTemp);
                        if (dst_min > dst_temp)
                        {
                            if (dst_min != 10000000)
                            {
                                triangleIndex_inner = triangleIndex_outer;
                                // innerIntersectionPoint = outerIntersectionPoint;
                                dst_2nd = dst_min;
                                vtx1_2nd = vtx1;
                                vtx2_2nd = vtx2;
                                vtx3_2nd = vtx3;
                            }
                            dst_min = dst_temp;
                            triangleIndex_outer = i;
                            // outerIntersectionPoint = intersectionTemp;
                            vtx1 = temp[i];
                            vtx2 = temp[i + 1];
                            vtx3 = temp[i + 2];
                            continue;
                        }
                        if (dst_2nd > dst_temp)
                        {
                            triangleIndex_inner = i;
                            // innerIntersectionPoint = intersectionTemp;
                            dst_2nd = dst_temp;
                            vtx1_2nd = temp[i];
                            vtx2_2nd = temp[i + 1];
                            vtx3_2nd = temp[i + 2];
                        }
                    }
                }

                if (dst_min != 10000000)
                {
                    // 여기에 따로 조건을 줘서 
                    if (Input.GetMouseButtonUp(0))
                    // if(Input.GetTouch(0).phase == TouchPhase.Ended)
                    {

                        foreach (int item in hashBoundaryVertices)
                        {
                            boundaryVertices.Add(item);
                        }
                        hashBoundaryVertices.Clear();

                        foreach (int item in innerHashBoundaryVertices)
                        {
                            innerBoundaryVertices.Add(item);
                        }
                        innerHashBoundaryVertices.Clear();

                        check_function = true;
                        cutmode = true;
                        cutting = false;
                        Debug.Log("cutmode");
                    }

                    // 어떤 특정값에 따라 triangle / 가장 가까운 점에 위치한 주변 adjacency triangle / 한겹추가
                    // cutNum ==0 일 때 triangle 하나 지우기
                    if (m_cuttingSizeBar.value <= 0.25)
                    {
                        wholeTriangleIndexSet.Add(triangleIndex_inner);
                        wholeTriangleIndexSet.Add(triangleIndex_outer);
                        // innerBoundaryTriangleIndexSet.Add(triangleIndex_inner);
                        // outerBoundaryTriangleIndexSet.Add(triangleIndex_outer);

                        colors[vtx1] = Color.black;
                        colors[vtx2] = Color.black;
                        colors[vtx3] = Color.black;

                        colors[vtx1_2nd] = Color.black;
                        colors[vtx2_2nd] = Color.black;
                        colors[vtx3_2nd] = Color.black;
                        mesh.colors = colors;
                    }
                    else if(m_cuttingSizeBar.value<=0.5)
                    {
                        foreach (int item in connectedTriangles[vtx1])
                        {
                            // outerBoundaryTriangleIndexSet.Add(item);
                            wholeTriangleIndexSet.Add(item);
                            colors[temp[item]] = Color.black;
                            colors[temp[item+1]] = Color.black;
                            colors[temp[item+2]] = Color.black;
                        }

                        foreach (int item in connectedTriangles[vtx1_2nd])
                        {
                            // innerBoundaryTriangleIndexSet.Add(item);
                            wholeTriangleIndexSet.Add(item);
                            colors[temp[item]] = Color.black;
                            colors[temp[item + 1]] = Color.black;
                            colors[temp[item + 2]] = Color.black;
                        }
                        mesh.colors = colors;
                    } 
                    else if(m_cuttingSizeBar.value<=0.75)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int a, b;
                            if(i==0)
                            {
                                a = vtx1;
                                b = vtx1_2nd;
                            }
                            else if(i==1)
                            {
                                a = vtx2;
                                b = vtx2_2nd;
                            }
                            else
                            {
                                a = vtx3;
                                b = vtx3_2nd;
                            }
                            foreach (int item in connectedTriangles[a])
                            {
                                // outerBoundaryTriangleIndexSet.Add(item);
                                wholeTriangleIndexSet.Add(item);
                                colors[temp[item]] = Color.black;
                                colors[temp[item + 1]] = Color.black;
                                colors[temp[item + 2]] = Color.black;
                            }
                            foreach (int item in connectedTriangles[b])
                            {
                                // innerBoundaryTriangleIndexSet.Add(item);
                                wholeTriangleIndexSet.Add(item);
                                colors[temp[item]] = Color.black;
                                colors[temp[item + 1]] = Color.black;
                                colors[temp[item + 2]] = Color.black;
                            }
                        }
                        mesh.colors = colors;
                    }

                    else if (m_cuttingSizeBar.value<=1.0)
                    {
                        foreach (int item in connectedVertices[vtx1])
                        {
                            foreach (int item2 in connectedTriangles[item])
                            {
                                wholeTriangleIndexSet.Add(item2);
                                colors[temp[item2]] = Color.black;
                                colors[temp[item2 + 1]] = Color.black;
                                colors[temp[item2 + 2]] = Color.black;
                            }
                        }

                        foreach (int item in connectedVertices[vtx1_2nd])
                        {
                            foreach (int item2 in connectedTriangles[vtx1_2nd])
                            {
                                wholeTriangleIndexSet.Add(item2);
                                colors[temp[item2]] = Color.black;
                                colors[temp[item2 + 1]] = Color.black;
                                colors[temp[item2 + 2]] = Color.black;
                            }
                        }
                        mesh.colors = colors;
                    }
                }
            }
        } // cutMode
        */

        if (cutting && !cutmode)
        {
            Debug.Log("cutmode ininiin");
            if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            // if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                // colors = new Color[mesh.vertexCount];
                float dst_min = 10000000;
                float dst_2nd = 10000000;

                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;

                int triangleIndex_outer = -1;
                int triangleIndex_inner = -1;
                int vtx1 = 0, vtx2 = 0, vtx3 = 0;
                int vtx1_2nd = 0, vtx2_2nd = 0, vtx3_2nd = 0;

                for (int i = 0; i < temp.Length; i += 3)
                {
                    // ray triangle intersection 할 때 vertex 좌표들은 world 좌표로 받아야함.
                    if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.mousePosition)))
                    // if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.GetTouch(0).position)))
                    {
                        // float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.GetTouch(0).position).origin - intersectionTemp);
                        float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.mousePosition).origin - intersectionTemp);
                        if (dst_min > dst_temp)
                        {
                            if (dst_min != 10000000)
                            {
                                triangleIndex_inner = triangleIndex_outer;
                                innerIntersectionPoint = outerIntersectionPoint;
                                dst_2nd = dst_min;
                                vtx1_2nd = vtx1;
                                vtx2_2nd = vtx2;
                                vtx3_2nd = vtx3;
                            }
                            triangleIndex_outer = i;
                            dst_min = dst_temp;
                            outerIntersectionPoint = intersectionTemp;
                            vtx1 = temp[i];
                            vtx2 = temp[i + 1];
                            vtx3 = temp[i + 2];
                            continue;
                        }
                        if (dst_2nd > dst_temp)
                        {
                            triangleIndex_inner = i;
                            innerIntersectionPoint = intersectionTemp;
                            dst_2nd = dst_temp;
                            vtx1_2nd = temp[i];
                            vtx2_2nd = temp[i + 1];
                            vtx3_2nd = temp[i + 2];
                        }
                    }
                }

                if (dst_min != 10000000)
                {
                    // 각 point에 대한 정보만 받아 놓고 실시간으로 ray만 뿌려주고 endpoint까지 찍은 후 나눈다.
                    if (firstRay)
                    {
                        Debug.Log("mouse button down");
                        // 여기다가 start point에 대한 정보 triangle 등
                        // 일단은 outer point에 대해서만 정의 해놓고 나중에 inner까지 추가시키는걸로 가자.
                        screenStartOrigin = cam.ScreenPointToRay(Input.mousePosition).origin;
                        screenStartDirection = cam.ScreenPointToRay(Input.mousePosition).direction;

                        // 이름들 다 바꾸는게 좋음.
                        boundaryOuterStartPoint = outerIntersectionPoint;
                        boundaryInnerStartPoint = innerIntersectionPoint;

                        boundaryOuterStartPointIdx = triangleIndex_outer;
                        boundaryInnerStartPointIdx = triangleIndex_inner;

                        // 여기서 line renderer 시작 
                        Debug.Log("boundary cut start");
                        Debug.Log("start triangle idx : " + boundaryOuterStartPointIdx);
                        firstRay = false;
                    }
                    // 이 조건을 up 했을때가 아니라 일정거리가 멀어졌을때 해야됨.
                    else if (Vector3.Distance(outerIntersectionPoint, boundaryOuterStartPoint) > 5.0f)
                    // if(Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        Debug.Log("inininininininininin");
                        screenEndOrigin = cam.ScreenPointToRay(Input.mousePosition).origin;
                        screenEndDirection = cam.ScreenPointToRay(Input.mousePosition).direction;

                        boundaryOuterEndPoint = outerIntersectionPoint;
                        boundaryInnerEndPoint = innerIntersectionPoint;

                        boundaryOuterEndPointIdx = triangleIndex_outer;
                        boundaryInnerEndPointIdx = triangleIndex_inner;

                        Debug.Log("boundary end");
                        Debug.Log("start point : " + boundaryOuterStartPoint);
                        Debug.Log("end point : " + boundaryOuterEndPoint);
                        // 여기서 incision false 값으로 바뀌고, triangle division 실행 
                        var boundaryLine = new GameObject("Incision line");
                        var lineRenderer = boundaryLine.AddComponent<LineRenderer>();
                        lineRenderer.material.color = Color.black;
                        lineRenderer.SetPositions(new Vector3[] { boundaryOuterStartPoint - screenStartDirection, boundaryOuterEndPoint - screenEndDirection });
                        
                        boundaryCut = true;
                    }
                    else
                        boundaryCut = false;
                }
            }
        } // boundary cut mode

        if (measuring)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("get mouse button down");
                Debug.Log(Input.mousePosition);
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                
                float dst_min = 1000000;
                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
                Debug.Log(ray.direction);
                for (int i = 0; i < temp.Length; i += 3)
                {
                    if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.mousePosition)))
                    {
                        float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.mousePosition).origin - intersectionTemp);
                        if (dst_min > dst_temp)
                        {
                            intersectionPoint = intersectionTemp;
                            dst_min = dst_temp;
                        }
                    }
                }

                if (dst_min != 1000000)
                {
                    if (!measured_check)
                    {
                        // Destroy(GameObject.FindObjectsOfTypeAll(PrimitiveType.Sphere));
                        sp1.active = true;
                        sp2.active = false;
                        Destroy(GameObject.Find("distance"));
                        GameObject anyobj = new GameObject("p1");
                        anyobj = sp1;
                        p1_vec = intersectionPoint;
                        anyobj.transform.localPosition = intersectionPoint;
                        
                        measured_check = true;
                    }
                    else
                    {
                        sp2.active = true;
                        GameObject anyobj = new GameObject("p2");
                        anyobj = sp2;
                        anyobj.transform.localPosition = intersectionPoint;
                        measured_check = false;
                        float dst = Vector3.Distance(p1_vec, intersectionPoint);
                        heartScale = obj.transform.lossyScale.z;
                        dst = dst / heartScale;
                        m_distance.text = dst + "mm";
                        Debug.Log("거리 : " + dst);
                        
                        var boundaryLine = new GameObject("distance");
                        var lineRenderer = boundaryLine.AddComponent<LineRenderer>();
                        lineRenderer.material.color = Color.black;
                        lineRenderer.SetPositions(new Vector3[] { p1_vec, intersectionPoint });
                        distanceNumber++;
                        Destroy(GameObject.Find("/p1"));
                        Destroy(GameObject.Find("/p2"));
                    }
                }
            }
        }

        if (reCalculate)
        {
            if(m_Posbar.value != oldValue_height || m_bar.value != oldValue_curve)
                GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = false;
            else
                GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = true;

            oldValue_height = m_Posbar.value;
            oldValue_curve = m_bar.value;
            
            patchCenterPos = weightCenterPos + ((m_Posbar.value-0.5f) * 40) * avgNorm;
            patchWeight = m_bar.value * 20.0f;
            Debug.Log(patchWeight);
            RecalculateNormal();
        }
         
        if (patching)
        {
            if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
            // if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                float dst_min = 1000000;
                int[] temp = obj.GetComponent<MeshFilter>().mesh.triangles;

                Vector3 incisionInnerPoint = Vector3.zero;
                Vector3 incisionOuterPoint = Vector3.zero;

                for (int i = 0; i < temp.Length; i += 3)
                {
                    // ray triangle intersection 할 때 vertex 좌표들은 world 좌표로 받아야함.
                    if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.mousePosition)))
                    // if (RayTriangleIntersection(test_world[temp[i]], test_world[temp[i + 1]], test_world[temp[i + 2]], cam.ScreenPointToRay(Input.GetTouch(0).position)))
                    {
                        float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.mousePosition).origin - intersectionTemp);
                        // float dst_temp = Vector3.Magnitude(cam.ScreenPointToRay(Input.GetTouch(0).position).origin - intersectionTemp);
                        if (dst_min > dst_temp)
                        {
                            intersectionPoint = intersectionTemp;
                            incisionInnerPoint = incisionOuterPoint;
                            dst_min = dst_temp;
                        }
                        incisionOuterPoint = intersectionTemp;
                    }
                }

                if (dst_min != 1000000)
                {
                    patchedCount++;
                    if (patchedCount == 1)
                    {
                        OldIntersectionPoint = intersectionPoint;
                        // oldScreenPoint = cam.ScreenPointToRay(Input.mousePosition).origin;

                        patchVertices.Add(intersectionPoint);

                        return;
                    }
                    
                    if (Vector3.Distance(OldIntersectionPoint, intersectionPoint) < 3)
                    {
                        patchedCount--;
                        return;
                    }

                    var boundaryLine = new GameObject("Line"+patchedCount);
                    var lineRenderer = boundaryLine.AddComponent<LineRenderer>();
                    lineRenderer.material.color = Color.black;
                    lineRenderer.SetPositions(new Vector3[] { OldIntersectionPoint, intersectionPoint });

                    // lineRenderer.SetPositions(new Vector3[] { oldScreenPoint, cam.ScreenPointToRay(Input.mousePosition).origin });
                    // oldScreenPoint = cam.ScreenPointToRay(Input.mousePosition).origin;

                    OldIntersectionPoint = intersectionPoint;
                    patchVertices.Add(intersectionPoint);
                    return;
                }
            }
            else if(Input.GetMouseButtonUp(0) && patchedCount != 0)
            // else if (Input.GetTouch(0).phase == TouchPhase.Ended && patchedCount != 0)
            {
                GeneratePatchTriangles();
                patchWeight = m_bar.value * 20.0f;
                CalculateNormal();
                
                patching = false;
                for (int i = 0; i < patchedCount; i++)
                    Destroy(GameObject.Find("Line"+(1+i)));
                reCalculate = true;
                GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = true;
            }
        }
    }

    


    private void ConnectedTriangles()
    {
        // 이거 가지고 최후를 저장해야 하는데
        int t_length = obj.GetComponent<MeshFilter>().sharedMesh.triangles.Length;
        int v_length = obj.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
        int[] m_triangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;

        for (int i = 0; i < v_length; i++)
        {
            connectedTriangles.Add(i, new HashSet<int>());
        }

        for (int i = 0; i < t_length; i += 3)
        {
            connectedTriangles[m_triangles[i]].Add(i);
            connectedTriangles[m_triangles[i + 1]].Add(i);
            connectedTriangles[m_triangles[i + 2]].Add(i);
        }
    }

    private void ConnectedVertices()
    {
        int v_length = obj.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
        int t_length = obj.GetComponent<MeshFilter>().sharedMesh.triangles.Length;
        int[] m_triangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;

        edgeList = new List<Edge>();

        for (int j = 0; j < v_length; j++)
        {
            connectedVertices.Add(j, new HashSet<int>());
        }

        for (int i = 0; i < t_length; i += 3)
        {
            // 여기서 edge list 도 받아야함.
            connectedVertices[m_triangles[i]].Add(m_triangles[i + 1]);
            connectedVertices[m_triangles[i]].Add(m_triangles[i + 2]);
            edgeList.Add(new Edge(m_triangles[i], m_triangles[i + 1], i, -1));

            connectedVertices[m_triangles[i + 1]].Add(m_triangles[i]);
            connectedVertices[m_triangles[i + 1]].Add(m_triangles[i + 2]);
            edgeList.Add(new Edge(m_triangles[i+1], m_triangles[i + 2], i, -1));

            connectedVertices[m_triangles[i + 2]].Add(m_triangles[i]);
            connectedVertices[m_triangles[i + 2]].Add(m_triangles[i + 1]);
            edgeList.Add(new Edge(m_triangles[i+2], m_triangles[i], i, -1));
        }

        Debug.Log(t_length);
        Debug.Log("Done2");
    }

    private void RemoveTriangles()
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;
        int[] newTriangles = new int[triangles.Length - (wholeTriangles.Count * 3)];
        Debug.Log(triangles.Length - (wholeTriangles.Count * 3));
        wholeTriangles.Sort();
        int triangleCount = 0, tempCount = 0;
        for (int i = 0; i < triangles.Length; i+=3)
        {
            if (wholeTriangles[tempCount] == i)
            {
                tempCount++;
                if (tempCount == wholeTriangles.Count)
                    tempCount--;
                continue;
            }
            newTriangles[triangleCount++] = triangles[i];
            newTriangles[triangleCount++] = triangles[i+1];
            newTriangles[triangleCount++] = triangles[i+2];
        }

        mesh.triangles = newTriangles;
        return;

    }

    private void GeneratePatchTriangles()
    {
        for (int i = 0; i < patchVertices.Count; i++)
            patchCenterPos += patchVertices[i];
        patchCenterPos /= patchVertices.Count;

        float minDst = 1000000;
        Vector3[] vtrList = new Vector3[patchVertices.Count];
        for (int i = 0; i < patchVertices.Count; i++)
        {
            vtrList[i] = patchCenterPos - patchVertices[i];
            float dst = Vector3.Distance(patchVertices[i], patchCenterPos);
            if (dst < minDst)
                minDst = dst;
        }

        insidePatchVertices = new List<Vector3>[5];
        for (int i = 0; i < 5; i++)
        {
            insidePatchVertices[i] = new List<Vector3>();
        }

        for (int i = 0; i < patchVertices.Count; i++)
        {
            insidePatchVertices[0].Add(patchVertices[i]);
            insidePatchVertices[1].Add(patchVertices[i] + (vtrList[i] / 5 * 1));
            insidePatchVertices[2].Add(patchVertices[i] + (vtrList[i] / 5 * 2));
            insidePatchVertices[3].Add(patchVertices[i] + (vtrList[i] / 5 * 3));
            insidePatchVertices[4].Add(patchVertices[i] + (vtrList[i] / 5 * 4));
        }
        // 여기서 일단 생성은 됨.
        GeneratePatchesTriangles();
        return;
    }

    private void GeneratePatchesTriangles()
    {
        // 이거 넘버링 해줘야됨.
        GameObject patchObj = new GameObject("Patch" + patchNumber, typeof(MeshFilter), typeof(MeshRenderer)); 
        Mesh mesh = new Mesh();
        patchObj.GetComponent<MeshFilter>().mesh = mesh;
        patchObj.transform.parent = GameObject.Find("HumanHeart").transform;
        // rendering 
        Renderer rend = patchObj.GetComponent<Renderer>();
        rend.material.color = Color.white;
        
        Vector3[] points = new Vector3[patchVertices.Count*5 + 1];
        int[] triangles = new int[((patchVertices.Count*2)*4+patchVertices.Count) *3];

        for (int j = 0; j < 5; j++)
        {
            for (int i = patchVertices.Count * j; i < patchVertices.Count*(j+1); i++)
            {
                points[i] = insidePatchVertices[j][i%patchVertices.Count];
            }
        }
        points[patchVertices.Count * 5] = patchCenterPos;

        BuildMesh(ref triangles, 5-1);

        mesh.vertices = points;
        mesh.triangles = triangles;

        return;
    }

    private void BuildMesh(ref int[] triangles, int loopCount)
    {
        int temp_num = patchVertices.Count;
        int TN = 0; // triangle idx
        for (int i = 0; i < loopCount; i++)
        {
            for (int j = 0; j < temp_num; j++)
            {
                int F = temp_num * i + j;
                int K = F + 1;
                int N = F + temp_num;
                int M = N + 1;
                
                if (F % temp_num == temp_num-1)
                    K = temp_num * i; 

                if (N % temp_num == temp_num - 1)
                    M = temp_num * (i + 1); 

                triangles[TN++] = F;
                triangles[TN++] = K;
                triangles[TN++] = N;

                triangles[TN++] = K;
                triangles[TN++] = M;
                triangles[TN++] = N;
            }
        }

        for (int i = 0; i < temp_num; i++)
        {
            int N = loopCount * temp_num + i; 
            int K = N + 1;

            if (K == temp_num * (loopCount + 1))
                K = temp_num * loopCount;

            triangles[TN++] = N;
            triangles[TN++] = K;
            triangles[TN++] = temp_num * (loopCount+1);
        }
        return;
    }
 
    private void RecalculateNormal()
    {
        Mesh mesh = GameObject.Find("Patch" + patchNumber).GetComponent<MeshFilter>().mesh;
        int tempNum = patchVertices.Count;
        Vector3[] asdf = mesh.vertices;
        for (int i = 0; i < tempNum; i++)
        {
            Vector3 p1 = insidePatchVertices[0][i];
            Vector3 p2 = insidePatchVertices[2][i] + avgNorm * patchWeight;
            Vector3 p3 = patchCenterPos;
            asdf[i + tempNum] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.2f), Vector3.Lerp(p2, p3, 0.2f), 0.2f);
            asdf[i + tempNum * 2] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.4f), Vector3.Lerp(p2, p3, 0.4f), 0.4f);
            asdf[i + tempNum * 3] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.6f), Vector3.Lerp(p2, p3, 0.6f), 0.6f);
            asdf[i + tempNum * 4] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.8f), Vector3.Lerp(p2, p3, 0.8f), 0.8f);
        }

        asdf[mesh.normals.Length - 1] = patchCenterPos;

        mesh.vertices = asdf;
        mesh.RecalculateNormals();
    }

    private void CalculateNormal()
    {
        Mesh mesh = GameObject.Find("Patch"+patchNumber).GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals();
        int tempNum = patchVertices.Count;
        avgNorm = new Vector3(0, 0, 0);

        for (int i = 0; i < mesh.normals.Length; i++)
            avgNorm += mesh.normals[i];
        avgNorm /= mesh.normals.Length;
        patchCenterPos += 10 * avgNorm;
        weightCenterPos = patchCenterPos;
        Vector3[] asdf = mesh.vertices;
        for (int i = 0; i < tempNum; i++)
        {
            Vector3 p1 = insidePatchVertices[0][i];
            Vector3 p2 = insidePatchVertices[2][i] + avgNorm * patchWeight;
            Vector3 p3 = patchCenterPos;
            asdf[i+ tempNum] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.2f), Vector3.Lerp(p2, p3, 0.2f), 0.2f);
            asdf[i+ tempNum*2] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.4f), Vector3.Lerp(p2, p3, 0.4f), 0.4f);
            asdf[i+ tempNum*3] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.6f), Vector3.Lerp(p2, p3, 0.6f), 0.6f);
            asdf[i+ tempNum*4] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.8f), Vector3.Lerp(p2, p3, 0.8f), 0.8f);
        }

        asdf[mesh.normals.Length-1] = patchCenterPos;

        mesh.vertices = asdf;
        mesh.RecalculateNormals();
        return;
    }

    private void MeshRecalculate()
    {
        Debug.Log("mesh recalculate");
        int j = 0;
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        test = mesh.vertices;
        test_world = mesh.vertices;
        Debug.Log("triangles 개수" + mesh.triangles.Length);
        Debug.Log("normal vector 개수" + mesh.vertices.Length);
        for (int i = 0; i < test.Length; i++)
        {
            test_world[i] = obj.transform.TransformPoint(test[i]);
        }
        /*
        foreach (Transform child in obj.GetComponent<Transform>())
        {
            test[j] = child.transform.localPosition;
            test_world[j] = child.transform.position;
            j++;
        }
        */
        mesh.vertices = test;
        mesh.RecalculateNormals();
        Debug.Log("트라이앵글 크기 : " + mesh.triangles.Length);

        return;
    }

    
    private bool isLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }
    
    private void BFS_Boundary()
    {
        // start point부터 end point까지 겹치는 point들 전부가 boundary list에 들어가야됨.
        // 일단 incision에서 사용한 기능부터 만들어 보고
        // 여기는 그냥 BFS만 구현 하면됨.
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        // temp.Enqueue(vertex_num);

        /*
         * 여기에 boundary list를 
        foreach (int item in collection)
        {
            duplicateCheck.Add(item);
        }
        */
        while (temp.Count != 0)
        {
            foreach (int item in connectedVertices[temp.Dequeue()])
            {
                Debug.Log(item);
                bool temp_check = false;
                foreach (int check in duplicateCheck)
                {
                    if (check == item)
                    {
                        temp_check = true;
                        break;
                    }
                }
                if (temp_check)
                    continue;

                duplicateCheck.Add(item);
                temp.Enqueue(item);
            }
        }
        return;
    }


    private void BFS_Circle(int vertex_num, Vector3 startPoint, Vector3 endPoint, bool _left)
    {
        // 지금 여기에 오류가 있는데 찾을 수가 없네
        Vector3 center = Vector3.Lerp(startPoint, endPoint, 0.5f);
        float dst = Vector2.Distance(startPoint, endPoint) / 2;
        
        // vertex_num
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        duplicateCheck.Add(vertex_num);

        if (_left)
        {
            leftSide.Add(vertex_num);
            foreach (int item in leftSide)
                duplicateCheck.Add(item);
        }
        else
        {
            rightSide.Add(vertex_num);
            foreach (int item in rightSide)
                duplicateCheck.Add(item);
        }
        Debug.Log(vertex_num);
        temp.Enqueue(vertex_num);
        // outerVertices.Add(vertex_num);
        GameObject v_t = new GameObject();
        v_t = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        v_t.transform.position = test_world[vertex_num];

        while (temp.Count != 0)
        {
            foreach (int item in connectedVertices[temp.Dequeue()])
            {
                Debug.Log(item);
                bool temp_check = false;
                // vtx에서 다른거로 어떻게 넘어갈까 흠..
                foreach (int check in duplicateCheck)
                {
                    if(check==item)
                    {
                        temp_check = true;
                        break;
                    }
                }
                if (temp_check)
                    continue;
                // 원에 포함된다면
                // perpendicular vector를 추가로 더해줘서 안쪽에 포함 안되도록

                if (Vector2.Distance(center, test_world[item]) < dst)
                {
                    if (isLeft(startPoint, endPoint, test_world[item]))
                    {
                        if(_left)
                        {
                            duplicateCheck.Add(item);
                            temp.Enqueue(item);
                            leftSide.Add(item);

                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            v_test.transform.position = test_world[item];
                        }
                        else
                            continue;
                    }
                    else 
                    {
                        if (!_left)
                        {
                            duplicateCheck.Add(item);
                            temp.Enqueue(item);
                            rightSide.Add(item);
                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            v_test.transform.position = test_world[item];
                        }
                        else
                            continue;
                    }
                }
            }
        }
    }


    
        /// <summary>
        /// 각 triangle 별로 side 구분 해야됨.
        /// 여기서 반환해야하는 값은 intersection된 edgeIdx, _new의 index값 두 개(left, right side)

        /// </summary>
        /// <param name="_center"> start point 좌표</param>
        /// <param name="_new"> intersection point 좌표</param>
        /// <param name="_triangleIdx"> 현재 triangle index</param>
        /// <param name="_edgeIdx"> intersection된 edge</param>
        /// <param name="_vtxIdx"> intersection된 edge의 반대편 vtx</param>
        /// 

    // incision
    public void DivideTrianglesStartFromVtx(int _centerIdx, Vector3 _new, int _triangleIdx, int _edgeIdx)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 3];
        Vector3[] vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 1];

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        newVertices[vertices.Length] = obj.transform.InverseTransformPoint(_new);

        newTriangles[_triangleIdx] = _centerIdx;
        newTriangles[_triangleIdx + 1] = vertices.Length;
        newTriangles[_triangleIdx + 2] = edgeList[_edgeIdx].vtx2;

        int _triLen = triangles.Length;
        newTriangles[_triLen++] = _centerIdx;
        newTriangles[_triLen++] = edgeList[_edgeIdx].vtx1;
        newTriangles[_triLen++] = vertices.Length;

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }

    public void DivideTrianglesStart(Vector3 _center, Vector3 _new, int _triangleIdx, int _edgeIdx, int _vtxIdx, int _isInner)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 9];
        Vector3[] vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 3];

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        if (_isInner == 0)
        {
            // outer
            leftSide.Add(vertices.Length + 1);
            rightSide.Add(vertices.Length + 2);

            leftSide.Add(edgeList[_edgeIdx].vtx1);
            rightSide.Add(edgeList[_edgeIdx].vtx2);

        }
        else
        {
            // inner
            rightSide.Add(vertices.Length + 1);
            leftSide.Add(vertices.Length + 2);

            rightSide.Add(edgeList[_edgeIdx].vtx1);
            leftSide.Add(edgeList[_edgeIdx].vtx2);

        }

        newVertices[vertices.Length] = obj.transform.InverseTransformPoint(new Vector3(_center.x, _center.y, _center.z));
        // left side vtx
        newVertices[vertices.Length + 1] = obj.transform.InverseTransformPoint(_new);
        // right side vtx
        newVertices[vertices.Length + 2] = obj.transform.InverseTransformPoint(_new);

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

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }

    public void DivideTrianglesEnd(Vector3 _newCenter, int _triangleIdx, int _edgeIdx, int side, int _isInner, int check)
    {
        // 일단 _center랑 _new를 vertices에 추가하고
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 9];
        Vector3[] vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 1];

        // 이걸 미리하게되면 idx가 바뀌어버리나??  추가 생성이니까 상관 없을 것 같은데?
        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        newVertices[vertices.Length] = obj.transform.InverseTransformPoint(_newCenter);

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
        if(_isInner == 0)
        {
            // outer
            boundaryOuterEndVtx = vertices.Length;
            leftSide.Add(edgeList[newEdgeIdx].vtx2);
            rightSide.Add(edgeList[newEdgeIdx].vtx1);
        }
        else
        {
            // inner
            boundaryInnerEndVtx = vertices.Length;
            rightSide.Add(edgeList[newEdgeIdx].vtx2);
            leftSide.Add(edgeList[newEdgeIdx].vtx1);
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

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }

    public void DivideTrianglesClockWise(Vector3 _new, int _triangleIdx, int _intersectEdgeIdx, int _leftVtxIdx, int _rightVtxIdx, int _isInner)
    {
        // 이건 edge에 대해 다음 new vtx를 찾을 때 기존의 edge idx와 next edge idx를 비교해서 clock 인지 counterclock 인지를 판단 후 triangle 쪼개기
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 6];
        Vector3[] vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 2];
        int nextLength = -2;
        if (_intersectEdgeIdx - _triangleIdx != 2)
            nextLength = 1;
        // 이걸 미리하게되면 idx가 바뀌어버리나??  추가 생성이니까 상관 없을 것 같은데?
        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        if(_isInner==0)
        {
            leftSide.Add(vertices.Length);
            rightSide.Add(vertices.Length + 1);
            
            leftSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            rightSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            rightSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx2);
            
        }
        else
        {
            rightSide.Add(vertices.Length);
            leftSide.Add(vertices.Length + 1);
            
            rightSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            leftSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            leftSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx2);
            
        }
        // left
        newVertices[vertices.Length] = obj.transform.InverseTransformPoint(_new);
        
        // right
        newVertices[vertices.Length+1] = obj.transform.InverseTransformPoint(_new);
        

        // triangle 만드는 과정을 제대로 해야됨
        // 이 triangle은 지금 left side
        // intersectEdgeIdx는 지금 old edge임.
        newTriangles[_triangleIdx] = _leftVtxIdx;
        newTriangles[_triangleIdx + 1] = edgeList[_intersectEdgeIdx].vtx1;
        newTriangles[_triangleIdx + 2] = vertices.Length;
        int _triLen = triangles.Length;

        // 이 triangles는 right side
        newTriangles[_triLen++] = _rightVtxIdx;
        newTriangles[_triLen++] = vertices.Length+1;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx].vtx2;
        newTriangles[_triLen++] = _rightVtxIdx;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx1;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx2;
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }

    public void DivideTrianglesCounterClockWise(Vector3 _new, int _triangleIdx, int _intersectEdgeIdx, int _leftVtxIdx, int _rightVtxIdx, int _isInner)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        int[] triangles = obj.GetComponent<MeshFilter>().mesh.triangles;
        int[] newTriangles = new int[triangles.Length + 6];
        Vector3[] vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length + 2];
        
        int nextLength = 2;
        if (_intersectEdgeIdx - _triangleIdx != 0)
            nextLength = -1;

        for (int i = 0; i < triangles.Length; i++)
            newTriangles[i] = triangles[i];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

        // left
        newVertices[vertices.Length] = obj.transform.InverseTransformPoint(_new);
        if (_isInner == 0)
        {
            leftSide.Add(vertices.Length);
            rightSide.Add(vertices.Length + 1);
            
            rightSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            leftSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            leftSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx1);
            
        }
        else
        {
            rightSide.Add(vertices.Length);
            leftSide.Add(vertices.Length + 1);
            
            leftSide.Add(edgeList[_intersectEdgeIdx].vtx2);
            rightSide.Add(edgeList[_intersectEdgeIdx].vtx1);
            rightSide.Add(edgeList[_intersectEdgeIdx + nextLength].vtx1);
            
        }
        // right
        newVertices[vertices.Length+1] = obj.transform.InverseTransformPoint(_new);
        
            
        // right side
        newTriangles[_triangleIdx] = _rightVtxIdx;
        newTriangles[_triangleIdx + 1] = vertices.Length+1;
        newTriangles[_triangleIdx + 2] = edgeList[_intersectEdgeIdx].vtx2;
            
        int _triLen = triangles.Length;

        // left side
        newTriangles[_triLen++] = _leftVtxIdx;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx].vtx1;
        newTriangles[_triLen++] = vertices.Length;
        
        newTriangles[_triLen++] = _leftVtxIdx;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx1;
        newTriangles[_triLen++] = edgeList[_intersectEdgeIdx + nextLength].vtx2;
        
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }


    private int TriangleEdgeIntersection(ref int side, ref int edgeIdx, ref Vector3 edgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, int incisionPointIdx)
    {
        int intersectionCount = 0;
        int tempEdge = edgeIdx;
        
        Vector3 intersectionPoint = Vector3.zero;
        Vector3[] vertices = obj.GetComponent<MeshFilter>().sharedMesh.vertices;
        for (int i = 0; i < 3; i++)
        {
            int checkIntersectionCount = 2;
            if (tempEdge != -1 && edgeList[tempEdge].vtx1 == edgeList[incisionPointIdx + i].vtx2 && edgeList[tempEdge].vtx2 == edgeList[incisionPointIdx + i].vtx1)
                continue;
            // triangle : 1) s origin, e origin, incision start 2) e origin, incision start, incision end
            // intersectionTemp : test_world[edgeList[incisionPointIdx + i].vtx1], test_world[edgeList[incisionPointIdx + i].vtx2] - test_world[edgeList[incisionPointIdx + i].vtx1]
            if (RayTriangleIntersection(screenStartOrigin, screenEndOrigin, incisionStartPoint + screenStartDirection * 5, test_world[edgeList[incisionPointIdx + i].vtx1], test_world[edgeList[incisionPointIdx + i].vtx2] - test_world[edgeList[incisionPointIdx + i].vtx1]))
            {
                Debug.Log("ray triangle intersection");
                if (intersectionTemp.x < Mathf.Min(test_world[edgeList[incisionPointIdx + i].vtx1].x, test_world[edgeList[incisionPointIdx + i].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[incisionPointIdx + i].vtx1].x, test_world[edgeList[incisionPointIdx + i].vtx2].x))
                    checkIntersectionCount--;
                else
                    intersectionPoint = intersectionTemp;
            }
            else
                checkIntersectionCount--;

            if (RayTriangleIntersection(screenEndOrigin, incisionEndPoint + screenEndDirection * 5, incisionStartPoint + screenStartDirection * 5, test_world[edgeList[incisionPointIdx + i].vtx1], test_world[edgeList[incisionPointIdx + i].vtx2] - test_world[edgeList[incisionPointIdx + i].vtx1]))
            {
                Debug.Log("ray triangle intersection");
                if (intersectionTemp.x < Mathf.Min(test_world[edgeList[incisionPointIdx + i].vtx1].x, test_world[edgeList[incisionPointIdx + i].vtx2].x) || intersectionTemp.x > Mathf.Max(test_world[edgeList[incisionPointIdx + i].vtx1].x, test_world[edgeList[incisionPointIdx + i].vtx2].x))
                    checkIntersectionCount--;
                else
                    intersectionPoint = intersectionTemp;
            }
            else
                checkIntersectionCount--;
            
            if (checkIntersectionCount != 0)
            {
                edgeIdx = incisionPointIdx + i;
                edgePoint = intersectionPoint;
                intersectionCount++;
                /*
                GameObject v_test = new GameObject();
                v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                v_test.transform.position = intersectionPoint;
                */
            }
        }

        if (tempEdge == -1)
        {
            return intersectionCount;
        }
        else if (edgeList[tempEdge].vtx1 == edgeList[edgeIdx].vtx1)
        {
            side = 1;
            // counter clock wise
            // 어떤값을 넘기지?
        }
        else if (edgeList[tempEdge].vtx2 == edgeList[edgeIdx].vtx2)
        {
            // clock wise
            side = 2;
        }
        Debug.Log(side);
        Debug.Log(intersectionCount);
        // 여기서 추가로 clock wise인지 아닌지에대해서도 판단하는게 좋을듯 싶음.
        return intersectionCount;
    }

    private void SetColor()
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        colors = new Color[mesh.vertexCount];
        int j = 0;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            // colors[j++] = Color.red;
            colors[j++] = Color.white;
        }
        mesh.colors = colors;
    }

    private IEnumerator CreateVerticesToObj()
    {
        Vector3[] v_pos = obj.GetComponent<MeshFilter>().sharedMesh.vertices;
        GameObject v_test = new GameObject();
        for (int i = 0; i < v_pos.Length; i++)
        {
            v_test = new GameObject();
            //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            v_test.transform.SetParent(obj.transform);
            v_test.name = "v_test" + i;
            v_test.transform.localPosition = v_pos[i];
        }
        Debug.Log("stop coroutine");
        yield return null;
    }
    
    // Ray/Triangle intersection 
    private bool RayTriangleIntersection(Vector3 v0, Vector3 v1, Vector3 v2, Ray ray)
    {
        Vector3 e1, e2, T, P;
        float Epsilon = 0.000001f;
        e1 = v1 - v0;
        e2 = v2 - v0;

        T = ray.origin - v0;

        P = Vector3.Cross(ray.direction, e2);
        var det = Vector3.Dot(e1, P);

        if (det > -Epsilon && det < Epsilon)
            return false;

        var invDet = 1 / det;


        var u = invDet * Vector3.Dot(T, P);

        if (u > 1 || u < 0)
            return false;

        var v = invDet * Vector3.Dot(ray.direction, Vector3.Cross(T, e1));

        if (v < 0 || u + v > 1)
            return false;

        var t = invDet * Vector3.Dot(e2, Vector3.Cross(T, e1));

        if (t > Epsilon)
        {
            intersectionTemp = ray.origin + ray.direction * t;
            return true;
        }
        return false;
    }

    private bool RayTriangleIntersection(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 origin, Vector3 direction)
    {
        Vector3 e1, e2, T, P;
        float Epsilon = 0.000001f;
        e1 = v1 - v0;
        e2 = v2 - v0;

        T = origin - v0;

        P = Vector3.Cross(direction, e2);
        var det = Vector3.Dot(e1, P);

        if (det > -Epsilon && det < Epsilon)
            return false;

        var invDet = 1 / det;


        var u = invDet * Vector3.Dot(T, P);

        if (u > 1 || u < 0)
            return false;

        var v = invDet * Vector3.Dot(direction, Vector3.Cross(T, e1));

        if (v < 0 || u + v > 1)
            return false;

        var t = invDet * Vector3.Dot(e2, Vector3.Cross(T, e1));

        if (t > Epsilon)
        {
            intersectionTemp = origin + (direction * t);
            return true;
        }

        return false;
    }

    private double QuadraticEquation(double x1, double y1, double x2, double y2, double radius)
    {
        double a = x2 * x2 + y2 * y2;
        double b = 2 * x1 * x2 + 2 * y1 * y2;
        double c = x1 * x1 + y1 * y1 - radius * radius;
        double d = (b * b) - (4 * a * c);
        if (d > 0)
        {
            double e = Math.Sqrt(d);
            if ((-b + e) / (2.0 * a) >= 0)
                return (-b + e) / (2.0 * a);
            else
                return (-b - e) / (2.0 * a);
        }
        else
        {
            if (d == 0)
                return (-b) / (2.0 * a);
            else
                Debug.Log("허근");
        }
        return 0;
    }
}
