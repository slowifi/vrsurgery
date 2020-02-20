using UnityEngine;

public class ObjManager : Singleton<ObjManager>
{
    public Camera cam;
    public Transform objTransform;
    public GameObject startMeasurePoint;
    public GameObject endMeasurePoint;

    public void ObjUpdate()
    {
        objTransform = objTransform.GetComponent<Transform>();
    }
}
