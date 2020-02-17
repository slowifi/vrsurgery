using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : Singleton<MeshManager>
{
    public GameObject heart;
    public Mesh mesh;
    public int vertexCount;
    public int[] triangles;
    public Vector3[] vertices;

    public void MeshUpdate()
    {
        // heart = GameObject.Find("heart_2");
        mesh = heart.GetComponent<MeshFilter>().mesh;
        vertexCount = mesh.vertexCount;
        triangles = mesh.triangles;
        vertices = mesh.vertices;
    }
}
