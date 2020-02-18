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
    private GeneratePatch generatePatch;

    public float patchWeight;
    // patchWeight은 bar value * objManager.lossyscale * 20으로 만들어져야됨.
    // scale이 들어가 있어서 매번 업데이트 되어야함.

    public void Generate()
    {
        generatePatch = new GeneratePatch();
        generatePatch.GenerateInit();
    }

    public void AddVertex(Vector3 newVertexPosition)
    {
        generatePatch.AddPatchVerticesList(newVertexPosition);
    }

    public void GenerateMesh()
    {
        generatePatch.GeneratePatchTriangle();
    }

    public void UpdateCurve(int patchIndex)
    {
        float heightValue = UIManager.Instance.heightBar.value;
        float curveValue = UIManager.Instance.curveBar.value;

        patchCenterPos[patchIndex] = weightCenterPos[patchIndex] + ((heightValue - 0.4f) * 40) * avgNorm[patchIndex];
        Debug.Log(weightCenterPos[patchIndex]);
        patchWeight = curveValue * 20.0f * ObjManager.Instance.objTransform.lossyScale.z;
        generatePatch.RecalculateNormal(patchIndex);
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
