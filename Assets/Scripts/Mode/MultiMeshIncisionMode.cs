using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMeshIncisionMode : MonoBehaviour
{
    private GameObject FirstHitObject;
    private int HitOBJIndex;
    private bool once = true;

    private bool firstIncision;
    private float oldExtendValue;
    private Vector3 oldPosition;
    private LineRendererManipulate lineRenderer;
    private string mode;

    private MultiMeshIncisionManager IncisionManager;
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
        IncisionManager = this.gameObject.AddComponent<MultiMeshIncisionManager>();
        oldExtendValue = 0;
        firstIncision = false;
        mode = "incision";
        oldPosition = Vector3.zero;
        lineRenderer = new LineRendererManipulate(transform);
        // 꺼져있으면 못찾는구나. 그냥 생성을 해줄까.
        Object prefab = Resources.Load("Prefab/IncisionDistanceText");
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

        IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);
        bool checkInside = intersectedValues.Intersected;

        if (Input.GetMouseButtonDown(0) && checkInside)
        {
            Debug.Log("OK");
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
            firstIncision = true;
            incisionDistance.SetActive(true);
            oldPosition = intersectedValues.IntersectedPosition;
            IncisionManager.IncisionUpdate();
            MultiMeshAdjacencyList.Instance.ListsUpdate();
            IncisionManager.SetStartVerticesDF(HitOBJIndex);
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

            incisionDistance.GetComponent<Text>().text = (Vector3.Distance(oldPos, curPos) / MultiMeshManager.Instance.pivotTransform.localScale.z).ToString("N3") + " mm";

            lineRenderer.SetFixedLineRenderer(oldPos, curPos);
        }
        else if (Input.GetMouseButtonUp(0) && firstIncision)
        {
            if (checkInside)
            {
                Vector3 currentPosition = intersectedValues.IntersectedPosition;
                if (Vector3.Distance(oldPosition, currentPosition) < 2.5f * MultiMeshManager.Instance.pivotTransform.lossyScale.z)
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
            IncisionManager.SetEndVerticesDF(HitOBJIndex);
            IncisionManager.SetDividingListDF(ref checkEdge, HitOBJIndex);
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
            Debug.Log(MultiMeshManager.Instance.meshes[HitOBJIndex].vertexCount);
            IncisionManager.ExecuteDividing(HitOBJIndex);
            MultiMeshAdjacencyList.Instance.ListsUpdate();
            IncisionManager.GenerateIncisionList(HitOBJIndex);
            MultiMeshMakeDoubleFace.Instance.MeshUpdateInnerFaceVertices(HitOBJIndex);
            //MeshManager.Instance.SaveCurrentMesh();
            IncisionManager.currentIndex++;
            MultiMeshManager.Instance.meshes[HitOBJIndex].RecalculateNormals();
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
            IncisionManager.Extending(IncisionManager.currentIndex - 1, UIManager.Instance.extendBar.value, oldExtendValue, HitOBJIndex);
            oldExtendValue = UIManager.Instance.extendBar.value;
            MultiMeshMakeDoubleFace.Instance.MeshUpdateInnerFaceVertices(HitOBJIndex);
            //IncisionManager.TestGenerateCGAL();
            //testbool = true;
            MultiMeshManager.Instance.meshes[HitOBJIndex].RecalculateNormals();
        }
    }
}
