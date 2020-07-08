using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MultiMeshBoundaryCutMode : MonoBehaviour
{
    private bool isLast;
    private int boundaryCount;
    private bool isFirst;
    private bool isIntersected;

    private Ray oldRay;
    private Ray firstRay;
    private List<Ray> rayList;
    private List<Vector3> intersectedVerticesPos;

    // 라인렌더러는 한곳에서 관리하는게 나을지도 모르겠음.
    private LineRendererManipulate lineRenderer;

    private Material heartMaterial;
    private GameObject FirstHitObject;
    private GameObject SecondHitObject;
    private int HitOBJIndex;


    void Awake()
    {
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
            MultiMeshAdjacencyList.Instance.ListsUpdate();
            isFirst = false;
            boundaryCount = 0;
            return;
        }
        else if (isLast)
        {
            MultiMeshCGALCut();
            MultiMeshAdjacencyList.Instance.ListsUpdate();
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            Destroy(lineRenderer.lineObject);
            Destroy(this);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
            RaycastHit FirstHit;
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out FirstHit, 1000f))
                FirstHitObject = FirstHit.collider.gameObject;
            else
            { 
                Debug.Log("빈공간입니다.");
                return;
            }
            
            for (int i=0;i<MultiMeshManager.Instance.Size;i++)
            {
                if (FirstHitObject.name == GameObject.Find("PartialModel").transform.GetChild(i).name)
                    HitOBJIndex = i;
            }

            GameObject.Find("Main").GetComponent<CHD>().MeshIndex = HitOBJIndex;  // Save index

            IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);

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
        else if (Input.GetMouseButton(0))
        {
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit SecondHit;
            
            if (Physics.Raycast(ray, out SecondHit, 1000f))
                SecondHitObject = SecondHit.collider.gameObject;
            else
            {
                Debug.Log("영역을 이탈했습니다.");
                Destroy(lineRenderer.lineObject);
                Destroy(this);
                return;
            }
            Debug.Log("name : " + SecondHitObject.name);
            if (SecondHitObject.name != FirstHitObject.name)
            {
                Debug.Log("영역을 이탈했습니다.");
                Destroy(lineRenderer.lineObject);
                Destroy(this);
                return;
            }

            IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);

            lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);

            if (boundaryCount > 8 && Vector3.Distance(firstRay.origin, ray.origin) < 0.01f)
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
                    Debug.Log("intersectedVerticesPosCount : " + intersectedVerticesPos.Count);
                    intersectedVerticesPos.Add(intersectedValues.IntersectedPosition);
                }
                else
                {
                    isIntersected = false;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("MouseUP");
            lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, firstRay.origin + firstRay.direction * 100f);
            
            isLast = true;
        }
    }
    
    private void MultiMeshCGALCut()
    {
        if (isIntersected)
        {
            MultiMeshAdjacencyList.Instance.ListsUpdate();
            Debug.Log("intersected true");
            IntPtr HeartPart = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(MultiMeshAdjacencyList.Instance.MultiMeshWorldPositionVertices[HitOBJIndex].ToArray());
        
            if (CGAL.BuildPolyhedron(HeartPart,
                verticesCoordinate,
                verticesCoordinate.Length / 3,
                MultiMeshManager.Instance.meshes[HitOBJIndex].triangles,
                MultiMeshManager.Instance.meshes[HitOBJIndex].triangles.Length / 3) == -1)
            {
                Debug.Log(" 만들어지지 않음");
                return;
            }

            Vector3[] newVertices = new Vector3[rayList.Count * 2];
            int[] newTriangles = new int[rayList.Count * 6];
            CGAL.MultiMeshGenerateStampWithHeart(intersectedVerticesPos, rayList, ref newVertices, ref newTriangles, HitOBJIndex);


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
            if (CGAL.ClipPolyhedronByMesh(HeartPart, stamp) == -1)
            {
                Debug.Log("Clip error");
                return;
            }
            MultiMeshManager.Instance.SetNewObjects(CGAL.GenerateNewObject(HeartPart, heartMaterial, HitOBJIndex), HitOBJIndex);
            MultiMeshManager.Instance.InitSingleFace();
            //MultiMeshMakeDoubleFace.Instance.Reinitialize();            
        }
        else
        {
            IntPtr HeartPart = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(MultiMeshAdjacencyList.Instance.MultiMeshWorldPositionVertices[HitOBJIndex].ToArray());

            if (CGAL.BuildPolyhedron(HeartPart,
                verticesCoordinate,
                verticesCoordinate.Length / 3,
                MultiMeshManager.Instance.meshes[HitOBJIndex].triangles,
                MultiMeshManager.Instance.meshes[HitOBJIndex].triangles.Length / 3) == -1)
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

            if (CGAL.ClipPolyhedronByMesh(HeartPart, stamp) == -1)
            {
                Debug.Log("Clip error");
                return;
            }
            MultiMeshManager.Instance.SetNewObjects(CGAL.GenerateNewObject(HeartPart, heartMaterial,HitOBJIndex),HitOBJIndex);
            MultiMeshManager.Instance.InitSingleFace();
            //MultiMeshMakeDoubleFace.Instance.Reinitialize();
            //// 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.

            ////MeshManager.Instance.Heart.SetActive(false);
        }
    }
}