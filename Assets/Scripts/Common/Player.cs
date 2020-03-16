﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 행동제어를 아예 player쪽에서 하기.






    void Update()
    {
#if UNITY_STANDALONE_WIN

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel!=0)
        {
            // 여기에 UI 추가. + init scale저장 해놓고 0 밑으로 안가도록.
            ObjManager.Instance.pivotTransform.localScale += Vector3.one * (wheel * 0.8f);

            if (transform.localScale.x <= 0.2f)
                transform.localScale = Vector3.one * 0.2f;
            AdjacencyList.Instance.WorldPositionUpdate();
        }
        else if (Input.GetMouseButton(1))
        {
            // 여기에 UI 추가.
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            ObjManager.Instance.pivotTransform.RotateAround(Vector3.up, (-x * 0.1f));
            ObjManager.Instance.pivotTransform.RotateAround(Vector3.right, (-y * 0.1f));
            AdjacencyList.Instance.WorldPositionUpdate();
        }
        else if (Input.GetMouseButton(2))
        {
            // 여기에 UI 추가.
            // 전체 이동.
            float xPos = Input.GetAxis("Mouse X");
            float yPos = Input.GetAxis("Mouse Y");

            ObjManager.Instance.pivotTransform.position += Vector3.left * (xPos * 2f);
            ObjManager.Instance.pivotTransform.position += Vector3.up * (yPos * 2f);
            AdjacencyList.Instance.WorldPositionUpdate();
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

        if (Input.touchCount == 1) // For Touch Input with a finger
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                float XaxisRotation = Input.GetTouch(0).deltaPosition.x / XResolution * -90.0f;
                float YaxisRotation = Input.GetTouch(0).deltaPosition.y / YResolution * -90.0f;
                // select the axis by which you want to rotate the GameObject                               
                ObjManager.Instance.pivotTransform.RotateAround(Vector3.zero, Vector3.up, XaxisRotation);
                ObjManager.Instance.pivotTransform.RotateAround(Vector3.zero, Vector3.right, YaxisRotation);
            }
            AdjacencyList.Instance.WorldPositionUpdate();
            return;
        }


        if (Input.touchCount == 2)
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
            ObjManager.Instance.pivotTransform.localScale += Vector3.one * deltaMagnitudeDiff/1000;

            if (transform.localScale.x <= 0.2f)
                transform.localScale = Vector3.one * 0.2f;
            AdjacencyList.Instance.WorldPositionUpdate();

            return;
        }

        if (Input.touchCount == 3)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                float xPos = Input.GetTouch(0).deltaPosition.x / XResolution * -90.0f;
                float yPos = Input.GetTouch(0).deltaPosition.y / YResolution * -90.0f;
                // select the axis by which you want to rotate the GameObject                               
                ObjManager.Instance.pivotTransform.position += Vector3.left * xPos;
                ObjManager.Instance.pivotTransform.position += Vector3.up * yPos;
            }
            AdjacencyList.Instance.WorldPositionUpdate();
        }
#endif

    }
}
