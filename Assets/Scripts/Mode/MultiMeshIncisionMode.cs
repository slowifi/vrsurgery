using UnityEngine;
using UnityEngine.UI;

public class MultiMeshIncisionMode : MonoBehaviour
{
    private MultiMeshIncisionManager IncisionManager;
    
    private LineRendererManipulate lineRenderer;
    
    private GameObject FirstHitObject;
    private GameObject incisionDistance;

    private int HitOBJIndex;
    private int Size;

    private float oldExtendValue;

    private bool firstIncision;

    private Vector3 oldPosition;

    private string mode;

    private Canvas rectCanvas;

    void Awake()
    {
        Object prefab = Resources.Load("Prefab/IncisionDistanceText");
        GameObject newCanvas = GameObject.Find("UICanvas");

        IncisionManager = this.gameObject.AddComponent<MultiMeshIncisionManager>();
        lineRenderer = new LineRendererManipulate(transform);
        rectCanvas = newCanvas.GetComponent<Canvas>();

        oldExtendValue = 0;
        oldPosition = Vector3.zero;

        firstIncision = false;
        mode = "incision";

        incisionDistance = (GameObject)Instantiate(prefab);
        incisionDistance.transform.SetParent(newCanvas.transform);
        incisionDistance.transform.localScale = Vector3.one;
        incisionDistance.SetActive(false);

        Size = MultiMeshManager.Instance.Size;
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
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit FirstHit;

            if (Physics.Raycast(ray, out FirstHit, 1000f))
                FirstHitObject = FirstHit.collider.gameObject;
            else
                return;

            for (int i = 0; i < Size; i++)
                if (FirstHitObject.name == GameObject.Find("PartialModel").transform.GetChild(i).name)
                {
                    HitOBJIndex = i;
                    MultiMeshManager.Instance.MeshIndex = HitOBJIndex;
                }

            GameObject.Find("Main").GetComponent<CHD>().MeshIndex = HitOBJIndex;
        }

        IntersectedValues intersectedValues = Intersections.MultiMeshGetIntersectedValues(HitOBJIndex);
        bool checkInside = intersectedValues.Intersected;

        if (Input.GetMouseButtonDown(0) && checkInside)
        {
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");

            firstIncision = true;
            incisionDistance.SetActive(true);
            oldPosition = intersectedValues.IntersectedPosition;

            IncisionManager.IncisionUpdate();
            IncisionManager.SetStartVerticesDF(HitOBJIndex);

            MultiMeshAdjacencyList.Instance.ListUpdate();
        }
        else if (Input.GetMouseButton(0))
        {
            if (!firstIncision)
                return;

            if (!checkInside)
            {
                ChatManager.Instance.GenerateMessage(" 심장을 벗어났습니다.");
                EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                Destroy(lineRenderer.lineObject);
                Destroy(incisionDistance);
                Destroy(this);
            }

            Vector3 currentPosition = intersectedValues.IntersectedPosition;
            Vector3 curPos = currentPosition;
            Vector3 oldPos = oldPosition;

            Vector2 newRectPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas.transform as RectTransform, Input.mousePosition, rectCanvas.worldCamera, out newRectPos);
            incisionDistance.GetComponent<RectTransform>().localPosition = newRectPos;
            incisionDistance.GetComponent<Text>().text = (Vector3.Distance(oldPos, curPos) / MultiMeshManager.Instance.PivotTransform.localScale.z).ToString("N3") + " mm";

            lineRenderer.SetFixedLineRenderer(oldPos, curPos);
        }
        else if (Input.GetMouseButtonUp(0) && firstIncision)
        {
            if (checkInside)
            {
                Vector3 currentPosition = intersectedValues.IntersectedPosition;

                if (Vector3.Distance(oldPosition, currentPosition) < 2.5f * MultiMeshManager.Instance.PivotTransform.lossyScale.z)
                {
                    EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                    ChatManager.Instance.GenerateMessage(" incision 거리가 너무 짧습니다.");
                    IncisionManager.IncisionUpdate();
                    firstIncision = false;
                    Destroy(incisionDistance);
                    Destroy(lineRenderer.lineObject);
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
                IncisionManager.IncisionUpdate();
                EventManager.Instance.Events.InvokeModeManipulate("EndAll");
                Destroy(incisionDistance);
                Destroy(this);
                return;
            }

            // 위에서 잘못되면 끊어야됨.
            IncisionManager.ExecuteDividing(HitOBJIndex);
            MultiMeshAdjacencyList.Instance.ListUpdate();
            IncisionManager.GenerateIncisionList(HitOBJIndex);
            IncisionManager.currentIndex++;

            MultiMeshManager.Instance.Meshes[HitOBJIndex].RecalculateNormals();
            Destroy(incisionDistance);
            mode = "extand";
            MultiMeshManager.Instance.IncisionOk = true;

            EventManager.Instance.Events.InvokeModeManipulate("EndWithoutScaling");
            ChatManager.Instance.GenerateMessage(" 절개하였습니다. 확장이 가능합니다.");
        }
    }
    private void handleExtand()
    {
        if (oldExtendValue != UIManager.Instance.extendBar.value)
        {
            IncisionManager.Extending(IncisionManager.currentIndex - 1, UIManager.Instance.extendBar.value, oldExtendValue, HitOBJIndex);
            oldExtendValue = UIManager.Instance.extendBar.value;
            MultiMeshManager.Instance.Meshes[HitOBJIndex].RecalculateNormals();
        }
    }
}