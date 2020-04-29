using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IncisionMode : MonoBehaviour
{
    private bool firstIncision;
    private float oldExtendValue;
    private Vector3 oldPosition;
    private LineRendererManipulate lineRenderer;
    private string mode;
    private bool testbool = false;
    private IncisionManager IncisionManager;

    void Awake()
    {
        IncisionManager = new IncisionManager();
        oldExtendValue = 0;
        firstIncision = false;
        mode = "incision";
        oldPosition = Vector3.zero;
        lineRenderer = new LineRendererManipulate(transform);
    }

    void Update()
    {
        switch (mode)
        {
            case "incision":
                handleIncision();
                break;
            case "extand":
                handleExtand();
                break;
        }
    }
    private void handleIncision()
    {
        IntersectedValues intersectedValues = Intersections.GetIntersectedValues();
        bool checkInside = intersectedValues.Intersected;

        if (Input.GetMouseButtonDown(0) && checkInside)
        {
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
            firstIncision = true;

            oldPosition = intersectedValues.IntersectedPosition;

            IncisionManager.IncisionUpdate();
            AdjacencyList.Instance.ListUpdate();
            IncisionManager.SetStartVerticesDF();
        }
        else if (Input.GetMouseButton(0))
        {
            // 여기안에 지속적으로 거리계산되는 txt넣는게 좋을듯.
            if (!firstIncision)
            {
                return;
            }
            //var line = lineRenderer.GetComponent<LineRenderer>();

            if (!checkInside)
            {
                Destroy(lineRenderer.lineObject);
                ChatManager.Instance.GenerateMessage(" 심장을 벗어났습니다.");
                EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                Destroy(this);
            }
            Vector3 currentPosition = intersectedValues.IntersectedPosition;
            Vector3 curPos = currentPosition;
            Vector3 oldPos = oldPosition;
            //curPos.z += 1f;
            //oldPos.z += 1f;
            //line.material.color = Color.black;
            //line.SetPositions(new Vector3[] { oldPos, curPos });
            lineRenderer.SetFixedLineRenderer(oldPos, curPos);
        }
        else if (Input.GetMouseButtonUp(0) && firstIncision)
        {
            if (checkInside)
            {
                Vector3 currentPosition = intersectedValues.IntersectedPosition;
                if (Vector3.Distance(oldPosition, currentPosition) < 2.5f * MeshManager.Instance.pivotTransform.lossyScale.z)
                {
                    Destroy(lineRenderer.lineObject);
                    EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                    ChatManager.Instance.GenerateMessage(" incision 거리가 너무 짧습니다.");
                    IncisionManager.IncisionUpdate();
                    firstIncision = false;
                    return;
                }
            }
            Destroy(lineRenderer.lineObject);
            bool checkEdge = false;
            IncisionManager.SetEndVerticesDF();
            IncisionManager.SetDividingListDF(ref checkEdge);
            if (checkEdge)
            {
                IncisionManager.leftSide.RemoveAt(IncisionManager.currentIndex);
                IncisionManager.rightSide.RemoveAt(IncisionManager.currentIndex);
                //incisionCount--;
                IncisionManager.IncisionUpdate();
                EventManager.Instance.Events.InvokeModeManipulate("EndAll");

                Destroy(this);
                // return true;
            }

            // 위에서 잘못되면 끊어야됨.
            Debug.Log(MeshManager.Instance.mesh.vertexCount);
            IncisionManager.ExecuteDividing();
            AdjacencyList.Instance.ListUpdate();
            IncisionManager.GenerateIncisionList();
            MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
            MeshManager.Instance.SaveCurrentMesh();
            IncisionManager.currentIndex++;
            MeshManager.Instance.mesh.RecalculateNormals();
            // chatmanager 대신 popup manager에서 팝업 호출하기.
            ChatManager.Instance.GenerateMessage(" 절개하였습니다. 확장이 가능합니다.");
            mode = "extand";
            EventManager.Instance.Events.InvokeModeManipulate("EndWithoutScaling");
        }
    }
    private void handleExtand()
    {
        //추후 incision된 파트들 indexing 해서 관리를 해줘야됨 + undo를 위한 작업도 미리미리 해놓는게 좋음.
        
        if (oldExtendValue != UIManager.Instance.extendBar.value)
        {
            IncisionManager.Extending(IncisionManager.currentIndex - 1, UIManager.Instance.extendBar.value, oldExtendValue);
            oldExtendValue = UIManager.Instance.extendBar.value;
            MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
            //IncisionManager.TestGenerateCGAL();
            //testbool = true;
        }
    }
}