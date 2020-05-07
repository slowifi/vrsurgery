using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customPlaneManager : MonoBehaviour
{
    public Plane mPlane;
    private Vector3 initNormal;
    private float initDistance;
    private Quaternion axisInitRotation;    
    private Renderer[] visualPlane;
    public Vector3 meshCenter;
    private bool initialized = false;

    void Start()
    {
        initNormal = new Vector3(0.5773503f, -0.5773503f, 0.5773503f);
        mPlane = new Plane(initNormal, 0.55f);
        
        visualPlane = GetComponentsInChildren<Renderer>();

        foreach (Renderer mRend in visualPlane)
        {
            mRend.enabled = false;
        }

        //transform.localScale = new Vector3(10f, 1f, 10f);

        this.transform.rotation = Quaternion.FromToRotation(Vector3.up, initNormal);
        this.transform.position = mPlane.normal * mPlane.distance;
        initDistance = mPlane.distance;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(mPlane.normal);
    }

    public void planeVisibility(bool onoff, Quaternion pivot)
    {
        foreach (Renderer mRend in visualPlane)
        {
            mRend.enabled = onoff;
        }
        if (!initialized)
        {
            initialized = true;
            Debug.Log("Maharaga: InitPivotting");
            axisInitRotation = pivot;
        }
    }

    public void rotateThisPlane(Quaternion rotation)
    {
        Matrix4x4 mAnchor = Matrix4x4.Rotate(Quaternion.Inverse(axisInitRotation));
        Matrix4x4 m = Matrix4x4.Rotate(rotation);
        mPlane.normal = mAnchor.MultiplyPoint3x4(initNormal);
        mPlane.normal = m.MultiplyPoint3x4(mPlane.normal);

        this.transform.rotation = Quaternion.FromToRotation(Vector3.up, mPlane.normal);
        this.transform.position = mPlane.normal * mPlane.distance;

    }


    public void translateThisPlane()
    { 
        this.transform.position = mPlane.normal * mPlane.distance;
    }
}
