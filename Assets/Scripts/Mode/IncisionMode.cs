using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class IncisionMode : MonoBehaviour
{
    private bool firstIncision;
    private float oldExtendValue;
    private Vector3 oldPosition;
    private LineRendererManipulate lineRenderer;
    private string mode;
    
    private IncisionManager IncisionManager;
    private GameObject incisionDistance;
    private Canvas rectCanvas;

    private void OnDestroy()
    {
        Destroy(incisionDistance);
        Destroy(lineRenderer.lineObject);
        EventManager.Instance.Events.InvokeModeChanged("ResetButton");
    }

    void Awake()
    {
        IncisionManager = this.gameObject.AddComponent<IncisionManager>();
        oldExtendValue = 0;
        firstIncision = false;
        mode = "incision";
        oldPosition = Vector3.zero;
        lineRenderer = new LineRendererManipulate(transform);
        // 꺼져있으면 못찾는구나. 그냥 생성을 해줄까.
        Object prefab =  Resources.Load("Prefab/IncisionDistanceText");
        incisionDistance = (GameObject)Instantiate(prefab);
        GameObject newCanvas = GameObject.Find("UICanvas");
        incisionDistance.transform.SetParent(newCanvas.transform);
        incisionDistance.transform.localScale = Vector3.one;
        rectCanvas = newCanvas.GetComponent<Canvas>();
        incisionDistance.SetActive(false);
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
            incisionDistance.SetActive(true);
            oldPosition = intersectedValues.IntersectedPosition;
            IncisionManager.IncisionUpdate();
            AdjacencyList.Instance.ListUpdate();
            IncisionManager.SetStartVerticesDF();
        }
        else if (Input.GetMouseButton(0))
        {
            if (!firstIncision)
            {
                return;
            }

            if (!checkInside)
            {
                Destroy(lineRenderer.lineObject);
                ChatManager.Instance.GenerateMessage(" 심장을 벗어났습니다.");
                EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                Destroy(incisionDistance);
                Destroy(this);
            }
            Vector3 currentPosition = intersectedValues.IntersectedPosition;
            Vector3 curPos = currentPosition;
            Vector3 oldPos = oldPosition;

            Vector2 newRectPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas.transform as RectTransform, Input.mousePosition, rectCanvas.worldCamera, out newRectPos);
            incisionDistance.GetComponent<RectTransform>().localPosition = newRectPos;
            
            incisionDistance.GetComponent<Text>().text = (Vector3.Distance(oldPos, curPos) / MeshManager.Instance.pivotTransform.localScale.z).ToString("N3") + " mm";
            
            lineRenderer.SetFixedLineRenderer(oldPos, curPos);
        }
        else if (Input.GetMouseButtonUp(0) && firstIncision)
        {
            if (checkInside)
            {
                
                Vector3 currentPosition = intersectedValues.IntersectedPosition;
                if (Vector3.Distance(oldPosition, currentPosition) < 2.5f * MeshManager.Instance.pivotTransform.lossyScale.z)
                {
                    Destroy(incisionDistance);
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
                Destroy(incisionDistance);
                Destroy(this);
                return;
            }

            // 위에서 잘못되면 끊어야됨.
            Debug.Log(MeshManager.Instance.mesh.vertexCount);
            IncisionManager.ExecuteDividing();
            AdjacencyList.Instance.ListUpdate();
            IncisionManager.GenerateIncisionList();
            MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
            //MeshManager.Instance.SaveCurrentMesh();
            IncisionManager.currentIndex++;
            MeshManager.Instance.mesh.RecalculateNormals();
            // chatmanager 대신 popup manager에서 팝업 호출하기.
            ChatManager.Instance.GenerateMessage(" 절개하였습니다. 확장이 가능합니다.");
            Destroy(incisionDistance);
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
            MeshManager.Instance.mesh.RecalculateNormals();
        }
    }
}