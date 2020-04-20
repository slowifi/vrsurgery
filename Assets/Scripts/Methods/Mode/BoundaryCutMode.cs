using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class BoundaryCutMode : Mode
{
    private bool isLastBoundaryCut;
    private int boundaryCount;
    private bool isFirst;
    private bool isIntersected;
    //private GameObject lineRenderer;
    private Vector3 firstPosition;
    private Vector3 oldPosition;
    private Ray oldRay;
    private Ray firstRay;
    public GameObject playerObject;
    public GameObject mainObject;
    //private BoundaryCutManager BoundaryCutManager;
    private List<Ray> rayList;
    private List<Vector3> intersectedVerticesPos;

    private LineRendererManipulate lineRenderer;

    private Material leftMaterial;
    private Material rightMaterial;

    //private void Cut()
    //{
    //    bool checkError = true;
    //    // 이걸 뒤에 넣어서 한프레임 늦게 실행 되도록 하기.
    //    checkError = BoundaryCutManager.PostProcess();
    //    if (!checkError)
    //    {
    //        Destroy(lineRenderer);
    //        Destroy(this);
    //        // return true;
    //    }
    //    MeshManager.Instance.mesh.RecalculateNormals();

    //    Destroy(lineRenderer);
    //    AdjacencyList.Instance.ListUpdate();
    //    if (!BoundaryCutManager.AutomaticallyRemoveTriangles())
    //    {
    //        ChatManager.Instance.GenerateMessage(" 영역이 잘못 지정되었습니다.");
    //        MeshManager.Instance.LoadOldMesh();
    //    }
    //    else
    //        MeshManager.Instance.SaveCurrentMesh();
    //    AdjacencyList.Instance.ListUpdate();
    //    MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
    //    BoundaryCutManager.BoundaryCutUpdate();
    //    Destroy(this);
    //    // return true;
    //}

    void Awake()
    {
        leftMaterial = Resources.Load("Materials/LeftMaterial", typeof(Material)) as Material;
        rightMaterial = Resources.Load("Materials/RightMaterial", typeof(Material)) as Material;
        //BoundaryCutManager = gameObject.AddComponent<BoundaryCutManager>();
        lineRenderer = new LineRendererManipulate();
        rayList = new List<Ray>();
        intersectedVerticesPos = new List<Vector3>();
        boundaryCount = 0;
        isFirst = true;
        playerObject = gameObject;
        isLastBoundaryCut = false;
        isIntersected = true;
    }

    void Update()
    {
        Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
        bool checkInside = intersectedValues.Intersected;

        // 조건을 잘 짜야됨.
        if (isFirst)
        {
            Debug.Log("Boundary cut 실행");
            //playerObject.SetActive(false);
            MeshManager.Instance.SaveCurrentMesh();
            AdjacencyList.Instance.ListUpdate();
            isFirst = false;
            boundaryCount = 0;
            return;
        }

        //if (isLastBoundaryCut)
        //{
        //    Cut();
        //    return;
        //}
        if(Input.GetMouseButtonDown(0))
        {
            rayList.Add(ray);
            oldRay = ray;
            firstRay = ray;
            if (intersectedValues.Intersected)
            {
                intersectedVerticesPos.Add(intersectedValues.IntersectedPosition);
            }
            else
            {
                isIntersected = false;
            }
        }
        else if(Input.GetMouseButton(0))
        {
            lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
            if (Vector3.Distance(oldRay.origin, ray.origin) > 0.005f)
            {
                Debug.Log("intersected");
                lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
                oldRay = ray;
                //oldPosition = ray.origin;
                rayList.Add(ray);
                if (intersectedValues.Intersected)
                {
                    intersectedVerticesPos.Add(intersectedValues.IntersectedPosition);
                }
                else
                {
                    isIntersected = false;
                }
            }
            
        }
        else if(Input.GetMouseButtonUp(0))
        {
            lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, firstRay.origin + firstRay.direction * 100f);
            CGALCut();
            Destroy(lineRenderer.lineObject);
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("실행");
        //    //test();
        //    //AdjacencyList.Instance.ListUpdate();
        //    boundaryCount = 0;
        //    oldPosition = Vector3.zero;
        //    Vector3 startVertexPosition = Vector3.zero;
        //    int startTriangleIndex = -1;

        //    startVertexPosition = intersectedValues.IntersectedPosition;
        //    startTriangleIndex = intersectedValues.TriangleIndex;

        //    if (checkInside)
        //    {
        //        BoundaryCutManager.rays.Add(ray);
        //        BoundaryCutManager.intersectedPosition.Add(startVertexPosition);
        //        BoundaryCutManager.startTriangleIndex = startTriangleIndex;

        //        oldPosition = startVertexPosition;
        //        firstPosition = oldPosition;
        //        boundaryCount++;
        //    }
        //    else
        //    {
        //        ChatManager.Instance.GenerateMessage("intersect 되지 않음.");
        //    }
        //}
        //else if (Input.GetMouseButton(0))
        //{
        //    Vector3 currentPosition = Vector3.zero;
        //    if (checkInside)
        //    {
        //        Debug.Log(boundaryCount);
        //        currentPosition = intersectedValues.IntersectedPosition;
        //        if (boundaryCount > 3 && Vector3.Distance(currentPosition, firstPosition) < 2f * ObjManager.Instance.pivotTransform.lossyScale.z)
        //        {

        //            var line = lineRenderer.GetComponent<LineRenderer>();
        //            line.positionCount++;
        //            //line.positionCount++;

        //            line.SetPosition(boundaryCount - 1, oldPosition);
        //            line.SetPosition(boundaryCount, firstPosition);
        //            line.GetComponent<LineRenderer>().material.color = Color.blue;
        //            //EditorApplication.isPaused = true;
        //            ChatManager.Instance.GenerateMessage(" 작업이 진행중입니다아122.");
        //            isLastBoundaryCut = true;
        //        }
        //        else if (Vector3.Distance(currentPosition, oldPosition) < 1.5f * ObjManager.Instance.pivotTransform.lossyScale.z)
        //        {
        //            if (oldPosition == Vector3.zero)
        //                return;
        //            if (lineRenderer)
        //            {
        //                var line = lineRenderer.GetComponent<LineRenderer>();
        //                line.SetPosition(boundaryCount - 1, currentPosition);
        //            }

        //            return;
        //        }
        //        else if (boundaryCount == 1)
        //        {
        //            BoundaryCutManager.rays.Add(ray);
        //            BoundaryCutManager.intersectedPosition.Add(currentPosition);
        //            lineRenderer = new GameObject("Boundary Line", typeof(LineRenderer));
        //            lineRenderer.layer = 8;
        //            var line = lineRenderer.GetComponent<LineRenderer>();
        //            line.numCornerVertices = 45;
        //            line.material.color = Color.black;
        //            //var line = lineRenderer.GetComponent<LineRenderer>();

        //            line.SetPosition(0, oldPosition);
        //            line.SetPosition(boundaryCount++, currentPosition);

        //            oldPosition = currentPosition;
        //        }
        //        else
        //        {
        //            if (boundaryCount == 0)
        //                return;
        //            var line = lineRenderer.GetComponent<LineRenderer>();
        //            line.positionCount++;
        //            line.SetPosition(boundaryCount++, currentPosition);

        //            BoundaryCutManager.rays.Add(ray);
        //            BoundaryCutManager.intersectedPosition.Add(currentPosition);

        //            oldPosition = currentPosition;
        //            //boundaryCount++;
        //        }
        //    }
        //    else
        //    {
        //        if (boundaryCount == 0)
        //            return;
        //        Destroy(lineRenderer);
        //        BoundaryCutManager.BoundaryCutUpdate();
        //        ChatManager.Instance.GenerateMessage(" 심장이 아닙니다.");
        //        Destroy(this);
        //        // return true;
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    if (boundaryCount == 0)
        //        return;

        //    if (checkInside)
        //    {
        //        var line = lineRenderer.GetComponent<LineRenderer>();
        //        line.positionCount++;
        //        line.material.color = Color.blue;
        //        line.SetPosition(boundaryCount - 1, oldPosition);
        //        line.SetPosition(boundaryCount, firstPosition);
        //        //EditorApplication.isPaused = true;
        //        ChatManager.Instance.GenerateMessage(" 작업이 진행중입니다.");

        //        isLastBoundaryCut = true;
        //    }
        //}
    }

    private void CGALCut()
    {
        if (isIntersected)
        {
            AdjacencyList.Instance.ListUpdate();
            Debug.Log("intersected true");
            IntPtr left = CGAL.CreateMeshObject();
            //IntPtr right = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

            if (CGAL.BuildPolyhedron(left,
                verticesCoordinate,
                verticesCoordinate.Length / 3,
                MeshManager.Instance.mesh.triangles,
                MeshManager.Instance.mesh.triangles.Length / 3) == -1)
            {
                Debug.Log(" 만들어지지 않음");
            }

            Vector3[] newVertices = new Vector3[rayList.Count * 2];
            int[] newTriangles = new int[rayList.Count * 6];
            CGAL.GenerateStampWithHeart(intersectedVerticesPos, rayList, ref newVertices, ref newTriangles);


            float[] newVerticesCoordinate = CGAL.ConvertToFloatArray(newVertices);
            if (CGAL.BuildPolyhedron(
                stamp,
                newVerticesCoordinate,
                newVerticesCoordinate.Length / 3,
                newTriangles,
                newTriangles.Length / 3
                ) == -1)
            {
                Debug.Log(" 만들어지지 않음");
            }
            CGAL.FillHole(stamp);

            
            CGAL.ClipPolyhedronByMesh(left, stamp);
            CGAL.GenerateNewObject(left, leftMaterial);
            //CGAL.GenerateNewObject(stamp, leftMaterial);
            // 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.
            // 라인렌더러 넣어줘야함.

            MeshManager.Instance.Heart.SetActive(false);
            
        }
        else
        {
            IntPtr left = CGAL.CreateMeshObject();
            //IntPtr right = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

            if (CGAL.BuildPolyhedron(left,
                verticesCoordinate,
                verticesCoordinate.Length / 3,
                MeshManager.Instance.mesh.triangles,
                MeshManager.Instance.mesh.triangles.Length / 3) == -1)
            {
                Debug.Log(" 만들어지지 않음");
            }

            Vector3[] newVertices = new Vector3[rayList.Count * 2];
            int[] newTriangles = new int[rayList.Count * 6];
            CGAL.GenerateStamp(rayList, ref newVertices, ref newTriangles);


            float[] newVerticesCoordinate = CGAL.ConvertToFloatArray(newVertices);
            if (CGAL.BuildPolyhedron(
                stamp,
                newVerticesCoordinate,
                newVerticesCoordinate.Length / 3,
                newTriangles,
                newTriangles.Length / 3
                ) == -1)
            {
                Debug.Log(" 만들어지지 않음");
            }
            CGAL.FillHole(stamp);

            CGAL.ClipPolyhedronByMesh(left, stamp);
            CGAL.GenerateNewObject(left, leftMaterial);
            
            // 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.
            // 라인렌더러 넣어줘야함.

            MeshManager.Instance.Heart.SetActive(false);
        }
    }
}