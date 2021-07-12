using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTest : MonoBehaviour
{
    public Transform target;
    public Transform worldUp;
    public bool lookAtSwitch = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtSwitch)
        {
            if (worldUp == null)
                transform.LookAt(target);
            else
                transform.LookAt(target, worldUp.position);
        }
        else
        {
            if (worldUp == null)
                transform.rotation = Quaternion.LookRotation(target.position);
            else
                transform.rotation = Quaternion.LookRotation(target.position, worldUp.position);
        }
    }
}
