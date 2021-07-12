using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIsometricView : MonoBehaviour
{
    // Axis
    Vector3 inputAxis;
    Vector2 jointRotation;
    float zoomAxis;

    [Header("Other Transforms Connection")]
    public Transform target;
    public Transform joint;
    public Transform cam;
    public Transform zoomNearest;               // Camera's nearest position
    public Transform zoomFarest;                // Camera's farest position

    [Header("Camera Movement")]
    [Range(-90f, 0f)] public float VerticalrotationMin = -40f;
    [Range(0f, 90f)] public float VerticalrotationMax = 40f;
    [Range(1f, 10f)] public float camSpeedRotation = 5f;
    [Range(1f, 10f)] public float camSpeedZoom = 5f;



    void Update()
    {
        // Input
        inputAxis.x = Input.GetAxis("Horizontal");
        inputAxis.y = Input.GetAxis("Vertical");
        inputAxis.z = Input.GetAxis("Mouse ScrollWheel");

        // Rotation
        jointRotation.y -= inputAxis.x * camSpeedRotation;
        jointRotation.x += inputAxis.y * camSpeedRotation;
        jointRotation.x = Mathf.Clamp(jointRotation.x, VerticalrotationMin, VerticalrotationMax);

        // Zoom
        zoomAxis = Mathf.Clamp(zoomAxis + inputAxis.z, 0f, 1f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Follow Target
        transform.position = target.position;

        // Rotation
        joint.localRotation = Quaternion.Euler(jointRotation.x, jointRotation.y, 0f);

        // Zoom
        cam.localPosition = Vector3.Lerp(zoomFarest.localPosition, zoomNearest.localPosition, zoomAxis);
        cam.localRotation = Quaternion.Lerp(zoomFarest.localRotation, zoomNearest.localRotation, zoomAxis);
        /*
        cam.localPosition = Vector3.Lerp(cam.localPosition, zoomGoalPosition, Time.smoothDeltaTime * camSpeedZoom);
        cam.localRotation = Quaternion.Lerp(cam.localRotation, zoomGoalRotation, Time.smoothDeltaTime * camSpeedZoom);
        */
    }
}
