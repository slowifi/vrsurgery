using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshMeasureManager : MonoBehaviour
{
    private float distanceStartToEnd = 0f;
    private Vector3 measureStart;
    private Vector3 measureEnd;
    private Vector2 startMousePos;
    private Vector2 endMousePos;

    public float MeasureDistance(Vector3 vertexPosition, Ray cameraRay)
    {
        if (vertexPosition != Vector3.zero)
        {
            GameObject startPoint = MultiMeshManager.Instance.StartMeasurePoint;
            GameObject endPoint = MultiMeshManager.Instance.EndMeasurePoint;

            if (!startPoint.activeSelf)
            {
                measureStart = cameraRay.direction;
                startMousePos = Input.mousePosition;

                startPoint.SetActive(true);
                startPoint.transform.position = vertexPosition;
            }
            else if (!endPoint.activeSelf)
            {
                endMousePos = Input.mousePosition;
                endPoint.SetActive(true);
                endPoint.transform.position = vertexPosition;
                measureEnd = cameraRay.direction;
                distanceStartToEnd = Vector3.Distance(endPoint.transform.position, startPoint.transform.position);
            }
            else
            {
                Destroy(GameObject.Find("MeasureLine"));
                measureStart = cameraRay.direction;
                startPoint.transform.position = vertexPosition;
                endPoint.SetActive(false);
            }
        }
        return distanceStartToEnd;
    }
    public Vector3 vertexPosition(Ray cameraRay, int MeshIndex)
    {
        float dst_min = 1000000;
        int[] triangles = MultiMeshManager.Instance.Meshes[MeshIndex].triangles;
        List<Vector3> worldPositionVertices = MultiMeshAdjacencyList.Instance.WorldPositionVertices[MeshIndex];

        Vector3 intersectionTemp = Vector3.zero;
        Vector3 intersectionPoint = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (Intersections.RayTriangleIntersection(worldPositionVertices[triangles[i]], worldPositionVertices[triangles[i + 1]], worldPositionVertices[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);

                if (dst_min > dst_temp)
                {
                    intersectionPoint = intersectionTemp;
                    dst_min = dst_temp;
                }
            }
        }

        if (dst_min != 1000000)
            return intersectionPoint;

        return Vector3.zero;
    }
}
