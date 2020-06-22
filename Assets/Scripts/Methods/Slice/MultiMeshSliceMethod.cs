using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MultiMeshSliceMethod
{
    private Ray firstRay = new Ray();
    private Ray secondRay = new Ray();
    private Material leftMaterial = Resources.Load("Materials/LeftMaterial", typeof(Material)) as Material;
    private Material rightMaterial = Resources.Load("Materials/RightMaterial", typeof(Material)) as Material;

    private List<Vector3> leftWorldPos;
    private List<Vector3> rightWorldPos;
    private Vector3 middlePosition;
    private IntPtr[] MultiMeshLeft = new IntPtr[MultiMeshManager.Instance.Size];
    private IntPtr[] MultiMeshRight = new IntPtr[MultiMeshManager.Instance.Size];
    private GameObject[] LeftHeartPart = new GameObject[MultiMeshManager.Instance.Size];
    private GameObject[] RightHeartPart = new GameObject[MultiMeshManager.Instance.Size];
    private GameObject[] LeftResult = new GameObject[MultiMeshManager.Instance.Size];
    private GameObject[] RightResult = new GameObject[MultiMeshManager.Instance.Size];
    private GameObject[] TotalResult = new GameObject[MultiMeshManager.Instance.Size * 2];
    private List<float[]> verticesCoordinates;

    public void SetIntersectedValues(string type, Ray value)
    {
        switch (type)
        {
            case "first":
                firstRay = value;
                break;
            case "second":
                secondRay = value;
                break;
        }
    }

    public GameObject[] MultiMeshSlicing()
    {
        int k = 0;
        MultiMeshAdjacencyList.Instance.ListsUpdate();

        for (int j = 0; j < MultiMeshManager.Instance.Size; j++)
        {
            MultiMeshLeft[j] = CGAL.CreateMeshObject();
            MultiMeshRight[j] = CGAL.CreateMeshObject();
        }

        middlePosition = Vector3.Lerp(firstRay.origin, secondRay.origin, 0.5f);
        verticesCoordinates = new List<float[]>();
        for (int w = 0; w < MultiMeshManager.Instance.Size; w++)
        {
            verticesCoordinates.Add(CGAL.ConvertToFloatArray(MultiMeshAdjacencyList.Instance.MultiMeshWorldPositionVertices[w].ToArray()));
            if (CGAL.BuildPolyhedron(MultiMeshLeft[w],
                verticesCoordinates.ElementAt(w),
                verticesCoordinates.ElementAt(w).Length / 3,
                MultiMeshManager.Instance.meshes[w].triangles,
                MultiMeshManager.Instance.meshes[w].triangles.Length / 3) == -1)
            {
                Debug.Log("만들어지지 않음");
            }
            if (CGAL.BuildPolyhedron(MultiMeshRight[w],
                verticesCoordinates.ElementAt(w),
                verticesCoordinates.ElementAt(w).Length / 3,
                MultiMeshManager.Instance.meshes[w].triangles,
                MultiMeshManager.Instance.meshes[w].triangles.Length / 3) == -1)
            {
                Debug.Log("만들어지지 않음");
            }
        }
        try
        {
            // ClipPolyhedronByPlane 함수 실행중에 에러가 생기는 것같습니다!

            for (int w = 0; w < MultiMeshManager.Instance.Size; w++)
            {
                if (
                    CGAL.ClipPolyhedronByPlane(
                    MultiMeshLeft[w],
                    CGAL.GeneratePlane(
                        middlePosition,
                        firstRay.origin + firstRay.direction * 10f,
                        secondRay.origin + secondRay.direction * 10f)) == -1)
                {
                    Debug.Log("만들어지지 않음");
                }

                if (CGAL.ClipPolyhedronByPlane(
                    MultiMeshRight[w],
                    CGAL.GeneratePlane(
                        middlePosition,
                        secondRay.origin + secondRay.direction * 10f,
                        firstRay.origin + firstRay.direction * 10f)) == -1)
                {
                    Debug.Log("만들어지지 않음");
                }

                LeftHeartPart[w] = CGAL.GenerateLeftNewObject(MultiMeshLeft[w], leftMaterial, w);
                RightHeartPart[w] = CGAL.GenerateRightNewObject(MultiMeshRight[w], rightMaterial, w);
                MultiMeshManager.Instance.HeartParts[w].SetActive(false);

                LeftResult[w] = LeftHeartPart[w];
                RightResult[w] = RightHeartPart[w];

                TotalResult[k++] = LeftResult[w];
                TotalResult[k++] = RightResult[w];
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return TotalResult;
    }
}
