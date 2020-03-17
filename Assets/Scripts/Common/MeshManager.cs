using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : Singleton<MeshManager>
{
    public GameObject heart;
    private GameObject disableHeart;
    public Mesh testMesh;
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
    }



    public void SaveCurrentMesh()
    {
        disableHeart = Instantiate(heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        int[] triangles = (int[])heart.GetComponent<MeshFilter>().mesh.triangles.Clone();
        Vector3[] vertices = (Vector3[])heart.GetComponent<MeshFilter>().mesh.vertices.Clone();

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
