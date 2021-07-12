using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIKTest : MonoBehaviour
{
    Animator animator;

    [Range(0f, 1f)]public float ikWeight = 1f;
    float leftFootWeight;
    float rightFootWeight;

    public LayerMask terrainLayer;
    public float footRange = 0.07f;
    public Vector3 footOffset;

    public Transform lookTarget;
    public Transform leftHandObj;
    public Transform rightHandObj;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {

        // Setting LookAt Target to Camera
        if (CameraController.instance.cameraMode == CameraController.CameraMode.FPS)
        {
            // Set the look target position, if one has been assigned
            animator.SetLookAtWeight(ikWeight);
            animator.SetLookAtPosition(Camera.main.transform.forward * 100f);
        }
        else
        {
            animator.SetLookAtWeight(0f);
        }

        if (leftHandObj != null)
        {
            // Set the left hand target position and rotation, if one has been assigned
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);

            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);

        }

        if (rightHandObj != null)
        {
            // Set the right hand target position and rotation, if one has been assigned
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);

            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
        }

        // Left Foot
        RaycastHit leftFootHit;
        Vector3 leftFootPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        if (Physics.Raycast(leftFootPos + Vector3.up, Vector3.down, out leftFootHit, 1f + footRange * 2f, terrainLayer)) //Throw raycast to down
        {
            // Weight Setting
            if(leftFootHit.distance < 1f + footRange)
            {
                leftFootWeight = ikWeight;
            }
            else
            {
                leftFootWeight = leftFootHit.distance / (1f + footRange * 2f);
            }

            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootHit.point + footOffset);
            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, leftFootHit.normal), leftFootHit.normal);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, footRotation);
        }
        else
        {
            //Raycast does not hit anything, so we keep original position and rotation
            leftFootWeight = 0f;
        }
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        //SetFootWeight();


        // Right Foot
        RaycastHit rightFootHit;
        Vector3 rightFootPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        if (Physics.Raycast(rightFootPos + Vector3.up, Vector3.down, out rightFootHit, 1f + footRange * 2f, terrainLayer)) //Throw raycast to down
        {
            // Weight Setting
            if (rightFootHit.distance < 1f + footRange)
            {
                rightFootWeight = ikWeight;
            }
            else
            {
                rightFootWeight = rightFootHit.distance / (1f + footRange * 2f);
            }

            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootHit.point + footOffset);
            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, rightFootHit.normal), rightFootHit.normal);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, footRotation);
        }
        else
        {
            //Raycast does not hit anything, so we keep original position and rotation
            rightFootWeight = 0f;
        }
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
    }
}
