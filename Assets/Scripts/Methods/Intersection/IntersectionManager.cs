using UnityEngine;
using System.Collections.Generic;

public class IntersectionManager : Singleton<IntersectionManager>
{
    public bool RayTriangleIntersection(Vector3 v0, Vector3 v1, Vector3 v2, Ray ray, ref Vector3 intersectionTemp)
    {
        Vector3 e1, e2, T, P;
        float Epsilon = 0.000001f;
        e1 = v1 - v0;
        e2 = v2 - v0;

        T = ray.origin - v0;

        P = Vector3.Cross(ray.direction, e2);
        var det = Vector3.Dot(e1, P);

        if (det > -Epsilon && det < Epsilon)
            return false;

        var invDet = 1 / det;


        var u = invDet * Vector3.Dot(T, P);

        if (u > 1 || u < 0)
            return false;

        var v = invDet * Vector3.Dot(ray.direction, Vector3.Cross(T, e1));

        if (v < 0 || u + v > 1)
            return false;

        var t = invDet * Vector3.Dot(e2, Vector3.Cross(T, e1));

        if (t > Epsilon)
        {
            intersectionTemp = ray.origin + ray.direction * t;
            return true;
        }
        return false;
    }

    public bool RayTriangleIntersection(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 origin, Vector3 direction, ref Vector3 intersectionTemp)
    {
        Vector3 e1, e2, T, P;
        float Epsilon = 0.000001f;
        e1 = v1 - v0;
        e2 = v2 - v0;

        T = origin - v0;

        P = Vector3.Cross(direction, e2);
        var det = Vector3.Dot(e1, P);

        if (det > -Epsilon && det < Epsilon)
            return false;

        var invDet = 1 / det;


        var u = invDet * Vector3.Dot(T, P);

        if (u > 1 || u < 0)
            return false;

        var v = invDet * Vector3.Dot(direction, Vector3.Cross(T, e1));

        if (v < 0 || u + v > 1)
            return false;

        var t = invDet * Vector3.Dot(e2, Vector3.Cross(T, e1));

        if (t > Epsilon)
        {
            intersectionTemp = origin + (direction * t);
            return true;
        }

        return false;
    }

    private int TriangleEdgeIntersection(ref int side, ref int edgeIdx, ref Vector3 edgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, int incisionPointIdx, Ray screenStartRay, Ray screenEndRay)
    {
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Vector3[] worldVertices = AdjacencyList.Instance.worldPositionVertices;
        int intersectionCount = 0;
        int tempEdge = edgeIdx;

        Vector3 intersectionPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;
        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        for (int i = 0; i < 3; i++)
        {
            int checkIntersectionCount = 2;
            if (tempEdge != -1 && edgeList[tempEdge].vtx1 == edgeList[incisionPointIdx + i].vtx2 && edgeList[tempEdge].vtx2 == edgeList[incisionPointIdx + i].vtx1)
                continue;
            // triangle : 1) s origin, e origin, incision start 2) e origin, incision start, incision end
            // intersectionTemp : test_world[edgeList[incisionPointIdx + i].vtx1], test_world[edgeList[incisionPointIdx + i].vtx2] - test_world[edgeList[incisionPointIdx + i].vtx1]
            if (RayTriangleIntersection(screenStartRay.origin, screenEndRay.origin, incisionStartPoint + screenStartRay.direction * 5, worldVertices[edgeList[incisionPointIdx + i].vtx1], worldVertices[edgeList[incisionPointIdx + i].vtx2] - worldVertices[edgeList[incisionPointIdx + i].vtx1], ref intersectionTemp))
            {
                Debug.Log("ray triangle intersection");
                if (intersectionTemp.x < Mathf.Min(worldVertices[edgeList[incisionPointIdx + i].vtx1].x, worldVertices[edgeList[incisionPointIdx + i].vtx2].x) || intersectionTemp.x > Mathf.Max(worldVertices[edgeList[incisionPointIdx + i].vtx1].x, worldVertices[edgeList[incisionPointIdx + i].vtx2].x))
                    checkIntersectionCount--;
                else
                    intersectionPoint = intersectionTemp;
            }
            else
                checkIntersectionCount--;

            if (RayTriangleIntersection(screenEndRay.origin, incisionEndPoint + screenEndRay.direction * 5, incisionStartPoint + screenStartRay.direction * 5, worldVertices[edgeList[incisionPointIdx + i].vtx1], worldVertices[edgeList[incisionPointIdx + i].vtx2] - worldVertices[edgeList[incisionPointIdx + i].vtx1], ref intersectionTemp))
            {
                Debug.Log("ray triangle intersection");
                if (intersectionTemp.x < Mathf.Min(worldVertices[edgeList[incisionPointIdx + i].vtx1].x, worldVertices[edgeList[incisionPointIdx + i].vtx2].x) || intersectionTemp.x > Mathf.Max(worldVertices[edgeList[incisionPointIdx + i].vtx1].x, worldVertices[edgeList[incisionPointIdx + i].vtx2].x))
                    checkIntersectionCount--;
                else
                    intersectionPoint = intersectionTemp;
            }
            else
                checkIntersectionCount--;

            if (checkIntersectionCount != 0)
            {
                edgeIdx = incisionPointIdx + i;
                edgePoint = intersectionPoint;
                intersectionCount++;
            }
        }

        if (tempEdge == -1)
        {
            return intersectionCount;
        }
        else if (edgeList[tempEdge].vtx1 == edgeList[edgeIdx].vtx1)
        {
            // counter-clockwise
            side = 1;
        }
        else if (edgeList[tempEdge].vtx2 == edgeList[edgeIdx].vtx2)
        {
            // clockwise
            side = 2;
        }
        Debug.Log(side);
        
        return intersectionCount;
    }
}
