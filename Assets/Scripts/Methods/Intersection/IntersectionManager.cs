﻿using UnityEngine;
using System.Collections.Generic;

public class IntersectionManager : Singleton<IntersectionManager>
{
    public void GetIntersectedValues(Ray cameraRay, ref int outerTriangleIndex, ref int innerTriangleIndex)
    {
        float dst_min = 10000000;
        float dst_2nd = 10000000;

        int[] triangels = MeshManager.Instance.triangles;
        Vector3[] worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangels.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangels[i]], worldPosition[triangels[i + 1]], worldPosition[triangels[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    if (dst_min != 10000000)
                    {
                        innerTriangleIndex = outerTriangleIndex;
                        dst_2nd = dst_min;
                        
                    }
                    dst_min = dst_temp;
                    outerTriangleIndex = i;
                    continue;
                }
                if (dst_2nd > dst_temp)
                {
                    innerTriangleIndex = i;
                    dst_2nd = dst_temp;
                }
            }
        }
        if(outerTriangleIndex == -1)
        {
            Debug.Log("triangle이랑 intersect 되지 않음.");
            // 여기다가 예외처리를 어떻게 해줄 것인지.
        }
        else if(innerTriangleIndex == -1)
        {
            Debug.Log("triangle이랑 intersect 되지 않음.");
        }
        
    }

    public void GetIntersectedValues(Ray cameraRay, ref Vector3 outerIntersectionPosition, ref Vector3 innerIntersectionPosition)
    {
        float dst_min = 10000000;
        float dst_2nd = 10000000;

        int[] triangels = MeshManager.Instance.triangles;
        Vector3[] worldPosition = AdjacencyList.Instance.worldPositionVertices;

        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangels.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangels[i]], worldPosition[triangels[i + 1]], worldPosition[triangels[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    if (dst_min != 10000000)
                    {
                        innerIntersectionPosition = outerIntersectionPosition;
                        dst_2nd = dst_min;
                    }
                    dst_min = dst_temp;
                    outerIntersectionPosition = intersectionTemp;
                    continue;
                }
                if (dst_2nd > dst_temp)
                {
                    innerIntersectionPosition = intersectionTemp;
                    dst_2nd = dst_temp;
                }
            }
        }

        if (outerIntersectionPosition == Vector3.zero)
        {
            Debug.Log("triangle이랑 intersect 되지 않음.");
            // 여기다가 예외처리를 어떻게 해줄 것인지.
        }
        else if (innerIntersectionPosition == Vector3.zero)
        {
            Debug.Log("triangle이랑 intersect 되지 않음.");
        }

    }

    public void GetIntersectedValues(Ray cameraRay, ref Vector3 outerIntersectionPosition, ref Vector3 innerIntersectionPosition, ref int outerTriangleIndex, ref int innerTriangleIndex)
    {
        float dst_min = 10000000;
        float dst_2nd = 10000000;

        int[] triangels = MeshManager.Instance.triangles;
        Vector3[] worldPosition = AdjacencyList.Instance.worldPositionVertices;

        //Vector3 intersectionInnerPoint = Vector3.zero;
        //Vector3 intersectionOuterPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangels.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangels[i]], worldPosition[triangels[i + 1]], worldPosition[triangels[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    if (dst_min != 10000000)
                    {
                        innerTriangleIndex = outerTriangleIndex;
                        innerIntersectionPosition = outerIntersectionPosition;
                        dst_2nd = dst_min;
                    }
                    dst_min = dst_temp;
                    outerTriangleIndex = i;
                    outerIntersectionPosition = intersectionTemp;
                    continue;
                }
                if (dst_2nd > dst_temp)
                {
                    innerTriangleIndex = i;
                    innerIntersectionPosition = intersectionTemp;
                    dst_2nd = dst_temp;
                }
            }
        }
        if (outerTriangleIndex == -1)
        {
            Debug.Log("triangle이랑 intersect 되지 않음.");
            // 여기다가 예외처리를 어떻게 해줄 것인지.
        }
        else if (innerTriangleIndex == -1)
        {
            Debug.Log("triangle이랑 intersect 되지 않음.");
        }

    }
    
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

    public int TriangleEdgeIntersection(ref int edgeIdx, ref Vector3 edgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, ref int incisionTriangleIndex, Ray screenStartRay, Ray screenEndRay)
    {
        // return side number : 1, 2, 0 / 1 : couter-clockwise, 2 : clockwise, 0 : 첫 시작, -1 : error 반환
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        Vector3[] worldVertices = AdjacencyList.Instance.worldPositionVertices;

        int currentEdgeIndex = edgeIdx;

        Vector3 intersectionPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;

        Vector3 screenMiddlePoint = Vector3.Lerp(screenStartRay.origin, screenEndRay.origin, 0.5f);

        Vector3[] vertices = MeshManager.Instance.mesh.vertices;

        for (int i = 0; i < 3; i++)
        {
            bool checkIntersection = true;

            if (currentEdgeIndex != -1 && edgeList[currentEdgeIndex].vtx1 == edgeList[incisionTriangleIndex + i].vtx2 && edgeList[currentEdgeIndex].vtx2 == edgeList[incisionTriangleIndex + i].vtx1)
                continue;

            if (RayTriangleIntersection(screenMiddlePoint, incisionStartPoint + screenStartRay.direction * 10, incisionEndPoint + screenEndRay.direction * 10, worldVertices[edgeList[incisionTriangleIndex + i].vtx1], worldVertices[edgeList[incisionTriangleIndex + i].vtx2] - worldVertices[edgeList[incisionTriangleIndex + i].vtx1], ref intersectionTemp))
            {
                Debug.Log("ray triangle intersection");
                if (!(intersectionTemp.x < Mathf.Min(worldVertices[edgeList[incisionTriangleIndex + i].vtx1].x, worldVertices[edgeList[incisionTriangleIndex + i].vtx2].x)) && !(intersectionTemp.x > Mathf.Max(worldVertices[edgeList[incisionTriangleIndex + i].vtx1].x, worldVertices[edgeList[incisionTriangleIndex + i].vtx2].x)))
                    intersectionPoint = intersectionTemp;
                else
                    checkIntersection = false;
            }
            else
                checkIntersection = false;

            if (checkIntersection)
            {
                edgeIdx = incisionTriangleIndex + i;
                incisionTriangleIndex = edgeList[edgeIdx].tri2;
                edgePoint = intersectionPoint;
                break;
            }
        }

        if (currentEdgeIndex == -1)
        {
            if (edgeIdx != -1)
                return 0;
            else
                return -1;
        }
        else if (edgeList[currentEdgeIndex].vtx1 == edgeList[edgeIdx].vtx2)
        {
            // counter-clockwise
            return 1;
        }
        else if (edgeList[currentEdgeIndex].vtx2 == edgeList[edgeIdx].vtx1)
        {
            // clockwise
            return 2;
        }

        return -1;
    }
}
