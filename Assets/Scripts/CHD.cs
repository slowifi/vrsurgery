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
    public Vector3 firstMousePosition;
    public Vector3 oldMousePosition;


    // test
    public bool isFirstPatch = true;
    public bool isPatchUpdate = false;


    // double face test
    public bool isDoubleFace = true;

    void Start()
    {
        ObjManager.Instance.ObjUpdate();
        // MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        PatchManager.Instance.Initialize();
        IncisionManager.Instance.Initialize();
        BoundaryCutManager.Instance.Initialize();
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
                    AdjacencyList.Instance.ListUpdate();
                    isFirstPatch = false;
                }

                if (isExtend)
                {
                    IncisionManager.Instance.ExecuteDividing();
                    AdjacencyList.Instance.ListUpdate();
                    IncisionManager.Instance.Extending();
                    isExtend = false;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    IncisionManager.Instance.SetStartVertices();
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    IncisionManager.Instance.SetEndVertices();
                    IncisionManager.Instance.SetDividingList();
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
                    ChatManager.Instance.GenerateMessage("첫 진입");
                    BoundaryCutManager.Instance.SetStartVertices();
                    oldMousePosition = Input.mousePosition;
                    firstMousePosition = oldMousePosition;
                    boundaryCount++;
                }
                else if(Input.GetMouseButton(0))
                {
                    if (boundaryCount > 3 && Vector3.Distance(Input.mousePosition, firstMousePosition) < 3.0f)
                    {
                        ChatManager.Instance.GenerateMessage("마지막 처음과 가까워짐.");
                        BoundaryCutManager.Instance.ResetIndex();
                        BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                        BoundaryCutManager.Instance.SetDividingList();
                        BoundaryCutManager.Instance.ExecuteDividing();
                        isBoundaryCutMode = false;
                    }
                    else if (Vector3.Distance(Input.mousePosition, oldMousePosition) < 50.0f)
                        return;
                    else if (boundaryCount == 1)
                    {
                        ChatManager.Instance.GenerateMessage("첫 진입 후 생성");
                        // BoundaryCutManager.Instance.ResetIndex();
                        BoundaryCutManager.Instance.SetEndVertices();
                        BoundaryCutManager.Instance.SetDividingList();
                        BoundaryCutManager.Instance.ExecuteDividing();
                        AdjacencyList.Instance.ListUpdate();
                        oldMousePosition = Input.mousePosition;
                        boundaryCount++;
                        isFirstPatch = true;
                    }
                    else
                    {
                        ChatManager.Instance.GenerateMessage("중간과정");
                        BoundaryCutManager.Instance.ResetIndex();
                        BoundaryCutManager.Instance.SetEndVertices();
                        BoundaryCutManager.Instance.SetDividingList();
                        BoundaryCutManager.Instance.ExecuteDividing();
                        AdjacencyList.Instance.ListUpdate();
                        oldMousePosition = Input.mousePosition;
                        boundaryCount++;
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    ChatManager.Instance.GenerateMessage("마지막 마우스 버튼 업");
                    BoundaryCutManager.Instance.ResetIndex();
                    BoundaryCutManager.Instance.SetEndVtxToStartVtx();
                    BoundaryCutManager.Instance.SetDividingList();
                    BoundaryCutManager.Instance.ExecuteDividing();
                    isBoundaryCutMode = false;
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
        else if(isDoubleFace)
        {
            isDoubleFace = false;
            MakeDoubleFaceMesh.Instance.MakeDoubleFace();
        }
    }


}
