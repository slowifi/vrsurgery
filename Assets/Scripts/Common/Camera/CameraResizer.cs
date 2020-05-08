using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraResizer : MonoBehaviour
{
    private const float defaultSize = 6.4f;
    private const float defaultAspectRatio = 16f / 9f;

    private void Awake()
    {
        Camera camera = GetComponent<Camera>();

        var aspectRatio = (float)Screen.height / Screen.width;

        if (camera.orthographic)
        {
            //직교 투영
            var orthoSize = defaultSize * aspectRatio / defaultAspectRatio;
            camera.orthographicSize = orthoSize;
        }
        else
        {
            //원근 투영
            float orgFOV = camera.fieldOfView;
            var orthoSize = orgFOV * aspectRatio / defaultAspectRatio;
            camera.fieldOfView = orthoSize;
        }
    }
}
