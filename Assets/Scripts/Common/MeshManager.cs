using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : Singleton<MeshManager>
{
    public GameObject heart;
    private GameObject disableHeart;
    public Material material;
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

    public void Reinitialize()
    {
        Renderer mat = heart.GetComponent<Renderer>();
        mat.material = material;
        mesh = heart.GetComponent<MeshFilter>().mesh;
        disableHeart = Instantiate(heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        mesh.RecalculateNormals();
        Debug.Log(mesh.normals.Length);
        Debug.Log(mesh.vertexCount);
    }

    protected override void InitializeChild()
    {
        Renderer mat = heart.GetComponent<Renderer>();
        mat.material = material;
        mesh = heart.GetComponent<MeshFilter>().mesh;
        disableHeart = Instantiate(heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        mesh.RecalculateNormals();
        ChatManager.Instance.GenerateMessage(" " + mesh.normals.Length);
        Debug.Log(mesh.normals.Length);
        Debug.Log(mesh.vertexCount);
    }
}
