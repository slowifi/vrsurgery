﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDoubleFaceMesh : Singleton<MakeDoubleFaceMesh>
{
    /// <summary>
    /// 이것도 객체가 하나가 아니라 여러개 메쉬 다룰 때마다 각각의 안쪽메쉬가 필요함. 그래서 singleton말고 다르게 해야됨.
    /// </summary>
    public GameObject oppositeObject;
    public GameObject testObject;
    private Mesh originalMesh;
    private Mesh oppositeMesh;

    public void MakePatchInnerFace(GameObject patch)
    {
        GameObject innerPatch = MeshManager.Instance.PatchList[MeshManager.Instance.PatchList.Count - 1].InnerPatch;
        //innerPatch.GetComponent<MeshFilter>().mesh;
        innerPatch.transform.SetParent(MeshManager.Instance.pivotTransform);
        innerPatch.transform.localPosition = patch.transform.localPosition;
        innerPatch.transform.localRotation = patch.transform.localRotation;
        innerPatch.transform.localScale = patch.transform.localScale;
        Mesh innerMesh = innerPatch.GetComponent<MeshFilter>().mesh;

        int[] triangles = (int[])patch.GetComponent<MeshFilter>().mesh.triangles.Clone();
        innerMesh.vertices = patch.GetComponent<MeshFilter>().mesh.vertices;
        int[] newTriangles = (int[])triangles.Clone();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        innerMesh.triangles = newTriangles;
        innerMesh.RecalculateNormals();

        MeshRenderer ren = innerPatch.GetComponent<MeshRenderer>();
        ren.material.color = Color.white;
    }

    public void PatchUpdateInnerFaceVertices(int patchIndex)
    {
        Mesh patchMesh = MeshManager.Instance.PatchList[patchIndex].OuterPatch.GetComponent<MeshFilter>().mesh;
        Mesh innerMesh = MeshManager.Instance.PatchList[patchIndex].InnerPatch.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = patchMesh.vertices;
        int[] triangles = patchMesh.triangles;
        int[] newTriangles = (int[])triangles.Clone();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        innerMesh.vertices = vertices;
        innerMesh.triangles = newTriangles;
        innerMesh.RecalculateNormals();
    }

    public void MakeDoubleFace()
    {
        originalMesh = MeshManager.Instance.mesh;
        oppositeMesh = oppositeObject.GetComponent<MeshFilter>().mesh;
        
        int[] triangles = (int[])originalMesh.triangles.Clone();
        Vector3[] vertices = originalMesh.vertices;
        oppositeMesh.vertices = vertices;
        int[] newTriangles = (int[])triangles.Clone();

        for (int i = 0; i < triangles.Length; i+=3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        oppositeMesh.triangles = newTriangles;
    }

    public void MeshUpdateInnerFaceVertices()
    {
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = (int[])triangles.Clone();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        oppositeMesh.vertices = vertices;
        oppositeMesh.triangles = newTriangles;
        oppositeMesh.RecalculateNormals();
    }

    public void Reinitialize()
    {
        Destroy(oppositeObject);
        originalMesh = MeshManager.Instance.mesh;
        GameObject newHeart = new GameObject("Heart_Inner", typeof(MeshFilter), typeof(MeshRenderer));

        oppositeObject = newHeart;
        oppositeObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.Heart.GetComponent<MeshRenderer>().material;
        oppositeObject.transform.SetParent(MeshManager.Instance.objTransform.parent);
        oppositeObject.transform.localRotation = Quaternion.identity;
        oppositeObject.transform.localPosition = Vector3.zero;
        oppositeObject.transform.localScale = Vector3.one;
        
        oppositeMesh = oppositeObject.GetComponent<MeshFilter>().mesh;

        int[] triangles = (int[])originalMesh.triangles.Clone();
        oppositeMesh.vertices = originalMesh.vertices;
        int[] newTriangles = (int[])triangles.Clone();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        oppositeMesh.triangles = newTriangles;
        oppositeMesh.RecalculateNormals();
    }

    protected override void InitializeChild()
    {
        originalMesh = MeshManager.Instance.mesh;
        oppositeObject = new GameObject("Heart_Inner", typeof(MeshFilter), typeof(MeshRenderer));
        oppositeObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.Heart.GetComponent<MeshRenderer>().material;
        
        oppositeObject.transform.SetParent(GameObject.Find("PartialModel").transform);
        oppositeObject.transform.localRotation = Quaternion.identity;
        oppositeObject.transform.localPosition = Vector3.zero;
        oppositeObject.transform.localScale = Vector3.one;
        //oppositeObject.transform.SetPositionAndRotation(MeshManager.Instance.objTransform.position, MeshManager.Instance.objTransform.rotation);
        oppositeMesh = oppositeObject.GetComponent<MeshFilter>().mesh;

        int[] triangles = (int[])originalMesh.triangles.Clone();
        oppositeMesh.vertices = originalMesh.vertices;
        int[] newTriangles = (int[])triangles.Clone();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        oppositeMesh.triangles = newTriangles;
        oppositeMesh.RecalculateNormals();
    }
}
