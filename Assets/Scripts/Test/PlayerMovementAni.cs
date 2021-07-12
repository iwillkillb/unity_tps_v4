using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAni : PlayerBehaviour
{
    // Components
    Animator animator;
    PlayerInput playerInput;
    CapsuleCollider capsuleCollider;
    new Rigidbody rigidbody;

    [Header("Terrain Check")]
    public LayerMask terrainLayer;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;
    bool isGrounded;

    public float slopeCheckDistance = 0.1f;

    public Transform wallCheckPoint;
    public float wallCheckRadius = 0.5f;

    [Header("Move")]
    public bool isStaringFront = false;
    [Range(0f, 1f)] public float runAxis = 0f;
    public float slopeForce = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 2f;
    float rotationAngleDifference = 0f;

    [Header("Jump")]
    public float jumpHeight = 5f;
    public float jumpDistance = 2f;

    // Animator Hash
    int hash_locomotion = Animator.StringToHash("Base Layer.Locomotion");
    int hash_jumpStart = Animator.StringToHash("Base Layer.Jump Start");
    int hash_Fall = Animator.StringToHash("Base Layer.Fall");



    void Awake()
    {
        // Components connecting
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, terrainLayer);
        animator.SetBool("isGrounded", isGrounded);

        Movement(playerInput.inputH, playerInput.inputV, isStaringFront);
        Rotation(playerInput.inputH, playerInput.inputV, isStaringFront);
    }

    Vector3 GetGroundNormal()
    {
        // Slope Check
        RaycastHit slopeHit;
        Vector3 groundNormal = Vector3.up;

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeCheckDistance))
        {
            groundNormal = slopeHit.normal;
        }

        return groundNormal;
    }

    bool WallCheck()
    {
        if (Physics.CheckSphere(wallCheckPoint.position, wallCheckRadius, terrainLayer))
            return true;

        return false;
    }

    void Movement(float axisHor, float axisVer, bool isStaringFront)
    {
        runAxis += Input.GetAxis("Mouse ScrollWheel");
        runAxis = Mathf.Clamp01(runAxis);

        float animParaHor = axisHor;
        float animParaVer = axisVer;

        // Movement
        if (!isStaringFront)
        {
            if (animator.GetFloat("h") != 0f)
                animParaHor = 0f;
            animParaVer = Mathf.Max(Mathf.Abs(axisHor), Mathf.Abs(axisVer)) * (runAxis + 1f);
        }

        // Set Parameter
        animator.SetFloat("h", animParaHor, 0.2f, Time.deltaTime);

        // Aerial Wall Check -> Zero Speed
        if (!isGrounded && WallCheck())
            animator.SetFloat("v", 0f);
        else
            animator.SetFloat("v", animParaVer, 0.2f, Time.deltaTime);

        // Jump : Move to upward, If you are on a slope, add slopeForce to jumpHeight.
        if (isGrounded && playerInput.inputJump)
        {
            animator.SetTrigger("jump");

            Vector3 jumpVelocity = rigidbody.velocity;
            jumpVelocity.y = jumpHeight;
            rigidbody.velocity = jumpVelocity;
            //rigidbody.AddRelativeForce(Vector3.up * (jumpHeight + currentSlopeForce), ForceMode.VelocityChange);
        }

        // Aerial : Move to forward
        if (!animator.GetBool("isGrounded"))
        {
            rigidbody.AddRelativeForce(Vector3.forward * jumpDistance * animator.GetFloat("v"), ForceMode.VelocityChange);
        }

        // Slope Force : Activates when moving on a descending slope
        Vector3 groundNormal = GetGroundNormal();
        if (groundNormal != Vector3.up && Vector3.Cross(transform.right, groundNormal).y < 0f)
        {
            rigidbody.AddRelativeForce(Vector3.down * slopeForce, ForceMode.VelocityChange);
        }
    }

    void Rotation(float axisHor, float axisVer, bool isStaringFront)
    {
        // Character Rotation : Character and camera move in the apposite direction (in Y axis).
        // Example : 
        // 1. Input Right key -> inputAngle is 90
        // 2. Character moves Right -> Character rotates (inputAngle + Camera's current angle)

        // inputAngle : Front 0, Back 180, Left -90, Right 90
        float inputAngle = Mathf.Atan2(axisHor, axisVer) * Mathf.Rad2Deg;

        // Rotation backup
        Quaternion newRot = transform.rotation;  // Set new direction rotation value.

        // Stare
        // True  : Character stares Camera's direction.
        // False : The character looks in the direction in which it moves.
        if (isStaringFront)
        {
            newRot = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
        }
        // No stare -> Rotate only moving
        else if (axisHor != 0f || axisVer != 0f)
        {
            newRot = Quaternion.Euler(Vector3.up * (inputAngle + Camera.main.transform.eulerAngles.y));

            // Get angle's difference between Current euler angle and Goal euler angle. (-180 ~ 180)
            rotationAngleDifference = inputAngle + Camera.main.transform.eulerAngles.y - transform.eulerAngles.y;
            if (rotationAngleDifference > 180f)
                rotationAngleDifference -= 360f;
            else if (rotationAngleDifference < -180f)
                rotationAngleDifference += 360f;
        }
        else
        {
            // Initialization in stoppint time
            if (rotationAngleDifference != 0f)
                rotationAngleDifference = 0f;
        }

        // Actual Rotation : Lean to the terrain + Rotate by direction of movement.
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
    }
}
