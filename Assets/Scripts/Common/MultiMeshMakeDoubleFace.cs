using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshMakeDoubleFace : Singleton<MultiMeshMakeDoubleFace>
{
    public GameObject[] oppositeObjects;
    public Mesh[] originalMeshes;
    public Mesh[] oppositeMeshes;

    public void Reinitialize()
    {
        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            Destroy(oppositeObjects[i]);

        oppositeObjects = new GameObject[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
        originalMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
        oppositeMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];

        for(int i = 0; i< MultiMeshManager.Instance.Size;i++)
        {
            originalMeshes[i] = MultiMeshManager.Instance.meshes[i];
            GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).name = GameObject.Find("PartialModel").transform.GetChild(i).name + "_Outer";
            oppositeObjects[i] = new GameObject(GameObject.Find("PartialModel").transform.GetChild(i).name + "_Inner", typeof(MeshFilter), typeof(MeshRenderer));
            oppositeObjects[i].GetComponent<MeshRenderer>().material = MultiMeshManager.Instance.HeartParts[i].GetComponent<MeshRenderer>().material;

            oppositeObjects[i].transform.SetParent(GameObject.Find(GameObject.Find("PartialModel").transform.GetChild(i).name).transform);
            oppositeObjects[i].transform.localRotation = Quaternion.identity;
            oppositeObjects[i].transform.localPosition = Vector3.zero;
            oppositeObjects[i].transform.localScale = Vector3.one;
            oppositeMeshes[i] = oppositeObjects[i].GetComponent<MeshFilter>().mesh;

            int[] triangles = (int[])originalMeshes[i].triangles.Clone();
            oppositeMeshes[i].vertices = originalMeshes[i].vertices;
            int[] newTriangles = (int[])triangles.Clone();

            for (int j = 0; j < triangles.Length; j += 3)
            {
                newTriangles[j + 1] = triangles[j + 2];
                newTriangles[j + 2] = triangles[j + 1];
            }
            oppositeMeshes[i].triangles = newTriangles;
            oppositeMeshes[i].RecalculateNormals();
        }
    }
    protected override void InitializeChild()
    {
        oppositeObjects = new GameObject[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
        originalMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
        oppositeMeshes = new Mesh[GameObject.Find("MultiMeshManager").GetComponent<MultiMeshManager>().Size];
        for(int i=0;i<MultiMeshManager.Instance.Size;i++)
        {
            originalMeshes[i] = MultiMeshManager.Instance.meshes[i];
            GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).name = GameObject.Find("PartialModel").transform.GetChild(i).name + "_Outer";
            oppositeObjects[i] = new GameObject(GameObject.Find("PartialModel").transform.GetChild(i).name + "_Inner", typeof(MeshFilter), typeof(MeshRenderer));
            oppositeObjects[i].GetComponent<MeshRenderer>().material = MultiMeshManager.Instance.HeartParts[i].GetComponent<MeshRenderer>().material;
            
            oppositeObjects[i].transform.SetParent(GameObject.Find(GameObject.Find("PartialModel").transform.GetChild(i).name).transform);
            oppositeObjects[i].transform.localRotation = Quaternion.identity;
            oppositeObjects[i].transform.localPosition = Vector3.zero;
            oppositeObjects[i].transform.localScale = Vector3.one;
            oppositeMeshes[i] = oppositeObjects[i].GetComponent<MeshFilter>().mesh;

            int[] triangles = (int[])originalMeshes[i].triangles.Clone();
            oppositeMeshes[i].vertices = originalMeshes[i].vertices;
            int[] newTriangles = (int[])triangles.Clone();

            for(int j = 0; j < triangles.Length; j += 3)
            {
                newTriangles[j + 1] = triangles[j + 2];
                newTriangles[j + 2] = triangles[j + 1];
            }
            oppositeMeshes[i].triangles = newTriangles;
            oppositeMeshes[i].RecalculateNormals();
        }
    }
}
