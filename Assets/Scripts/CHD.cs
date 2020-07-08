using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class CHD : MonoBehaviour
{
    public bool CutModeState = false;
    public bool SliceModeState = false;
    public bool PatchModeState = false;
    public bool IncisionModeSate = false;
    public int MeshIndex;
    private GameObject mode;
    private int PatchNum = 0;
    public int Detect_Second = 0;
    public void SliceMode()
    {
        //AdjacencyList.Instance.WorldPositionUpdate();
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("SliceMode");
        mode.AddComponent<MultiMeshSliceMode>();
        SliceModeState = true;
    }   

    public void CutMode()
    {
        //AdjacencyList.Instance.WorldPositionUpdate();
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("CutMode");
        //mode.AddComponent<BoundaryCutMode>();
        mode.AddComponent<MultiMeshBoundaryCutMode>();
        CutModeState = true;

    }

    public void PatchMode()
    {
        //AdjacencyList.Instance.WorldPositionUpdate();
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("PatchMode");
        //mode.AddComponent<MultiMeshPatchMode>();
        mode.AddComponent<MultiMeshPatchMode>();
        PatchModeState = true;
    }

    public void MeasureMode()
    {
        //AdjacencyList.Instance.WorldPositionUpdate();
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("MeasureMode");
        mode.AddComponent<MultiMeshMeasureMode>();
    }

    public void IncisionMode()
    {
        //AdjacencyList.Instance.WorldPositionUpdate();
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("IncisionMode");
        mode.AddComponent<MultiMeshIncisionMode>();
        UIManager.Instance.extendBar.value = 0.1f;
        IncisionModeSate = true;
    }

    public void MeasureDiameterMode()
    {
        //AdjacencyList.Instance.WorldPositionUpdate();
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("MeasureDiameterMode");
        mode.AddComponent<MeasureDiameterMode>();
    }

    public void Exit()
    {
        EventManager.Instance.Events.InvokeModeManipulate("EndAll");
        //MeshManager.Instance.SaveCurrentMesh();

        if (mode != null)
        {
            if (mode.name == "PatchMode")
            {
                if(GameObject.Find("OuterPatch")!=null)
                {
                    GameObject.Find("OuterPatch").name = "OuterPatch" + PatchNum.ToString();
                    GameObject.Find("InnerPatch").name = "InnerPatch" + PatchNum.ToString();
                    PatchNum++;
                }
                //GameObject patchObject = GameObject.Find("Patch");// + (PatchManager.Instance.newPatch.Count - 1));
                GameObject outerPatchObject = MultiMeshManager.Instance.PatchList[MultiMeshManager.Instance.PatchList.Count - 1].OuterPatch;
                GameObject innerPatchObject = MultiMeshManager.Instance.PatchList[MultiMeshManager.Instance.PatchList.Count - 1].InnerPatch;
                if (outerPatchObject)
                {
                    MeshRenderer outerRen = outerPatchObject.GetComponent<MeshRenderer>();
                    MeshRenderer innerRen = innerPatchObject.GetComponent<MeshRenderer>();
                    if (outerRen.material.color != new Color32(115, 0, 0, 255))
                    {
                        outerPatchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        innerPatchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        outerRen.material.color = new Color32(115, 0, 0, 255);
                        innerRen.material.color = new Color32(115, 0, 0, 255);
                    }
                }
            }
            Destroy(mode);
        }
        //MeshManager.Instance.startMeasurePoint.SetActive(false);
        //MeshManager.Instance.endMeasurePoint.SetActive(false);
        MultiMeshManager.Instance.MultiMeshStartMeasurePoint.SetActive(false);
        MultiMeshManager.Instance.MultiMeshEndMeasurePoint.SetActive(false);
    }

    public void ResetMain()
    {

        if (mode != null)
        {
            if (mode.name == "PatchMode")
            {
                // 수정해야됨.
                GameObject outerPatchObject = MeshManager.Instance.PatchList[MeshManager.Instance.PatchList.Count - 1].OuterPatch;
                GameObject innerPatchObject = MeshManager.Instance.PatchList[MeshManager.Instance.PatchList.Count - 1].InnerPatch;
                if (outerPatchObject)
                {
                    MeshRenderer outerRen = outerPatchObject.GetComponent<MeshRenderer>();
                    MeshRenderer innerRen = innerPatchObject.GetComponent<MeshRenderer>();
                    if (outerRen.material.color != new Color32(115, 0, 0, 255))
                    {
                        outerPatchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        innerPatchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        outerRen.material.color = new Color32(115, 0, 0, 255);
                        innerRen.material.color = new Color32(115, 0, 0, 255);
                    }
                }
            }
            Destroy(mode);
        }

        //MeshManager.Instance.ObjUpdate();
        //AdjacencyList.Instance.ListUpdate();
        //MeshManager.Instance.Reinitialize();
        //MakeDoubleFaceMesh.Instance.Reinitialize();
        //MeshManager.Instance.startMeasurePoint.SetActive(false);
        //MeshManager.Instance.endMeasurePoint.SetActive(false);

        MultiMeshManager.Instance.ObjsUpdate();
        MultiMeshAdjacencyList.Instance.ListsUpdate();
        MultiMeshManager.Instance.Reinitialize();
        MultiMeshMakeDoubleFace.Instance.Reinitialize();
        MultiMeshManager.Instance.MultiMeshStartMeasurePoint.SetActive(false);
        MultiMeshManager.Instance.MultiMeshEndMeasurePoint.SetActive(false);
    }

    private void Events_OnModeChanged(string mode)
    {
        switch (mode)
        {
            case "IncisionMode":
                IncisionMode();
                GameObject.Find("Slicing Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Patching Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Cutting Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = false;
                //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = false;
                Debug.Log("incision 실행");
                Detect_Second++;
                break;
            case "CutMode":
                CutMode();
                GameObject.Find("Incision Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Slicing Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Patching Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = false;
                //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = false;
                Debug.Log("cut 실행");
                Detect_Second++;
                break;
            case "PatchMode":
                PatchMode();
                GameObject.Find("Incision Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Slicing Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Cutting Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = false;
                //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = false;
                Debug.Log("patch 실행");
                Detect_Second++;
                break;
            case "SliceMode":
                SliceMode();
                GameObject.Find("Incision Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Patching Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Cutting Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = false;
                //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = false;
                Debug.Log("slice 실행");
                Detect_Second++;
                break;
            case "MeasureMode":
                MeasureMode();
                GameObject.Find("Incision Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Slicing Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Patching Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Cutting Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = false;
                //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = false;
                Debug.Log("measure 실행");
                break;
            case "MeasureDiameterMode":
                MeasureDiameterMode();
                GameObject.Find("Incision Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Slicing Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Patching Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Cutting Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = false;
                Debug.Log("Measure Diameter 실행");
                break;
            case "Exit":
                Exit();
                GameObject.Find("Incision Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Slicing Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Patching Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Cutting Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Undo Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Redo Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = true;
                //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = true;
                Detect_Second++;
                Debug.Log("Exit");
                if (CutModeState == true)
                {
                    GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().SaveIncisionBoundaryMode(MeshIndex);
                    CutModeState = false;
                }
                else if(SliceModeState == true)
                {
                    GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().SaveSliceMode();
                    SliceModeState = false;
                }
                else if(PatchModeState == true)
                {
                    GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().SavePatchMode();
                    PatchModeState = false;
                }
                else if(IncisionModeSate == true)
                {
                    GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().SaveIncisionBoundaryMode(MeshIndex);
                    IncisionModeSate = false;
                }
                break;
        }
    }

    public int Detect_Second_ButtonCall()
    {
        return Detect_Second;
    }
    public void Detect_Second_NumReset()
    {
        Detect_Second = 0;
    }

    void Awake()
    {
        EventManager.Instance.Events.OnModeChanged += Events_OnModeChanged;

        //MeshManager.Instance.ObjUpdate();
        //MeshManager.Instance.Initialize();
        //AdjacencyList.Instance.Initialize();
        //MakeDoubleFaceMesh.Instance.Initialize();

        MultiMeshManager.Instance.Invoke("ObjsUpdate", 0.1f);
        MultiMeshManager.Instance.Invoke("Initialize", 0.1f);
        MultiMeshAdjacencyList.Instance.Invoke("Initialize", 0.1f);
        MultiMeshMakeDoubleFace.Instance.Invoke("Initialize", 0.1f);
    }


    void Update()
    {
        
        //// 여기서는 아예 update를 안돌려도 될 듯. 
        //if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        //{
        //    AdjacencyList.Instance.WorldPositionUpdate();
        //    IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
        //    bool checkInside = intersectedValues.Intersected;
        //    if (!checkInside)
        //        return;
        //}
        
    }
}
