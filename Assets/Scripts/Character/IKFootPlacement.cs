using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    // Components
    Animator animator;

    [Range(0f, 1f)]public float distanceToGround = 0.1f;
    public LayerMask terrainLayer;


    private void Awake()
    {
        // Components connecting
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // Weight setting
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

        // Left foot
        Ray lfRay = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        RaycastHit lfHit;
        if(Physics.Raycast(lfRay, out lfHit, distanceToGround + 1f, terrainLayer))
        {
            Vector3 footPosition = lfHit.point;
            footPosition.y += distanceToGround;
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, lfHit.normal));
        }

        // Right foot
        Ray rfRay = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        RaycastHit rfHit;
        if (Physics.Raycast(rfRay, out rfHit, distanceToGround + 1f, terrainLayer))
        {
            Vector3 footPosition = rfHit.point;
            footPosition.y += distanceToGround;
            animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rfHit.normal));
        }
        
    }
}
