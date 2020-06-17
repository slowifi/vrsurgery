using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler
{
    // 행동제어를 아예 player쪽에서 하기.
    public Camera UIcamera;
    private bool boolRotation = true;
    private bool boolScaling = true;
    private bool boolTranslation = true;
    private bool boolAllStop = false;

    private void Awake()
    {
        EventManager.Instance.Events.OnModeChanged += Events_OnChanged;
        EventManager.Instance.Events.OnModeManipulate += Events_OnModeManipulate;
    }

    private void SetTrueTransformBool()
    {
        boolRotation = true;
        boolScaling = true;
        boolTranslation = true;
    }

    private void SetFalseTransformBool()
    {
        boolRotation = false;
        boolScaling = false;
        boolTranslation = false;
    }

    private void Events_OnModeManipulate(string action)
    {
        switch (action)
        {
            case "StopAll":
                SetFalseTransformBool();
                break;
            case "EndAll":
                SetTrueTransformBool();
                break;
            case "EndWithoutScaling":
                SetTrueTransformBool();
                boolScaling = false;
                break;
        }
    }

    private void Events_OnChanged(string mode)
    {
        switch (mode)
        {
            case "Incision":
                Debug.Log("incision 실행");
                break;
            case "Cut":
                Debug.Log("cut 실행");
                break;
            case "Patch":
                Debug.Log("patch 실행");
                break;
            case "Slice":
                Debug.Log("slice 실행");
                break;
            case "Measure":
                Debug.Log("measure 실행");
                break;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerPress.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerPress.name + "123123");
    }


    void Update()
    {
        Ray cameraRay = UIcamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.layer == 5)
            {
                boolAllStop = true;
            }
            boolAllStop = false;
            return;
        }
        if (!boolAllStop)
        {
#if UNITY_STANDALONE_WIN
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel != 0 && boolScaling)
            {
                /*// Single Object
                // 여기에 UI 추가. + init scale저장 해놓고 0 밑으로 안가도록.
                MeshManager.Instance.pivotTransform.localScale += Vector3.one * (wheel * 0.8f);
                
                if (MeshManager.Instance.pivotTransform.localScale.x <= 0.2f)
                    MeshManager.Instance.pivotTransform.localScale = Vector3.one * 0.2f;

                AdjacencyList.Instance.WorldPositionUpdate();*/
                
                //Multi Object
                MultiMeshManager.Instance.pivotTransform.localScale += Vector3.one * (wheel * 0.8f);

                if (MultiMeshManager.Instance.pivotTransform.localScale.x <= 0.2f)
                    MultiMeshManager.Instance.pivotTransform.localScale = Vector3.one * 0.2f;

                MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
            }
            else if (Input.GetMouseButton(1) && boolRotation)
            {
                /*// 여기에 UI 추가.
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                MeshManager.Instance.pivotTransform.RotateAround(Vector3.up, (-x * 0.1f));
                MeshManager.Instance.pivotTransform.RotateAround(Vector3.right, (-y * 0.1f));
                AdjacencyList.Instance.WorldPositionUpdate();*/

                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                MultiMeshManager.Instance.pivotTransform.RotateAround(Vector3.up, (-x * 0.1f));
                MultiMeshManager.Instance.pivotTransform.RotateAround(Vector3.right, (-y * 0.1f));
                MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
            }
            else if (Input.GetMouseButton(2) && boolTranslation)
            {
                /*// 여기에 UI 추가.
                // 전체 이동.
                float xPos = Input.GetAxis("Mouse X");
                float yPos = Input.GetAxis("Mouse Y");

                MeshManager.Instance.pivotTransform.position += Vector3.left * (xPos * 2f);
                MeshManager.Instance.pivotTransform.position += Vector3.up * (yPos * 2f);
                AdjacencyList.Instance.WorldPositionUpdate();*/

                float xPos = Input.GetAxis("Mouse X");
                float yPos = Input.GetAxis("Mouse Y");

                MultiMeshManager.Instance.pivotTransform.position += Vector3.left * (xPos * 2f);
                MultiMeshManager.Instance.pivotTransform.position += Vector3.up * (yPos * 2f);
                MultiMeshAdjacencyList.Instance.WorldPositionUpdate();
            }
            // 특정 움직임이 있었을 때만 업데이트 하도록 해야됨.
            // AdjacencyList.Instance.WorldPositionUpdate();
#endif

            // android용은 수정 해야됨.
#if UNITY_ANDROID
            //int fingerCount = 0;

            Resolution resolutions = Screen.currentResolution;
        float XResolution = resolutions.width;
        float YResolution = resolutions.height;

        if (Input.touchCount == 1 && boolRotation) // For Touch Input with a finger
        {
            
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                float XaxisRotation = Input.GetTouch(0).deltaPosition.x / XResolution * -90.0f;
                float YaxisRotation = Input.GetTouch(0).deltaPosition.y / YResolution * -90.0f;
                // select the axis by which you want to rotate the GameObject                               
                MeshManager.Instance.pivotTransform.RotateAround(Vector3.up, XaxisRotation/20f);
                MeshManager.Instance.pivotTransform.RotateAround(Vector3.right, YaxisRotation/20f);
            }
            AdjacencyList.Instance.WorldPositionUpdate();
            return;
            
        }

        if (Input.touchCount == 2 && boolScaling)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            //여기서 조건 두개임. 하나는 그냥 확대고, 하나는 ui에서 스크롤 조절하게.

            MeshManager.Instance.pivotTransform.localScale += Vector3.one * deltaMagnitudeDiff/700;

            if (MeshManager.Instance.pivotTransform.localScale.x <= 0.2f)
                MeshManager.Instance.pivotTransform.localScale = Vector3.one * 0.2f;
            AdjacencyList.Instance.WorldPositionUpdate();

            return;
        }
        else if(Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            float extendValue = UIManager.Instance.extendBar.value + deltaMagnitudeDiff / 500;
            if (extendValue >= 1)
                UIManager.Instance.extendBar.value = 1;
            else if (extendValue <= 0)
                UIManager.Instance.extendBar.value = 0;
            else
                UIManager.Instance.extendBar.value = extendValue;
            return;
        }
        

        if (Input.touchCount == 3 && boolTranslation)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                float xPos = Input.GetTouch(0).deltaPosition.x / XResolution * -90.0f;
                float yPos = Input.GetTouch(0).deltaPosition.y / YResolution * -90.0f;
                // select the axis by which you want to rotate the GameObject                               
                MeshManager.Instance.pivotTransform.position += Vector3.left * -xPos;
                MeshManager.Instance.pivotTransform.position += Vector3.up * -yPos;
            }
            AdjacencyList.Instance.WorldPositionUpdate();
        }
#endif
        }
    }
}
