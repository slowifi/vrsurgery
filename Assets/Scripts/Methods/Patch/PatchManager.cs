using System.Collections.Generic;
using UnityEngine;

public class PatchManager : Singleton<PatchManager>
{
    public List<GameObject> newPatch;
    public List<Vector3> avgNorm;
    public List<Vector3> weightCenterPos;
    public List<Vector3> patchCenterPos;
    public List<List<Vector3>[]> insidePatchVertices;
    public List<int> patchVerticesCount;

    public float patchVerticesIntervalValue;
    public float patchWeight;

    private GeneratePatch _generatePatch;


    public void Generate()
    {
        _generatePatch = gameObject.AddComponent<GeneratePatch>();
        _generatePatch.GenerateInit();
    }

    public void AddVertex(Vector3 newVertexPosition)
    {
        _generatePatch.AddPatchVerticesList(newVertexPosition);
    }

    public void GenerateMesh()
    {
        _generatePatch.GeneratePatchTriangle();
    }

    public void UpdateCurve(int patchIndex)
    {
        float heightValue = UIManager.Instance.heightBar.value;
        float curveValue = UIManager.Instance.curveBar.value;

        patchCenterPos[patchIndex] = weightCenterPos[patchIndex] + ((heightValue - 0.5f) * 40) * avgNorm[patchIndex];
        patchWeight = curveValue * 20.0f * ObjManager.Instance.objTransform.lossyScale.z;
        _generatePatch.RecalculateNormal(patchIndex);
    }

    protected override void InitializeChild()
    {
        newPatch = new List<GameObject>();
        avgNorm = new List<Vector3>();
        weightCenterPos = new List<Vector3>();
        patchCenterPos = new List<Vector3>();

        insidePatchVertices = new List<List<Vector3>[]>();
        patchVerticesCount = new List<int>();
        patchVerticesIntervalValue = 3.0f;
        
        patchWeight = 0f;
    }

}
