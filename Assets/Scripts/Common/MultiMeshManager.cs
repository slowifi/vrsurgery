using System.Collections;
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
    public GameObject MultiMeshStartMeasurePoint;
    public GameObject MultiMeshEndMeasurePoint;
    public GameObject[] HeartParts;

    public Camera cam;
     
    public Material HeartPartMaterial;
    
    public Transform[] objsTransform;
    public Transform pivotTransform;
    
    public Mesh[] meshes;
    
    public int Size;

    public void GetObjsSize()
    {
        Size = GameObject.Find("ImportButton").GetComponent<ImportMesh>().length;
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
    
    // BoudaryCutMode , SliceMode
    // BoudartCut, Slice, Incision etc. 매서드들 전부 각각 해당 매시에만 영향을 주고 그 이외에는
    // 영향이 가지 않게 설정해야함
    // (= 메모리 최적화 방식으로 작업해야 함)
    // 여기서 한번에 처리할지 아니면 각각의 메서드 안에 가서 처리할지 두고봐야함
    public void SetNewObjects()
    {
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
        Renderer[] mat = new Renderer[Size];
        for(int i=0;i<Size;i++)
        {
            mat[i] = HeartParts[i].GetComponent<Renderer>();
            mat[i].material = HeartPartMaterial;
            meshes[i] = HeartParts[i].GetComponent<MeshFilter>().mesh;

            meshes[i].RecalculateNormals();

            //Check Path need reinitialize !!!
        }
    }
    protected override void InitializeChild()
    {
        Renderer[] mat = new Renderer[Size];
        for (int i = 0; i < Size; i++)
        {
            mat[i] = HeartParts[i].GetComponent<Renderer>();
            mat[i].material = HeartPartMaterial;
            meshes[i] = HeartParts[i].GetComponent<MeshFilter>().mesh;

            meshes[i].RecalculateNormals();
        }
    }
}