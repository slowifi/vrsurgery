using UnityEngine;
using System.Collections.Generic;

public class IntersectionManager : Singleton<IntersectionManager>
{
    public int GetIntersectedValues(Ray cameraRay)
    {

        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;
        int vertexIndex = -1;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    dst_min = dst_temp;
                    vertexIndex = triangles[i];
                }
            }
        }
        return vertexIndex;

    }

    public void GetIntersectedValues(Ray cameraRay, ref int triangleIndex)
    {
        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    dst_min = dst_temp;
                    triangleIndex = i;
                }
            }
        }
    }

    public void GetIntersectedValues(Ray cameraRay, ref Vector3 intersectionPoint)
    {
        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    dst_min = dst_temp;
                    intersectionPoint = intersectionTemp;
                }
            }
        }
    }

    public void GetIntersectedValues(Ray cameraRay, ref Vector3 intersectionPoint, ref int triangleIndex)
    {
        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    dst_min = dst_temp;
                    intersectionPoint = intersectionTemp;
                    triangleIndex = i;
                }
            }
        }
    }

    public void GetIntersectedValues(Ray cameraRay, ref int outerTriangleIndex, ref int innerTriangleIndex)
    {
        float dst_min = 10000000;
        float dst_2nd = 10000000;

        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
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

        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
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

        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;

        //Vector3 intersectionInnerPoint = Vector3.zero;
        //Vector3 intersectionOuterPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
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

    public bool RayObjectIntersection(Ray cameraRay)
    {
        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                    dst_min = dst_temp;
            }
        }
        if (dst_min != 10000000)
            return true;
        else
            return false;

    }

    public bool RayObjectIntersection(Ray cameraRay, ref Vector3 intersectionPoint)
    {
        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    dst_min = dst_temp;
                    intersectionPoint = intersectionTemp;
                }
            }
        }
        if (dst_min != 10000000)
            return true;
        else
            return false;
                
    }

    public bool RayObjectIntersection(Ray cameraRay, ref Vector3 intersectionPoint, ref int triangleIndex)
    {
        float dst_min = 10000000;
        int[] triangles = MeshManager.Instance.mesh.triangles;
        List<Vector3> worldPosition = AdjacencyList.Instance.worldPositionVertices;
        Vector3 intersectionTemp = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (RayTriangleIntersection(worldPosition[triangles[i]], worldPosition[triangles[i + 1]], worldPosition[triangles[i + 2]], cameraRay, ref intersectionTemp))
            {
                float dst_temp = Vector3.Magnitude(cameraRay.origin - intersectionTemp);
                if (dst_min > dst_temp)
                {
                    dst_min = dst_temp;
                    intersectionPoint = intersectionTemp;
                    triangleIndex = i;
                }
            }
        }

        if (dst_min != 10000000)
            return true;
        else
            return false;
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
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;

        int currentEdgeIndex = edgeIdx;

        Vector3 intersectionPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;

        Vector3 screenMiddlePoint = Vector3.Lerp(screenStartRay.origin, screenEndRay.origin, 0.5f);

        Vector3[] vertices = MeshManager.Instance.mesh.vertices;
        // 여기를 다시 한 번 보자.
        int intersectionCount = 0;
        for (int i = 0; i < 3; i++)
        {
            bool checkIntersection = true;

            if (currentEdgeIndex != -1 && edgeList[currentEdgeIndex].vtx1 == edgeList[incisionTriangleIndex + i].vtx1 && edgeList[currentEdgeIndex].vtx2 == edgeList[incisionTriangleIndex + i].vtx2)
                continue;

            // 이거 자체를 바꿔야하나?
            if (RayTriangleIntersection(screenMiddlePoint, incisionStartPoint + screenStartRay.direction * 10, incisionEndPoint + screenEndRay.direction * 10, worldVertices[edgeList[incisionTriangleIndex + i].vtx1], worldVertices[edgeList[incisionTriangleIndex + i].vtx2] - worldVertices[edgeList[incisionTriangleIndex + i].vtx1], ref intersectionTemp))
            {
                //intersectionPoint = intersectionTemp;
                if (!(intersectionTemp.x <= Mathf.Min(worldVertices[edgeList[incisionTriangleIndex + i].vtx1].x, worldVertices[edgeList[incisionTriangleIndex + i].vtx2].x)) && !(intersectionTemp.x >= Mathf.Max(worldVertices[edgeList[incisionTriangleIndex + i].vtx1].x, worldVertices[edgeList[incisionTriangleIndex + i].vtx2].x)))
                {
                    intersectionPoint = intersectionTemp;
                    Debug.Log("ray triangle intersection");
                }
                else
                    checkIntersection = false;
            }
            else
                checkIntersection = false;

            if (checkIntersection)
            {
                intersectionCount++;
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
            {
                Debug.Log("intersection이 안됨.");
                return -1;
            }
        }
        else if (edgeList[currentEdgeIndex].vtx1 == edgeList[edgeIdx].vtx2)
        {
            // counter-clockwise
            // 이게 왜 여기서 쓸 때 반대가 됐을까?
            return 1;
        }
        else if (edgeList[currentEdgeIndex].vtx2 == edgeList[edgeIdx].vtx1)
        {
            // clockwise
            return 2;
        }
        //Debug.Log(edgeIdx);
        //Debug.Log(currentEdgeIndex);
        //Debug.Log(intersectionCount);
        //int[] triangles = MeshManager.Instance.mesh.triangles;
        //GameObject v_test = new GameObject();
        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = incisionStartPoint;

        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = incisionEndPoint;

        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = worldVertices[edgeList[edgeIdx].vtx1];

        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = worldVertices[edgeList[edgeIdx].vtx2];


        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = worldVertices[triangles[incisionTriangleIndex]];

        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = worldVertices[triangles[incisionTriangleIndex+1]];

        //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //v_test.transform.position = worldVertices[triangles[incisionTriangleIndex+2]];

        //Debug.Break();
        return -1;
    }


    public int PlaneEdgeIntersection(ref int edgeIdx, ref Vector3 edgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, ref int incisionTriangleIndex, Ray screenStartRay, Ray screenEndRay)
    {
        // return side number : 1, 2, 0 / 1 : couter-clockwise, 2 : clockwise, 0 : 첫 시작, -1 : error 반환
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;

        int currentEdgeIndex = edgeIdx;

        Vector3 intersectionPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;

        // 이 밑에 두개 값들은 고정된 값이므로 여기서 계산하지말고 밖에서 한번만 계산해서 넘겨주는식으로.
        Vector3 screenMiddlePoint = Vector3.Lerp(screenStartRay.origin, screenEndRay.origin, 0.5f);
        Vector3 planeNormal = AlgorithmsManager.Instance.GetPlaneNormal(screenMiddlePoint, incisionStartPoint, incisionEndPoint);

        // 여기를 다시 한 번 보자.
        int intersectionCount = 0;
        for (int i = 0; i < 3; i++)
        {
            bool checkIntersection = true;

            if (currentEdgeIndex != -1 && edgeList[currentEdgeIndex].vtx1 == edgeList[incisionTriangleIndex + i].vtx1 && edgeList[currentEdgeIndex].vtx2 == edgeList[incisionTriangleIndex + i].vtx2)
                continue;

            //처음에 방향 설정 관련
            if (LinePlaneIntersection(ref intersectionTemp, worldVertices[edgeList[incisionTriangleIndex + i].vtx1], worldVertices[edgeList[incisionTriangleIndex + i].vtx2]- worldVertices[edgeList[incisionTriangleIndex + i].vtx1], planeNormal, incisionStartPoint))
            {
                //첫 시작에서 두개가 잡혀야됨. 버텍스에서 시작할 때가 문제인데 아닌 버텍스인경우 엣지를 아예 검사하지 말아야됨.
                Debug.Log("line plane intersection");
                intersectionPoint = intersectionTemp;
            }
            else
                checkIntersection = false;

            if (checkIntersection)
            {
                intersectionCount++;
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
            {
                Debug.Break();
                return -1;
            }
        }
        else if (edgeList[currentEdgeIndex].vtx1 == edgeList[edgeIdx].vtx2)
        {
            // counter-clockwise
            // 이게 왜 여기서 쓸 때 반대가 됐을까?
            return 1;
        }
        else if (edgeList[currentEdgeIndex].vtx2 == edgeList[edgeIdx].vtx1)
        {
            // clockwise
            return 2;
        }
        return -1;
    }

    public int PlaneEdgeIntersectionStart(ref int edgeIdx, ref Vector3 edgePoint, Vector3 incisionStartPoint, Vector3 incisionEndPoint, ref int incisionTriangleIndex, Ray screenStartRay, Ray screenEndRay)
    {
        // 여기서 근데 3개가 겹쳐야 정상인거 같은데?
        // return side number : 1, 2, 0 / 1 : couter-clockwise, 2 : clockwise, 0 : 첫 시작, -1 : error 반환
        List<AdjacencyList.Edge> edgeList = AdjacencyList.Instance.edgeList;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;

        int currentEdgeIndex = edgeIdx;

        Vector3 intersectionPoint = Vector3.zero;
        Vector3 intersectionTemp = Vector3.zero;

        // 이 밑에 두개 값들은 고정된 값이므로 여기서 계산하지말고 밖에서 한번만 계산해서 넘겨주는식으로.
        Vector3 screenMiddlePoint = Vector3.Lerp(screenStartRay.origin, screenEndRay.origin, 0.5f);
        Vector3 planeNormal = AlgorithmsManager.Instance.GetPlaneNormal(screenMiddlePoint, incisionStartPoint, incisionEndPoint);

        // 여기를 다시 한 번 보자.
        int intersectionCount = 0;
        float distance = 1000000;
        for (int i = 0; i < 3; i++)
        {
            bool checkIntersection = true;

            if (currentEdgeIndex != -1 && edgeList[currentEdgeIndex].vtx1 == edgeList[incisionTriangleIndex + i].vtx1 && edgeList[currentEdgeIndex].vtx2 == edgeList[incisionTriangleIndex + i].vtx2)
                continue;

            //처음에 방향 설정 관련
            if (LinePlaneIntersectionModified(ref intersectionTemp, worldVertices[edgeList[incisionTriangleIndex + i].vtx1], worldVertices[edgeList[incisionTriangleIndex + i].vtx2] - worldVertices[edgeList[incisionTriangleIndex + i].vtx1], planeNormal, incisionStartPoint))
            {
                //첫 시작에서 두개가 잡혀야됨. 버텍스에서 시작할 때가 문제인데 아닌 버텍스인경우 엣지를 아예 검사하지 말아야됨.
                Debug.Log("line plane intersection");
                intersectionPoint = intersectionTemp;
            }
            else
                checkIntersection = false;

            if (checkIntersection)
            {
                float curDistance = Vector3.Distance(intersectionPoint, incisionEndPoint);
                if(curDistance<distance)
                {
                    distance = curDistance;
                    edgeIdx = incisionTriangleIndex + i;
                    incisionTriangleIndex = edgeList[edgeIdx].tri2;
                    edgePoint = intersectionPoint;
                }
            }
        }

        if (currentEdgeIndex == -1)
        {

            if (edgeIdx != -1)
                return 0;
            else
            {
                Debug.Break();
                return -1;
            }
        }
        else if (edgeList[currentEdgeIndex].vtx1 == edgeList[edgeIdx].vtx2)
        {
            // counter-clockwise
            // 이게 왜 여기서 쓸 때 반대가 됐을까?
            return 1;
        }
        else if (edgeList[currentEdgeIndex].vtx2 == edgeList[edgeIdx].vtx1)
        {
            // clockwise
            return 2;
        }
        return -1;
    }

    public bool LinePlaneIntersection(ref Vector3 intersectionPoint, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point

            vector = Vector3.Normalize(lineVec) * length;

            //get the coordinates of the line-plane intersection point
            intersectionPoint = linePoint + vector;

            // 이 경우는 점에 딱 맞는 경우는 없다고 가정한다.
            if (linePoint.x == (linePoint + lineVec).x)
            {
                if (intersectionPoint.y < linePoint.y)
                {
                    if (intersectionPoint.y > (linePoint + lineVec).y)
                        return true;
                    else
                        return false;
                }
                else if (intersectionPoint.y > linePoint.y)
                {
                    if (intersectionPoint.y < (linePoint + lineVec).y)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
            {
                if (intersectionPoint.x < linePoint.x)
                {
                    if (intersectionPoint.x > (linePoint + lineVec).x)
                        return true;
                    else
                        return false;
                }
                else if (intersectionPoint.x > linePoint.x)
                {
                    if (intersectionPoint.x < (linePoint + lineVec).x)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        //output not valid
        else
        {
            return false;
        }
    }

    public bool LinePlaneIntersectionModified(ref Vector3 intersectionPoint, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        float denom = Vector3.Dot(planeNormal, lineVec);
        if (Mathf.Abs(denom) > 0.0001f) // your favorite epsilon
        {
            float t = Vector3.Dot( (planePoint - linePoint), planeNormal) / denom;
            if (t >= 0)
            {
                intersectionPoint = linePoint + t * lineVec;
                if(intersectionPoint.x < linePoint.x)
                {
                    if (intersectionPoint.x > (linePoint + lineVec).x)
                        return true;
                    else
                        return false;
                }
                else if(intersectionPoint.x > linePoint.x)
                {
                    if (intersectionPoint.x < (linePoint + lineVec).x)
                        return true;
                    else
                        return false;
                }
                else
                    return false; // you might want to allow an epsilon here too
            }
        }
        return false;
    }
}
