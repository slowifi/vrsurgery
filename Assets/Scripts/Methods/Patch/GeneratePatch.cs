using UnityEngine;
using System.Collections.Generic;

public class GeneratePatch : MonoBehaviour
{
    private List<Vector3>[] insidePatchVertices = new List<Vector3>[5];

    private void AddPatchVerticesList(Vector3 vertexPosition)
    {
        Vector3 patchVertexPosition = PatchManager.Instance.patchVertexPosition;
        // 입력받는걸 다른 곳에서 계속 받을까
        if (patchVertexPosition == Vector3.zero)
        {
            patchVertexPosition = vertexPosition;
            PatchManager.Instance.patchVertices.Add(patchVertexPosition);
        }
        else if (Vector3.Distance(patchVertexPosition, vertexPosition) > PatchManager.Instance.patchVerticesIntervalValue)
        {
            patchVertexPosition = vertexPosition;
            PatchManager.Instance.patchVertices.Add(patchVertexPosition);
        }
    }

    private void GeneratePatchTriangle()
    {
        List<Vector3> patchVertices = PatchManager.Instance.patchVertices;
        Vector3 patchCenterPos = PatchManager.Instance.patchCenterPos;

        List<Vector3>[] insidePatchVertices = new List<Vector3>[5];
        Vector3[] vtrList = new Vector3[patchVertices.Count];
        float minDst = 1000000;

        for (int i = 0; i < patchVertices.Count; i++)
            patchCenterPos += patchVertices[i];
        patchCenterPos /= patchVertices.Count;
        PatchManager.Instance.patchCenterPos = patchCenterPos;

        for (int i = 0; i < patchVertices.Count; i++)
        {
            vtrList[i] = patchCenterPos - patchVertices[i];
            float dst = Vector3.Distance(patchVertices[i], patchCenterPos);
            if (dst < minDst)
                minDst = dst;
        }

        for (int i = 0; i < 5; i++)
            insidePatchVertices[i] = new List<Vector3>();

        for (int i = 0; i < patchVertices.Count; i++)
        {
            insidePatchVertices[0].Add(patchVertices[i]);
            insidePatchVertices[1].Add(patchVertices[i] + (vtrList[i] / 5 * 1));
            insidePatchVertices[2].Add(patchVertices[i] + (vtrList[i] / 5 * 2));
            insidePatchVertices[3].Add(patchVertices[i] + (vtrList[i] / 5 * 3));
            insidePatchVertices[4].Add(patchVertices[i] + (vtrList[i] / 5 * 4));
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
        int patchVertexCount = PatchManager.Instance.patchVertices.Count;

        Renderer rend = patchObj.GetComponent<Renderer>();
        rend.material.color = Color.white;

        Vector3[] points = new Vector3[patchVertexCount * 5 + 1];
        int[] triangles = new int[((patchVertexCount * 2) * 4 + patchVertexCount) * 3];

        for (int j = 0; j < 5; j++)
            for (int i = patchVertexCount * j; i < patchVertexCount * (j + 1); i++)
                points[i] = insidePatchVertices[j][i % patchVertexCount];

        points[patchVertexCount * 5] = PatchManager.Instance.patchCenterPos;

        BuildMesh(ref triangles, 5 - 1, patchVertexCount);

        mesh.vertices = points;
        mesh.triangles = triangles;
    }

    private void BuildMesh(ref int[] triangles, int loopCount, int patchVertexCount)
    {
        int temp_num = patchVertexCount;
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
    
    /*
    private void RecalculateNormal(int patchIndex)
    {
        Mesh mesh = GameObject.Find("Patch" + PatchManager.Instance.patchIndex).GetComponent<MeshFilter>().mesh;
        int patchVertexCount = PatchManager.Instance.patchVertices.Count;
        Vector3[] asdf = mesh.vertices;
        for (int i = 0; i < patchVertexCount; i++)
        {
            Vector3 p1 = insidePatchVertices[0][i];
            Vector3 p2 = insidePatchVertices[2][i] + avgNorm * patchWeight;
            Vector3 p3 = patchCenterPos;
            asdf[i + patchVertexCount] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.2f), Vector3.Lerp(p2, p3, 0.2f), 0.2f);
            asdf[i + patchVertexCount * 2] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.4f), Vector3.Lerp(p2, p3, 0.4f), 0.4f);
            asdf[i + patchVertexCount * 3] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.6f), Vector3.Lerp(p2, p3, 0.6f), 0.6f);
            asdf[i + patchVertexCount * 4] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.8f), Vector3.Lerp(p2, p3, 0.8f), 0.8f);
        }

        asdf[mesh.normals.Length - 1] = patchCenterPos;

        mesh.vertices = asdf;
        mesh.RecalculateNormals();
    }

    private void CalculateNormal()
    {
        Mesh mesh = GameObject.Find("Patch" + PatchManager.Instance.patchIndex).GetComponent<MeshFilter>().mesh;
        mesh.RecalculateNormals();
        int tempNum = patchVertices.Count;
        avgNorm = new Vector3(0, 0, 0);

        for (int i = 0; i < mesh.normals.Length; i++)
            avgNorm += mesh.normals[i];
        avgNorm /= mesh.normals.Length;
        patchCenterPos += 10 * avgNorm;
        weightCenterPos = patchCenterPos;
        Vector3[] asdf = mesh.vertices;
        for (int i = 0; i < tempNum; i++)
        {
            Vector3 p1 = insidePatchVertices[0][i];
            Vector3 p2 = insidePatchVertices[2][i] + avgNorm * patchWeight;
            Vector3 p3 = patchCenterPos;
            asdf[i + tempNum] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.2f), Vector3.Lerp(p2, p3, 0.2f), 0.2f);
            asdf[i + tempNum * 2] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.4f), Vector3.Lerp(p2, p3, 0.4f), 0.4f);
            asdf[i + tempNum * 3] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.6f), Vector3.Lerp(p2, p3, 0.6f), 0.6f);
            asdf[i + tempNum * 4] = Vector3.Lerp(Vector3.Lerp(p1, p2, 0.8f), Vector3.Lerp(p2, p3, 0.8f), 0.8f);
        }

        asdf[mesh.normals.Length - 1] = patchCenterPos;

        mesh.vertices = asdf;
        mesh.RecalculateNormals();
        return;
    }
    */

}
