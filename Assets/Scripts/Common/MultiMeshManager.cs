using System.Collections.Generic;
using UnityEngine;

public class MultiPatchs
{
    public GameObject Patch;
}
public class MultiMeshManager : Singleton<MultiMeshManager>
{
    public List<MultiPatchs> PatchList;

    public Camera cam;

    public Shader NewShader;

    public Material PartMaterial;

    public GameObject[] Parts;

    public GameObject StartMeasurePoint;
    public GameObject EndMeasurePoint;

    public Mesh[] Meshes;
    
    public Transform[] Transforms;
    
    public Transform PivotTransform;

    public int Size;
    public int PatchNum = 0;
    public int MeshIndex;

    public bool IncisionOk = false;
    public bool PatchOk = false;

    public void InitSize()
    {
        Size = GameObject.Find("ImportButton").GetComponent<ImportMesh>().Length;
        Transforms = new Transform[Size];
        Meshes = new Mesh[Size];
        Parts = new GameObject[Size];
        PatchList = new List<MultiPatchs>();
    }
    protected override void InitializeChild()
    {
        Renderer[] MeshRenderer = new Renderer[Size];

        for (int i = 0; i < Size; i++)
        {
            Transforms[i] = Transforms[i].GetComponent<Transform>();

            MeshRenderer[i] = Parts[i].GetComponent<MeshRenderer>();
            MeshRenderer[i].material = PartMaterial;

            Meshes[i] = Parts[i].GetComponent<MeshFilter>().mesh;
            Meshes[i].RecalculateNormals();

            GetHeartPartChild(i).name = GameObject.Find("PartialModel").transform.GetChild(i).name;
            GetHeartPartChild(i).gameObject.AddComponent<MeshCollider>();
            GetHeartPartChild(i).gameObject.GetComponent<MeshRenderer>().material.shader = NewShader;
        }

        PivotTransform = PivotTransform.GetComponent<Transform>();
    }
    // (만약)Initialize에서 PatchList 선언해주면 여기서는 Clear시켜줘야함
    // 근데 Initialize와 달라지는 내용이 없으면 이부분 지우고 그냥 Initialize 써도됨(일단은 살려 놓음)
    public void ReInitialize()
    {
        Renderer[] MeshRenderer = new Renderer[Size];

        for (int i = 0; i < Size; i++)
        {
            Transforms[i] = Transforms[i].GetComponent<Transform>();

            MeshRenderer[i] = Parts[i].GetComponent<MeshRenderer>();
            MeshRenderer[i].material = PartMaterial;

            Meshes[i] = Parts[i].GetComponent<MeshFilter>().mesh;
            Meshes[i].RecalculateNormals();

            GetHeartPartChild(i).name = GameObject.Find("PartialModel").transform.GetChild(i).name;
            GetHeartPartChild(i).gameObject.AddComponent<MeshCollider>();
            GetHeartPartChild(i).gameObject.GetComponent<MeshRenderer>().material.shader = NewShader;
        }

        PivotTransform = PivotTransform.GetComponent<Transform>();
    }
    public Transform GetHeartPartChild(int i)
    {
        return GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0);
    }
    public void MeshUpdate()
    {
        for (int i = 0; i < Size; i++)
        {
            Meshes[i] = Parts[i].GetComponent<MeshFilter>().mesh;
            Meshes[i].RecalculateNormals();
        }
    }
    public void SetNewObject(GameObject newPart, int i)
    {
        Destroy(Parts[i]);
        Parts[i] = newPart;
        Transforms[i] = Parts[i].transform;
        Meshes[i] = Parts[i].GetComponent<MeshFilter>().mesh;
        Parts[i].GetComponent<MeshRenderer>().material = PartMaterial;
        Parts[i].GetComponent<MeshRenderer>().material.shader = NewShader;
        Parts[i].name = GameObject.Find("PartialModel").transform.GetChild(i).name;
    }
}