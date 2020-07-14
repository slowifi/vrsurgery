using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MultiMeshSliceMethod
{
    private Ray firstRay;
    private Ray secondRay;

    private IntPtr[] Left;
    private IntPtr[] Right;

    private Vector3 middlePosition;

    private Material leftMaterial = Resources.Load("Materials/LeftMaterial", typeof(Material)) as Material;
    private Material rightMaterial = Resources.Load("Materials/RightMaterial", typeof(Material)) as Material;

    private List<float[]> verticesCoordinates;

    private GameObject[] LeftPart;
    private GameObject[] RightPart;
    private GameObject[] LeftResult;
    private GameObject[] RightResult;
    private GameObject[] TotalResult;

    private int Size = MultiMeshManager.Instance.Size;

    public void Initialize()
    {
        firstRay = new Ray();
        secondRay = new Ray();

        Left = new IntPtr[Size];
        Right = new IntPtr[Size];

        LeftPart = new GameObject[Size];
        RightPart = new GameObject[Size];
        LeftResult = new GameObject[Size];
        RightResult = new GameObject[Size];
        TotalResult = new GameObject[Size * 2];
    }
    public void SetIntersectedValue(string type, Ray value)
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
    public GameObject[] Slicing()
    {
        MultiMeshAdjacencyList.Instance.Initialize();
        verticesCoordinates = new List<float[]>();

        int ResultIndex = 0;

        for (int j = 0; j < Size; j++)
        {
            Left[j] = CGAL.CreateMeshObject();
            Right[j] = CGAL.CreateMeshObject();
        }

        middlePosition = Vector3.Lerp(firstRay.origin, secondRay.origin, 0.5f);

        for (int j = 0; j < Size; j++)
        {
            verticesCoordinates.Add(CGAL.ConvertToFloatArray(MultiMeshAdjacencyList.Instance.WorldPositionVertices[j].ToArray()));

            if (CGAL.BuildPolyhedron(Left[j],
                verticesCoordinates.ElementAt(j),
                verticesCoordinates.ElementAt(j).Length / 3,
                MultiMeshManager.Instance.Meshes[j].triangles,
                MultiMeshManager.Instance.Meshes[j].triangles.Length / 3) == -1)
            {
                Debug.Log("만들어지지 않음");
            }

            if (CGAL.BuildPolyhedron(Right[j],
                verticesCoordinates.ElementAt(j),
                verticesCoordinates.ElementAt(j).Length / 3,
                MultiMeshManager.Instance.Meshes[j].triangles,
                MultiMeshManager.Instance.Meshes[j].triangles.Length / 3) == -1)
            {
                Debug.Log("만들어지지 않음");
            }
        }
        for (int j = 0; j < Size; j++)
        {
            if (
                CGAL.ClipPolyhedronByPlane(
                Left[j],
                CGAL.GeneratePlane(
                    middlePosition,
                    firstRay.origin + firstRay.direction * 10f,
                    secondRay.origin + secondRay.direction * 10f)) == -1)
            {
                Debug.Log("만들어지지 않음");
            }

            if (CGAL.ClipPolyhedronByPlane(
                Right[j],
                CGAL.GeneratePlane(
                    middlePosition,
                    secondRay.origin + secondRay.direction * 10f,
                    firstRay.origin + firstRay.direction * 10f)) == -1)
            {
                Debug.Log("만들어지지 않음");
            }
            LeftPart[j] = CGAL.GenerateLeftNewObject(Left[j], leftMaterial, j);
            RightPart[j] = CGAL.GenerateRightNewObject(Right[j], rightMaterial, j);
            MultiMeshManager.Instance.Parts[j].SetActive(false);

            LeftResult[j] = LeftPart[j];
            RightResult[j] = RightPart[j];

            TotalResult[ResultIndex++] = LeftResult[j];
            TotalResult[ResultIndex++] = RightResult[j];
        }
        return TotalResult;
    }
}
