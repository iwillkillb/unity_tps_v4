using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotMovement : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform trackingTarget;

    [Header("Animator")]
    public Animator animator;
    public string apMove = "move";
    public string apDirection = "direction";
    public string apIsGrounded = "isGrounded";

    // Start is called before the first frame update
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.SetDestination(trackingTarget.position);

        SetAnimationParameter(0f, navMeshAgent.velocity.magnitude / navMeshAgent.speed, false);
    }

    void SetAnimationParameter(float axisHor, float axisVer, bool isStaringFront)
    {
        if (animator == null)
        {
            return;
        }

        if (isStaringFront)
        {
            animator.SetFloat(apMove, axisVer);
            animator.SetFloat(apDirection, axisHor);
        }
        else
        {
            //animator.SetFloat("move", Mathf.Max(Mathf.Abs(inputAxisHor), Mathf.Abs(inputAxisVer)));
            animator.SetFloat(apMove, Mathf.Max(Mathf.Abs(axisHor), Mathf.Abs(axisVer)), 0.1f, Time.deltaTime);
        }

        //animator.SetBool(apIsGrounded, characterController.isGrounded);
    }
}
