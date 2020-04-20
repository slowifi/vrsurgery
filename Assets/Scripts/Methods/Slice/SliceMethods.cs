using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public class SliceMethods
{
    private IntersectedValues firstIntersectedValues = new IntersectedValues();
    private IntersectedValues secondIntersectedValues = new IntersectedValues();
    private Material leftMaterial = Resources.Load("Materials/LeftMaterial", typeof(Material)) as Material;
    private Material rightMaterial = Resources.Load("Materials/RightMaterial", typeof(Material)) as Material;

    private List<Vector3> leftWorldPos;
    private List<Vector3> rightWorldPos;
    private Vector3 middlePosition;

    public void SetIntersectedValues(string type, IntersectedValues value)
    {
        switch (type)
        {
            case "first":
                firstIntersectedValues = value;
                break;
            case "second":
                secondIntersectedValues = value;
                break;
        }
    }

    public string CheckSelected(GameObject leftHeart, GameObject rightHeart)
    {
        string result = "none";
        Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        IntersectedValues valuesLeft = Intersections.GetIntersectedValues(ray, leftHeart.GetComponent<MeshFilter>().mesh.triangles, leftWorldPos);
        IntersectedValues valuesRight = Intersections.GetIntersectedValues(ray, rightHeart.GetComponent<MeshFilter>().mesh.triangles, rightWorldPos);

        if (valuesLeft.Intersected)
        {
            result = "left";
        }
        else if (valuesRight.Intersected)
        {
            result = "right";
        }
        return result;
    }

    public GameObject[] Slicing()
    {
        // left right를 각각 뒤집어 씌울 material을 만들고 색을 다르게해서 각각 잘리면 나눠서 색을 입힘. 그다음에 유저가 선택하면 선택한 mesh만 살아남도록. 허공을 누르면 다시 오리지널 메쉬로 넘어가게.
        IntPtr left = CGAL.CreateMeshObject();
        IntPtr right = CGAL.CreateMeshObject();
        GameObject[] result = new GameObject[2];

        float[] verticesCoordinate = CGAL.ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

        middlePosition = Vector3.Lerp(firstIntersectedValues.ray.origin, secondIntersectedValues.ray.origin, 0.5f);


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

        GameObject leftHeart = CGAL.GenerateNewObject(left, leftMaterial);
        GameObject rightHeart = CGAL.GenerateNewObject(right, rightMaterial);
        leftWorldPos = new List<Vector3>();
        rightWorldPos = new List<Vector3>();
        leftWorldPos = AdjacencyList.Instance.LocalToWorldPosition(leftHeart.GetComponent<MeshFilter>().mesh);
        rightWorldPos = AdjacencyList.Instance.LocalToWorldPosition(rightHeart.GetComponent<MeshFilter>().mesh);

        MeshManager.Instance.Heart.SetActive(false);
        // 여기까지 했고 선택하면 하나 잘리도록 하기.

        result[0] = leftHeart;
        result[1] = rightHeart;
        return result;
    }
}