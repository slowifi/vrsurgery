﻿using UnityEngine;

public class CHD : MonoBehaviour
{
    // main
    public bool isCutMode = false;
    public bool isMeasureMode = false;
    public bool isPatchMode = false;

    public bool isIncisionMode = false;
    public bool isExtend = false;

    public bool isEraseMode = false;

    public bool isBoundaryCutMode = false;
    public int boundaryCount;
    public Vector3 firstPosition;
    public Vector3 oldPosition;


    // test
    public bool isFirstPatch = true;
    public bool isPatchUpdate = false;
    public bool isTestMode = true;

    public bool isDoubleFace = true;

    void Start()
    {
        ObjManager.Instance.ObjUpdate();
        // MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        PatchManager.Instance.Initialize();
        IncisionManager.Instance.Initialize();
        BoundaryCutManager.Instance.Initialize();
        MakeDoubleFaceMesh.Instance.Initialize();
        boundaryCount = 0;
    }

    void Update()
    {
        if(isCutMode)
        {
            if(isIncisionMode)
            {
                if (isFirstPatch)
                {
                    
                    isFirstPatch = false;
                }

                if (isExtend)
                {
                    // 여기에 손가락 움직임에 따른 value의 변화량이 측정되면 그거 가지고 받아서 여기에 넣으면 됨.
                    IncisionManager.Instance.Extending(0);
                    MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                    isExtend = false;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.SetStartVerticesDF();
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    IncisionManager.Instance.SetEndVerticesDF();
                    IncisionManager.Instance.SetDividingListDF();
                    IncisionManager.Instance.ExecuteDividing();
                    //MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
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
                if (isFirstPatch)
                {
                    AdjacencyList.Instance.ListUpdate();
                    isFirstPatch = false;
                    //boundaryCount = 0;
                }
                
                //조건짜는게 까다로움.
                if (Input.GetMouseButtonDown(0))
                {
                    boundaryCount = 0;
                    //ChatManager.Instance.GenerateMessage("첫 진입");
                    oldPosition = Vector3.zero;
                    Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                    Vector3 startVertexPosition = Vector3.zero;
                    int startTriangleIndex = -1;
                    if (IntersectionManager.Instance.RayObjectIntersection(ray, ref startVertexPosition, ref startTriangleIndex))
                    {
                        BoundaryCutManager.Instance.SetStartVertices(ray, startVertexPosition, startTriangleIndex);
                        oldPosition = startVertexPosition;
                        firstPosition = oldPosition;
                        GameObject v_test = new GameObject();
                        v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        v_test.transform.position = startVertexPosition;
                        boundaryCount++;
                    }
                    else
                    {
                        ChatManager.Instance.GenerateMessage("intersect 되지 않음.");
                    }
                }
                else if(Input.GetMouseButton(0))
                {
                    Vector3 currentPosition = Vector3.zero;
                    if(IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition))
                    {
                        if (boundaryCount > 3 && Vector3.Distance(currentPosition, firstPosition) < 4.0f)
                        {
                            AdjacencyList.Instance.ListUpdateOnlyTriangles();
                            //AdjacencyList.Instance.ListUpdate();
                            //ChatManager.Instance.GenerateMessage("마지막 처음과 가까워짐.");
                            BoundaryCutManager.Instance.ResetIndex();
                            BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                            BoundaryCutManager.Instance.SetDividingList();
                            BoundaryCutManager.Instance.ExecuteDividing();
                            isBoundaryCutMode = false;
                        }
                        else if (Vector3.Distance(currentPosition, oldPosition) < 4.0f)
                            return;
                        else if (boundaryCount == 1)
                        {
                            //ChatManager.Instance.GenerateMessage("첫 진입 후 생성");
                            //BoundaryCutManager.Instance.ResetIndex(); 

                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            v_test.transform.position = currentPosition;
                            BoundaryCutManager.Instance.SetEndVertices();
                            BoundaryCutManager.Instance.SetDividingList();
                            BoundaryCutManager.Instance.ExecuteDividing();
                            //AdjacencyList.Instance.ListUpdate();
                            AdjacencyList.Instance.ListUpdateOnlyTriangles();
                            oldPosition = currentPosition;
                            boundaryCount++;
                            isFirstPatch = true;
                        }
                        else
                        {
                            var boundaryLine = new GameObject("Boundary Line");
                            var lineRenderer = boundaryLine.AddComponent<LineRenderer>();
                            Vector3 curPos = currentPosition;
                            Vector3 oldPos = oldPosition;
                            curPos.z += 1f;
                            oldPos.z += 1f;
                            lineRenderer.material.color = Color.black;
                            lineRenderer.SetPositions(new Vector3[] { oldPos, curPos });

                            //GameObject v_test = new GameObject();
                            //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            //v_test.transform.position = currentPosition;
                            //AdjacencyList.Instance.ListUpdate();

                            //ChatManager.Instance.GenerateMessage("중간과정");
                            BoundaryCutManager.Instance.ResetIndex();
                            BoundaryCutManager.Instance.SetEndVertices();
                            BoundaryCutManager.Instance.SetDividingList();
                            BoundaryCutManager.Instance.ExecuteDividing();
                            //AdjacencyList.Instance.ListUpdate();
                            AdjacencyList.Instance.ListUpdateOnlyTriangles();
                            oldPosition = currentPosition;
                            boundaryCount++;
                        }
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
                    {
                        AdjacencyList.Instance.ListUpdate();
                        ChatManager.Instance.GenerateMessage("마지막 마우스 버튼 업");
                        BoundaryCutManager.Instance.ResetIndex();
                        BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                        BoundaryCutManager.Instance.SetDividingList();
                        BoundaryCutManager.Instance.ExecuteDividing();
                        isBoundaryCutMode = false;
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
