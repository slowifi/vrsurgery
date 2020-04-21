using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMesh : MonoBehaviour
{
    public GameObject mainScript;
    public GameObject buttonPressScript;

    public void LoadFirstMesh()
    {
        mainScript.SendMessage("Exit");
        buttonPressScript.SendMessage("ResetButton");

        int[] triangles = (int[])MeshManager.Instance.firstMesh.triangles.Clone();
        Vector3[] vertices = (Vector3[])MeshManager.Instance.firstMesh.vertices.Clone();

        MeshManager.Instance.mesh.triangles = triangles;
        MeshManager.Instance.mesh.vertices = vertices;
        MeshManager.Instance.mesh.RecalculateNormals();

        for (int i = 0; i < PatchManager.Instance.newPatch.Count; i++)
        {
            Destroy(PatchManager.Instance.newPatch[i]);
            Destroy(GameObject.Find("Patch" + i + "_Inner"));
        }
        PatchManager.Instance.Reinitialize();

        MeshManager.Instance.pivotTransform.localPosition = Vector3.zero;
        MeshManager.Instance.pivotTransform.localScale = Vector3.one;
        MeshManager.Instance.pivotTransform.localEulerAngles = Vector3.zero;

        Destroy(MakeDoubleFaceMesh.Instance.oppositeObject);
        MakeDoubleFaceMesh.Instance.Reinitialize();

        AdjacencyList.Instance.ListUpdate();
    }
}
