using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatchMode : Mode
{
    private bool isFirstPatch;
    private bool isPatchUpdate;
    private int patchCount;
    private Vector3 oldPosition;
    private Vector3 firstPosition;
    private GameObject lineRenderer;
    public GameObject playerObject;
    public GameObject mainObject;

    void Awake()
    {
        playerObject = gameObject;
        isFirstPatch = true;
        isPatchUpdate = false;
        oldPosition = Vector3.zero;
        patchCount = 0;
    }
    void Update()
    {
        // 처음에 실행되어야함.
        if (isFirstPatch)
        {
            Debug.Log("Patch 실행");
            isFirstPatch = false;
            return;
        }
        else if (isPatchUpdate)
        {
            // 숫자에 patch index들어가는게 좋을듯. 지금 patch, incision 관련해서는 리스트화는 시켜놨음. 추후 undo등 작업 가능.
            playerObject.SetActive(true);
            PatchManager.Instance.UpdateCurve(PatchManager.Instance.newPatch.Count - 1);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
            if (vertexPosition != Vector3.zero)
            {
                firstPosition = vertexPosition;
                playerObject.SetActive(false);
                AdjacencyList.Instance.ListUpdate();
                PatchManager.Instance.Generate();
                PatchManager.Instance.AddVertex(vertexPosition);
                oldPosition = vertexPosition;

                Vector3 oldPos = oldPosition;
                oldPos.z += 1f;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (oldPosition == Vector3.zero)
                return;
            Destroy(lineRenderer);
            PatchManager.Instance.GenerateMesh();
            isPatchUpdate = true;
        }
        else if (Input.GetMouseButton(0))
        {
            if (oldPosition == Vector3.zero)
                return;
            Vector3 vertexPosition = MeasureManager.Instance.vertexPosition(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
            if (vertexPosition != Vector3.zero)
            {
                //first position이 저장되어 있어야함.
                if (patchCount > 8 && Vector3.Distance(firstPosition, vertexPosition) < 2.0f * ObjManager.Instance.pivotTransform.lossyScale.z)
                {
                    Destroy(lineRenderer);
                    PatchManager.Instance.GenerateMesh();
                    isPatchUpdate = true;
                    return;
                }

                PatchManager.Instance.AddVertex(vertexPosition);
                LineRenderer line;

                if (patchCount != 0)
                {
                    line = lineRenderer.GetComponent<LineRenderer>();
                    line.positionCount++;
                }
                else
                {
                    lineRenderer = new GameObject("Patch Line", typeof(LineRenderer));
                    lineRenderer.layer = 8;
                    line = lineRenderer.GetComponent<LineRenderer>();
                    line.numCornerVertices = 45;
                    line.material.color = Color.black;
                    line.SetPosition(0, oldPosition);
                }

                line.SetPosition(patchCount + 1, vertexPosition);
                patchCount++;
                oldPosition = vertexPosition;
                return;
            }
            else
            {
                if (patchCount == 0)
                    return;
                Destroy(lineRenderer);
                PatchManager.Instance.RemovePatchVariables();
                ChatManager.Instance.GenerateMessage(" 패치 라인이 심장을 벗어났습니다.");
                // 이게 또 겹쳐부렀네


                Destroy(this);
                // return true;
            }
        }
        return;
    }
}