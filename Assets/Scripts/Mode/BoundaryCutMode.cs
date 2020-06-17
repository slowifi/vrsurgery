using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class BoundaryCutMode : MonoBehaviour
{
    private bool isLast;
    private int boundaryCount;
    private bool isFirst;
    private bool isIntersected;

    private Ray oldRay;
    private Ray firstRay;
    private List<Ray> rayList;
    private List<Vector3> intersectedVerticesPos;
    private List<Vector3>[] MultiMeshintersectedVerticesPos;

    // 라인렌더러는 한곳에서 관리하는게 나을지도 모르겠음.
    private LineRendererManipulate lineRenderer;

    private Material heartMaterial;

    void Awake()
    {
        MultiMeshintersectedVerticesPos = new List<Vector3>[MultiMeshManager.Instance.Size];
        lineRenderer = new LineRendererManipulate(transform);
        intersectedVerticesPos = new List<Vector3>();
        rayList = new List<Ray>();

        boundaryCount = 0;

        isFirst = true;
        
        isLast = false;
        isIntersected = true;

        heartMaterial = Resources.Load("Materials/Heart", typeof(Material)) as Material;
    }

    void Update()
    {
        if (isFirst)
        {
            Debug.Log("Boundary cut 실행");
            //MeshManager.Instance.SaveCurrentMesh();
            //AdjacencyList.Instance.ListUpdate();
            MultiMeshAdjacencyList.Instance.ListsUpdate();
            isFirst = false;
            boundaryCount = 0;
            return;
        }
        else if(isLast)
        {
            CGALCut();
            AdjacencyList.Instance.ListUpdate();
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            //EventManager.Instance.Events.InvokeModeChanged("ResetButton");
            Destroy(lineRenderer.lineObject);
            Destroy(this);
        }
        else if(Input.GetMouseButtonDown(0))
        {
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
            //Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues[] intersectedValues = new IntersectedValues[MultiMeshManager.Instance.Size];
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                intersectedValues[i] = Intersections.MultiMeshGetIntersectedValues(i);

                rayList.Add(ray);
                oldRay = ray;
                firstRay = ray;
                boundaryCount++;

                if (intersectedValues[i].Intersected)
                    intersectedVerticesPos.Add(intersectedValues[i].IntersectedPosition);
                else
                    isIntersected = false;

            }
            //ntersectedValues intersectedValues = Intersections.GetIntersectedValues();
            
            //rayList.Add(ray);
            //oldRay = ray;
            //firstRay = ray;
            //boundaryCount++;

            //if (intersectedValues.Intersected)
            //{
            //    intersectedVerticesPos.Add(intersectedValues.IntersectedPosition);
            //}
            //else
            //{
            //    isIntersected = false;
        }
        else if(Input.GetMouseButton(0))
        {
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.GetIntersectedValues();

            lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
            if(boundaryCount>8 && Vector3.Distance(firstRay.origin, ray.origin)<0.01f)
            {
                lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, firstRay.origin + firstRay.direction * 100f);
                isLast = true;
            }
            else if (Vector3.Distance(oldRay.origin, ray.origin) > 0.005f)
            {
                boundaryCount++;
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
            isLast = true;
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
                return;
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
                return;
            }
            if(CGAL.FillHole(stamp)==-1)
            {
                Debug.Log("fillhole error");
                return;
            }

            if (CGAL.ClipPolyhedronByMesh(heart, stamp) == -1)
            {
                Debug.Log("Clip error");
                return;
            }
            MeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(heart, heartMaterial));
            MakeDoubleFaceMesh.Instance.Reinitialize();
            ////CGAL.GenerateNewObject(stamp, leftMaterial);
            //// 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.
            ////MeshManager.Instance.Heart.SetActive(false);
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
                return;
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
                return;
            }

            if (CGAL.FillHole(stamp) == -1)
            {
                Debug.Log("fillhole error");
                return;
            }

            if (CGAL.ClipPolyhedronByMesh(heart, stamp) == -1)
            {
                Debug.Log("Clip error");
                return;
            }
            MeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(heart, heartMaterial));
            MakeDoubleFaceMesh.Instance.Reinitialize();
            //// 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.

            ////MeshManager.Instance.Heart.SetActive(false);
        }
    }
}