using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDoubleFaceMesh : Singleton<MakeDoubleFaceMesh>
{
    public GameObject originalObject;
    public GameObject oppositeObject;
    public Mesh originalMesh;
    public Mesh oppositeMesh;

    public void MakeDoubleFace()
    {
        originalMesh = originalObject.GetComponent<MeshFilter>().mesh;
        oppositeObject = new GameObject("Aorta_In", typeof(MeshFilter), typeof(MeshRenderer));
        oppositeMesh = oppositeObject.GetComponent<MeshFilter>().mesh;
        
        int[] triangles = (int[])originalMesh.triangles.Clone();
        Vector3[] vertices = (Vector3[])originalMesh.vertices.Clone();
        oppositeMesh.vertices = vertices;
        int[] newTriangles = (int[])triangles.Clone();

        for (int i = 0; i < triangles.Length; i+=3)
        {
            newTriangles[i + 1] = triangles[i + 2];
            newTriangles[i + 2] = triangles[i + 1];
        }
        oppositeMesh.triangles = newTriangles;
    }
}
