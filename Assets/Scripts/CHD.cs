using UnityEngine;

public class CHD : MonoBehaviour
{
    // 행동 제약?.
    // main
    public bool isCutMode = false;
    public bool isMeasureMode = false;
    public bool isPatchMode = true;

    public bool isIncisionMode = false;
    public bool isEraseMode = false;
    public bool isBoundaryCutMode = false;
    
    // test
    public bool isFirstPatch = true;
    public bool isPatchUpdate = false;

    void Start()
    {
        MeshManager.Instance.Initialize();
        AdjacencyList.Instance.Initialize();
        PatchManager.Instance.Initialize();
    }


    void Update()
    {
        if(isCutMode)
        {
            if(isIncisionMode)
            {

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
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
                float dst = MeasureManager.Instance.MeasureDistance(vertexPosition);
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
    }


}
