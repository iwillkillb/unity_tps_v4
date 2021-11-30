using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singletone
    public static CameraController instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CameraController found.");
            return;
        }

        instance = this;
    }
    #endregion

    public enum CameraMode { FPS, TPS, DeadCam };
    public bool usingRootMotion = false;

    [Header("Main Setting")]
    public CameraMode cameraMode = CameraMode.TPS;
    CameraMode cameraModeBackup;
    public LayerMask terrainLayer;
    public float rotationSpeed = 5f;
    float camXRotation;
    float camYRotation;

    [Header("Target")]
    [Tooltip("target : Player object\ntargetAfterDied : Player's ragdoll")]
    public Transform target;
    public Transform targetAfterDied;

    [Header("FPS")]
    public Transform fpsCamPosition;

    [Header("TPS")]
    public Vector3 tpsCamOffset = new Vector3(1f, 1f, -2f);
    [Range(-90f, 0f)] public float camMinXAxis = -45f;           // X Axis has limit.
    [Range(0f, 90f)] public float camMaxXAxis = 75f;

    [Header("Dead Cam")]
    public Vector3 deadCamOffset = new Vector3(0f, 2f, 0f);



    private void Start()
    {
        // Cursor in screen -> Hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

        PlayerManager.instance.onPlayerDied += OnPlayerDied;
        PlayerManager.instance.onPlayerRevived += OnPlayerRevived;
    }

    void OnPlayerDied()
    {
        // Camera Mode Initialization
        cameraModeBackup = cameraMode;

        // Change Camera Mode
        cameraMode = CameraMode.DeadCam;
    }

    void OnPlayerRevived()
    {
        // Revive Camera Mode
        cameraMode = cameraModeBackup;
    }



    private void Update()
    {
        // Mouse input
        camYRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        camXRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
        // 국내에서는 다음과 같은 방법을 사용하지만, 해외에서는 -대신 +를 해서 반전시키는 것을 많이 사용한다.
    }

    private void LateUpdate()
    {
        switch (cameraMode)
        {
            case CameraMode.FPS:
                // Limit TPS Camera's X Rotation
                camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);

                transform.position = fpsCamPosition.position;
                transform.rotation = target.rotation * Quaternion.Euler(camXRotation, 0f, 0f);
                break;

            case CameraMode.TPS:
                // Limit TPS Camera's X Rotation
                camXRotation = Mathf.Clamp(camXRotation, camMinXAxis, camMaxXAxis);

                // Rotation by input
                Quaternion camRot = Quaternion.Euler(camXRotation, camYRotation, 0f);

                // Position = tpsCamOffset.y + (camRotation * tpsCamOffset.xz)
                Vector3 camPivot = target.position + Vector3.up * tpsCamOffset.y;
                Vector3 camPos = camPivot + camRot * (Vector3.right * tpsCamOffset.x + Vector3.forward * tpsCamOffset.z);

                // Zoom by Collider
                float zoomRatio = 1f;
                float distanceToCam = Vector3.Distance(camPos, camPivot);    // Distance to Camera Position
                RaycastHit hit;
                if (Physics.SphereCast(camPivot, 0.1f, camPos - camPivot, out hit, distanceToCam, terrainLayer))
                {
                    // Lesser Zoom Axis -> Near
                    zoomRatio = hit.distance / distanceToCam;
                }

                // Setting
                transform.position = Vector3.Lerp(camPivot, camPos, zoomRatio);
                transform.rotation = camRot;
                break;

            case CameraMode.DeadCam:
                transform.position = targetAfterDied.position + deadCamOffset;
                transform.LookAt(targetAfterDied);
                break;
        }
    }
}
