using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class CHD : MonoBehaviour
{
    public GameObject[] Buttons = new GameObject[8];

    private GameObject mode;
    public GameObject ExtendBar;
    public GameObject HeightBar;
    public GameObject CurveBar;
    public GameObject PatchText;

    public bool CutModeState = false;
    public bool SliceModeState = false;
    public bool PatchModeState = false;
    public bool IncisionModeSate = false;

    public int BtnIndex;
    public int MeshIndex;

    void Awake()
    {
        EventManager.Instance.Events.OnModeChanged += Events_OnModeChanged;

        MultiMeshManager.Instance.Invoke("Initialize", 0.1f);
        MultiMeshAdjacencyList.Instance.Invoke("Initialize", 0.1f);
        GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().Invoke("Initialize", 0.1f);
    }
    public void SliceMode()
    {
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("SliceMode");
        mode.AddComponent<MultiMeshSliceMode>();
    }
    public void IncisionMode()
    {
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("IncisionMode");
        mode.AddComponent<MultiMeshIncisionMode>();
        UIManager.Instance.extendBar.value = 0.1f;
    }
    public void CutMode()
    {
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("CutMode");
        mode.AddComponent<MultiMeshBoundaryCutMode>();
    }
    public void PatchMode()
    {
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("PatchMode");
        mode.AddComponent<MultiMeshPatchMode>();
    }
    public void MeasureMode()
    {
        MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
        mode = new GameObject("MeasureMode");
        mode.AddComponent<MultiMeshMeasureMode>();
    }
    private void Events_OnModeChanged(string mode)
    {
        switch (mode)
        {
            case "IncisionMode":
                IncisionMode();
                ButtonInteractable("Incision Button");
                Debug.Log("incision 실행");
                break;
            case "CutMode":
                CutMode();
                ButtonInteractable("Cutting Button");
                Debug.Log("cut 실행");
                break;
            case "PatchMode":
                PatchMode();
                ButtonInteractable("Patching Button");
                Debug.Log("patch 실행");
                break;
            case "SliceMode":
                SliceMode();
                ButtonInteractable("Slicing Button");
                Debug.Log("slice 실행");
                break;
            case "MeasureMode":
                MeasureMode();
                ButtonInteractable("Extended Measure Distance Button");
                Debug.Log("measure 실행");
                break;
            case "Exit":
                Exit();
                AllButtonInteractable();
                Debug.Log("Exit");
                break;
        }
    }
    public void Exit()
    {
        EventManager.Instance.Events.InvokeModeManipulate("EndAll");

        if (mode != null)
        {
            if (mode.name == "PatchMode")
            {
                GameObject PatchObject = MultiMeshManager.Instance.PatchList[MultiMeshManager.Instance.PatchList.Count - 1].Patch;
                if (PatchObject)
                {
                    MeshRenderer outerRen = PatchObject.GetComponent<MeshRenderer>();
                    if (outerRen.material.color != new Color32(115, 0, 0, 255))
                    {
                        PatchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        outerRen.material.color = new Color32(115, 0, 0, 255);
                    }
                }
            }
            Destroy(mode);
        }
        MultiMeshManager.Instance.StartMeasurePoint.SetActive(false);
        MultiMeshManager.Instance.EndMeasurePoint.SetActive(false);
    }
    public void ResetMain()
    {
        if (mode != null)
        {
            if (mode.name == "PatchMode")
            {
                // 수정해야됨.
                GameObject PatchObject = MultiMeshManager.Instance.PatchList[MultiMeshManager.Instance.PatchList.Count - 1].Patch;

                if (PatchObject)
                {
                    MeshRenderer Ren = PatchObject.GetComponent<MeshRenderer>();

                    if (Ren.material.color != new Color32(115, 0, 0, 255))
                    {
                        PatchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        Ren.material.color = new Color32(115, 0, 0, 255);
                    }
                }
            }
            Destroy(mode);
        }
        MultiMeshManager.Instance.Invoke("ReInitialize", 0.1f);
        MultiMeshAdjacencyList.Instance.Invoke("Initialize", 0.1f);
        MultiMeshManager.Instance.StartMeasurePoint.SetActive(false);
        MultiMeshManager.Instance.EndMeasurePoint.SetActive(false);
    }
    private void ButtonInteractable(string ButtonName)
    {
        for(int i=0;i<Buttons.Length;i++)
            if (ButtonName == Buttons[i].name)
                BtnIndex = i;

        for(int i=0;i<Buttons.Length;i++)
            if (i != BtnIndex)
                Buttons[i].GetComponent<Button>().interactable = false;

        if(ButtonName == "Incision Button")
            ExtendBar.SetActive(true);
        else if(ButtonName == "Patching Button")
            SetPatchUI(true);
    }
    public void AllButtonInteractable()
    {
        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].GetComponent<Button>().interactable = true;
    }
    public void AllButtonInteractableFalse()
    {
        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].GetComponent<Button>().interactable = false;

        ExtendBar.SetActive(false);
        SetPatchUI(false);
    }
    public void SetPatchUI(bool State)
    {
        HeightBar.SetActive(State);
        CurveBar.SetActive(State);
        PatchText.SetActive(State);
    }
    public void InitButtons()
    {
        for (int i = 1; i < 9; i++)
            Buttons[i-1] = GameObject.Find("Methods").transform.GetChild(i).gameObject;
    }
}
