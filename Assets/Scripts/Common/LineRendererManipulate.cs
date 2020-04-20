using UnityEngine;
using UnityEditor;

public class LineRendererManipulate
{
    private GameObject lineRenderer;
    public LineRendererManipulate()
    {
        lineRenderer = new GameObject("LineRenderer", typeof(LineRenderer));
        lineRenderer.layer = 8;
    }
    
    public static void SetLineRenderer()
    {


    }


    public GameObject DestroyLineRenderer()
    {
        return lineRenderer;
    }















}