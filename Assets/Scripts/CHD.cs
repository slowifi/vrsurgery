using UnityEngine;
using System.Collections.Generic;

public class CHD : MonoBehaviour
{
    // main
    public bool isCutMode = false;
    public bool isMeasureMode = false;
    public bool isPatchMode = false;

    public bool isIncisionMode = false;
    public bool isExtend = false;
    private float oldExtendValue;
    private int incisionCount;

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
    }

    public void PatchMode()
    {
        isCutMode = false;
        isBoundaryCutMode = false;
        isMeasureMode = false;
        isPatchMode = true;
        isIncisionMode = false;
        isExtend = false;
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
        incisionCount++;
        oldExtendValue = 0;
        UIManager.Instance.extendBar.value = 0;
    }

    public void Exit()
    {
        isCutMode = false;
        isBoundaryCutMode = false;
        isMeasureMode = false;
        isPatchMode = false;
        isIncisionMode = false;
        isExtend = false;
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
                    }
                    else
                        return;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    IncisionManager.Instance.IncisionUpdate();
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.SetStartVerticesDF();
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    IncisionManager.Instance.SetEndVerticesDF();
                    IncisionManager.Instance.SetDividingListDF();
                    IncisionManager.Instance.ExecuteDividing();
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.GenerateIncisionList();
                    MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                    IncisionManager.Instance.currentIndex++;
                    isExtend = true;
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
                        isLastBoundaryCut = false;
                        isBoundaryCutMode = false;
                        isFirstPatch = true;
                    }
                    return;
                }

                
                if (Input.GetMouseButtonDown(0))
                {
                    AdjacencyList.Instance.ListUpdate();
                    boundaryCount = 0;
                    //ChatManager.Instance.GenerateMessage("첫 진입");
                    oldPosition = Vector3.zero;
                    Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);

                    Vector3 startVertexPosition = Vector3.zero;
                    int startTriangleIndex = -1;
                    if (IntersectionManager.Instance.RayObjectIntersection(ray, ref startVertexPosition, ref startTriangleIndex))
                    {
                        BoundaryCutManager.Instance.rays.Add(ray);
                        BoundaryCutManager.Instance.intersectedPosition.Add(startVertexPosition);
                        BoundaryCutManager.Instance.startTriangleIndex = startTriangleIndex;

                        //// 이부분이 추후 작업에 들어가야됨.
                        //BoundaryCutManager.Instance.SetStartVertices(ray, startVertexPosition, startTriangleIndex);

                        //GameObject v_test = new GameObject();
                        //v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //v_test.transform.position = startVertexPosition;

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
                            //추후작업
                            //BoundaryCutManager.Instance.ResetIndex();
                            //BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                            //BoundaryCutManager.Instance.SetDividingList();
                            //BoundaryCutManager.Instance.ExecuteDividing();
                            lineRenderers.Add(new GameObject("Boundary Line", typeof(LineRenderer)));
                            var lineRenderer = lineRenderers[boundaryCount - 1].GetComponent<LineRenderer>();
                            Vector3 curPos = firstPosition;
                            Vector3 oldPos = oldPosition;
                            curPos.z += 1f;
                            oldPos.z += 1f;
                            lineRenderer.material.color = Color.black;
                            lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });

                            BoundaryCutManager.Instance.PostProcess();
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

                        BoundaryCutManager.Instance.PostProcess();
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
                AdjacencyList.Instance.ListUpdate();
                PatchManager.Instance.Generate();
                return;
            }
            else if(isPatchUpdate)
            {
                // isPatchUpdate = false;
                PatchManager.Instance.UpdateCurve(0);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                PatchManager.Instance.GenerateMesh();
                isPatchUpdate = true;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                PatchManager.Instance.AddVertex(vertexPosition);
            }
        }
        else if(isTestMode)
        {

        }
    }


}
