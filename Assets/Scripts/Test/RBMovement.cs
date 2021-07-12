using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBMovement : MonoBehaviour
{
    new Rigidbody rigidbody;
    Animator animator;

    public float moveSpeed = 5f;
    public float slopeForce = 5f;
    public float rotateSpeed = 5f;
    public float jumpPower = 5f;
    float inputH, inputV;
    bool inputJump;

    [Header("Lean")]
    public float leanSpeed = 1f;
    Quaternion currentLean;
    Quaternion goalLean;

    [Header("Terrain Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;
    public Transform slopeCheckPoint;
    public float slopeCheckDistance = 1f;
    public Transform wallCheckPoint;
    public float wallCheckRadius = 0.5f;
    public LayerMask terrainLayer;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, terrainLayer);
        //animator.SetBool("isGrounded", isGrounded);

        // Input
        inputH = Input.GetAxis("Horizontal");
        inputV = WallCheck() ? 0f : Input.GetAxis("Vertical");
        inputJump = Input.GetKeyDown(KeyCode.Space);

        // Slope Force
        Vector3 groundNormal = GetGroundNormal();
        /*
        if (!inputJump && !isGrounded)
        {
            rigidbody.AddForce(-groundNormal * slopeForce, ForceMode.VelocityChange);
        }*/

        // Lean
        currentLean = transform.rotation;
        goalLean = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(currentLean, goalLean, leanSpeed * Time.deltaTime);

        // Curve
        if (inputH != 0f)
        {
            transform.Rotate(Vector3.up * inputH * 90f * Time.deltaTime);
        }

        // Move
        transform.Translate(Vector3.forward * inputV * moveSpeed * Time.deltaTime);

        // Jump
        if (inputJump && isGrounded)
        {
            rigidbody.AddRelativeForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }

    }
    Vector3 GetGroundNormal()
    {
        // Slope Check
        RaycastHit slopeHit;
        Vector3 groundNormal = Vector3.up;

        if (Physics.Raycast(slopeCheckPoint.position, -slopeCheckPoint.up, out slopeHit, slopeCheckDistance, terrainLayer))
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
    /*
    // Collision detection.
    private void OnCollisionStay(Collision collision)
    {
        GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
        GetComponent<CapsuleCollider>().material.staticFriction = 0f;
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
        GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
    }*/
}
