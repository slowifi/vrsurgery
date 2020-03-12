using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmsManager : Singleton<AlgorithmsManager>
{
    public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }

    public double QuadraticEquation(double x1, double y1, double x2, double y2, double radius)
    {
        double a = x2 * x2 + y2 * y2;
        double b = 2 * x1 * x2 + 2 * y1 * y2;
        double c = x1 * x1 + y1 * y1 - radius * radius;
        double d = (b * b) - (4 * a * c);
        if (d > 0)
        {
            double e = Math.Sqrt(d);
            if ((-b + e) / (2.0 * a) >= 0)
                return (-b + e) / (2.0 * a);
            else
                return (-b - e) / (2.0 * a);
        }
        else
        {
            if (d == 0)
                return (-b) / (2.0 * a);
            else
                Debug.Log("허근");
        }
        return 0;
    }

    public Vector3 GetPlaneNormal(Vector3 vertexPoint1, Vector3 vertexPoint2, Vector3 vertexPoint3)
    {
        Vector3 crossVec1 = vertexPoint2 - vertexPoint1;
        Vector3 crossVec2 = vertexPoint3 - vertexPoint1;
        Vector3 planeNormal = Vector3.Cross(crossVec1, crossVec2);
        return planeNormal;
    }


}
