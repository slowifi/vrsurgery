using UnityEngine;

public class ObjManager : Singleton<ObjManager>
{
    public Camera cam;
    public Transform objTransform;
    public GameObject startMeasurePoint;
    public GameObject endMeasurePoint;

    public void Initializing()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        objTransform = GameObject.Find("heart_2").transform;
    }
}
