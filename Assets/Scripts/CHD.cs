using UnityEngine;

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
    public bool isLastBoundaryCut = false;

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
                // 무한 루프 관련해서 조정이 필요함.

                if (isFirstPatch)
                {
                    AdjacencyList.Instance.ListUpdate();
                    isFirstPatch = false;
                    boundaryCount = 0;
                }

                if(isLastBoundaryCut)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        AdjacencyList.Instance.ListUpdate();
                        int vertexIndex = IntersectionManager.Instance.GetIntersectedValues(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                        if (vertexIndex == -1)
                            return;
                        else
                            BFS.Instance.BFS_Boundary(vertexIndex, BoundaryCutManager.Instance.removeBoundaryVertices);
                        MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                        isLastBoundaryCut = false;
                        isBoundaryCutMode = false;
                        return;
                    }
                }

                // 일단 모든 정보들 다 받아놓고, 그 다음에 하나씩 진행하도록 대신 다음 triangle index를 찾는문제에서 그때그때 검색을 해줘야함. 그 포지션에 무슨 트라이앵글이 존재하는지에 대해 ray의 벡터도 저장을 해서 다시 한번 쏴주는게 답일듯. 생성된 네개의 트라이앵글 중에 하나가 그거니깐















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
                        BoundaryCutManager.Instance.rays.Add(ray);
                        //BoundaryCutManager.Instance.intersectedPosition.Add(startVertexPosition);
                        BoundaryCutManager.Instance.intersectedPosition.Add(startVertexPosition);
                        BoundaryCutManager.Instance.startTriangleIndex = startTriangleIndex;
                        
                        //// 이부분이 추후 작업에 들어가야됨.
                        //BoundaryCutManager.Instance.SetStartVertices(ray, startVertexPosition, startTriangleIndex);
                        //firstPosition = oldPosition;

                        GameObject v_test = new GameObject();
                        v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        v_test.transform.position = startVertexPosition;

                        oldPosition = startVertexPosition;
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
                        if (boundaryCount > 3 && Vector3.Distance(currentPosition, firstPosition) < 2.5f)
                        {

                            //추후작업
                            //BoundaryCutManager.Instance.ResetIndex();
                            //BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                            //BoundaryCutManager.Instance.SetDividingList();
                            //BoundaryCutManager.Instance.ExecuteDividing();
                            BoundaryCutManager.Instance.PostProcess();
                            isLastBoundaryCut = true;
                        }
                        else if (Vector3.Distance(currentPosition, oldPosition) < 2.5f)
                            return;
                        else if (boundaryCount == 1)
                        {
                            BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                            BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);

                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            v_test.transform.position = currentPosition;

                            ////추후 작업
                            //BoundaryCutManager.Instance.SetEndVertices();
                            //BoundaryCutManager.Instance.SetDividingList();
                            //BoundaryCutManager.Instance.ExecuteDividing();
                            //AdjacencyList.Instance.ListUpdate();

                            oldPosition = currentPosition;
                            boundaryCount++;
                            //isFirstPatch = true;
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

                            BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                            BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);

                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            v_test.transform.position = currentPosition;
                            //AdjacencyList.Instance.ListUpdate();

                            ////추후 작업
                            //BoundaryCutManager.Instance.ResetIndex();
                            //BoundaryCutManager.Instance.SetEndVertices();
                            //BoundaryCutManager.Instance.SetDividingList();
                            //BoundaryCutManager.Instance.ExecuteDividing();
                            //AdjacencyList.Instance.ListUpdate();

                            oldPosition = currentPosition;
                            boundaryCount++;
                        }
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
                    {
                        ////추후 작업
                        //AdjacencyList.Instance.ListUpdate();
                        //ChatManager.Instance.GenerateMessage("마지막 마우스 버튼 업");
                        //BoundaryCutManager.Instance.ResetIndex();
                        //BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                        //BoundaryCutManager.Instance.SetDividingList();
                        //BoundaryCutManager.Instance.ExecuteDividing();
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
