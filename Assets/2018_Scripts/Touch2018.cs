﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch2018 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject humanHeart;
    private GameObject pivot;
    private Transform originPivot;
    private GameObject cuttingPlaneObject;
    public Transform pivotOffset;
    public Touch touch1;
    public Touch touch2;

    public bool breathingEnabled = false;

    private float zoomConstant = 0.1f;
    private float _currentScale;
    private float TargetScale;
    private float InitScale;
    private int FramesCount;
    private float AnimationTimeSeconds;
    private float _deltaTime;
    private float _dx;
    private bool _upScale;


    // Button status
    int[] mButtonStatus = new int[3];


    // CurrentMode - 1: Partial(SMRA) 2: Partial SMRV 3: Partial SMLA, 4: Partial SMLV, 5: SMPA, 6: SMAORTA
    // 1 1 1 1 1 1  (All)    
    private long currentMode;


    private Vector3 centerOfMass;

    private GameObject currentPlane;
    private customMeshRenderer currentPartRenderer;

    private GameObject PAPlane;
    private GameObject AortaPlane;
    private customMeshRenderer PARenderer;
    private customMeshRenderer AortaRenderer;


    private IEnumerator Breathing()
    {
        while (true)
        {
            while (_upScale)
            {
                if (breathingEnabled)
                {
                    _currentScale += _dx;
                    if (_currentScale > TargetScale)
                    {
                        _upScale = false;
                        _currentScale = TargetScale;
                    }
                }

                humanHeart.transform.localScale = Vector3.one * _currentScale;

                humanHeart.transform.position = -pivotOffset.position * _currentScale;


                yield return new WaitForSeconds(_deltaTime);
            }

            while (!_upScale)
            {
                if (breathingEnabled)
                {
                    _currentScale -= _dx;
                    if (_currentScale < InitScale)
                    {
                        _upScale = true;
                        _currentScale = InitScale;
                    }
                }

                humanHeart.transform.localScale = Vector3.one * _currentScale;

                humanHeart.transform.position = -pivotOffset.position * _currentScale;

                yield return new WaitForSeconds(_deltaTime);
            }
        }
    }

    void setByMode()
    {
        long tempMode = currentMode;
        // CurrentMode - 1: Partial(SMRA) 2: Partial SMRV 3: Partial SMLA, 4: Partial SMLV, 5: SMPA, 6: SMAORTA
        // 1 1 1 1 1 1  (All)        

        if (tempMode / 1000000 == 1)
        {
            GameObject.Find("Coronary").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("Coronary").GetComponent<MeshRenderer>().enabled = false;
        tempMode %= 1000000;


        if (tempMode / 100000 == 1)
        {
            GameObject.Find("LV").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("LV").GetComponent<MeshRenderer>().enabled = false;
        tempMode %= 100000;

        if (tempMode / 10000 == 1)
        {
            GameObject.Find("LA").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("LA").GetComponent<MeshRenderer>().enabled = false;
        tempMode %= 10000;

        if (tempMode / 1000 == 1)
        {
            GameObject.Find("RV").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("RV").GetComponent<MeshRenderer>().enabled = false;
        tempMode %= 1000;

        if (tempMode / 100 == 1)
        {
            GameObject.Find("RA").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("RA").GetComponent<MeshRenderer>().enabled = false;
        tempMode %= 100;


        if (tempMode / 10 == 1)
        {
            GameObject.Find("Aorta").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("Aorta").GetComponent<MeshRenderer>().enabled = false;
        tempMode %= 10;

        if (tempMode / 1 == 1)
        {
            GameObject.Find("PA").GetComponent<MeshRenderer>().enabled = true;
        }
        else GameObject.Find("PA").GetComponent<MeshRenderer>().enabled = false;


        Debug.Log("Maharaga: Current mode: " + currentMode);
    }

    private Bounds CalculateLocalBounds()
    {
        Quaternion currentRotation = this.transform.rotation;
        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
        int i = 1;
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (i == 1)
            {
                i = 0;
                bounds = new Bounds(renderer.bounds.center, renderer.bounds.size);
            }
            else bounds.Encapsulate(renderer.bounds);
        }
        Vector3 localCenter = bounds.center - this.transform.position;
        bounds.center = localCenter;
        this.transform.rotation = currentRotation;
        return bounds;
    }


    void Start()
    {
        currentMode = 1111111;

        setByMode();


        // Estimating local bounds of a model set and setting a pivot point against the model
        humanHeart = GameObject.Find("HumanHeart");
        if (humanHeart == null)
            Debug.Log("Null Model");
        else Debug.Log("Heart Model Loaded");

        if (pivot == null)
            pivot = new GameObject();
        Debug.Log(pivot.transform.position);
        pivotOffset = pivot.transform;
        pivotOffset.position = CalculateLocalBounds().center;
        transform.SetParent(pivot.transform);
        humanHeart.transform.position = -pivotOffset.position;
        Debug.Log("Maharaga: " + humanHeart.transform.position + pivotOffset.position);


        TargetScale = 1.1f;
        InitScale = 1f;
        _currentScale = InitScale;
        FramesCount = 100;
        AnimationTimeSeconds = 2;
        _deltaTime = AnimationTimeSeconds / FramesCount;
        _dx = (TargetScale - InitScale) / FramesCount;
        _upScale = true;

        for (int i = 0; i < 3; i++)
            mButtonStatus[i] = 0;



        StartCoroutine(Breathing());
    }
    // Scaling
    Vector3 updateCoM(Mesh mesh)
    {
        var vertices = mesh.vertices;
        mesh.RecalculateBounds();
        return mesh.bounds.center;
    }

    public void ToggleParts(ButtonPressDetection.buttonData mItem)
    {
        long divider = (long)Mathf.Pow(10, mItem.buttonIndex);
        if (currentMode % (divider * 10) / divider == 1)
            currentMode -= divider;
        else currentMode += divider;
        setByMode();
    }

    public void ResetPivot(ButtonPressDetection.buttonData mItem)
    {
        float x = humanHeart.transform.lossyScale.x;
        pivot.transform.position = new Vector3(-23.95925f, -13.67178f, -43.37923f);
        pivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        humanHeart.transform.localScale = Vector3.one;
        InitScale += (humanHeart.transform.localScale.x - x);
        TargetScale += (humanHeart.transform.localScale.x - x);
        _currentScale += (humanHeart.transform.localScale.x - x);

        updatePlaneModel();
    }

    public void ProjectParts(ButtonPressDetection.buttonData mItem)
    {

        if (currentPartRenderer != null && currentPlane != null)
        {
            // Turn off current plane
            if (currentPartRenderer.Equals(GameObject.Find("PA").GetComponent<customMeshRenderer>()))
            {
                PAPlane.GetComponent<customPlaneManager>().planeVisibility(false, pivot.transform.rotation);
                PARenderer.offFocusCutting();
            }
            else if (currentPartRenderer.Equals(GameObject.Find("Aorta").GetComponent<customMeshRenderer>()))
            {
                AortaPlane.GetComponent<customPlaneManager>().planeVisibility(false, pivot.transform.rotation);
                AortaRenderer.offFocusCutting();
            }
            currentPlane.GetComponent<customPlaneManager>().planeVisibility(false, pivot.transform.rotation);
            currentPartRenderer.offFocusCutting();

            if (currentPartRenderer.Equals(GameObject.Find(mItem.name).GetComponent<customMeshRenderer>()))
            {
                if (mItem.name == "PA")
                {
                    PARenderer = null;
                    PAPlane = null;
                }
                else if (mItem.name == "Aorta")
                {
                    AortaRenderer = null;
                    AortaPlane = null;
                }
                currentPartRenderer = null;
                currentPlane = null;
                return;
            }
        }

        // Find Mesh Renderer of human heart part for turning on the cutting plane        
        currentPartRenderer = GameObject.Find(mItem.name).GetComponent<customMeshRenderer>();
        if (mItem.name == "PA")
        {
            PARenderer = GameObject.Find("PAValve").GetComponent<customMeshRenderer>();
            PARenderer.setCuttingPlane();
            PAPlane = GameObject.Find("PAValve Plane");
            PAPlane.GetComponent<customPlaneManager>().meshCenter = PARenderer.rend.bounds.center;
            PAPlane.GetComponent<customPlaneManager>().planeVisibility(true, pivot.transform.rotation);
            PARenderer.syncPlane(PAPlane.GetComponent<customPlaneManager>());
        }
        else if (mItem.name == "Aorta")
        {
            AortaRenderer = GameObject.Find("AortaValve").GetComponent<customMeshRenderer>();
            AortaRenderer.setCuttingPlane();
            AortaPlane = GameObject.Find("AortaValve Plane");
            AortaPlane.GetComponent<customPlaneManager>().meshCenter = AortaRenderer.rend.bounds.center;
            AortaPlane.GetComponent<customPlaneManager>().planeVisibility(true, pivot.transform.rotation);
            AortaRenderer.syncPlane(AortaPlane.GetComponent<customPlaneManager>());
        }

        currentPartRenderer.setCuttingPlane();
        // Turn on the cutting plane
        currentPlane = GameObject.Find(mItem.name + " Plane");
        currentPlane.GetComponent<customPlaneManager>().meshCenter = currentPartRenderer.rend.bounds.center;
        currentPlane.GetComponent<customPlaneManager>().planeVisibility(true, pivot.transform.rotation);

        // Synchronize cutting parameters between current cutting plane and the custom mesh renderer of human heart part
        currentPartRenderer.syncPlane(currentPlane.GetComponent<customPlaneManager>()); ;
    }

    void updatePlaneModel()
    {
        foreach (customMeshRenderer mRend in GameObject.Find("PartialModel").GetComponentsInChildren<customMeshRenderer>())
        {
            GameObject updatePlane = GameObject.Find(mRend.name + " Plane");
            if (mRend.getCuttingStatus() == 1)
            {
                updatePlane.GetComponent<customPlaneManager>().meshCenter = mRend.rend.bounds.center;
                updatePlane.GetComponent<customPlaneManager>().rotateThisPlane(pivot.transform.rotation);
                mRend.syncPlane(updatePlane.GetComponent<customPlaneManager>());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        int fingerCount = 0;
        Resolution resolutions = Screen.currentResolution;
        float XResolution = resolutions.width;
        float YResolution = resolutions.height;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            float XaxisRotation = 10f;
            pivot.transform.RotateAround(Vector3.zero, Vector3.up, XaxisRotation);

            updatePlaneModel();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            float YaxisRotation = 10f;
            pivot.transform.RotateAround(Vector3.zero, Vector3.right, YaxisRotation);

            updatePlaneModel();

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            InitScale += zoomConstant;
            TargetScale += zoomConstant;
            _currentScale += zoomConstant;

            Breathing();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            InitScale -= zoomConstant;
            TargetScale -= zoomConstant;
            _currentScale -= zoomConstant;

            Breathing();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Maharaga: Number 1, " + currentMode);
            if (currentMode / 100000 == 1) currentMode = currentMode - 100000;
            else currentMode = currentMode + 100000;
            Debug.Log("Maharaga: Number 1 after, " + currentMode);
            setByMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Maharaga: Number 2, " + currentMode);
            if (currentMode % 100000 / 10000 == 1) currentMode -= 10000;
            else currentMode += 10000;
            Debug.Log("Maharaga: Number 2 after, " + currentMode);
            setByMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentMode % 10000 / 1000 == 1) currentMode -= 1000;
            else currentMode += 1000;
            setByMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (currentMode % 1000 / 100 == 1) currentMode -= 100;
            else currentMode += 100;
            setByMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (currentMode % 100 / 10 == 1) currentMode -= 10;
            else currentMode += 10;
            setByMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (currentMode % 10 == 1) currentMode -= 1;
            else currentMode += 1;
            setByMode();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Maharaga: global Plane " + currentPlane.GetComponent<customPlaneManager>().mPlane.normal + currentPlane.GetComponent<customPlaneManager>().mPlane.distance);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Maharaga: global Plane " + currentPlane.GetComponent<customPlaneManager>().mPlane.normal + currentPlane.GetComponent<customPlaneManager>().mPlane.distance);
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;

        }
        if (fingerCount == 1) // For Touch Input with a finger
        // Drag: Convert 2D translational movement to 3D rotation
        // Touch: Select an anchor
        {
            touch1 = Input.GetTouch(0);
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                float XaxisRotation = Input.GetTouch(0).deltaPosition.x / XResolution * -90.0f;
                float YaxisRotation = Input.GetTouch(0).deltaPosition.y / YResolution * -90.0f;
                // select the axis by which you want to rotate the GameObject                               
                pivot.transform.RotateAround(Vector3.zero, Vector3.up, XaxisRotation);
                pivot.transform.RotateAround(Vector3.zero, Vector3.right, YaxisRotation);

                updatePlaneModel();
            }

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

            zoomConstant = 1f + deltaMagnitudeDiff / 1000f;
            InitScale *= zoomConstant;
            TargetScale *= zoomConstant;
            _currentScale *= zoomConstant;

            if (TargetScale >= 3.0f)
            {
                TargetScale = 3.0f;
                InitScale = 2.9f;
                _currentScale = 2.9f;
            }

            Breathing();
        }
        /*
        if (Input.touchCount == 3)
        {
            // 여기서 매쉬 랜더러로 보내기.
            foreach (customMeshRenderer mRend in GameObject.Find("PartialModel").GetComponentsInChildren<customMeshRenderer>())
            {
                GameObject updatePlane = GameObject.Find(mRend.name + " Plane");
                if (mRend.getCuttingStatus() == 1)
                {
                    updatePlane.GetComponent<customPlaneManager>().meshCenter = mRend.rend.bounds.center;
                    updatePlane.GetComponent<customPlaneManager>().rotateThisPlane(pivot.transform.rotation);
                    mRend.updateTouchInput(updatePlane.GetComponent<customPlaneManager>());
                    // mRend.syncPlane(updatePlane.GetComponent<customPlaneManager>());
                }
            }
            updatePlaneModel();
        }
        */
    }
}
