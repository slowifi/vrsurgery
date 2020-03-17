using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDoubleFaceMesh : Singleton<MakeDoubleFaceMesh>
{
    public GameObject oppositeObject;
    private Mesh originalMesh;
    private Mesh oppositeMesh;

    public void MakeDoubleFace()
    {
        originalMesh = MeshManager.Instance.mesh;
        oppositeMesh = oppositeObject.GetComponent<MeshFilter>().mesh;
        
        int[] triangles = (int[])originalMesh.triangles.Clone();
        Vector3[] vertices = originalMesh.vertices;
        //Vector3[] vertices = AdjacencyList.Instance.worldPositionVertices;
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
        //그럼 지금 vertex가 전달이 안되는건가?
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

    protected override void InitializeChild()
    {
        originalMesh = MeshManager.Instance.mesh;
        oppositeObject = new GameObject("Heart_In", typeof(MeshFilter), typeof(MeshRenderer));
        oppositeObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.heart.GetComponent<MeshRenderer>().material;
        
        oppositeObject.transform.SetParent(GameObject.Find("PartialModel").transform);
        oppositeObject.transform.SetPositionAndRotation(ObjManager.Instance.objTransform.position, ObjManager.Instance.objTransform.rotation);
        oppositeMesh = oppositeObject.GetComponent<MeshFilter>().mesh;

        int[] triangles = (int[])originalMesh.triangles.Clone();
        // vertex position은 reference 전달.
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
