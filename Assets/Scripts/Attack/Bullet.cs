using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject shooter;
    public int damage = 10;
    public float speed = 10f;
    public LayerMask blockLayers;
    public GameObject collisionEffect;

    private void OnEnable()
    {
        // Parent reset
        if (collisionEffect != null)
            collisionEffect.transform.parent = transform;
    }

    void FixedUpdate()
    {
        // Moving
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void AfterCollision()
    {
        if (collisionEffect != null)
        {
            // Effect object ON
            collisionEffect.SetActive(true);
            // Parent to null -> Don't deactive effect object with this object.
            collisionEffect.transform.parent = null;
        }
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Delete myself
        if (((1 << collision.gameObject.layer) & blockLayers) != 0)
        {
            AfterCollision();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Damage
        CharacterStats characterStats = other.GetComponent<CharacterStats>();
        if (characterStats != null && other.gameObject != shooter)
        {
            characterStats.TakeDamage(damage);
            // Delete myself
            AfterCollision();
        }

        // Delete myself
        if (((1 << other.gameObject.layer) & blockLayers) != 0)
        {
            AfterCollision();
        }
    }
}
