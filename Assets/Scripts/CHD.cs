using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class CHD : MonoBehaviour
{
    // main
    public bool isCutMode = false;
    public bool isMeasureMode = false;
    public bool isPatchMode = false;

    public bool isIncisionMode = false;
    private bool firstIncision = false;
    public bool isExtend = false;
    private float oldExtendValue;
    private int incisionCount;

    public GameObject playerObject;

    private int patchCount;

    public bool isEraseMode = false;

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
    public bool isLastBoundaryCut = false;

    private void IncisionInit()
    {

    }

    private void BoundaryCutInit()
    {

    }

    private void PatchInit()
    {

    }

    private void MeasureInit()
    {

    }




    public void CutMode()
    {
        playerObject.SendMessage("BoundaryModeOn");
        isCutMode = true;
        isBoundaryCutMode = true;
        isMeasureMode = false;
        isPatchMode = false;
        isIncisionMode = false;
        isExtend = false;
        isFirstPatch = true;
    }

    public void PatchMode()
    {
        isCutMode = false;
        isBoundaryCutMode = false;
        isMeasureMode = false;
        isPatchMode = true;
        isIncisionMode = false;
        isFirstPatch = true;
        isExtend = false;
        isPatchUpdate = false;
        oldPosition = Vector3.zero;
        patchCount = 0;
    }

    public void MeasureMode()
    {
        isCutMode = false;
        isBoundaryCutMode = false;
        isMeasureMode = true;
        isPatchMode = false;
        isIncisionMode = false;
        isExtend = false;
    }

    public void IncisionMode()
    {
        playerObject.SendMessage("IncisionModeOn");
        isCutMode = true;
        isBoundaryCutMode = false;
        isMeasureMode = false;
        isPatchMode = false;
        isIncisionMode = true;
        isExtend = false;
        firstIncision = false;
        oldExtendValue = 0;
        oldPosition = Vector3.zero;
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
        isCutMode = false;
        isBoundaryCutMode = false;
        isMeasureMode = false;
        isPatchMode = false;
        isIncisionMode = false;
        isExtend = false;
        isPatchUpdate = false;
        isFirstPatch = true;
        isLastBoundaryCut = false;
        Destroy(GameObject.Find("MeasureLine"));
        ObjManager.Instance.startMeasurePoint.SetActive(false);
        ObjManager.Instance.endMeasurePoint.SetActive(false);

        GameObject patchObject = GameObject.Find("Patch" + (PatchManager.Instance.newPatch.Count-1));
        if (patchObject)
        {
            MeshRenderer ren = patchObject.GetComponent<MeshRenderer>();
            if (ren.material.color != new Color32(115, 0, 0, 255))
                MakeDoubleFaceMesh.Instance.MakePatchInnerFace(patchObject);
            ren.material.color = new Color32(115, 0, 0, 255);
            
        }
    }

    public void ResetMain()
    {
        for (int i = 0; i < PatchManager.Instance.newPatch.Count; i++)
        {
            Destroy(PatchManager.Instance.newPatch[i]);
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
        

        isCutMode = false;
        isBoundaryCutMode = false;
        isMeasureMode = false;
        isPatchMode = false;
        isIncisionMode = false;
        isExtend = false;
        isPatchUpdate = false;
        isFirstPatch = true;
        isLastBoundaryCut = false;

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
        playerObject.SetActive(true);
        boundaryCount = 0;
        oldExtendValue = 0;
        incisionCount = -1;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            AdjacencyList.Instance.WorldPositionUpdate();
            if (!IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
                return;
        }
        
        if(isCutMode)
        {
            if(isIncisionMode)
            {
                if (isExtend)
                {
                    //추후 incision된 파트들 indexing 해서 관리를 해줘야됨 + undo를 위한 작업도 미리미리 해놓는게 좋음.
                    if (oldExtendValue != UIManager.Instance.extendBar.value)
                    {
                        IncisionManager.Instance.Extending(IncisionManager.Instance.currentIndex - 1, UIManager.Instance.extendBar.value, oldExtendValue);
                        oldExtendValue = UIManager.Instance.extendBar.value;
                        MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                        return;
                    }
                    else
                        return;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Incision 실행");
                    playerObject.SetActive(false);
                    lineRenderer = new GameObject("Incision Line", typeof(LineRenderer));
                    lineRenderer.layer = 8;
                    //incisionCount++;
                    firstIncision = true;
                    IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref oldPosition);
                    IncisionManager.Instance.IncisionUpdate();
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.SetStartVerticesDF();
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    Vector3 currentPosition = Vector3.zero;
                    bool checkInside = IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition);
                    if (checkInside)
                    {
                        if (Vector3.Distance(oldPosition, currentPosition) < 2.5f * ObjManager.Instance.pivotTransform.lossyScale.z)
                        {
                            ChatManager.Instance.GenerateMessage(" incision 거리가 너무 짧습니다.");
                            IncisionManager.Instance.IncisionUpdate();
                            firstIncision = false;
                            return;
                        }
                    }
                    Destroy(lineRenderer);
                    bool checkEdge = false;
                    IncisionManager.Instance.SetEndVerticesDF();
                    IncisionManager.Instance.SetDividingListDF(ref checkEdge);
                    if (checkEdge)
                    {
                        IncisionManager.Instance.leftSide.RemoveAt(IncisionManager.Instance.currentIndex);
                        IncisionManager.Instance.rightSide.RemoveAt(IncisionManager.Instance.currentIndex);
                        //incisionCount--;
                        IncisionManager.Instance.IncisionUpdate();
                        playerObject.SendMessage("IncisionModeOff");
                        ButtonOff();
                        return;
                    }
                    
                    // 위에서 잘못되면 끊어야됨.
                    IncisionManager.Instance.ExecuteDividing();
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.GenerateIncisionList();
                    MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                    MeshManager.Instance.SaveCurrentMesh();
                    IncisionManager.Instance.currentIndex++;
                    MeshManager.Instance.mesh.RecalculateNormals();
                    ChatManager.Instance.GenerateMessage(" 절개하였습니다. 확장이 가능합니다.");
                    playerObject.SetActive(true);
                    isExtend = true;
                }
                else if(Input.GetMouseButton(0))
                {
                    // 이걸 수정을 좀 해야되는데
                    if (!firstIncision)
                    {
                        if(playerObject.activeSelf)
                            playerObject.SendMessage("IncisionModeOff");
                        // 여기에다가 넣으면 여러개가 찍히는게 좀 에반데.
                        return;
                    }
                    var line = lineRenderer.GetComponent<LineRenderer>();
                    Vector3 currentPosition = Vector3.zero;
                    bool checkInside = IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition);
                    if(!checkInside)
                    {
                        Destroy(lineRenderer);
                        ChatManager.Instance.GenerateMessage(" 심장을 벗어났습니다.");
                        //incisionCount--;
                        ButtonOff();
                        playerObject.SendMessage("IncisionModeOff");
                        return;
                    }
                    Vector3 curPos = currentPosition;
                    Vector3 oldPos = oldPosition;
                    curPos.z += 1f;
                    oldPos.z += 1f;
                    line.material.color = Color.black;
                    line.SetPositions(new Vector3[] { oldPos, curPos });
                }
            }
            else if(isEraseMode)
            {

            }
            else if(isBoundaryCutMode)
            {
                // 조건을 잘 짜야됨.

                if (isFirstPatch)
                {
                    Debug.Log("Boundary cut 실행");
                    //playerObject.SetActive(false);
                    MeshManager.Instance.SaveCurrentMesh();
                    AdjacencyList.Instance.ListUpdate();
                    isFirstPatch = false;
                    boundaryCount = 0;
                    return;
                }

                if (isLastBoundaryCut)
                {
                    bool checkError = true;
                    // 이걸 뒤에 넣어서 한프레임 늦게 실행 되도록 하기.
                    checkError = BoundaryCutManager.Instance.PostProcess();
                    if (!checkError)
                    {
                        Destroy(lineRenderer);
                        ButtonOff();
                        return;
                    }
                    MeshManager.Instance.mesh.RecalculateNormals();

                    Destroy(lineRenderer);
                    AdjacencyList.Instance.ListUpdate();
                    if (!BoundaryCutManager.Instance.AutomaticallyRemoveTriangles())
                    {
                        ChatManager.Instance.GenerateMessage(" 영역이 잘못 지정되었습니다.");
                        MeshManager.Instance.LoadOldMesh();
                    }
                    else
                        MeshManager.Instance.SaveCurrentMesh();
                    AdjacencyList.Instance.ListUpdate();
                    MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                    BoundaryCutManager.Instance.BoundaryCutUpdate();
                    ButtonOff();
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //test();
                    //AdjacencyList.Instance.ListUpdate();
                    boundaryCount = 0;
                    oldPosition = Vector3.zero;
                    Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);

                    Vector3 startVertexPosition = Vector3.zero;
                    int startTriangleIndex = -1;
                    if (IntersectionManager.Instance.RayObjectIntersection(ray, ref startVertexPosition, ref startTriangleIndex))
                    {
                        BoundaryCutManager.Instance.rays.Add(ray);
                        BoundaryCutManager.Instance.intersectedPosition.Add(startVertexPosition);
                        BoundaryCutManager.Instance.startTriangleIndex = startTriangleIndex;

                        oldPosition = startVertexPosition;
                        firstPosition = oldPosition;
                        boundaryCount++;
                    }
                    else
                    {
                        ChatManager.Instance.GenerateMessage("intersect 되지 않음.");
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector3 currentPosition = Vector3.zero;
                    if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition))
                    {
                        if (boundaryCount > 3 && Vector3.Distance(currentPosition, firstPosition) < 2f * ObjManager.Instance.pivotTransform.lossyScale.z)
                        {
                            var line = lineRenderer.GetComponent<LineRenderer>();
                            line.positionCount++;
                            //line.positionCount++;

                            line.SetPosition(boundaryCount-1, oldPosition);
                            line.SetPosition(boundaryCount, firstPosition);
                            line.GetComponent<LineRenderer>().material.color = Color.blue;
                            //EditorApplication.isPaused = true;
                            ChatManager.Instance.GenerateMessage(" 작업이 진행중입니다.");
                            isLastBoundaryCut = true;
                        }
                        else if (Vector3.Distance(currentPosition, oldPosition) < 1.5f * ObjManager.Instance.pivotTransform.lossyScale.z)
                        {
                            if (oldPosition == Vector3.zero)
                                return;
                            if(lineRenderer)
                            {
                                var line = lineRenderer.GetComponent<LineRenderer>();
                                line.SetPosition(boundaryCount - 1, currentPosition);
                            }
                                
                            return;
                        }
                        else if (boundaryCount == 1)
                        {
                            BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                            BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);
                            lineRenderer = new GameObject("Boundary Line", typeof(LineRenderer));
                            lineRenderer.layer = 8;
                            var line = lineRenderer.GetComponent<LineRenderer>();
                            line.numCornerVertices = 45;
                            line.material.color = Color.black;
                            //var line = lineRenderer.GetComponent<LineRenderer>();

                            line.SetPosition(0, oldPosition);
                            line.SetPosition(boundaryCount++, currentPosition);

                            oldPosition = currentPosition;
                        }
                        else
                        {
                            if (boundaryCount == 0)
                                return;
                            var line = lineRenderer.GetComponent<LineRenderer>();
                            line.positionCount++;
                            line.SetPosition(boundaryCount++, currentPosition);

                            BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                            BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);

                            oldPosition = currentPosition;
                            //boundaryCount++;
                        }
                    }
                    else
                    {
                        if(boundaryCount==0)
                            return;
                        Destroy(lineRenderer);
                        BoundaryCutManager.Instance.BoundaryCutUpdate();
                        ChatManager.Instance.GenerateMessage(" 심장이 아닙니다.");
                        ButtonOff();
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (boundaryCount == 0)
                        return;
                    if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
                    {
                        var line = lineRenderer.GetComponent<LineRenderer>();
                        line.positionCount++;
                        line.material.color = Color.blue;
                        line.SetPosition(boundaryCount-1, oldPosition);
                        line.SetPosition(boundaryCount, firstPosition);
                        //EditorApplication.isPaused = true;
                        ChatManager.Instance.GenerateMessage(" 작업이 진행중입니다.");

                        isLastBoundaryCut = true;
                    }
                }
            }
        }
        else if(isMeasureMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                float dst = MeasureManager.Instance.MeasureDistance(vertexPosition, ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                dst = dst / ObjManager.Instance.objTransform.lossyScale.z;
                UIManager.Instance.distance.text = dst + "mm";
            }
        }
        else if(isPatchMode)
        {
            // 처음에 실행되어야함.
            if (isFirstPatch)
            {
                Debug.Log("Patch 실행");
                isFirstPatch = false;
                return;
            }
            else if(isPatchUpdate)
            {
                // 숫자에 patch index들어가는게 좋을듯. 지금 patch, incision 관련해서는 리스트화는 시켜놨음. 추후 undo등 작업 가능.
                playerObject.SetActive(true);
                PatchManager.Instance.UpdateCurve(PatchManager.Instance.newPatch.Count-1);
            }
            else if(Input.GetMouseButtonDown(0))
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
                if(vertexPosition!=Vector3.zero)
                {
                    //first position이 저장되어 있어야함.
                    if(patchCount > 3 && Vector3.Distance(firstPosition, vertexPosition) < 2.0f * ObjManager.Instance.pivotTransform.lossyScale.z)
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
                    
                    line.SetPosition(patchCount+1, vertexPosition);
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
