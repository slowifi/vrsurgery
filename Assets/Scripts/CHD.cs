using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class CHD : MonoBehaviour
{
    private GameObject mode;
    public int Detect_Second = 0;
    public void SliceMode()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("SliceMode");
        mode.AddComponent<SliceMode>();
    }

    public void CutMode()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("CutMode");
        mode.AddComponent<BoundaryCutMode>();
    }

    public void PatchMode()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("PatchMode");
        mode.AddComponent<PatchMode>();
    }

    public void MeasureMode()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("MeasureMode");
        mode.AddComponent<MeasureMode>();
    }

    public void IncisionMode()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("IncisionMode");
        mode.AddComponent<IncisionMode>();
        UIManager.Instance.extendBar.value = 0.1f;
    }

    public void MeasureDiameterMode()
    {
        AdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("MeasureDiameterMode");
        mode.AddComponent<MeasureDiameterMode>();
    }

    public void Exit()
    {
        EventManager.Instance.Events.InvokeModeManipulate("EndAll");
        MeshManager.Instance.SaveCurrentMesh();

        if (mode != null)
        {
            if (mode.name == "PatchMode")
            {
                //GameObject patchObject = GameObject.Find("Patch");// + (PatchManager.Instance.newPatch.Count - 1));
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
        MeshManager.Instance.startMeasurePoint.SetActive(false);
        MeshManager.Instance.endMeasurePoint.SetActive(false);
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

        MeshManager.Instance.ObjUpdate();
        MeshManager.Instance.Reinitialize();
        AdjacencyList.Instance.ListUpdate();
        MakeDoubleFaceMesh.Instance.Reinitialize();
        MeshManager.Instance.startMeasurePoint.SetActive(false);
        MeshManager.Instance.endMeasurePoint.SetActive(false);
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
        MeshManager.Instance.ObjUpdate();
        MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        MakeDoubleFaceMesh.Instance.Initialize();
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
