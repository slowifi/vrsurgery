using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiMeshBoundaryCutMode : MonoBehaviour
{
    private bool isLast;
    private bool isFirst;
    private bool isIntersected;

    private Ray oldRay;
    private Ray firstRay;
    private List<Ray> rayList;
    private List<Vector3> intersectedVerticesPos;

    private LineRendererManipulate lineRenderer;

    private Material heartMaterial;
    private GameObject FirstHitObject;
    private GameObject SecondHitObject;
    
    private int HitOBJIndex;
    private int boundaryCount;
    private int Size;

    void Awake()
    {
        heartMaterial = Resources.Load("Materials/Heart", typeof(Material)) as Material;
        lineRenderer = new LineRendererManipulate(transform);
        intersectedVerticesPos = new List<Vector3>();
        rayList = new List<Ray>();

        isFirst = true;
        isLast = false;
        isIntersected = true;

        boundaryCount = 0;
        Size = MultiMeshManager.Instance.Size;
    }
    void Update()
    {
        if (isFirst)
        {
            Debug.Log("Boundary cut 실행");
            MultiMeshAdjacencyList.Instance.Initialize();
            isFirst = false;
            boundaryCount = 0;
            return;
        }
        else if (isLast)
        {
            CGALCut();
            MultiMeshAdjacencyList.Instance.Initialize();
            GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().MeshList[HitOBJIndex].Add(Instantiate(MultiMeshManager.Instance.Meshes[HitOBJIndex]));
            //GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().MeshList[HitOBJIndex].Add(Instantiate(GameObject.Find("PartialModel").transform.GetChild(HitOBJIndex).transform.GetChild(0).GetComponent<MeshFilter>().mesh));
            GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().IBSave(GameObject.Find("Main").GetComponent<CHD>().MeshIndex);
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            EventManager.Instance.Events.InvokeModeChanged("ResetButton");
            GameObject.Find("Main").GetComponent<CHD>().AllButtonInteractable();
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
                return;

            for (int i = 0; i < Size; i++)
                if (FirstHitObject.name == GameObject.Find("PartialModel").transform.GetChild(i).name)
                    HitOBJIndex = i;

            GameObject.Find("Main").GetComponent<CHD>().MeshIndex = HitOBJIndex;
            IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);

            rayList.Add(ray);
            oldRay = ray;
            firstRay = ray;
            boundaryCount++;

            if (intersectedValues.Intersected)
                intersectedVerticesPos.Add(intersectedValues.IntersectedPosition);
            else
                isIntersected = false;
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
                    isIntersected = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100f, firstRay.origin + firstRay.direction * 100f);
            isLast = true;
        }
    }
    private void CGALCut()
    {
        if (isIntersected)
        {
            Debug.Log("intersected true");
            MultiMeshAdjacencyList.Instance.Initialize();

            IntPtr HeartPart = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();

            float[] verticesCoordinate = CGAL.ConvertToFloatArray(MultiMeshAdjacencyList.Instance.WorldPositionVertices[HitOBJIndex].ToArray());

            if (CGAL.BuildPolyhedron(HeartPart,
                verticesCoordinate,
                verticesCoordinate.Length / 3,
                MultiMeshManager.Instance.Meshes[HitOBJIndex].triangles,
                MultiMeshManager.Instance.Meshes[HitOBJIndex].triangles.Length / 3) == -1)
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

            MultiMeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(HeartPart, heartMaterial, HitOBJIndex), HitOBJIndex);
            MultiMeshManager.Instance.ReInitialize();
        }
        else
        {
            IntPtr HeartPart = CGAL.CreateMeshObject();
            IntPtr stamp = CGAL.CreateMeshObject();
            float[] verticesCoordinate = CGAL.ConvertToFloatArray(MultiMeshAdjacencyList.Instance.WorldPositionVertices[HitOBJIndex].ToArray());

            if (CGAL.BuildPolyhedron(HeartPart,
                verticesCoordinate,
                verticesCoordinate.Length / 3,
                MultiMeshManager.Instance.Meshes[HitOBJIndex].triangles,
                MultiMeshManager.Instance.Meshes[HitOBJIndex].triangles.Length / 3) == -1)
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
            MultiMeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(HeartPart, heartMaterial, HitOBJIndex), HitOBJIndex);
            MultiMeshManager.Instance.ReInitialize();
        }
    }
}