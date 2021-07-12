using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : MonoBehaviour
{
    public float maxGrabDistance = 2f;
    public float throwForce = 10f;
    public float lerpSpeed = 10f;
    public Transform holdPoint;

    Rigidbody grabbedRigidbody;
    Transform grabbedParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Holding
        if (grabbedRigidbody)
        {
            grabbedRigidbody.velocity = Vector3.zero;
            grabbedRigidbody.angularVelocity = Vector3.zero;
            grabbedRigidbody.position = Vector3.Lerp(grabbedRigidbody.position, holdPoint.position, Time.smoothDeltaTime * lerpSpeed);
            grabbedRigidbody.rotation = Quaternion.Lerp(grabbedRigidbody.rotation, holdPoint.rotation, Time.smoothDeltaTime * lerpSpeed);
            //grabbedRigidbody.MovePosition(holdPoint.position);
            //grabbedRigidbody.position = holdPoint.position;
            //grabbedRigidbody.rotation = holdPoint.rotation;
            //grabbedRigidbody.MovePosition(Vector3.Lerp(grabbedRigidbody.position, holdPoint.position, Time.smoothDeltaTime * lerpSpeed));
            //grabbedRigidbody.MoveRotation(Quaternion.Lerp(grabbedRigidbody.rotation, holdPoint.rotation, Time.smoothDeltaTime * lerpSpeed));

            // When the distance from the object increases while holding the object, the object is released.
            if (Vector3.Distance(grabbedRigidbody.position, holdPoint.position) > maxGrabDistance)
            {
                Release();
            }
        }

        // Grab
        if (Input.GetMouseButtonDown(1))
        {
            Grab();
        }
        // Release
        if (Input.GetMouseButtonUp(1))
        {
            Release();
        }
    }

    void Grab()
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitMouse;

        if (Physics.Raycast(rayMouse, out hitMouse, maxGrabDistance))
        {
            // Connect
            grabbedRigidbody = hitMouse.collider.GetComponent<Rigidbody>();
            if (grabbedRigidbody)
            {
                grabbedRigidbody.useGravity = false;
                //grabbedRigidbody.isKinematic = true;
                
                // Parent Connect
                grabbedParent = grabbedRigidbody.transform.parent;
                grabbedRigidbody.transform.parent = holdPoint;
                
            }
        }
    }

    void Release()
    {
        if (grabbedRigidbody == null)
            return;

        grabbedRigidbody.useGravity = true;
        //grabbedRigidbody.isKinematic = false;
        
        // Parent Disconnect
        grabbedRigidbody.transform.parent = grabbedParent;
        
        // Throw
        grabbedRigidbody.AddForce(Camera.main.transform.forward * throwForce, ForceMode.VelocityChange);

        // Disconnect
        grabbedRigidbody = null;
    }
}
