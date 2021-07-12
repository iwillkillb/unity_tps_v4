using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSMovement : MonoBehaviour
{
    CharacterController characterController;

    [Header("Movement")]
    public float speed = 10f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    float verticalVelocity;

    [Header("Terrain Check")]
    public Transform groundCheckPoint;
    public Transform ceilingCheckPoint;
    public float terrainCheckRadius = 0.5f;
    public LayerMask terrainLayer;
    bool isGrounded;
    bool isCeiling;

    [Header("Camera")]
    public Transform cam;
    public Vector3 camOffset = new Vector3(1f, 1f, -2f);
    public float mouseSensitivity = 300f;

    // Input
    float mouseX;
    float mouseY;

    public float rotationSpeed = 5f;
    [Range(-90f, 0f)] public float minXAxis = -45f;           // X Axis has limit.
    [Range(0f, 90f)] public float maxXAxis = 75f;
    float angleXAxis;
    float angleYAxis;






    // Start is called before the first frame update
    void Start()
    {
        // Cursor in screen -> Hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, terrainCheckRadius, terrainLayer);
        isCeiling = Physics.CheckSphere(ceilingCheckPoint.position, terrainCheckRadius, terrainLayer);

        // Slope force
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Ceiling Check
        if (verticalVelocity > 0f && isCeiling)
        {
            verticalVelocity = 0f;
        }

        // Movement
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        // Get Camera's y rotation.
        Quaternion moveDir = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
        Vector3 move = Vector3.right * inputX + Vector3.forward * inputZ;
        Vector3 groundNormal = isGrounded ? GetGroundNormal() : Vector3.up;
        characterController.Move(moveDir * Quaternion.FromToRotation(Vector3.up, groundNormal) * move * speed * Time.deltaTime);

        // Rotation
        if (inputX != 0f || inputZ != 0f)
        {
            // inputAngle : Front 0, Back 180, Left -90, Right 90
            float inputAngle = Mathf.Atan2(inputX, inputZ) * Mathf.Rad2Deg;
            Quaternion newRot = Quaternion.Euler(Vector3.up * (inputAngle + Camera.main.transform.eulerAngles.y));
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Vertical velocity
        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
    
    private void LateUpdate()
    {
        // Camera

        // Take input
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        // Camera Rotation
        angleXAxis -= mouseY * rotationSpeed;
        angleXAxis = Mathf.Clamp(angleXAxis, minXAxis, maxXAxis);
        angleYAxis += mouseX * rotationSpeed;
        Quaternion camRot = Quaternion.Euler(angleXAxis, angleYAxis, 0f);

        // Camera Position
        Vector3 camPivot = transform.position + Vector3.up * camOffset.y;
        Vector3 camPos = transform.position + camRot * camOffset;

        // Camera Zoom by Collider
        float zoomRatio = 1f;
        float distanceToCam = Vector3.Distance(camPos, camPivot);    // Distance to Camera Position
        RaycastHit hit;
        if (Physics.SphereCast(camPivot, 0.1f, camPos - camPivot, out hit, distanceToCam, terrainLayer))
        {
            // Lesser Zoom Axis -> Near
            zoomRatio = hit.distance / distanceToCam;
        }

        // Setting
        cam.position = Vector3.Lerp(camPivot, camPos, zoomRatio);
        cam.rotation = camRot;
    }

    Vector3 GetGroundNormal()
    {
        // Slope Check
        RaycastHit slopeHit;
        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(transform.position, -transform.up, out slopeHit, 1f))
        {
            groundNormal = slopeHit.normal;
        }

        return groundNormal;
    }
}
