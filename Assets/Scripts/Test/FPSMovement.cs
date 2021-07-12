using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
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
    public float mouseSensitivity = 300f;
    float camXRotation;


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

        // Rotation by camera
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity * Time.smoothDeltaTime);
        
        // XZ Movement by Ground's normal angle
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        Vector3 groundNormal = isGrounded ? GetGroundNormal() : Vector3.up;
        characterController.Move(Quaternion.FromToRotation(Vector3.up, groundNormal) * move * speed * Time.deltaTime);

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
        camXRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.smoothDeltaTime;
        camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(camXRotation, 0f, 0f);
        //transform.position = target.position;
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
