using UnityEngine;
using System.Collections.Generic;

public class GeneratePatch : MonoBehaviour
{
    private Vector3 _patchVertexPosition;
    private List<Vector3> _patchVertices;
    private List<Vector3>[] _insidePatchVertices;
    private int _patchIndex;

    public void GenerateInit()
    {
        PatchManager.Instance.newPatch.Add(new GameObject("Patch", typeof(MeshFilter), typeof(MeshRenderer)));
        PatchManager.Instance.avgNorm.Add(Vector3.zero);
        PatchManager.Instance.weightCenterPos.Add(Vector3.zero);
        PatchManager.Instance.patchCenterPos.Add(Vector3.zero);
        PatchManager.Instance.insidePatchVertices.Add(new List<Vector3>[5]);
        PatchManager.Instance.patchVerticesIntervalValue = 3.0f;
        PatchManager.Instance.patchWeight = UIManager.Instance.curveBar.value * 20f * ObjManager.Instance.objTransform.lossyScale.z;

        _patchVertexPosition = Vector3.zero;
        _patchVertices = new List<Vector3>();
        _insidePatchVertices = new List<Vector3>[5];
        _patchIndex = PatchManager.Instance.newPatch.Count-1;

        for (int i = 0; i < 5; i++)
            _insidePatchVertices[i] = new List<Vector3>();
    }

    public void AddPatchVerticesList(Vector3 newVertexPosition)
    {
        Vector3 patchVertexPosition = _patchVertexPosition;
        
        if (patchVertexPosition == Vector3.zero)
        {
            _patchVertexPosition = newVertexPosition;
            _patchVertices.Add(_patchVertexPosition);
        }
        else if (Vector3.Distance(patchVertexPosition, newVertexPosition) > PatchManager.Instance.patchVerticesIntervalValue)
        {
            _patchVertexPosition = newVertexPosition;
            _patchVertices.Add(_patchVertexPosition);
        }
    }

    public void GeneratePatchTriangle()
    {
        
        Vector3 _patchCenterPos = PatchManager.Instance.patchCenterPos[_patchIndex];
        Vector3[] vtrList = new Vector3[_patchVertices.Count];

        float minDst = 1000000;

        for (int i = 0; i < _patchVertices.Count; i++)
            _patchCenterPos += _patchVertices[i];
        _patchCenterPos /= _patchVertices.Count;

        PatchManager.Instance.patchCenterPos[_patchIndex] = _patchCenterPos;

        for (int i = 0; i < _patchVertices.Count; i++)
        {
            vtrList[i] = _patchCenterPos - _patchVertices[i];
            float dst = Vector3.Distance(_patchVertices[i], _patchCenterPos);
            if (dst < minDst)
                minDst = dst;
        }

        for (int i = 0; i < _patchVertices.Count; i++)
        {
            _insidePatchVertices[0].Add(_patchVertices[i]);
            _insidePatchVertices[1].Add(_patchVertices[i] + (vtrList[i] / 5 * 1));
            _insidePatchVertices[2].Add(_patchVertices[i] + (vtrList[i] / 5 * 2));
            _insidePatchVertices[3].Add(_patchVertices[i] + (vtrList[i] / 5 * 3));
            _insidePatchVertices[4].Add(_patchVertices[i] + (vtrList[i] / 5 * 4));
        }

        foreach (var item in _patchVertices)
        {
            GameObject v_test = new GameObject();
            v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            v_test.transform.position = item;
        }
        GeneratePatchObj();
    }

    private void GeneratePatchObj()
    {
        PatchManager.Instance.newPatch.Add(new GameObject("Patch", typeof(MeshFilter), typeof(MeshRenderer)));
        GameObject patchObj = PatchManager.Instance.newPatch[0];
        Mesh mesh = new Mesh();
        patchObj.GetComponent<MeshFilter>().mesh = mesh;
        patchObj.transform.parent = GameObject.Find("HumanHeart").transform;

        int patchVertexCount = _patchVertices.Count;

        Renderer rend = patchObj.GetComponent<Renderer>();
        rend.material.color = Color.white;

        Vector3[] points = new Vector3[patchVertexCount * 5 + 1];
        int[] triangles = new int[((patchVertexCount * 2) * 4 + patchVertexCount) * 3];
        // int[] triangles = new int[((patchVertices.Count * 2) * 4 + patchVertices.Count) * 3];
        for (int j = 0; j < 5; j++)
        {
            for (int i = patchVertexCount * j; i < patchVertexCount * (j + 1); i++)
                points[i] = _insidePatchVertices[j][i % patchVertexCount];
        }

        points[patchVertexCount * 5] = PatchManager.Instance.patchCenterPos[_patchIndex];


        BuildMesh(ref triangles, 5 - 1);

        mesh.vertices = points;
        mesh.triangles = triangles;
        CalculateNormal();
    }

    private void BuildMesh(ref int[] triangles, int loopCount)
    {
        int temp_num = _patchVertices.Count;
        int TN = 0; // triangle idx
        for (int i = 0; i < loopCount; i++)
        {
            for (int j = 0; j < temp_num; j++)
            {
                int F = temp_num * i + j;
                int K = F + 1;
                int N = F + temp_num;
                int M = N + 1;

                if (F % temp_num == temp_num - 1)
                    K = temp_num * i;

                if (N % temp_num == temp_num - 1)
                    M = temp_num * (i + 1);

                triangles[TN++] = F;
                triangles[TN++] = K;
                triangles[TN++] = N;

                triangles[TN++] = K;
                triangles[TN++] = M;
                triangles[TN++] = N;
            }
        }

        for (int i = 0; i < temp_num; i++)
        {
            int N = loopCount * temp_num + i;
            int K = N + 1;

            if (K == temp_num * (loopCount + 1))
                K = temp_num * loopCount;

            triangles[TN++] = N;
            triangles[TN++] = K;
            triangles[TN++] = temp_num * (loopCount + 1);
        }
        return;
    }

    private void CalculateNormal()
    {
        PatchManager.Instance.patchVerticesCount.Add(_patchVertices.Count);
        PatchManager.Instance.insidePatchVertices[_patchIndex] = _insidePatchVertices;

        Mesh mesh = PatchManager.Instance.newPatch[_patchIndex].GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals();

        int tempNum = _patchVertices.Count;
        Vector3 _avgNorm = Vector3.zero;

        for (int i = 0; i < mesh.normals.Length; i++)
            _avgNorm += mesh.normals[i];
        _avgNorm /= mesh.normals.Length;

        PatchManager.Instance.avgNorm[_patchIndex] = _avgNorm;
        PatchManager.Instance.patchCenterPos[_patchIndex] += 10 * _avgNorm;
        Vector3 newCenterPos = PatchManager.Instance.patchCenterPos[_patchIndex];
        PatchManager.Instance.weightCenterPos[_patchIndex] = newCenterPos;
        Vector3[] patchVertexPosition = mesh.vertices;

        for (int i = 0; i < tempNum; i++)
        {
            Vector3 p1 = _insidePatchVertices[0][i];
            Vector3 p2 = _insidePatchVertices[2][i] + _avgNorm * PatchManager.Instance.patchWeight;
            Vector3 p3 = newCenterPos;
            patchVertexPosition[i + tempNum] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.2f), Vector3.Lerp(p2, p3, 0.2f), 0.2f);
            patchVertexPosition[i + tempNum * 2] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.4f), Vector3.Lerp(p2, p3, 0.4f), 0.4f);
            patchVertexPosition[i + tempNum * 3] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.6f), Vector3.Lerp(p2, p3, 0.6f), 0.6f);
            patchVertexPosition[i + tempNum * 4] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.8f), Vector3.Lerp(p2, p3, 0.8f), 0.8f);
        }

        patchVertexPosition[mesh.normals.Length - 1] = newCenterPos;

        mesh.vertices = patchVertexPosition;
        mesh.RecalculateNormals();
        return;
    }

    // 업데이트용
    public void RecalculateNormal(int patchIndex)
    {
        Mesh mesh = PatchManager.Instance.newPatch[patchIndex].GetComponent<MeshFilter>().mesh;
        int patchVertexCount = PatchManager.Instance.patchVerticesCount[patchIndex];
        List<Vector3>[] _insidePatchVertices = PatchManager.Instance.insidePatchVertices[patchIndex];
        Vector3[] patchVertexPosition = mesh.vertices;

        for (int i = 0; i < patchVertexCount; i++)
        {
            Vector3 p1 = _insidePatchVertices[0][i];
            Vector3 p2 = _insidePatchVertices[2][i] + PatchManager.Instance.avgNorm[patchIndex] * PatchManager.Instance.patchWeight;
            Vector3 p3 = PatchManager.Instance.patchCenterPos[patchIndex];
            patchVertexPosition[i + patchVertexCount] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.2f), Vector3.Lerp(p2, p3, 0.2f), 0.2f);
            patchVertexPosition[i + patchVertexCount * 2] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.4f), Vector3.Lerp(p2, p3, 0.4f), 0.4f);
            patchVertexPosition[i + patchVertexCount * 3] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.6f), Vector3.Lerp(p2, p3, 0.6f), 0.6f);
            patchVertexPosition[i + patchVertexCount * 4] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.8f), Vector3.Lerp(p2, p3, 0.8f), 0.8f);
        }
        patchVertexPosition[mesh.normals.Length - 1] = PatchManager.Instance.patchCenterPos[patchIndex];

        mesh.vertices = patchVertexPosition;
    }
}

