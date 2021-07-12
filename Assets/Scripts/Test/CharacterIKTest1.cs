using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIKTest1 : MonoBehaviour
{
    // This script requires the animator parameters "LeftFootCurve" and "RightFootCurve".
    // Also, the values of these two parameters must match the position of the foot in the walking or running animation.


    Animator animator;

    Vector3 leftFootPosition, rightFootPosition, leftFootIKPosition, rightFootIKPosition;
    Quaternion leftFootIKRotation, rightFootIKRotation;
    float lastPelvisPositionY, lastLeftFootPositionY, lastRightFootPositionY;
    [Range(0f, 2f)] public float heightFromGroundRaycast = 1.14f;
    [Range(0f, 2f)] public float raycastDownDistance = 1.5f;
    public LayerMask terrainLayer;
    public float pelvisOffset = 0f;
    [Range(0f, 1f)] public float pelvisUpAndDownSpeed = 0.28f;
    [Range(0f, 1f)] public float feetToIKPositionSpeed = 0.5f;

    [Header("Animator Parameters")]
    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    [Header("Other Settings")]
    public bool useProIKFeature = false;    // Adjust the angle of your feet to the terrain.
    public bool showSolverDebug = true;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // We are updating the AdjustFeetTarget Method and also find the position of each foot inside our solver position.
    private void FixedUpdate()
    {
        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        // Find and raycast to the ground to find positions.
        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);    // Handle the solver for right foot.
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);       // Handle the solver for left foot.
    }

    private void OnAnimatorIK(int layerIndex)
    {
        MovePelvisHeight();

        // Right foot IK position and rotation - Utilise the pro features in here
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat(rightFootAnimVariableName));
        }
        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);


        // Left foot IK position and rotation - Utilise the pro features in here
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat(leftFootAnimVariableName));
        }
        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
    }

    void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = animator.GetIKPosition(foot);

        if (positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
            targetIKPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIKPosition = transform.TransformPoint(targetIKPosition);

            animator.SetIKRotation(foot, rotationIKHolder);
        }
        animator.SetIKPosition(foot, targetIKPosition);
    }

    void MovePelvisHeight()
    {
        if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0f)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIKPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIKPosition.y - transform.position.y;
        float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;
        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;
        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

        // Setting pelvis position
        animator.bodyPosition = newPelvisPosition;

        // Backup pelvis position
        lastPelvisPositionY = animator.bodyPosition.y;
    }

    // We are locating the feet position via a raycast and then solving.
    void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        // Raycast handling selection
        RaycastHit feetOutHit;

        if (showSolverDebug)
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, terrainLayer))
        {
            // Finding our feet ik positions from the sky position
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = feetOutHit.point.y + pelvisOffset;
            feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }
        feetIKPositions = Vector3.zero; // It didn't work.
    }

    void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;
    }
}
