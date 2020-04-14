using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeasureMode : Mode
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
            Ray cameraRay = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            Vector3 vertexPosition = MeasureManager.vertexPosition(cameraRay);
            float dst = MeasureManager.MeasureDistance(vertexPosition, cameraRay);
            dst = dst / ObjManager.Instance.objTransform.lossyScale.z;
            UIManager.Instance.distance.text = dst + "mm";
        }
    }
    void OnDestroy()
    {
        Destroy(MeasureManager);
        Destroy(GameObject.Find("MeasureLine"));
    }
}