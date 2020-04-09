using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class CHD : MonoBehaviour
{
    // main
    public bool isExtend = false;
    private float oldExtendValue;
    private int incisionCount;

    public GameObject playerObject;



    // test
    public bool isFirstPatch = true;
    public bool isPatchUpdate = false;

    private string _mode = "none";
    private string mode
    {
        get
        {
            return _mode;
        }
        set
        {
            _mode = value;
        }
    }

    public void CutMode()
    {
        playerObject.SendMessage("BoundaryModeOn");
        mode = "boundaryCut";

    }

    public void StartPatchMode()
    {
        mode = "patch";
        PatchMode.Instance.Start();
    }

    public void StartMeasureMode()
    {
        mode = "measure";
    }

    public void StartIncisionMode()
    {
        mode = "incision";
        IncisionMode.Instance.Start();
        playerObject.SendMessage("IncisionModeOn");
    }

    public void ButtonOff()
    {
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

        mode = "none";

        BoundaryCutMode.Instance.SetIsLastBoundaryCut(false);

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
        BoundaryCutManager.Instance.Reinitialize();
        MakeDoubleFaceMesh.Instance.Reinitialize();
        playerObject.SetActive(true);
        playerObject.SendMessage("IncisionModeOff");
        playerObject.SendMessage("BoundaryModeOff");
        //lineRenderer = new GameObject>();
        Destroy(GameObject.Find("MeasureLine"));
        ObjManager.Instance.startMeasurePoint.SetActive(false);
        ObjManager.Instance.endMeasurePoint.SetActive(false);

        mode = "none";

        BoundaryCutMode.Instance.SetIsLastBoundaryCut(false);

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
        IncisionMode.Instance.Initialize();
        BoundaryCutMode.Instance.Initialize();
        playerObject.SetActive(true);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            AdjacencyList.Instance.WorldPositionUpdate();
            if (!IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
                return;
        }

        if (mode == "incision")
        {
            if (IncisionMode.Instance.OnUpdate(playerObject)) { ButtonOff(); }
        }
        else if (mode == "boundaryCut")
        {
            if (BoundaryCutMode.Instance.OnUpdate()) { ButtonOff(); }
        }
        else if (mode == "measure")
        {
            MeasureMode.Instance.OnUpdate();
        }
        else if (mode == "patch")
        {
            if (PatchMode.Instance.OnUpdate(playerObject)) { ButtonOff(); }
        }

    }


}
