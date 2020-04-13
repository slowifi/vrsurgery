using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IncisionMode : Mode
{
    private bool isExtend = false;
    private float oldExtendValue = 0;
    private GameObject lineRenderer;
    public GameObject playerObject;
    public GameObject mainObject;
    private bool firstIncision = false;
    private Vector3 oldPosition = Vector3.zero;

    void Awake()
    {
        playerObject = gameObject;
        oldExtendValue = 0;
        firstIncision = false;
        isExtend = false;
        oldPosition = Vector3.zero;

    }

    void Update()
    {
        if (isExtend)
        {
            //추후 incision된 파트들 indexing 해서 관리를 해줘야됨 + undo를 위한 작업도 미리미리 해놓는게 좋음.
            if (oldExtendValue != UIManager.Instance.extendBar.value)
            {
                IncisionManager.Instance.Extending(IncisionManager.Instance.currentIndex - 1, UIManager.Instance.extendBar.value, oldExtendValue);
                oldExtendValue = UIManager.Instance.extendBar.value;
                MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
                return;
            }
            else
                return;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Incision 실행");
            playerObject.SetActive(false);
            lineRenderer = new GameObject("Incision Line", typeof(LineRenderer));
            lineRenderer.layer = 8;
            //incisionCount++;
            firstIncision = true;
            IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref oldPosition);
            IncisionManager.Instance.IncisionUpdate();
            AdjacencyList.Instance.ListUpdate();
            IncisionManager.Instance.SetStartVerticesDF();
        }
        else if (Input.GetMouseButtonUp(0) && firstIncision)
        {
            Vector3 currentPosition = Vector3.zero;
            bool checkInside = IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition);
            if (checkInside)
            {
                if (Vector3.Distance(oldPosition, currentPosition) < 2.5f * ObjManager.Instance.pivotTransform.lossyScale.z)
                {
                    Destroy(lineRenderer);
                    ChatManager.Instance.GenerateMessage(" incision 거리가 너무 짧습니다.");
                    IncisionManager.Instance.IncisionUpdate();
                    firstIncision = false;
                    return;
                }
            }
            Destroy(lineRenderer);
            bool checkEdge = false;
            IncisionManager.Instance.SetEndVerticesDF();
            IncisionManager.Instance.SetDividingListDF(ref checkEdge);
            if (checkEdge)
            {
                IncisionManager.Instance.leftSide.RemoveAt(IncisionManager.Instance.currentIndex);
                IncisionManager.Instance.rightSide.RemoveAt(IncisionManager.Instance.currentIndex);
                //incisionCount--;
                IncisionManager.Instance.IncisionUpdate();
                if (playerObject.activeSelf)
                    playerObject.SendMessage("IncisionModeOff");
                mainObject.SendMessage("ButtonOff");
                // return true;
            }

            // 위에서 잘못되면 끊어야됨.
            IncisionManager.Instance.ExecuteDividing();
            AdjacencyList.Instance.ListUpdate();
            IncisionManager.Instance.GenerateIncisionList();
            MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
            MeshManager.Instance.SaveCurrentMesh();
            IncisionManager.Instance.currentIndex++;
            MeshManager.Instance.mesh.RecalculateNormals();
            ChatManager.Instance.GenerateMessage(" 절개하였습니다. 확장이 가능합니다.");
            playerObject.SetActive(true);
            isExtend = true;
        }
        else if (Input.GetMouseButton(0))
        {
            // 이걸 수정을 좀 해야되는데
            if (!firstIncision)
            {
                if (playerObject.activeSelf)
                    playerObject.SendMessage("IncisionModeOff");
                // 여기에다가 넣으면 여러개가 찍히는게 좀 에반데.
                return;
            }
            var line = lineRenderer.GetComponent<LineRenderer>();
            Vector3 currentPosition = Vector3.zero;
            bool checkInside = IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition);
            if (!checkInside)
            {
                Destroy(lineRenderer);
                ChatManager.Instance.GenerateMessage(" 심장을 벗어났습니다.");
                //incisionCount--;

                playerObject.SendMessage("IncisionModeOff");
                mainObject.SendMessage("ButtonOff");
                // return true;
            }
            Vector3 curPos = currentPosition;
            Vector3 oldPos = oldPosition;
            curPos.z += 1f;
            oldPos.z += 1f;
            line.material.color = Color.black;
            line.SetPositions(new Vector3[] { oldPos, curPos });
        }
    }
}