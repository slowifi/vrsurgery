using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshMeasureMode : MonoBehaviour
{
    MultiMeshMeasureManager MeasureManager;
    LineRendererManipulate lineRenderer;
    private bool once = true;
    private GameObject FirstHitObject;
    private int HitOBJIndex;

    void Awake()
    {
        MeasureManager = new MultiMeshMeasureManager();
        lineRenderer = new LineRendererManipulate(transform);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (once == true)
            {
                Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit FirstHit;
                if (Physics.Raycast(ray, out FirstHit, 1000f))
                    FirstHitObject = FirstHit.collider.gameObject;
                else
                {
                    Debug.Log("빈공간입니다.");
                    return;
                }
                for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                {
                    if (FirstHitObject.name == GameObject.Find("PartialModel").transform.GetChild(i).name + "_Outer")
                        HitOBJIndex = i;
                }
                once = false;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            Vector3 vertexPosition = MeasureManager.vertexPosition(cameraRay, HitOBJIndex);
            float dst = MeasureManager.MeasureDistance(vertexPosition, cameraRay);
            dst = dst / MultiMeshManager.Instance.objsTransform[HitOBJIndex].lossyScale.z;
            UIManager.Instance.Distance.text = dst + "mm";
        }
    }
    void OnDestroy()
    {
        Destroy(MeasureManager);
        //EventManager.Instance.Events.InvokeModeChanged("ResetButton");
        Destroy(GameObject.Find("MeasureLine"));
    }
}
