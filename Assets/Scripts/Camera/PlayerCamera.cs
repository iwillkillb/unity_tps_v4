using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour
{
    #region Singletone
    public static PlayerCamera instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerCamera found.");
            return;
        }

        instance = this;
    }
    #endregion

    // How to Use?
    // 1. Make this hierarchy
    //  Component's object
    //      > Y Axis (0, 0, 0) All Zero Position
    //          > X Axis (0, 0, 0) All Zero Position
    //              > Main Camera
    //              > Zoom out limit (x, y, z) Free Position
    // 2. Set target and other public objects.

    [Header("Transform Connection")]
    public Transform cam;               // Camera's moving position by zooming
    public Transform FarestPoint;       // Camera's farest position
    public Transform NearestPoint;      // Camera's nearest position

    [Header("Terrain Check")]
    public LayerMask terrainLayerMask;  // Don't put target's layer in terrainLayerMask!!
    RaycastHit hit;

    // Input
    float inputAxisX;
    float inputAxisY;
    float inputAxisZ;

    [Header("Position")]
    [Tooltip("If target is Null, it is not tracked.")]
    public Transform target;
    public Transform targetAfterDied;
    Transform currentTarget;

    // Camera's rotation sequence
    // Angle -> Quaternion -> Transform
    [Header("Rotation")]
    public float rotationSpeed = 5f;
    [Range(-90f, 0f)] public float minXAxis = -45f;           // X Axis has limit.
    [Range(0f, 90f)] public float maxXAxis = 75f;
    float angleXAxis;
    float angleYAxis;

    [Header("Zoom")]
    public float radiusFromCollision = 0.1f;
    float zoomAxis = 1f;                    // Default zoom axis, Nearest : 0



    private void Start()
    {
        // Hide mouse cousor on game screen.
        Cursor.lockState = CursorLockMode.Locked;

        currentTarget = target;

        PlayerManager.instance.onPlayerDied += OnPlayerDied;
        PlayerManager.instance.onPlayerRevived += OnPlayerRevived;
    }

    void OnPlayerDied()
    {
        currentTarget = targetAfterDied;
    }

    void OnPlayerRevived()
    {
        currentTarget = target;
    }

    private void Update()
    {
        /*
        if (EventSystem.current.IsPointerOverGameObject())
        {
            inputAxisX = 0;
            inputAxisY = 0;
            inputAxisZ = 0;
            return;
        }*/

        // Take input
        inputAxisX = Input.GetAxis("Mouse X");
        inputAxisY = Input.GetAxis("Mouse Y");
        inputAxisZ = Input.GetAxis("Mouse ScrollWheel");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Shift Key -> Camera Stop
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }

        // Positioning
        if (currentTarget != null)
            transform.position = currentTarget.position;

        // Rotation
        angleXAxis -= inputAxisY * rotationSpeed;
        angleXAxis = Mathf.Clamp(angleXAxis, minXAxis, maxXAxis);
        angleYAxis += inputAxisX * rotationSpeed;
        transform.rotation = Quaternion.Euler(angleXAxis, angleYAxis, 0f);

        // Zooming
        zoomAxis -= inputAxisZ;
        Vector3 camPosByZoomAxis = Vector3.Lerp(NearestPoint.position, FarestPoint.position, zoomAxis);
        float distanceToCam = Vector3.Distance(transform.position, camPosByZoomAxis);    // Distance to Camera Position

        // Collision Check
        float zoomAxisByDistanceToCollider = 1f;
        if (Physics.SphereCast(transform.position, radiusFromCollision, camPosByZoomAxis - transform.position, out hit, distanceToCam, terrainLayerMask))
        {
            // Lesser Zoom Axis -> Near
            zoomAxisByDistanceToCollider = hit.distance / distanceToCam;
        }
        cam.position = Vector3.Lerp(transform.position, camPosByZoomAxis, zoomAxisByDistanceToCollider);
    }
}
