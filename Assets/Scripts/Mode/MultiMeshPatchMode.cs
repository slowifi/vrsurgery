using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshPatchMode : MonoBehaviour
{
    private bool once = true;
    private GameObject FirstHitObject;
    private int HitOBJIndex;

    private bool isPatchUpdate;
    private bool isLastPatch;
    private int patchCount;
    private Vector3 oldPosition;
    private Vector3 firstPosition;
    private Ray oldRay;
    private Ray firstRay;
    private LineRendererManipulate lineRenderer;
    private MultiMeshPatchManager patchManager;
    // patch manager 싹 다 손봐야됨.

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
        this.gameObject.AddComponent<MultiMeshPatchManager>();
        patchManager = GetComponent<MultiMeshPatchManager>();
    }

    void Update()
    {
        // 패치 방법부터 바꿔야됨.
        if(Input.GetMouseButtonDown(0))
        {
            if(once == true)
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

        Ray cameraRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        if (isLastPatch)
        {
            Destroy(lineRenderer.lineObject);
            patchManager.GenerateMesh();
            isPatchUpdate = true;
            isLastPatch = false;
        }
        else if (isPatchUpdate)
        {
            // 숫자에 patch index들어가는게 좋을듯. 지금 patch, incision 관련해서는 리스트화는 시켜놨음. 추후 undo등 작업 가능.
            patchManager.UpdateCurve(MultiMeshManager.Instance.PatchList.Count - 1);
        }
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
                MultiMeshAdjacencyList.Instance.ListsUpdate();
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
                //first position이 저장되어 있어야함.
                if (patchCount > 10 && Vector3.Distance(firstPosition, intersectedValues.IntersectedPosition) < 1.0f * MultiMeshManager.Instance.pivotTransform.lossyScale.z)
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

                if (Vector3.Distance(firstPosition, intersectedValues.IntersectedPosition) > 1.5f * MultiMeshManager.Instance.pivotTransform.lossyScale.z)
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
