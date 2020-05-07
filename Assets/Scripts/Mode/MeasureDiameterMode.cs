using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeasureDiameterMode : MonoBehaviour
{
    private bool isPatchUpdate;
    private bool isLastPatch;
    private int patchCount;
    private Vector3 oldPosition;
    private Vector3 firstPosition;
    private Ray oldRay;
    private Ray firstRay;
    private LineRendererManipulate lineRenderer;
    private PatchManager patchManager;

    private void FirstSet()
    {
        lineRenderer = new LineRendererManipulate(transform);
        isPatchUpdate = false;
        isLastPatch = false;
        oldPosition = Vector3.zero;
        patchCount = 0;
    }

    void Awake()
    {
        FirstSet();
        this.gameObject.AddComponent<PatchManager>();
        patchManager = GetComponent<PatchManager>();
    }
    void Update()
    {
        // 패치 방법부터 바꿔야됨.
        Ray cameraRay = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        if (isLastPatch)
        {
            Destroy(lineRenderer.lineObject);
            patchManager.GenerateMeshForMeasure();
            isLastPatch = false;
            Destroy(this);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
            if (intersectedValues.Intersected)
            {
                firstRay = ray;
                oldRay = ray;
                firstPosition = intersectedValues.IntersectedPosition;
                oldPosition = intersectedValues.IntersectedPosition;
                EventManager.Instance.Events.InvokeModeManipulate("StopAll");
                AdjacencyList.Instance.ListUpdate();
                patchManager.Generate();
                patchManager.AddVertex(intersectedValues.IntersectedPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (oldPosition == Vector3.zero)
                return;
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100, firstRay.origin + firstRay.direction * 100);
            isLastPatch = true;
        }
        else if (Input.GetMouseButton(0))
        {
            if (oldPosition == Vector3.zero)
                return;
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.GetIntersectedValues();

            if (intersectedValues.Intersected)
            {
                //first position이 저장되어 있어야함.
                if (patchCount > 10 && Vector3.Distance(firstPosition, intersectedValues.IntersectedPosition) < 1.0f * MeshManager.Instance.pivotTransform.lossyScale.z)
                {
                    EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                    isLastPatch = true;
                    return;
                }

                if (patchCount != 0)
                {
                    lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100, ray.origin + ray.direction * 100);
                }
                else
                {
                    lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100, ray.origin + ray.direction * 100);
                }

                if (Vector3.Distance(firstPosition, intersectedValues.IntersectedPosition) > 1.5f * MeshManager.Instance.pivotTransform.lossyScale.z)
                {
                    patchManager.AddVertex(intersectedValues.IntersectedPosition);
                    
                    patchCount++;
                    oldPosition = intersectedValues.IntersectedPosition;
                    oldRay = ray;
                    return;
                }
                
            }
            else
            {
                if (patchCount == 0)
                    return;
                Destroy(lineRenderer.lineObject);
                patchManager.RemovePatchVariables();
                ChatManager.Instance.GenerateMessage(" 패치 라인이 심장을 벗어났습니다.");
                FirstSet();
            }
        }
        return;
    }
}