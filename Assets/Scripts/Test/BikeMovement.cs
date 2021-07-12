using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BikeMovement : MonoBehaviour
{
    Rigidbody _Rigidbody;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    bool isGrounded = false;

    [Header("Slope Check")]
    public Transform slopeCheckPoint;
    public float slopeCheckRange = 5f;
    public float slopeForce = 1f;

    [Header("Rotation")]
    public float rotationSlerp = 5f;
    public bool isStaringFront = false;

    [Header("Movement")]
    public float maxSpeed = 50f;
    public float acceleration = 50f;
    public float jumpForce = 50f;





    // Input field
    public float inputAxisHor { get; set; }
    public float inputAxisVer { get; set; }
    public bool inputJump { get; set; }






    // Start is called before the first frame update
    void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Input
        inputAxisHor = Input.GetAxis("Horizontal");
        inputAxisVer = Input.GetAxis("Vertical");
        inputJump = Input.GetButtonDown("Jump");

        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        /*
        // Slope Force -> Moving on slope without bouncing.
        if (!inputJump && isGrounded)
        {
            //_Rigidbody.AddForce(-GetGroundNormal() * slopeForce, ForceMode.VelocityChange);
            _Rigidbody.AddRelativeForce(Vector3.down * slopeForce, ForceMode.VelocityChange);
        }*/

        Rotation(inputAxisHor, inputAxisVer, isStaringFront);
        Movement(inputAxisHor, inputAxisVer);

        // Jump on ground
        if (inputJump && isGrounded)
        {
            _Rigidbody.AddRelativeForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }



    Vector3 GetGroundNormal()
    {
        // Slope Check
        RaycastHit slopeHit;
        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(slopeCheckPoint.position, -slopeCheckPoint.up, out slopeHit, slopeCheckRange, groundLayer))
        {
            groundNormal = slopeHit.normal;
        }

        return groundNormal;
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
        if (isStaringFront)
        {
            newRot = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
        }
        // No stare -> Rotate only moving
        else if (axisHor != 0f || axisVer != 0f)
        {
            newRot = Quaternion.Euler(Vector3.up * (inputAngle + Camera.main.transform.eulerAngles.y));
        }

        // Actual Rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, GetGroundNormal()) * newRot, rotationSlerp * Time.deltaTime);
    }

    void Movement(float axisHor, float axisVer)
    {
        // Use Input data
        Vector3 moveAxis = Vector3.right * axisHor + Vector3.forward * axisVer;

        // Get Camera's y rotation.
        Quaternion moveDir = Quaternion.FromToRotation(transform.up, GetGroundNormal()) * Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);

        // Actual Moving
        if (_Rigidbody.velocity.magnitude < maxSpeed && isGrounded)
        {
            _Rigidbody.AddForce((moveDir * moveAxis) * acceleration, ForceMode.Acceleration);
        }
    }
}
