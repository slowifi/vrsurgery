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

    private Ray oldRay;
    private Ray firstRay;
    private List<Ray> rayList;
    private List<Vector3> intersectedVerticesPos;

    private LineRendererManipulate lineRenderer;

    private Material heartMaterial;
    void Awake()
    {
        lineRenderer = new LineRendererManipulate();
        intersectedVerticesPos = new List<Vector3>();
        rayList = new List<Ray>();

        boundaryCount = 0;

        isFirst = true;
        isLastBoundaryCut = false;
        isIntersected = true;

        heartMaterial = Resources.Load("Materials/Heart", typeof(Material)) as Material;
    }

    void Update()
    {
        if (isFirst)
        {
            Debug.Log("Boundary cut 실행");
            MeshManager.Instance.SaveCurrentMesh();
            AdjacencyList.Instance.ListUpdate();
            isFirst = false;
            boundaryCount = 0;
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.GetIntersectedValues();

            rayList.Add(ray);
            oldRay = ray;
            firstRay = ray;
            boundaryCount++;
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
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.GetIntersectedValues();

            lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
            if(boundaryCount>8 && Vector3.Distance(firstRay.origin, ray.origin)<0.01f)
            {
                lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, firstRay.origin + firstRay.direction * 100f);
                CGALCut();
                Destroy(lineRenderer.lineObject);
                Destroy(this);
            }
            else if (Vector3.Distance(oldRay.origin, ray.origin) > 0.005f)
            {
                boundaryCount++;
                Debug.Log("intersected");
                lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
                oldRay = ray;
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
            Destroy(this);
        }
    }

    private void CGALCut()
    {
        if (isIntersected)
        {
            AdjacencyList.Instance.ListUpdate();
            Debug.Log("intersected true");
            IntPtr heart = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

            if (CGAL.BuildPolyhedron(heart,
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

            
            CGAL.ClipPolyhedronByMesh(heart, stamp);
            MeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(heart, heartMaterial));
            MakeDoubleFaceMesh.Instance.Reinitialize();
            //CGAL.GenerateNewObject(stamp, leftMaterial);
            // 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.
            //MeshManager.Instance.Heart.SetActive(false);

        }
        else
        {
            IntPtr heart = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

            if (CGAL.BuildPolyhedron(heart,
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
            CGAL.ClipPolyhedronByMesh(heart, stamp);
            MeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(heart, heartMaterial));
            MakeDoubleFaceMesh.Instance.Reinitialize();
            // 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.

            //MeshManager.Instance.Heart.SetActive(false);
        }
    }
}