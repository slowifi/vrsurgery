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
        }
        PatchManager.Instance.Reinitialize();

        ObjManager.Instance.pivotTransform.localPosition = Vector3.zero;
        ObjManager.Instance.pivotTransform.localScale = Vector3.one;
        ObjManager.Instance.pivotTransform.localEulerAngles = Vector3.zero;

        Destroy(MakeDoubleFaceMesh.Instance.oppositeObject);
        MakeDoubleFaceMesh.Instance.Reinitialize();

        AdjacencyList.Instance.ListUpdate();
    }
}
