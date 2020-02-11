using UnityEngine;
public class Measure : Singleton<Measure>
{
    public Vector3 MeasurePosition(int[] triangles, Ray cameraRay, Vector3[] worldVertices)
    {
        float dst_min = 1000000;
        Vector3 _intersectionTemp = Vector3.zero;
        Vector3 _intersectionPoint = Vector3.zero;
        
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 1000, Color.yellow);
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (IntersectionManager.Instance.RayTriangleIntersection(worldVertices[triangles[i]], worldVertices[triangles[i + 1]], worldVertices[triangles[i + 2]], cameraRay, ref _intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - _intersectionTemp);
                if (dst_min > dst_temp)
                {
                    _intersectionPoint = _intersectionTemp;
                    dst_min = dst_temp;
                }
            }
        }

        if (dst_min != 1000000)
        {
            return _intersectionPoint;
        }

        return Vector3.zero;
    }
}

