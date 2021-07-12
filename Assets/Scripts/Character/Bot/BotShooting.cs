using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotShooting : MonoBehaviour
{
    public bool isShooting = false;
    public Transform bulletSpawnPoint;
    public string bulletName = "Bullet";

    public float delay = 1f;
    float currentDelay = 1f;
    public float shootingPerSec = 2f;
    private float cooldown = 0f;

    public event System.Action onAttack;




    // Start is called before the first frame update
    void Awake()
    {
        // Init delay
        currentDelay = delay;
    }

    void FixedUpdate()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }

        // Shot Input -> Delay -> Shot
        if (isShooting)
        {
            if (currentDelay > 0f)
            {
                currentDelay -= Time.deltaTime;
            }
            else
            {
                Shot();
            }
        }
        else
        {
            // Delay reset
            if (currentDelay < delay)
                currentDelay = delay;
        }
    }

    void Shot()
    {
        // Delay reset
        if (cooldown > 0f)
        {
            return;
        }
        cooldown = 1f / shootingPerSec;

        // CallBullet
        ObjectPool.instance.Call(bulletName, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // Event
        if (onAttack != null)
        {
            onAttack();
        }
    }
}
