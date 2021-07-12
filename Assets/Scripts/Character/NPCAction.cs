using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAction : MonoBehaviour
{
    public float chasingDistance = 10f;
    public float stoppingDistance = 1f;
    Transform target;

    // Components
    Animator _Animator;
    NavMeshAgent _NavMeshAgent;
    CharacterStats _CharacterStats;

    // Movement
    Quaternion moveDir;
    Vector3 moveAxis;

    /*
    [Header("Staring mode")]
    public Transform trnStaringTarget;
    public float upperBodyAngle;
    */

    // Input field
    public float inputMoveAxisHor { get; set; }
    public float inputMoveAxisVer { get; set; }


    void Awake()
    {
        // Components connecting
        _Animator = GetComponent<Animator>();
        _NavMeshAgent = GetComponent<NavMeshAgent>();
        _CharacterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        target = PlayerManager.instance.player.transform;
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        // Chase
        if (distance <= chasingDistance)
        {
            _NavMeshAgent.SetDestination(target.position);

            // Stop moving and...
            if (distance <= stoppingDistance)
            {
                // ...Attack
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                if (targetStats != null)
                {
                    //_CharacterStats.Attack(targetStats);
                }
                // ...Face target
                FaceTarget();
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chasingDistance);
    }

    void SetAnimatorParameters()
    {
        // No Animator Component -> Disable
        if (_Animator == null)
        {
            return;
        }

        float moveParameter = _NavMeshAgent.velocity.magnitude / _NavMeshAgent.speed;
        _Animator.SetFloat("move", moveParameter, 0.1f, Time.deltaTime);

        // Calculate difference of angle between upper and lower body.
        // Upper Body : Direction to target or Camera's angle.
        // Lower Body : This transform's rotation.
        // 0f : 0  (No difference)
        // 1f : 45 (Max difference)
        //float upperBodyAngle;

        // Animation parameter
        //_Animator.SetFloat("moveX", inputMoveAxisHor);
        //_Animator.SetFloat("moveZ", inputMoveAxisVer);

        //_Animator.SetFloat("move", Mathf.Max(Mathf.Abs(inputMoveAxisHor), Mathf.Abs(inputMoveAxisVer)));
    }

    float GetUpperBodyAngle(Transform trnStaringTarget)
    {
        // This calculates the angle(~180 ~ 180) from me to trnStaringTarget
        float result = 0f;

        Vector2 start = new Vector2(transform.position.x, transform.position.z);
        Vector2 end = new Vector2(trnStaringTarget.position.x, trnStaringTarget.position.z);
        Vector2 v2 = end - start;

        result = (Mathf.Atan2(v2.x, v2.y) * Mathf.Rad2Deg) - transform.eulerAngles.y;

        // Return Degree angle between Itself and Target. (-180 ~ 180)
        if (result < -180f)
        {
            result += 360f;
        }

        return result;
    }
}
