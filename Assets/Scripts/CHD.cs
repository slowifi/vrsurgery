using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class CHD : MonoBehaviour
{
    public GameObject playerObject;
    private Mode mode;
    private bool isOn = false;

    // private void UpdateByMode()
    // {
    //     if (mode == "Incision")
    //     {
    //         if (IncisionMode.Instance.OnUpdate(playerObject)) { ButtonOff(); }
    //     }
    //     else if (mode == "Boundary")
    //     {
    //         if (BoundaryCutMode.Instance.OnUpdate()) { ButtonOff(); }
    //     }
    //     else if (mode == "Measure")
    //     {
    //         MeasureMode.Instance.OnUpdate();
    //     }
    //     else if (mode == "Patch")
    //     {
    //         if (PatchMode.Instance.OnUpdate(playerObject)) { ButtonOff(); }
    //     }
    // }

    public void CutMode()
    {
        isOn = true;
        mode = playerObject.AddComponent<BoundaryCutMode>();
        playerObject.SendMessage("BoundaryModeOn");
    }

    public void StartPatchMode()
    {
        isOn = true;
        mode = playerObject.AddComponent<PatchMode>();
    }

    public void StartMeasureMode()
    {
        isOn = true;
        mode = playerObject.AddComponent<MeasureMode>();
    }

    public void StartIncisionMode()
    {
        isOn = true;
        mode = playerObject.AddComponent<IncisionMode>();
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

        Destroy(GameObject.Find("MeasureLine"));
        ObjManager.Instance.startMeasurePoint.SetActive(false);
        ObjManager.Instance.endMeasurePoint.SetActive(false);

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
        ObjManager.Instance.ObjUpdate();
        MeshManager.Instance.Reinitialize();
        AdjacencyList.Instance.ListUpdate();
        PatchManager.Instance.Reinitialize();
        IncisionManager.Instance.Reinitialize();
        Debug.Log("reined");
        BoundaryCutManager.Instance.Reinitialize();
        MakeDoubleFaceMesh.Instance.Reinitialize();
        playerObject.SetActive(true);
        playerObject.SendMessage("IncisionModeOff");
        playerObject.SendMessage("BoundaryModeOff");
        //lineRenderer = new GameObject>();
        Destroy(GameObject.Find("MeasureLine"));
        ObjManager.Instance.startMeasurePoint.SetActive(false);
        ObjManager.Instance.endMeasurePoint.SetActive(false);

        Destroy(mode);

    }


    void Start()
    {
        Debug.Log("Load되었습니다.");
        ObjManager.Instance.ObjUpdate();
        MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        PatchManager.Instance.Initialize();
        IncisionManager.Instance.Initialize();
        BoundaryCutManager.Instance.Initialize();
        MakeDoubleFaceMesh.Instance.Initialize();
        playerObject.SetActive(true);

    }

    void Update()
    {
        Ray cameraRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            AdjacencyList.Instance.WorldPositionUpdate();

            IntersectedValues intersectedValues = Intersections.GetIntersectedValues(cameraRay, triangles, worldPosition);
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
