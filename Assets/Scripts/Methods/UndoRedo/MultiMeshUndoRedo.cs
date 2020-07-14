using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEditor;
using System.IO;
using UnityEngine.UIElements;

public class MultiMeshUndoRedo : MonoBehaviour
{
    // ActionList
    // -1 = Original, 0 = Slice, 1 = IncisionBoundaryCut, 2 = Patch
    public List<Mesh>[] MeshList;
    public List<int> ActionList;
    public List<int>[] IBRowColList;
    public List<int> PatchList;
    public List<int>[] IndexList;

    public Mesh[] test;

    public int Size;
    public int ActionListIndex;
    public int IBRowColListIndex;
    public int PatchListIndex;

    public bool IBSaveOn = true;
    public bool PatchSaveOn = true;
    public bool IBRedo = true;

    public void Initialize()
    {
        Size = MultiMeshManager.Instance.Size;

        MeshList = new List<Mesh>[Size];
        ActionList = new List<int>();
        IBRowColList = new List<int>[2];
        PatchList = new List<int>();
        IndexList = new List<int>[Size];

        IBRowColList[0] = new List<int>();
        IBRowColList[1] = new List<int>();
        for (int i = 0; i < Size; i++)
        {
            MeshList[i] = new List<Mesh>();
            IndexList[i] = new List<int>();
        }

        test = new Mesh[Size];

        ActionListIndex = 0;
        IBRowColListIndex = 0;
        PatchListIndex = 0;

        SaveFirst();
    }
    public void SaveFirst()
    {
        ActionList.Add(-1);
        for (int i = 0; i < Size; i++)
        {
            MeshList[i].Add(GetIndexMesh(i));
            test[i] = MeshList[i].ElementAt(MeshList[i].Count-1);
        }
    }
    public Mesh GetIndexMesh(int i)
    {
        return Instantiate(GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).GetComponent<MeshFilter>().mesh);
    }
    public void SliceSave()
    {
        ActionList.Add(0);
        for (int i = 0; i < Size; i++)
        {
            MeshList[i].Add(GetIndexMesh(i));
            IndexList[i].Add(0);
            test[i] = MeshList[i].ElementAt(MeshList[i].Count - 1);
        }

        ActionListIndex++;
    }
    public void IBSave(int index)
    {
        ActionList.Add(1);
        IndexList[index].Add(0);

        IBRowColList[0].Add(index);
        IBRowColList[1].Add(MeshList[index].Count-1);

        ActionListIndex++;
        if(IBSaveOn == false)
            IBRowColListIndex++;
        if (IBRowColListIndex == 0)
            IBSaveOn = false;
    }
    public void PatchSave()
    {
        ActionList.Add(2);
        PatchList.Add(ActionList.Count - 1);

        ActionListIndex++;
        if (PatchSaveOn == false)
            PatchListIndex++;
        if (PatchListIndex == 0)
            PatchSaveOn = false;
    }
    public void UndoMesh()
    {
        if (ActionListIndex == 0)
            return;

        if (ActionList.ElementAt(ActionListIndex) == -1)
        {
            for (int i = 0; i < Size; i++)
            {
                GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).GetComponent<MeshFilter>().mesh = MeshList[i].ElementAt(IndexList[i].Count - 1); //MeshList[i].ElementAt(0);
                IndexList[i].RemoveAt(IndexList[i].Count - 1);
            }
        }
        else if(ActionList.ElementAt(ActionListIndex) == 0)
        {
            for (int i = 0; i < Size; i++)
            {
                GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).GetComponent<MeshFilter>().mesh = MeshList[i].ElementAt(IndexList[i].Count- 1);
                IndexList[i].RemoveAt(IndexList[i].Count - 1);
            }
        }
        else if (ActionList.ElementAt(ActionListIndex) == 1)
        {
            GameObject.Find("PartialModel").transform.GetChild(IBRowColList[0].ElementAt(IBRowColListIndex)).transform.GetChild(0).GetComponent<MeshFilter>().mesh = MeshList[IBRowColList[0].ElementAt(IBRowColListIndex)].ElementAt(IBRowColList[1].ElementAt(IBRowColListIndex)-1);
            IndexList[IBRowColList[0].ElementAt(IBRowColListIndex)].RemoveAt(IndexList[IBRowColList[0].ElementAt(IBRowColListIndex)].Count - 1);
            IBRowColListIndex--;
        }
        else if (ActionList.ElementAt(ActionListIndex) == 2)
        {
            GameObject.Find("Patch" + PatchListIndex.ToString()).transform.position = new Vector3(3000, 0, 0);
            PatchListIndex--;
        }
        ActionListIndex--;
    }
    public void RedoMesh()
    {
        if (ActionListIndex == ActionList.Count - 1)
            return;

        ActionListIndex++;
        if(ActionList.ElementAt(ActionListIndex) == 0)
        {
            for(int i=0;i<Size;i++)
            {
                IndexList[i].Add(0);
                GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).GetComponent<MeshFilter>().mesh = MeshList[i].ElementAt(IndexList[i].Count);
            }
        }
        else if(ActionList.ElementAt(ActionListIndex) == 1)
        {
            IBRowColListIndex++;
            IndexList[IBRowColList[0].ElementAt(IBRowColListIndex)].Add(0);
            GameObject.Find("PartialModel").transform.GetChild(IBRowColList[0].ElementAt(IBRowColListIndex)).transform.GetChild(0).GetComponent<MeshFilter>().mesh = MeshList[IBRowColList[0].ElementAt(IBRowColListIndex)].ElementAt(IndexList[IBRowColList[0].ElementAt(IBRowColListIndex)].Count);
        }
        else if(ActionList.ElementAt(ActionListIndex) == 2)
        {
            PatchListIndex++;
            GameObject.Find("Patch" + PatchListIndex.ToString()).transform.position = new Vector3(0, 0, 0);
        }
    }

    //// Slice = 1, Boundary,Incision = 2, Patch = 3
    //public int Slice = 1;
    //public int IncisionBoundary = 2;
    //public int Patch = 3;

    //private int j;
    //private bool exist = false;
    //private int IB_index;
    //private bool IB_exist = false;
    //private int S_index;
    //private bool S_exist = false;

    //private int IBCount = 0;

    //private bool IBfisrt = true;
    //private bool Pfirst = true;
    //[SerializeField]
    //public List<Mesh> OriginalMesh;

    //public List<int> TotalArray;
    ////public List<Mesh>[] SliceArray;
    //public List<Mesh[]> SliceArray;
    ////public List<Mesh>[] IncisionBoundaryArray;
    //public List<Mesh> IncisionBoundaryArray;
    //public List<int> PatchArray;
    //public List<int> MeshIndexArray;
    //public int[] MeshIndexCountArray;

    //public int CurrentIndex = 0;
    //public int MaxIndex = 0;
    //public int TotalIndex = 0;

    //public int SliceIndex = 0;
    //public int IncisionBoundaryIndex = 0;
    //public int PatchIndex = 0;
    //public int MeshIndex = 0;
    
    //public void Initialize()
    //{
    //    OriginalMesh = new List<Mesh>();
    //    TotalArray = new List<int>();
    //    SliceArray = new List<Mesh[]>();
    //    IncisionBoundaryArray = new List<Mesh>();
    //    PatchArray = new List<int>();
    //    MeshIndexArray = new List<int>();
    //    MeshIndexCountArray = new int[MultiMeshManager.Instance.Size];
    //    for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
    //        MeshIndexCountArray[i] = 0;
    //    SaveSliceFirst();
    //}
    //public void SaveSliceFirst()
    //{
    //    TotalArray.Add(Slice);
    //    SliceArray.Add(SlicedMesh());
    //}
    //public Mesh FindChildMesh(int MeshIndex)
    //{
    //    return Instantiate(GameObject.Find("PartialModel").transform.GetChild(MeshIndex).transform.GetChild(0).GetComponent<MeshFilter>().mesh);
    //}
    //public void SaveSliceMode()
    //{
    //    TotalArray.Add(Slice);
    //    SliceArray.Add(SlicedMesh());
    //    TotalIndex++;
    //    SliceIndex++;
    //}
    //public Mesh[] SlicedMesh()
    //{
    //    Mesh[] sliced = new Mesh[MultiMeshManager.Instance.Size];
        
    //    for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
    //        sliced[i] = FindChildMesh(i);

    //    return sliced;
    //}
    //public void SaveIncisionBoundaryMode(int MeshIndexes)
    //{
    //    if (IBfisrt == true)
    //    {
    //        TotalArray.Add(IncisionBoundary);
    //        IncisionBoundaryArray.Add(FindChildMesh(MeshIndexes));
    //        MeshIndexArray.Add(MeshIndexes);
    //        TotalIndex++;
    //        IBfisrt = false;
    //    }
    //    else
    //    {
    //        TotalArray.Add(IncisionBoundary);
    //        IncisionBoundaryArray.Add(FindChildMesh(MeshIndexes));
    //        MeshIndexArray.Add(MeshIndexes);
    //        TotalIndex++;
    //        IncisionBoundaryIndex++;
    //        MeshIndex++;
    //    }
    //}
    //public void SavePatchMode()
    //{
    //    if (Pfirst == true)
    //    {
    //        TotalArray.Add(Patch);
    //        TotalIndex++;
    //        PatchArray.Add(TotalIndex);
    //        Pfirst = false;
    //    }
    //    else
    //    {
    //        TotalArray.Add(Patch);
    //        TotalIndex++;
    //        PatchArray.Add(TotalIndex);
    //        PatchIndex++;
    //    }
    //}

    //public void DeletePatchMode()
    //{
        
    //}
    //public void UndoMesh()
    //{
    //    if(TotalArray[TotalIndex] == 1)
    //    {
    //        for(int i=TotalIndex-1;i>=0;i--)
    //        {
    //            if (TotalArray[i] == IncisionBoundary)
    //                IBCount++;

    //            if (TotalArray[i] == Slice)
    //                break;
    //        }

    //        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
    //            GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).GetComponent<MeshFilter>().mesh = SliceArray.ElementAt(SliceIndex - 1)[i];

    //        if (IBCount != 0)
    //        {
    //            for (int i = MeshIndex; i >= MeshIndex - (IBCount - 1); i--)
    //                GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[i]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = IncisionBoundaryArray[i];
    //        }
    //        IBCount = 0;
    //        SliceIndex--;
    //    }
    //    else if(TotalArray[TotalIndex] == 2)
    //    {
    //        if(TotalArray[TotalIndex - 1] == 1)
    //        {
    //            GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[MeshIndex]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = SliceArray.ElementAt(SliceIndex)[MeshIndexArray[MeshIndex]];
    //        }
    //        else if(TotalArray[TotalIndex - 1] == 2)
    //        {
    //            for (int i = MeshIndex - 1; i >= 0; i--)
    //            {
    //                if(MeshIndexArray[i] == MeshIndexArray[MeshIndex])
    //                {
    //                    j = i;
    //                    exist = true;
    //                    break;
    //                }
    //            }
    //            Debug.Log(exist);
    //            if (exist == true)
    //                GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[MeshIndex]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = IncisionBoundaryArray[j];
    //            else
    //                GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[MeshIndex]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = SliceArray.ElementAt(SliceIndex)[MeshIndexArray[MeshIndex]];

    //            exist = false;
    //        }
    //        else if(TotalArray[TotalIndex-1] == 3)
    //        {
    //            for(int i = TotalIndex-1;i>=0;i--)
    //            {
    //                if(TotalArray[i] == IncisionBoundary)
    //                {
    //                    IB_exist = true;
    //                    break;
    //                }
    //                else if(TotalArray[i] == Slice)
    //                {
    //                    S_exist = true;
    //                    break;
    //                }
    //            }
    //            Debug.Log("IB: "+IB_exist);
    //            Debug.Log("S: "+S_exist);
    //            if (IB_exist == true && S_exist == false)
    //            {
    //                for (int i = MeshIndex - 1; i >= 0; i--)
    //                {
    //                    if(MeshIndexArray[MeshIndex] == MeshIndexArray[i])
    //                    {
    //                        Debug.Log(MeshIndexArray[MeshIndex]);
    //                        IB_index = i;
    //                        break;
    //                    }
    //                }
    //                if (IB_index != 0)
    //                    GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[MeshIndex]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = IncisionBoundaryArray.ElementAt(IB_index);
    //                else
    //                    GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[MeshIndex]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = SliceArray.ElementAt(SliceIndex)[MeshIndexArray[MeshIndex]];
    //                IB_index = 0;
    //                IB_exist = false;
    //            }
    //            else if(IB_exist == false && S_exist == true)
    //            {
    //                GameObject.Find("PartialModel").transform.GetChild(MeshIndexArray[MeshIndex]).transform.GetChild(0).GetComponent<MeshFilter>().mesh = SliceArray.ElementAt(SliceIndex)[MeshIndexArray[MeshIndex]];
    //                S_exist = false;
    //            }
    //        }
    //        MeshIndex--;
    //        IncisionBoundaryIndex--;
    //    }
    //    else if(TotalArray[TotalIndex] == 3)
    //    {
    //        DisapearedPatch(PatchIndex);
    //        PatchIndex--;
    //    }
    //    TotalIndex--;
    //}
    //public void RedoMesh()
    //{
    //    if(TotalArray[CurrentIndex] == 1)
    //    {

    //    }
    //    else if(TotalArray[CurrentIndex] == 2)
    //    {

    //    }
    //    else if (TotalArray[CurrentIndex] == 3)
    //    {

    //    }
    //}
    //public void DisapearedPatch(int num)
    //{
    //    GameObject.Find("OuterPatch" + num.ToString()).transform.localPosition = new Vector3(3000, 0, 0);
    //    GameObject.Find("InnerPatch" + num.ToString()).transform.localPosition = new Vector3(3000, 0, 0);
    //}
}