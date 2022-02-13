using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class PlayerMovement : PlayerBehaviour
{
    // Components
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    [Header("Movement")]
    public bool usingRootMotion = false;
    public float aerialSpeed = 10f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 5f;
    float verticalVelocity;
    bool isRunning = false;

    [Header("Terrain Check")]
    public Transform groundCheckPosition;
    public Transform ceilingCheckPosition;
    public float terrainCheckRadius = 0.5f;
    public LayerMask terrainLayer;
    bool isGrounded;
    bool isCeiling;



    // Start is called before the first frame update
    void Start()
    {
        // Components connecting
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckPosition.position, terrainCheckRadius, terrainLayer);
        isCeiling = Physics.CheckSphere(ceilingCheckPosition.position, terrainCheckRadius, terrainLayer);
        
        // Ground Check
        if (isGrounded) {
            verticalVelocity = 0f;
            
            // Slope force
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }

        // Ceiling Check
        if (verticalVelocity > 0f && isCeiling)
        {
            verticalVelocity = 0f;
        }

        // Animation
        if (animator != null)
            SetAnimatorParameters();

        // Locomotion
        Rotation();
        if (!usingRootMotion)
        {
            // If player uses Root motion, character is moved by animation, not character controller.
            Movement();
        }

        // Jump
        if (playerInput.inputJump && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Vertical velocity
        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    // Root motion movement
    private void OnAnimatorMove()
    {
        // Movement by Root motion
        if (animator == null || !usingRootMotion)
            return;

        if (isGrounded)
        {
            // Root motion movement
            characterController.Move(animator.deltaPosition);
            // ----------------------------------------------------------------------------------------------------------------
            // Bug : In FPS mode, the character moves forward even though the player has entered to move sideways or backwards.
            //       I need Side moving animation.
            // ----------------------------------------------------------------------------------------------------------------
        }
        else
        {
            Vector3 inputDirection = Vector3.right * playerInput.inputH + Vector3.forward * playerInput.inputV;

            switch (CameraController.instance.cameraMode)
            {
                case CameraController.CameraMode.FPS:
                    inputDirection = transform.rotation * inputDirection;
                    // XZ Movement by Ground's normal angle
                    characterController.Move(inputDirection * aerialSpeed * Time.deltaTime);
                    break;

                case CameraController.CameraMode.TPS:
                    // Get Camera's y rotation.
                    Quaternion moveDir = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
                    characterController.Move(moveDir * inputDirection * aerialSpeed * Time.deltaTime);
                    break;
            }
        }
    }

    void SetAnimatorParameters()
    {
        // Check running
        if (playerInput.inputDoubleH || playerInput.inputDoubleV)
            isRunning = true;
        if (playerInput.inputH == 0f && playerInput.inputV == 0f)
            isRunning = false;

        float locomotionParameter = Mathf.Max(Mathf.Abs(playerInput.inputH), Mathf.Abs(playerInput.inputV));

        // Walk : Half of locomotion parameter
        if (!isRunning)
            locomotionParameter *= 0.5f;

        animator.SetFloat("v", locomotionParameter, 0.2f, Time.deltaTime);

        animator.SetBool("isGrounded", isGrounded);
    }

    void Rotation()
    {
        switch (CameraController.instance.cameraMode)
        {
            case CameraController.CameraMode.FPS:
                // Rotation with camera
                transform.Rotate(Vector3.up * playerInput.mouseX * rotationSpeed);
                break;

            case CameraController.CameraMode.TPS:
                if (playerInput.inputH != 0f || playerInput.inputV != 0f)
                {
                    // inputAngle : Front 0, Back 180, Left -90, Right 90
                    float inputAngle = Mathf.Atan2(playerInput.inputH, playerInput.inputV) * Mathf.Rad2Deg;
                    Quaternion newRot = Quaternion.Euler(Vector3.up * (inputAngle + Camera.main.transform.eulerAngles.y));
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
                }
                break;
        }
    }

    void Movement()
    {
        Vector3 inputDirection = Vector3.right * playerInput.inputH + Vector3.forward * playerInput.inputV;
        Vector3 groundNormal = isGrounded ? GetGroundNormal() : Vector3.up;

        switch (CameraController.instance.cameraMode)
        {
            case CameraController.CameraMode.FPS:
                inputDirection = transform.rotation * inputDirection;
                // XZ Movement by Ground's normal angle
                characterController.Move(Quaternion.FromToRotation(Vector3.up, groundNormal) * inputDirection * aerialSpeed * Time.deltaTime);
                break;

            case CameraController.CameraMode.TPS:
                // Get Camera's y rotation.
                Quaternion moveDir = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
                characterController.Move(Quaternion.FromToRotation(Vector3.up, groundNormal) * moveDir * inputDirection * aerialSpeed * Time.deltaTime);
                break;
        }
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
