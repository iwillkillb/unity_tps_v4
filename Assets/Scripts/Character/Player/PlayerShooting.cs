using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerShooting : PlayerBehaviour
{
    // Components
    PlayerInput playerInput;

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
        // Components connecting
        playerInput = GetComponent<PlayerInput>();

        // Init delay
        currentDelay = delay;
    }

    void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }

        // Shot Input -> Delay -> Shot
        if (playerInput.inputAttack)
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



    Quaternion GetAimRotation()
    {
        // Aim Screen's center

        //Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray rayMouse = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hitMouse;

        if (Physics.Raycast(rayMouse, out hitMouse))
        {
            return Quaternion.LookRotation(hitMouse.point - bulletSpawnPoint.position);
        }
        else
        {
            return Quaternion.LookRotation(rayMouse.direction);
        }
    }


    void Shot()
    {
        // Cooldown reset
        if (cooldown > 0f)
        {
            return;
        }
        cooldown = 1f / shootingPerSec;

        // CallBullet
        ObjectPool.instance.Call(bulletName, bulletSpawnPoint.position, GetAimRotation());

        // Event
        if (onAttack != null)
        {
            onAttack();
        }
    }
}
