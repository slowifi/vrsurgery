using System.Collections.Generic;
using UnityEngine;

// Check SetMeshInfo need?
// If oldmesh and firstmesh used, shold add oldmesh and firstmesh
public class MultiPatchs
{
    public GameObject OuterPatch;
    public GameObject InnerPatch;
}
public class MultiMeshManager : Singleton<MultiMeshManager>
{
    
    public List<MultiPatchs> PatchList;

    public GameObject ListButton;
    public GameObject MultiMeshStartMeasurePoint;
    public GameObject MultiMeshEndMeasurePoint;
    public GameObject[] HeartParts;

    public Camera cam;
     
    public Material HeartPartMaterial;
    
    public Transform[] objsTransform;
    public Transform pivotTransform;
    
    public Mesh[] meshes;
    
    public int Size;

    public Shader newShader;
    void Awake()
    {
        newShader = Resources.Load("Shaders/CulloffShader", typeof(Shader)) as Shader;
    }
    public void InitObjSize()
    {
        Size = GameObject.Find("ImportButton").GetComponent<ImportMesh>().Length;
        objsTransform = new Transform[Size];
        meshes = new Mesh[Size];
        HeartParts = new GameObject[Size];
    }
    public void GetObjsSize()
    {
        Size = GameObject.Find("ImportButton").GetComponent<ImportMesh>().Length;
        objsTransform = new Transform[Size];
        meshes = new Mesh[Size];
        HeartParts = new GameObject[Size];
    }
    public void ObjsUpdate()
    {
        //objsTransform = new Transform[Size];
        for (int i = 0; i < Size; i++)
            objsTransform[i] = objsTransform[i].GetComponent<Transform>();
        pivotTransform = pivotTransform.GetComponent<Transform>();
    }

    public void SetNewObjects(GameObject newHeartPart, int i)
    {
        Destroy(HeartParts[i]);
        HeartParts[i] = newHeartPart;
        objsTransform[i] = HeartParts[i].transform;
        meshes[i] = HeartParts[i].GetComponent<MeshFilter>().mesh;
        HeartParts[i].GetComponent<MeshRenderer>().material = HeartPartMaterial;
        HeartParts[i].GetComponent<MeshRenderer>().material.shader = newShader;
        HeartParts[i].name = GameObject.Find("PartialModel").transform.GetChild(i).name;
    }
    //Should Check Making Heart array is right?!
    public void MeshesUpdate()
    {
        for (int i = 0; i < Size; i++)
        {
            meshes[i] = HeartParts[i].GetComponent<MeshFilter>().mesh;
            meshes[i].RecalculateNormals();
        }
    }
    public void Reinitialize()
    {
        GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().Initialize();
        Renderer[] mat = new Renderer[Size];
        for(int i=0;i<Size;i++)
        {
            mat[i] = HeartParts[i].GetComponent<Renderer>();
            mat[i].material = HeartPartMaterial;
            meshes[i] = HeartParts[i].GetComponent<MeshFilter>().mesh;
            PatchList.Clear();
            meshes[i].RecalculateNormals();
            GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().OriginalMesh.Add(Instantiate(meshes[i]));
            //Check Path need reinitialize !!!
        }
    }
    protected override void InitializeChild()
    {
        GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().Initialize();
        Renderer[] mat = new Renderer[Size];
        for (int i = 0; i < Size; i++)
        {
            mat[i] = HeartParts[i].GetComponent<Renderer>();
            mat[i].material = HeartPartMaterial;
            meshes[i] = HeartParts[i].GetComponent<MeshFilter>().mesh;
            PatchList = new List<MultiPatchs>();
            meshes[i].RecalculateNormals();
            GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().OriginalMesh.Add(Instantiate(meshes[i]));
        }
    }
    public void InitSingleFace()
    {
        for(int i=0;i<Size;i++)
        {
            GetHeartPartChild(i).name = GameObject.Find("PartialModel").transform.GetChild(i).name;
            GetHeartPartChild(i).gameObject.AddComponent<MeshCollider>();
            GetHeartPartChild(i).gameObject.GetComponent<MeshRenderer>().material.shader = newShader;
        }
    }
    public Transform GetHeartPartChild(int i)
    {
        return GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0);
    }
}