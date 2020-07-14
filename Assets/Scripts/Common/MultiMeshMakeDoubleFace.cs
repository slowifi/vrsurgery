using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshMakeDoubleFace : Singleton<MultiMeshMakeDoubleFace>
{
    public GameObject[] oppositeObjects;
    public Mesh[] originalMeshes;
    public Mesh[] oppositeMeshes;

    //public void MakePatchInnerFace(GameObject patch)
    //{
    //    GameObject innerPatch = MultiMeshManager.Instance.PatchList[MultiMeshManager.Instance.PatchList.Count - 1].InnerPatch;
    //    //innerPatch.GetComponent<MeshFilter>().mesh;
    //    innerPatch.transform.SetParent(MultiMeshManager.Instance.pivotTransform);
    //    innerPatch.transform.localPosition = patch.transform.localPosition;
    //    innerPatch.transform.localRotation = patch.transform.localRotation;
    //    innerPatch.transform.localScale = patch.transform.localScale;
    //    Mesh innerMesh = innerPatch.GetComponent<MeshFilter>().mesh;

    //    int[] triangles = (int[])patch.GetComponent<MeshFilter>().mesh.triangles.Clone();
    //    innerMesh.vertices = patch.GetComponent<MeshFilter>().mesh.vertices;
    //    int[] newTriangles = (int[])triangles.Clone();

    //    for (int i = 0; i < triangles.Length; i += 3)
    //    {
    //        newTriangles[i + 1] = triangles[i + 2];
    //        newTriangles[i + 2] = triangles[i + 1];
    //    }
    //    innerMesh.triangles = newTriangles;
    //    innerMesh.RecalculateNormals();

    //    MeshRenderer ren = innerPatch.GetComponent<MeshRenderer>();
    //    ren.material.color = Color.white;
    //}
    //public void PatchUpdateInnerFaceVertices(int patchIndex)
    //{
    //    Mesh patchMesh = MultiMeshManager.Instance.PatchList[patchIndex].OuterPatch.GetComponent<MeshFilter>().mesh;
    //    Mesh innerMesh = MultiMeshManager.Instance.PatchList[patchIndex].InnerPatch.GetComponent<MeshFilter>().mesh;

    //    Vector3[] vertices = patchMesh.vertices;
    //    int[] triangles = patchMesh.triangles;
    //    int[] newTriangles = (int[])triangles.Clone();
    //    for (int i = 0; i < triangles.Length; i += 3)
    //    {
    //        newTriangles[i + 1] = triangles[i + 2];
    //        newTriangles[i + 2] = triangles[i + 1];
    //    }
    //    innerMesh.vertices = vertices;
    //    innerMesh.triangles = newTriangles;
    //    innerMesh.RecalculateNormals();
    //}

    //public void Reinitialize()
    //{
    //    for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
    //        Destroy(oppositeObjects[i]);

    //    oppositeObjects = new GameObject[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
    //    originalMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
    //    oppositeMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];

    //    for(int i = 0; i< MultiMeshManager.Instance.Size;i++)
    //    {
    //        originalMeshes[i] = MultiMeshManager.Instance.meshes[i];
    //        GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).name = GameObject.Find("PartialModel").transform.GetChild(i).name + "_Outer";
    //        GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
    //        oppositeObjects[i] = new GameObject(GameObject.Find("PartialModel").transform.GetChild(i).name + "_Inner", typeof(MeshFilter), typeof(MeshRenderer));
    //        oppositeObjects[i].GetComponent<MeshRenderer>().material = MultiMeshManager.Instance.HeartParts[i].GetComponent<MeshRenderer>().material;

    //        oppositeObjects[i].transform.SetParent(GameObject.Find(GameObject.Find("PartialModel").transform.GetChild(i).name).transform);
    //        oppositeObjects[i].transform.localRotation = Quaternion.identity;
    //        oppositeObjects[i].transform.localPosition = Vector3.zero;
    //        oppositeObjects[i].transform.localScale = Vector3.one;
    //        oppositeMeshes[i] = oppositeObjects[i].GetComponent<MeshFilter>().mesh;

    //        int[] triangles = (int[])originalMeshes[i].triangles.Clone();
    //        oppositeMeshes[i].vertices = originalMeshes[i].vertices;
    //        int[] newTriangles = (int[])triangles.Clone();

    //        for (int j = 0; j < triangles.Length; j += 3)
    //        {
    //            newTriangles[j + 1] = triangles[j + 2];
    //            newTriangles[j + 2] = triangles[j + 1];
    //        }
    //        oppositeMeshes[i].triangles = newTriangles;
    //        oppositeMeshes[i].RecalculateNormals();
    //    }
    //}
    //protected override void InitializeChild()
    //{
    //    oppositeObjects = new GameObject[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
    //    originalMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
    //    oppositeMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
    //    for(int i=0;i<MultiMeshManager.Instance.Size;i++)
    //    {
    //        originalMeshes[i] = MultiMeshManager.Instance.meshes[i];
    //        GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).name = GameObject.Find("PartialModel").transform.GetChild(i).name + "_Outer";
    //        GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
    //        oppositeObjects[i] = new GameObject(GameObject.Find("PartialModel").transform.GetChild(i).name + "_Inner", typeof(MeshFilter), typeof(MeshRenderer));
    //        oppositeObjects[i].GetComponent<MeshRenderer>().material = MultiMeshManager.Instance.HeartParts[i].GetComponent<MeshRenderer>().material;
            
    //        oppositeObjects[i].transform.SetParent(GameObject.Find(GameObject.Find("PartialModel").transform.GetChild(i).name).transform);
    //        oppositeObjects[i].transform.localRotation = Quaternion.identity;
    //        oppositeObjects[i].transform.localPosition = Vector3.zero;
    //        oppositeObjects[i].transform.localScale = Vector3.one;
    //        oppositeMeshes[i] = oppositeObjects[i].GetComponent<MeshFilter>().mesh;

    //        int[] triangles = (int[])originalMeshes[i].triangles.Clone();
    //        oppositeMeshes[i].vertices = originalMeshes[i].vertices;
    //        int[] newTriangles = (int[])triangles.Clone();

    //        for(int j = 0; j < triangles.Length; j += 3)
    //        {
    //            newTriangles[j + 1] = triangles[j + 2];
    //            newTriangles[j + 2] = triangles[j + 1];
    //        }
    //        oppositeMeshes[i].triangles = newTriangles;
    //        oppositeMeshes[i].RecalculateNormals();
    //    }
    //}
    //public void MeshUpdateInnerFaceVertices(int MeshIndex)
    //{
    //    Vector3[] vertices = MultiMeshManager.Instance.meshes[MeshIndex].vertices;
    //    int[] triangles = MultiMeshManager.Instance.meshes[MeshIndex].triangles;
    //    int[] newTriangles = (int[])triangles.Clone();
    //    for (int i = 0; i < triangles.Length; i += 3)
    //    {
    //        newTriangles[i + 1] = triangles[i + 2];
    //        newTriangles[i + 2] = triangles[i + 1];
    //    }
    //    oppositeMeshes[MeshIndex].vertices = vertices;
    //    oppositeMeshes[MeshIndex].triangles = newTriangles;
    //    oppositeMeshes[MeshIndex].RecalculateNormals();
    //}
}
