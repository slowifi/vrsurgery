using UnityEngine;
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



    private int patchCount;

    public bool isEraseMode = false;

    public bool isBoundaryCutMode = false;
    public int boundaryCount;
    public Vector3 firstPosition;
    public Vector3 oldPosition;

    // line renderer
    public List<GameObject> lineRenderers;

    // test
    public bool isFirstPatch = true;
    public bool isPatchUpdate = false;
    public bool isTestMode = true;
    public bool isLastBoundaryCut = false;


    public void CutMode()
    {
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
        MeshManager.Instance.LoadOldMesh();
        ButtonPress.Instance.ResetButton();
        Exit();
    }


    public void Exit()
    {
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
            MeshRenderer ren = GameObject.Find("Patch" + (PatchManager.Instance.newPatch.Count - 1)).GetComponent<MeshRenderer>();
            ren.material.color = new Color32(115, 0, 0, 255);
        }
    }


    void Start()
    {
        ObjManager.Instance.ObjUpdate();
        MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        PatchManager.Instance.Initialize();
        IncisionManager.Instance.Initialize();
        BoundaryCutManager.Instance.Initialize();
        MakeDoubleFaceMesh.Instance.Initialize();
        lineRenderers = new List<GameObject>();
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
                        IncisionManager.Instance.Extending(incisionCount, UIManager.Instance.extendBar.value, oldExtendValue);
                        oldExtendValue = UIManager.Instance.extendBar.value;
                        MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                        return;
                    }
                    else
                        return;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    lineRenderers.Add(new GameObject("Incision Line", typeof(LineRenderer)));
                    incisionCount++;
                    firstIncision = true;
                    IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref oldPosition);
                    IncisionManager.Instance.IncisionUpdate();
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.SetStartVerticesDF();
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    for (int i = 0; i < lineRenderers.Count; i++)
                        Destroy(lineRenderers[i]);
                    lineRenderers.Clear();
                    bool checkEdge = false;
                    IncisionManager.Instance.SetEndVerticesDF();
                    IncisionManager.Instance.SetDividingListDF(ref checkEdge);
                    if (checkEdge)
                    {
                        IncisionManager.Instance.leftSide.RemoveAt(IncisionManager.Instance.currentIndex);
                        IncisionManager.Instance.rightSide.RemoveAt(IncisionManager.Instance.currentIndex);
                        incisionCount--;
                        IncisionManager.Instance.IncisionUpdate();
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
                    isExtend = true;
                }
                else if(Input.GetMouseButton(0))
                {
                    // 이걸 수정을 좀 해야되는데
                    if (!firstIncision)
                    {
                        
                        return;
                    }
                    var lineRenderer = lineRenderers[0].GetComponent<LineRenderer>();
                    Vector3 currentPosition = Vector3.zero;
                    bool checkInside = IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition);
                    if(!checkInside)
                    {
                        for (int i = 0; i < lineRenderers.Count; i++)
                            Destroy(lineRenderers[i]);
                        lineRenderers.Clear();
                        ChatManager.Instance.GenerateMessage(" 심장을 벗어났습니다.");
                        incisionCount--;
                        ButtonOff();
                        return;
                    }
                    Vector3 curPos = currentPosition;
                    Vector3 oldPos = oldPosition;
                    curPos.z += 1f;
                    oldPos.z += 1f;
                    lineRenderer.material.color = Color.black;
                    lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });
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
                    MeshManager.Instance.SaveCurrentMesh();
                    AdjacencyList.Instance.ListUpdate();
                    isFirstPatch = false;
                    boundaryCount = 0;
                    return;
                }

                if (isLastBoundaryCut)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        for (int i = 0; i < lineRenderers.Count; i++)
                            Destroy(lineRenderers[i]);
                        lineRenderers.Clear();
                        AdjacencyList.Instance.ListUpdate();

                        int vertexIndex = IntersectionManager.Instance.GetIntersectedValues(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                        if (vertexIndex == -1)
                            return;
                        else
                            BFS.Instance.BFS_Boundary(vertexIndex, BoundaryCutManager.Instance.removeBoundaryVertices);

                        MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                        MeshManager.Instance.SaveCurrentMesh();
                        //MeshManager.Instance.LoadOldMesh();
                        BoundaryCutManager.Instance.BoundaryCutUpdate();
                        ButtonOff();
                    }
                    return;
                }

                
                if (Input.GetMouseButtonDown(0))
                {
                    AdjacencyList.Instance.ListUpdate();
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
                        if (boundaryCount > 3 && Vector3.Distance(currentPosition, firstPosition) < 3f*ObjManager.Instance.pivotTransform.lossyScale.z)
                        {
                            lineRenderers.Add(new GameObject("Boundary Line", typeof(LineRenderer)));
                            var lineRenderer = lineRenderers[boundaryCount - 1].GetComponent<LineRenderer>();
                            Vector3 curPos = firstPosition;
                            Vector3 oldPos = oldPosition;
                            curPos.z += 1f;
                            oldPos.z += 1f;
                            lineRenderer.material.color = Color.black;
                            lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });
                            bool checkError = true;
                            checkError = BoundaryCutManager.Instance.PostProcess();
                            if(!checkError)
                            {
                                for (int i = 0; i < lineRenderers.Count; i++)
                                    Destroy(lineRenderers[i]);
                                lineRenderers.Clear();
                                ButtonOff();
                                return;
                            }
                            isLastBoundaryCut = true;
                        }
                        else if (Vector3.Distance(currentPosition, oldPosition) < 2.5f * ObjManager.Instance.pivotTransform.lossyScale.z)
                            return;
                        else if (boundaryCount == 1)
                        {
                            BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                            BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);
                            lineRenderers.Add(new GameObject("Boundary Line", typeof(LineRenderer)));
                            var lineRenderer = lineRenderers[0].GetComponent<LineRenderer>();
                            Vector3 curPos = currentPosition;
                            Vector3 oldPos = oldPosition;
                            curPos.z += 1f;
                            oldPos.z += 1f;
                            lineRenderer.material.color = Color.black;
                            lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });

                            oldPosition = currentPosition;
                            boundaryCount++;
                        }
                        else
                        {
                            // 생성했던거를 그려지면 바로 지워지는 식으로 하면 되려나?
                            lineRenderers.Add(new GameObject("Boundary Line", typeof(LineRenderer)));
                            var lineRenderer = lineRenderers[boundaryCount - 1].GetComponent<LineRenderer>();
                            Vector3 curPos = currentPosition;
                            Vector3 oldPos = oldPosition;
                            curPos.z += 1f;
                            oldPos.z += 1f;
                            lineRenderer.material.color = Color.black;
                            lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });

                            BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                            BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);

                            oldPosition = currentPosition;
                            boundaryCount++;
                        }
                    }
                    else
                    {
                        if(boundaryCount==0)
                        {
                            return;
                        }
                        for (int i = 0; i < lineRenderers.Count; i++)
                            Destroy(lineRenderers[i]);
                        lineRenderers.Clear();
                        BoundaryCutManager.Instance.BoundaryCutUpdate();
                        ChatManager.Instance.GenerateMessage(" 심장이 아닙니다.");
                        ButtonOff();
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
                    {
                        lineRenderers.Add(new GameObject("Boundary Line", typeof(LineRenderer)));
                        var lineRenderer = lineRenderers[boundaryCount - 1].GetComponent<LineRenderer>();
                        Vector3 curPos = firstPosition;
                        Vector3 oldPos = oldPosition;
                        curPos.z += 1f;
                        oldPos.z += 1f;
                        lineRenderer.material.color = Color.black;
                        lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });
                        bool checkError = true;
                        checkError = BoundaryCutManager.Instance.PostProcess();
                        if (!checkError)
                        {
                            for (int i = 0; i < lineRenderers.Count; i++)
                                Destroy(lineRenderers[i]);
                            lineRenderers.Clear();
                            ButtonOff();
                            return;
                        }
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
                isFirstPatch = false;
                return;
            }
            else if(isPatchUpdate)
            {
                // 숫자에 patch index들어가는게 좋을듯. 지금 patch, incision 관련해서는 리스트화는 시켜놨음. 추후 undo등 작업 가능.
                PatchManager.Instance.UpdateCurve(PatchManager.Instance.newPatch.Count-1);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                for (int i = 0; i < lineRenderers.Count; i++)
                    Destroy(lineRenderers[i]);
                lineRenderers.Clear();
                PatchManager.Instance.GenerateMesh();
                isPatchUpdate = true;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                if(vertexPosition!=Vector3.zero)
                {
                    if(oldPosition!=Vector3.zero)
                    {
                        PatchManager.Instance.AddVertex(vertexPosition);

                        lineRenderers.Add(new GameObject("Patch Line", typeof(LineRenderer)));
                        var lineRenderer = lineRenderers[patchCount].GetComponent<LineRenderer>();
                        Vector3 curPos = vertexPosition;
                        Vector3 oldPos = oldPosition;
                        curPos.z += 1f;
                        oldPos.z += 1f;
                        lineRenderer.material.color = Color.black;
                        lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });
                        patchCount++;
                    }
                    else
                    {
                        AdjacencyList.Instance.ListUpdate();
                        PatchManager.Instance.Generate();
                        PatchManager.Instance.AddVertex(vertexPosition);
                    }
                    oldPosition = vertexPosition;
                    return;
                }
                else
                {
                    if (patchCount == 0)
                        return;
                    for (int i = 0; i < lineRenderers.Count; i++)
                        Destroy(lineRenderers[i]);
                    lineRenderers.Clear();
                    PatchManager.Instance.RemovePatchVariables();
                    ChatManager.Instance.GenerateMessage(" 패치 라인이 심장을 벗어났습니다.");
                    // 이게 또 겹쳐부렀네
                    ButtonOff();
                }
            }
        }
        
    }


}
