using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeasureMode : Mode
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
            float dst = MeasureManager.Instance.MeasureDistance(vertexPosition, ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
            dst = dst / ObjManager.Instance.objTransform.lossyScale.z;
            UIManager.Instance.distance.text = dst + "mm";
        }
    }
}