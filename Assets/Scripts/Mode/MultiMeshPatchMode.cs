using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshPatchMode : MonoBehaviour
{
    private LineRendererManipulate lineRenderer;
    private MultiMeshPatchManager patchManager;

    private Vector3 oldPosition;
    private Vector3 firstPosition;

    private Ray oldRay;
    private Ray firstRay;

    private bool isPatchUpdate;
    private bool isLastPatch;
    private bool once = true;

    private GameObject FirstHitObject;

    private int HitOBJIndex;    
    private int patchCount;    
    
    void Awake()
    {
        FirstSet();
        this.gameObject.AddComponent<MultiMeshPatchManager>();
        patchManager = GetComponent<MultiMeshPatchManager>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(once == true)
            {
                Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit FirstHit;

                if (Physics.Raycast(ray, out FirstHit, 1000f))
                    FirstHitObject = FirstHit.collider.gameObject;
                else
                    return;

                for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                    if (FirstHitObject.name == GameObject.Find("PartialModel").transform.GetChild(i).name)
                        HitOBJIndex = i;

                once = false;
            }
        }

        Ray cameraRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);

        if (isLastPatch)
        {
            Destroy(lineRenderer.lineObject);
            patchManager.GenerateMesh();
            isPatchUpdate = true;
            isLastPatch = false;
            MultiMeshManager.Instance.PatchOk = true;
        }
        else if (isPatchUpdate)
            patchManager.UpdateCurve(MultiMeshManager.Instance.PatchList.Count - 1);
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);

            if (intersectedValues.Intersected)
            {
                firstRay = ray;
                oldRay = ray;
                firstPosition = intersectedValues.IntersectedPosition;
                oldPosition = intersectedValues.IntersectedPosition;

                EventManager.Instance.Events.InvokeModeManipulate("StopAll");
                MultiMeshAdjacencyList.Instance.Initialize();

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

            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);

            if (intersectedValues.Intersected)
            {
                if (patchCount > 10 && Vector3.Distance(firstPosition, intersectedValues.IntersectedPosition) < 1.0f * MultiMeshManager.Instance.PivotTransform.lossyScale.z)
                {
                    EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                    isLastPatch = true;
                    return;
                }

                if (patchCount != 0)
                    lineRenderer.SetLineRenderer(oldRay.origin + oldRay.direction * 100, ray.origin + ray.direction * 100);
                else
                    lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100, ray.origin + ray.direction * 100);

                if (Vector3.Distance(firstPosition, intersectedValues.IntersectedPosition) > 1.5f * MultiMeshManager.Instance.PivotTransform.lossyScale.z)
                {
                    patchManager.AddVertex(intersectedValues.IntersectedPosition);
                    oldPosition = intersectedValues.IntersectedPosition;
                    patchCount++;
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
    private void FirstSet()
    {
        lineRenderer = new LineRendererManipulate(transform);
        isPatchUpdate = false;
        isLastPatch = false;
        oldPosition = Vector3.zero;
        patchCount = 0;
    }
}
