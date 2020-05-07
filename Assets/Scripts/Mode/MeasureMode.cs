using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeasureMode : MonoBehaviour
{
    MeasureManager MeasureManager;

    void Awake()
    {
        MeasureManager = new MeasureManager();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            Vector3 vertexPosition = MeasureManager.vertexPosition(cameraRay);
            float dst = MeasureManager.MeasureDistance(vertexPosition, cameraRay);
            dst = dst / MeshManager.Instance.objTransform.lossyScale.z;
            UIManager.Instance.distance.text = dst + "mm";
        }
    }
    void OnDestroy()
    {
        Destroy(MeasureManager);
        //EventManager.Instance.Events.InvokeModeChanged("ResetButton");
        Destroy(GameObject.Find("MeasureLine"));
    }
}