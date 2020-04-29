using UnityEngine;
using UnityEditor;

public class LineRendererManipulate
{
    public GameObject lineObject;
    public LineRendererManipulate(Transform parent)
    {
        lineObject = new GameObject("LineRenderer", typeof(LineRenderer));
        lineObject.layer = 8;
        lineObject.transform.SetParent(parent);
        lineObject.GetComponent<LineRenderer>().numCornerVertices = 45;
        lineObject.GetComponent<LineRenderer>().material = Resources.Load("Materials/LineRenderer", typeof(Material)) as Material;
    }

    public void SetLineRenderer(Vector3 rayOldOrigin, Vector3 rayNewOrigin)
    {
        LineRenderer line = lineObject.GetComponent<LineRenderer>();
        line.SetPosition(line.positionCount - 1, rayNewOrigin);
        line.positionCount++; // 이게 추가 되기때문에 자꾸 0으로 무언가 보이는데
        line.SetPosition(line.positionCount - 1, rayNewOrigin);
    }

    public void SetFixedLineRenderer(Vector3 rayOldOrigin, Vector3 rayNewOrigin)
    {
        LineRenderer line = lineObject.GetComponent<LineRenderer>();
        line.SetPosition(line.positionCount-2, rayOldOrigin);
        line.SetPosition(line.positionCount-1, rayNewOrigin);
    }


}