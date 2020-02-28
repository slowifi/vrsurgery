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
