using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : Singleton<MeshManager>
{
    public GameObject heart;
    private GameObject disableHeart;
    public Mesh mesh;
    public Mesh oldMesh;
    public int vertexCount;
    public int[] triangles;
    public Vector3[] vertices;

    public void MeshUpdate()
    {
        // heart = GameObject.Find("heart_2");
        mesh = heart.GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals();
        vertexCount = mesh.vertexCount;
        triangles = mesh.triangles;
        vertices = mesh.vertices;
    }

    public void SaveCurrentMesh()
    {
        disableHeart = Instantiate(heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        int[] triangles = (int[])mesh.triangles.Clone();
        Vector3[] vertices = (Vector3[])mesh.vertices.Clone();

        oldMesh.triangles = triangles;
        oldMesh.vertices = vertices;
    }

    public void LoadOldMesh()
    {
        int[] triangles = (int[])oldMesh.triangles.Clone();
        Vector3[] vertices = (Vector3[])oldMesh.vertices.Clone();

        mesh.triangles = triangles;
        mesh.vertices = vertices;
    }

    protected override void InitializeChild()
    {
        mesh = heart.GetComponent<MeshFilter>().mesh;
        disableHeart = Instantiate(heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        vertexCount = mesh.vertexCount;
        triangles = mesh.triangles;
        vertices = mesh.vertices;
        mesh.RecalculateNormals();
    }
}
