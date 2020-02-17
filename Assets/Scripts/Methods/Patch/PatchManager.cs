using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchManager : Singleton<PatchManager>
{
    public List<GameObject> newPatch;
    public List<Vector3> patchVertices = new List<Vector3>();
    public Vector3 patchVertexPosition = Vector3.zero;
    public Vector3 patchCenterPos = Vector3.zero;
    public float patchVerticesIntervalValue = 3.0f;

    public GameObject generatePatch;

    private void Start()
    {
        newPatch = new List<GameObject>();
        // generatePatch.GetComponent<GeneratePatch>().AddPatchVerticesList(Vector3.zero);
        
    }

    public void PatchMain()
    {




    }




}
