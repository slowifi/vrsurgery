using System.Collections.Generic;
using UnityEngine;

public class Erase : MonoBehaviour
{
    public void RemoveTriangles()
    {
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<int> removeTrianglesList = CutManager.Instance.removeTrianglesList;

        int[] newTriangles = new int[triangles.Length - (removeTrianglesList.Count * 3)];
        removeTrianglesList.Sort();

        int triangleCount = 0, tempCount = 0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (removeTrianglesList[tempCount] == i)
            {
                tempCount++;
                if (tempCount == removeTrianglesList.Count)
                    tempCount--;
                continue;
            }
            newTriangles[triangleCount++] = triangles[i];
            newTriangles[triangleCount++] = triangles[i + 1];
            newTriangles[triangleCount++] = triangles[i + 2];
        }

        MeshManager.Instance.mesh.triangles = newTriangles;
        return;
    }
}
