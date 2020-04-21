using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class CHD : MonoBehaviour
{
    public GameObject playerObject;
    private GameObject mode;
    private bool isOn = false;

    public void SliceMode()
    {
        isOn = true;
        mode = new GameObject("SliceMode");
        mode.AddComponent<SliceMode>();
    }

    public void CutMode()
    {
        isOn = true;
        mode = new GameObject("CutMode");
        mode.AddComponent<BoundaryCutMode>();
        //playerObject.SendMessage("BoundaryModeOn");
    }

    public void StartPatchMode()
    {
        isOn = true;
        mode = new GameObject("PatchMode");
        mode.AddComponent<PatchMode>();
    }

    public void StartMeasureMode()
    {
        Destroy(mode);
        isOn = true;
        //mode = new GameObject("MeasureMode").AddComponent<MeasureMode>();
    }

    public void StartIncisionMode()
    {
        Destroy(mode);
        isOn = true;
        //mode = new GameObject("IncisionMode").AddComponent<IncisionMode>();
        playerObject.SendMessage("IncisionModeOn");
        UIManager.Instance.extendBar.value = 0;
    }

    public void ButtonOff()
    {
        isOn = false;
        playerObject.SetActive(true);
        playerObject.SendMessage("BoundaryModeOff");
        playerObject.SendMessage("IncisionModeOff");
        MeshManager.Instance.LoadOldMesh();
        ButtonPress.Instance.ResetButton();
        Exit();
    }

    public void Exit()
    {

        Debug.Log("Exit");
        playerObject.SetActive(true);
        playerObject.SendMessage("BoundaryModeOff");
        playerObject.SendMessage("IncisionModeOff");
        MeshManager.Instance.SaveCurrentMesh();

        Destroy(mode);

        MeshManager.Instance.startMeasurePoint.SetActive(false);
        MeshManager.Instance.endMeasurePoint.SetActive(false);

        GameObject patchObject = GameObject.Find("Patch" + (PatchManager.Instance.newPatch.Count - 1));
        if (patchObject)
        {
            MeshRenderer ren = patchObject.GetComponent<MeshRenderer>();
            if (ren.material.color != new Color32(115, 0, 0, 255))
            {
                patchObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                MakeDoubleFaceMesh.Instance.MakePatchInnerFace(patchObject);
                ren.material.color = new Color32(115, 0, 0, 255);
            }
        }
    }

    public void ResetMain()
    {
        for (int i = 0; i < PatchManager.Instance.newPatch.Count; i++)
        {
            Destroy(PatchManager.Instance.newPatch[i]);
            Destroy(GameObject.Find("Patch" + i + "_Inner"));
        }
        MeshManager.Instance.ObjUpdate();
        MeshManager.Instance.Reinitialize();
        AdjacencyList.Instance.ListUpdate();
        PatchManager.Instance.Reinitialize();
        IncisionManager.Instance.Reinitialize();
        Debug.Log("reined");
        MakeDoubleFaceMesh.Instance.Reinitialize();
        playerObject.SetActive(true);
        playerObject.SendMessage("IncisionModeOff");
        playerObject.SendMessage("BoundaryModeOff");
        //lineRenderer = new GameObject>();
        MeshManager.Instance.startMeasurePoint.SetActive(false);
        MeshManager.Instance.endMeasurePoint.SetActive(false);

        Destroy(mode);

    }


    void Start()
    {
        Debug.Log("Load되었습니다.");
        MeshManager.Instance.ObjUpdate();
        MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        PatchManager.Instance.Initialize();
        IncisionManager.Instance.Initialize();
        MakeDoubleFaceMesh.Instance.Initialize();
        playerObject.SetActive(true);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            AdjacencyList.Instance.WorldPositionUpdate();

            IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
            bool checkInside = intersectedValues.Intersected;
            if (!checkInside)
                return;
        }

        if (mode == null && isOn)
        {
            ButtonOff();
        }

    }


}
