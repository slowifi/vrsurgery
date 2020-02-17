using UnityEngine;

public class CHD : MonoBehaviour
{
    // 행동 제약?.
    // main
    public bool isCutMode = false;
    public bool isMeasureMode = false;
    public bool isPatchMode = false;

    public bool isIncisionMode = false;
    public bool isEraseMode = false;
    public bool isBoundaryCutMode = false;

    void Start()
    {
        AdjacencyList.Instance.Initializing();
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
                // m_distance.text = dst + "mm";
            }
        }
        else if(isPatchMode)
        {
            if(Input.GetMouseButtonUp(0))
            {

            }
            else if (Input.GetMouseButton(0))
            {
                // Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));

                
            }
        }
    }
}
