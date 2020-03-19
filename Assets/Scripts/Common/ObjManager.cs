using UnityEngine;

public class ObjManager : Singleton<ObjManager>
{
    public Camera cam;
    public Transform objTransform;
    public Transform pivotTransform;
    public GameObject startMeasurePoint;
    public GameObject endMeasurePoint;

    public void ObjUpdate()
    {
        objTransform = objTransform.GetComponent<Transform>();
        //objTransform = GameObject.Find("PartialModel").transform;
        pivotTransform = pivotTransform.GetComponent<Transform>();
    }
}
