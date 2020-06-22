using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshPatchMode : MonoBehaviour
{
    private bool isPatchUpdate;
    private bool isLastPatch;
    private int patchCount;
    private Vector3 oldPosition;
    private Vector3 firstPosition;
    private Ray oldRay;
    private Ray firstRay;
    private LineRendererManipulate lineRenderer;
    private PatchManager patchManager;

    private void FirstSet()
    {
        lineRenderer = new LineRendererManipulate(transform);
        isPatchUpdate = false;
        isLastPatch = false;
        oldPosition = Vector3.zero;
        patchCount = 0;
    }

    void Awake()
    {
        FirstSet();
        this.gameObject.AddComponent<PatchManager>();
        patchManager = GetComponent<PatchManager>();
    }
        
    void Update()
    {
        Ray cameraRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        if(isLastPatch)
        {
        }
        else if(Input.GetMouseButtonDown(0))
        {
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            IntersectedValues[] intersectedValues = new IntersectedValues[MultiMeshManager.Instance.Size];
            
        }
    }
}
