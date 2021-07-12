using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMeleeAttack : PlayerBehaviour
{
    // Components
    PlayerInput playerInput;

    public int damage = 10;
    public float delay = 0.5f;
    float currentDelay = 0f;

    // Hitbox
    public Vector3 hitSphereCenter = Vector3.forward;
    public float hitSphereRadius = 1f;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        // Delay setting
        if (currentDelay < delay)
            currentDelay += Time.deltaTime;

        if (playerInput.inputAttack && currentDelay >= delay)
        {
            // Delay reset
            currentDelay = 0f;

            Attack();
        }
    }

    void Attack()
    {
        Collider[] cols = Physics.OverlapSphere(transform.TransformPoint(hitSphereCenter), hitSphereRadius);

        // Give damage to all of colliders in hitbox.
        foreach(Collider col in cols)
        {
            // It's me -> Nope.
            if (col.gameObject == gameObject)
                continue;

            // Damage
            CharacterStats characterStats = col.GetComponent<CharacterStats>();
            if (characterStats != null)
            {
                characterStats.TakeDamage(damage);
            }
        }

        // Play attack animation...
    }

    private void OnDrawGizmosSelected()
    {
        if (hitSphereRadius != 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.TransformPoint(hitSphereCenter), hitSphereRadius);
        }
    }
}
