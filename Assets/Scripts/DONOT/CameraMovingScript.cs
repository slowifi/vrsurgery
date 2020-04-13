using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovingScript : MonoBehaviour
{
    /// <summary>
    /// Camera Movement Script that rotates aroud the origin using a spherical coordinate
    /// </summary>
    // Start is called before the first frame update

    private Vector3 curCameraPosition;
    private Vector3 curCameraRotation;
    private float r, theta, phi;
    private const float rotationSpeed = 2f;

    public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
    {
        float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = a * Mathf.Sin(polar);
    }


    public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation)
    {
        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                        + (cartCoords.y * cartCoords.y)
                        + (cartCoords.z * cartCoords.z));
        outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
            outPolar += Mathf.PI;
        outElevation = Mathf.Asin(cartCoords.y / outRadius);
    }

    void Start()
    {
        curCameraPosition = new Vector3(0, 0, 150);
        curCameraRotation = new Vector3(0, 180, 0);
        CartesianToSpherical(curCameraPosition, out r, out theta, out phi);
        transform.LookAt(Vector3.zero);
    }



    // Update is called once per frame
    void Update()
    {        
        /*
        if (Input.touchCount == 3)
        {
            float zoomConstant = 1f;
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Touch touchTwo = Input.GetTouch(2);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;


            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = Mathf.Max((touchZeroPrevPos - touchOnePrevPos).magnitude, (touchZeroPrevPos - touchTwoPrevPos).magnitude, (touchTwoPrevPos - touchOnePrevPos).magnitude);
            float touchDeltaMag = Mathf.Max((touchZero.position - touchOne.position).magnitude, (touchZero.position - touchTwo.position).magnitude, (touchTwo.position - touchOne.position).magnitude);

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            zoomConstant = 1f + deltaMagnitudeDiff / 1000f;
            r *= zoomConstant;

            if (r >= 1000f)
                r = 1000f;
            else if (r <= 50f)
                r = 50f;

            theta += Input.GetTouch(0).deltaPosition.x * rotationSpeed / 1000f;
            phi += Input.GetTouch(0).deltaPosition.y * rotationSpeed / 1000f;

            SphericalToCartesian(r, theta, phi, out curCameraPosition);
            transform.position = curCameraPosition;
            transform.LookAt(Vector3.zero);
        }

    */
    }
}
