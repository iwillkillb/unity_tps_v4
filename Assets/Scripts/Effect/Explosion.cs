using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float force = 10f;
    public float radius = 5f;

    private void OnEnable()
    {
        Collider[] collidersInExplosion = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider collider in collidersInExplosion)
        {
            if (collider.attachedRigidbody != null)
                collider.attachedRigidbody.AddExplosionForce(force, transform.position, radius, 0f, ForceMode.VelocityChange);
        }
    }
}
