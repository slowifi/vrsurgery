using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class DrawingCutMode : Mode
{

    private Vector3 oldPosition;

    private List<Ray> rayList;

    private void Awake()
    {
        rayList = new List<Ray>();

    }
    private void Update()
    {
        handleDrawingCut();
    }


    private void handleDrawingCut()
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
            if (Vector3.Distance(oldPosition, ray.origin) > 0.01f)
            {
                Debug.Log("intersect됨");
                oldPosition = ray.origin;
                rayList.Add(ray);
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

        CGAL.ClipPolyhedronByMesh(left, stamp);

        // CGAL.GenerateNewObject(left, leftMaterial);
        // 여기에 이제 잘리고나서 작업 넣어줘야됨. 새로운 메쉬로 바꾸고 정리하는 형태가 되어야함.
        // 라인렌더러 넣어줘야함.


        MeshManager.Instance.Heart.SetActive(false);



    }

}