using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public class SliceMode : Mode
{
    private IntersectedValues firstIntersectedValues;
    private IntersectedValues secondIntersectedValues;
    private Vector3 middlePosition;
    private Vector3 oldPosition;
    private List<Ray> rayList;

    private Material leftMaterial;
    private Material rightMaterial;

    private bool isSlice;
    private bool isDrawingCut;

    private void Awake()
    {
        firstIntersectedValues = new IntersectedValues();
        secondIntersectedValues = new IntersectedValues();
        leftMaterial = Resources.Load("Materials/LeftMaterial", typeof(Material)) as Material;
        rightMaterial = Resources.Load("Materials/RightMaterial", typeof(Material)) as Material;
        rayList = new List<Ray>();
        isDrawingCut = true;
        isSlice = false;
    }

    private void Update()
    {
        
        if (isSlice)
        {
            if (Input.GetMouseButtonDown(0))
            {
                IntersectedValues values = Intersections.GetIntersectedValues();
                if (values.Intersected)
                {
                    firstIntersectedValues = values;
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                IntersectedValues values = Intersections.GetIntersectedValues();
                if (values.Intersected)
                {
                    secondIntersectedValues = values;
                    middlePosition = Vector3.Lerp(firstIntersectedValues.ray.origin, secondIntersectedValues.ray.origin, 0.5f);
                    Slicing();
                }
            }
        }
        else if (isDrawingCut)
        {
            ///이건 intersect되지 않아도 실행이 되어야함.
            ///
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                oldPosition = ray.origin;
                rayList.Add(ray);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("upbutton on");
                DrawingCut();
            }
            else if (Input.GetMouseButton(0))
            {
                Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                oldPosition = ray.origin;
                rayList.Add(ray);
                if (Vector3.Distance(oldPosition, ray.origin) > 0.001f)
                {
                    Debug.Log("intersect됨");
                    oldPosition = ray.origin;
                    rayList.Add(ray);
                }
            }
        }
    }

    private void DrawingCut()
    {
        IntPtr left = CGAL.CreateMeshObject();
        IntPtr right = CGAL.CreateMeshObject();
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
        if (CGAL.BuildPolyhedron(right,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3) == -1)
        {
            Debug.Log(" 만들어지지 않음");
        }

        ///left right 생성이 됨.
        ///이상태에서
        ///plane의 노말값만 바꿔서 슬라이싱함.
        ///
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
        CGAL.GenerateNewObject(stamp, leftMaterial);




        //CGAL.ClipPolyhedron(
        //    left,





        //    );

        //CGAL.ClipPolyhedron(
        //    right,





        //    );
        //CGAL.GenerateNewObject(left, leftMaterial);
        //CGAL.GenerateNewObject(right, rightMaterial);
        MeshManager.Instance.heart.SetActive(false);



    }



    private void Slicing()
    {
        // left right를 각각 뒤집어 씌울 material을 만들고 색을 다르게해서 각각 잘리면 나눠서 색을 입힘. 그다음에 유저가 선택하면 선택한 mesh만 살아남도록. 허공을 누르면 다시 오리지널 메쉬로 넘어가게.
        IntPtr left = CGAL.CreateMeshObject();
        IntPtr right = CGAL.CreateMeshObject();
        
        float[] verticesCoordinate = CGAL.ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

        if (CGAL.BuildPolyhedron(left,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3) == 0)
        {
            Debug.Log(" 만들어지지 않음");
        }
        if (CGAL.BuildPolyhedron(right,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3) == 0)
        {
            Debug.Log(" 만들어지지 않음");
        }

        ///left right 생성이 됨.
        ///이상태에서
        ///plane의 노말값만 바꿔서 슬라이싱함.
        CGAL.ClipPolyhedronByPlane(
            left,
            CGAL.GeneratePlane(
                middlePosition,
                firstIntersectedValues.IntersectedPosition,
                secondIntersectedValues.IntersectedPosition));

        CGAL.ClipPolyhedronByPlane(
            right,
            CGAL.GeneratePlane(
                middlePosition,
                secondIntersectedValues.IntersectedPosition,
                firstIntersectedValues.IntersectedPosition));

        CGAL.GenerateNewObject(left, leftMaterial);
        CGAL.GenerateNewObject(right, rightMaterial);
        MeshManager.Instance.heart.SetActive(false);
        // 여기까지 했고 선택하면 하나 잘리도록 하기.
    }


}