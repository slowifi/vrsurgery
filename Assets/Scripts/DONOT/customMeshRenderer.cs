using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customMeshRenderer : MonoBehaviour
{    
    private Vector3 rotVector;
    private customPlaneManager mPlaneManager;

    public Renderer rend;
    public bool cuttingModeEnabled = false;
    public Plane cuttingPlaneModel;    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        
        rend.material.SetVector("_section", new Vector3(0.5773503f, -0.5773503f, 0.5773503f));
        
        rend.material.SetFloat("_distance", 0.55f);

        rend.material.SetInt("_clipping", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (cuttingModeEnabled)
        {
            rend.material.SetVector("_section", mPlaneManager.mPlane.normal);
            rend.material.SetFloat("_distance", mPlaneManager.mPlane.distance);
            Debug.Log(rend.material.GetVector("_section").x);
            Debug.Log(rend.material.GetVector("_section").y);
            Debug.Log(rend.material.GetVector("_section").z);
            Debug.Log(rend.material.GetVector("_section").w);
            // 터치값이 3인걸 함수화 시키기.

            if (Input.touchCount == 3)
            {
                Resolution resolutions = Screen.currentResolution;

                float XResolution = resolutions.width;


                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                Touch touchTwo = Input.GetTouch(2);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;


                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = Mathf.Max((touchZeroPrevPos - touchOnePrevPos).magnitude, (touchZeroPrevPos - touchTwoPrevPos).magnitude, (touchTwoPrevPos - touchOnePrevPos).magnitude);
                float touchDeltaMag = Mathf.Max((touchZero.position - touchOne.position).magnitude, (touchZero.position - touchTwo.position).magnitude, (touchTwo.position - touchOne.position).magnitude);

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;


                mPlaneManager.mPlane.distance += Input.GetTouch(0).deltaPosition.x / XResolution * 50f;
                rend.material.SetVector("_section", mPlaneManager.mPlane.normal);
                rend.material.SetFloat("_distance", mPlaneManager.mPlane.distance);
                mPlaneManager.translateThisPlane();

                Debug.Log("Maharaga: Cutting distance" + mPlaneManager.mPlane.distance + " and delta: " + deltaMagnitudeDiff);


            }
            

            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Maharaga: Trying to Move");
                Debug.Log(mPlaneManager.mPlane.normal);
                mPlaneManager.mPlane.distance += 0.15f;
                rend.material.SetVector("_section", mPlaneManager.mPlane.normal);
                rend.material.SetFloat("_distance", mPlaneManager.mPlane.distance);
                Debug.Log(mPlaneManager.mPlane.normal);
                mPlaneManager.translateThisPlane();
                Debug.Log(mPlaneManager.mPlane.normal);
                Debug.Log("Maharaga: local Plane " + mPlaneManager.mPlane.normal + mPlaneManager.mPlane.distance);

            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log(mPlaneManager.mPlane.normal);
                mPlaneManager.mPlane.distance -= 0.15f;
                Debug.Log(mPlaneManager.mPlane.normal);
                rend.material.SetVector("_section", mPlaneManager.mPlane.normal);
                rend.material.SetFloat("_distance", mPlaneManager.mPlane.distance);
                mPlaneManager.translateThisPlane();
                Debug.Log(mPlaneManager.mPlane.normal);
                Debug.Log("Maharaga: local Plane " + mPlaneManager.mPlane.normal + mPlaneManager.mPlane.distance);
            }
        }
    }

    public void updateTouchInput(customPlaneManager inputPlaneManager)
    {
        if (Input.touchCount == 3)
        {
            Resolution resolutions = Screen.currentResolution;

            float XResolution = resolutions.width;


            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Touch touchTwo = Input.GetTouch(2);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;


            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = Mathf.Max((touchZeroPrevPos - touchOnePrevPos).magnitude, (touchZeroPrevPos - touchTwoPrevPos).magnitude, (touchTwoPrevPos - touchOnePrevPos).magnitude);
            float touchDeltaMag = Mathf.Max((touchZero.position - touchOne.position).magnitude, (touchZero.position - touchTwo.position).magnitude, (touchTwo.position - touchOne.position).magnitude);

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;


            inputPlaneManager.mPlane.distance += Input.GetTouch(0).deltaPosition.x / XResolution * 50f;
            rend.material.SetFloat("_distance", inputPlaneManager.mPlane.distance);
            inputPlaneManager.translateThisPlane();

            Debug.Log("Maharaga: Cutting distance" + inputPlaneManager.mPlane.distance + " and delta: " + deltaMagnitudeDiff);
        }
    }
    
    public void setCuttingPlane()
    {
        Debug.Log("Maharaga: cutting plane called in " + this.name);
        cuttingModeEnabled = true;
        rend.material.SetInt("_clipping", 1);
    }

    public void offCuttingPlane()
    {
        cuttingModeEnabled = false;
        rend.material.SetInt("_clipping", 0);
    }

    public void offFocusCutting()
    {
        cuttingModeEnabled = false;
    }

    public int getCuttingStatus()
    {
        return rend.material.GetInt("_clipping");
    }

    public void rotateLocal(float x, float y)
    {
        Debug.Log("Maharaga: before rot:" + mPlaneManager.mPlane.normal + mPlaneManager.mPlane.distance);
        transform.RotateAround(Vector3.zero, Vector3.up, x);
        transform.RotateAround(Vector3.zero, Vector3.right, y);
        rend.material.SetVector("_section", mPlaneManager.mPlane.normal);
        rend.material.SetFloat("_distance", mPlaneManager.mPlane.distance);
        Debug.Log("Maharaga: after rot:" + mPlaneManager.mPlane.normal + mPlaneManager.mPlane.distance);
    }

    public void syncPlane(customPlaneManager inPlaneManager)
    {
        mPlaneManager = inPlaneManager;

        rend.material.SetVector("_section", mPlaneManager.mPlane.normal);
        rend.material.SetFloat("_distance", mPlaneManager.mPlane.distance);
    }    
}

