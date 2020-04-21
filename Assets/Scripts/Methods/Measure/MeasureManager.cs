using UnityEngine;
using System.Collections.Generic;

public class MeasureManager : MonoBehaviour
{
    public float distanceStartToEnd = 0f;
    public Vector3 measureStart;
    public Vector3 measureEnd;


    public float MeasureDistance(Vector3 vertexPosition, Ray cameraRay)
    {
        if (vertexPosition != Vector3.zero)
        {
            GameObject startPoint = MeshManager.Instance.startMeasurePoint;
            GameObject endPoint = MeshManager.Instance.endMeasurePoint;
            if (!startPoint.activeSelf)
            {
                Destroy(GameObject.Find("MeasureLine"));
                measureStart = cameraRay.direction;
                startPoint.SetActive(true);
                startPoint.transform.position = vertexPosition;
            }
            else if (!endPoint.activeSelf)
            {
                endPoint.SetActive(true);
                endPoint.transform.position = vertexPosition;
                measureEnd = cameraRay.direction;
                // Debug.DrawLine(startPoint.transform.position, endPoint.transform.position, Color.yellow, 2, false);
                GameObject lineRenderer = new GameObject("MeasureLine");

                var line = lineRenderer.AddComponent<LineRenderer>();
                line.useWorldSpace = false;
                line.material.color = Color.white;
                line.SetWidth(0.6f, 0.6f);
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.SetPositions(new Vector3[] { startPoint.transform.position - measureStart * 0.5f * MeshManager.Instance.objTransform.lossyScale.z, endPoint.transform.position - measureEnd * 0.5f * MeshManager.Instance.objTransform.lossyScale.z });
                //line.SetPositions(new Vector3[] { startPoint.transform.position, endPoint.transform.position});
                line.transform.SetParent(MeshManager.Instance.pivotTransform);
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

    public Vector3 vertexPosition(Ray cameraRay)
    {
        float dst_min = 1000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPositionVertices = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;
        Vector3 intersectionPoint = Vector3.zero;
        // Debug.Log(worldPositionVertices.Length);
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
        {
            return intersectionPoint;
        }

        return Vector3.zero;
    }
}

