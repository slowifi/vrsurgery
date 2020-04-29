using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInformation
{
    public Mesh ObjectMesh;
    public List<Edge> EdgeList;
    public List<Vector3> WorldPosition;
    public Dictionary<int, HashSet<int>> ConnectedVertices;
    public Dictionary<int, HashSet<int>> ConnectedTriangles;
}

// 이 instance같은 경우는 이제 import하면 인스턴스가 생성되도록 해야됨.
public class MeshManager : Singleton<MeshManager>
{
    public MeshInformation MeshInfo;

    public Stack<Mesh> MeshList;

    public List<GameObject> PatchList;

    public GameObject Heart;
    public GameObject LeftHeart;
    public GameObject RightHeart;

    public Camera cam;

    public Transform objTransform;
    public Transform pivotTransform;

    public GameObject startMeasurePoint;
    public GameObject endMeasurePoint;

    private GameObject disableHeart;
    public Material material;

    public Mesh mesh;
    public Mesh oldMesh;
    public Mesh firstMesh;



    public void Awake()
    {
        
    }
    public void ObjUpdate()
    {
        objTransform = objTransform.GetComponent<Transform>();
        //objTransform = GameObject.Find("PartialModel").transform;
        pivotTransform = pivotTransform.GetComponent<Transform>();
    }
    public void SetMeshInfo()
    {
        // 주소값 전달해서 추후에는 같이 변환되게 해야됨.
        MeshInfo = new MeshInformation();
        MeshInfo.ConnectedTriangles = AdjacencyList.Instance.connectedTriangles;
        MeshInfo.ConnectedVertices = AdjacencyList.Instance.connectedVertices;
        MeshInfo.EdgeList = AdjacencyList.Instance.edgeList;
        MeshInfo.WorldPosition = AdjacencyList.Instance.worldPositionVertices;
        MeshInfo.ObjectMesh = mesh;
    }

    public void SetNewObject(GameObject newGameObject)
    {
        Destroy(Heart);
        Heart = newGameObject;
        objTransform = Heart.transform;
        mesh = Heart.GetComponent<MeshFilter>().mesh;
        Heart.GetComponent<MeshRenderer>().material = material;
    }

    public void MeshUpdate()
    {
        // heart = GameObject.Find("heart_2");
        mesh = Heart.GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals();
    }

    public void SaveCurrentMesh()
    {
        disableHeart = Instantiate(Heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        int[] triangles = (int[])Heart.GetComponent<MeshFilter>().mesh.triangles.Clone();
        Vector3[] vertices = (Vector3[])Heart.GetComponent<MeshFilter>().mesh.vertices.Clone();

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
        Debug.Log(mesh.bounds.size);
        Renderer mat = Heart.GetComponent<Renderer>();
        mat.material = material;
        mesh = Heart.GetComponent<MeshFilter>().mesh;
        disableHeart = Instantiate(Heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        firstMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);

        mesh.RecalculateNormals();
        foreach (var item in PatchList)
        {
            Destroy(item);
        }
        PatchList.Clear();
        Debug.Log(mesh.normals.Length);
        Debug.Log(mesh.vertexCount);
    }

    protected override void InitializeChild()
    {
        Renderer mat = Heart.GetComponent<Renderer>();
        mat.material = material;
        mesh = Heart.GetComponent<MeshFilter>().mesh;
        
        //Debug.Log(mesh.bounds.size);
        //y 기준으로 맞춰주면 되겠다.
        MeshManager.Instance.pivotTransform.localScale = Vector3.one * (80 / mesh.bounds.size.y);

        disableHeart = Instantiate(Heart);
        oldMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        firstMesh = disableHeart.GetComponent<MeshFilter>().mesh;
        Destroy(disableHeart);
        PatchList = new List<GameObject>();
        mesh.RecalculateNormals();
        ChatManager.Instance.GenerateMessage(" " + mesh.normals.Length);
        Debug.Log(mesh.normals.Length);
        Debug.Log(mesh.vertexCount);
    }
}
