using System.Collections.Generic;
using UnityEngine;

public class RemoveTriangles : MonoBehaviour
{
    private void Remove()
    {
        int[] triangles = ObjManager.Instance.mesh.triangles;
        List<int> removeTriangles = CutManager.Instance.removeTrianglesList;
        int[] newTriangles = new int[triangles.Length - (removeTriangles.Count * 3)];
        Debug.Log(triangles.Length - (removeTriangles.Count * 3));
        removeTriangles.Sort();
        int triangleCount = 0, tempCount = 0;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (removeTriangles[tempCount] == i)
            {
                tempCount++;
                if (tempCount == removeTriangles.Count)
                    tempCount--;
                continue;
            }
            newTriangles[triangleCount++] = triangles[i];
            newTriangles[triangleCount++] = triangles[i + 1];
            newTriangles[triangleCount++] = triangles[i + 2];
        }

        ObjManager.Instance.mesh.triangles = newTriangles;
        return;
    }
}
