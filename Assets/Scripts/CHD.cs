using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class CHD : MonoBehaviour
{
    // main
    private bool firstIncision = false;
    public bool isExtend = false;
    private float oldExtendValue;
    private int incisionCount;

    public GameObject playerObject;

    private int patchCount;

    public bool isBoundaryCutMode = false;
    public int boundaryCount;
    public Vector3 firstPosition;
    public Vector3 oldPosition;

    // line renderer
    public GameObject lineRenderer;

    // test
    public bool isFirstPatch = true;
    public bool isPatchUpdate = false;
    public bool isTestMode = true;


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

        isExtend = false;
        isFirstPatch = true;
    }

    public void PatchMode()
    {
        mode = "patch";


        isFirstPatch = true;
        isExtend = false;
        isPatchUpdate = false;
        oldPosition = Vector3.zero;
        patchCount = 0;
    }

    public void MeasureMode()
    {
        mode = "measure";

        isExtend = false;
    }

    public void StartIncisionMode()
    {
        mode = "incision";
        playerObject.SendMessage("IncisionModeOn");
        UIManager.Instance.extendBar.value = 0;
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


        isExtend = false;
        isPatchUpdate = false;

        isFirstPatch = true;
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

        isPatchUpdate = false;
        isFirstPatch = true;
        BoundaryCutMode.Instance.SetIsLastBoundaryCut(false);

        boundaryCount = 0;
        oldExtendValue = 0;
        incisionCount = -1;
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
        boundaryCount = 0;
        oldExtendValue = 0;
        incisionCount = -1;
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
            bool isButtonOff = IncisionMode.Instance.OnUpdate(playerObject);
            if (isButtonOff)
            {
                ButtonOff();
            }
        }
        else if (mode == "boundaryCut")
        {
            bool isButtonOff = BoundaryCutMode.Instance.OnUpdate();
            if (isButtonOff)
            {
                ButtonOff();
            }
        }
        else if (mode == "measure")
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                float dst = MeasureManager.Instance.MeasureDistance(vertexPosition, ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                dst = dst / ObjManager.Instance.objTransform.lossyScale.z;
                UIManager.Instance.distance.text = dst + "mm";
            }
        }
        else if (mode == "patch")
        {
            // 처음에 실행되어야함.
            if (isFirstPatch)
            {
                Debug.Log("Patch 실행");
                isFirstPatch = false;
                return;
            }
            else if (isPatchUpdate)
            {
                // 숫자에 patch index들어가는게 좋을듯. 지금 patch, incision 관련해서는 리스트화는 시켜놨음. 추후 undo등 작업 가능.
                playerObject.SetActive(true);
                PatchManager.Instance.UpdateCurve(PatchManager.Instance.newPatch.Count - 1);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                if (vertexPosition != Vector3.zero)
                {
                    firstPosition = vertexPosition;
                    playerObject.SetActive(false);
                    AdjacencyList.Instance.ListUpdate();
                    PatchManager.Instance.Generate();
                    PatchManager.Instance.AddVertex(vertexPosition);
                    oldPosition = vertexPosition;

                    Vector3 oldPos = oldPosition;
                    oldPos.z += 1f;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (oldPosition == Vector3.zero)
                    return;
                Destroy(lineRenderer);
                PatchManager.Instance.GenerateMesh();
                isPatchUpdate = true;
            }
            else if (Input.GetMouseButton(0))
            {
                if (oldPosition == Vector3.zero)
                    return;
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                if (vertexPosition != Vector3.zero)
                {
                    //first position이 저장되어 있어야함.
                    if (patchCount > 8 && Vector3.Distance(firstPosition, vertexPosition) < 2.0f * ObjManager.Instance.pivotTransform.lossyScale.z)
                    {
                        Destroy(lineRenderer);
                        PatchManager.Instance.GenerateMesh();
                        isPatchUpdate = true;
                        return;
                    }

                    PatchManager.Instance.AddVertex(vertexPosition);
                    LineRenderer line;

                    if (patchCount != 0)
                    {
                        line = lineRenderer.GetComponent<LineRenderer>();
                        line.positionCount++;
                    }
                    else
                    {
                        lineRenderer = new GameObject("Patch Line", typeof(LineRenderer));
                        lineRenderer.layer = 8;
                        line = lineRenderer.GetComponent<LineRenderer>();
                        line.numCornerVertices = 45;
                        line.material.color = Color.black;
                        line.SetPosition(0, oldPosition);
                    }

                    line.SetPosition(patchCount + 1, vertexPosition);
                    patchCount++;
                    oldPosition = vertexPosition;
                    return;
                }
                else
                {
                    if (patchCount == 0)
                        return;
                    Destroy(lineRenderer);
                    PatchManager.Instance.RemovePatchVariables();
                    ChatManager.Instance.GenerateMessage(" 패치 라인이 심장을 벗어났습니다.");
                    // 이게 또 겹쳐부렀네
                    ButtonOff();
                }
            }
        }

    }


}
