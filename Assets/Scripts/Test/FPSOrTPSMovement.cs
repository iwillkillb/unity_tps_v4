using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSOrTPSMovement : MonoBehaviour
{
    CharacterController characterController;

    [Header("Movement")]
    public float speed = 3.5f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    public float slopeForce = -2f;
    float verticalVelocity;

    [Header("Terrain Check")]
    public Transform groundCheckPoint;
    public Transform ceilingCheckPoint;
    public float terrainCheckRadius;
    public LayerMask terrainLayer;
    bool isGrounded;
    bool isCeiling;

    [Header("Camera")]
    public Transform fpsCam;
    public Transform tpsCam;
    public enum CameraMode {FPS, TPS};
    public CameraMode cameraMode = CameraMode.FPS;
    public float mouseSensitivity = 150f;
    float camXRotation;
    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;


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
            verticalVelocity = slopeForce;
        }

        // Ceiling Check
        if (verticalVelocity > 0f && isCeiling)
        {
            verticalVelocity = -verticalVelocity;
        }

        // Input
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Locomotion
        if (cameraMode == CameraMode.FPS)
        {
            // Camera rotation
            camXRotation -= mouseY * mouseSensitivity * Time.deltaTime;
            camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);
            fpsCam.localRotation = Quaternion.Euler(camXRotation, 0f, 0f);

            // Character rotation by camera
            transform.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);

            // XZ Movement
            characterController.Move((transform.right * inputX + transform.forward * inputZ) * speed * Time.deltaTime);
        }
        else if (cameraMode == CameraMode.TPS)
        {
            Vector3 direction = new Vector3(inputX, 0f, inputZ);
            if (direction.magnitude > 0f)
            {
                // Rotation
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + tpsCam.eulerAngles.y;

                Quaternion newRotation = Quaternion.Euler(Vector3.up * targetAngle);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5f * Time.deltaTime);

                //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Movement
                characterController.Move(Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * speed * Time.deltaTime);
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * slopeForce * gravity);
        }

        // Vertical velocity
        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
