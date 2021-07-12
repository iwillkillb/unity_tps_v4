using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : PlayerBehaviour
{
    public bool inputDownH { get; set; }
    public float inputH { get; set; }
    public bool inputUpH { get; set; }
    public bool inputDoubleH { get; set; }

    bool enableDoubleH = false;



    public bool inputDownV { get; set; }
    public float inputV { get; set; }
    public bool inputUpV { get; set; }
    public bool inputDoubleV { get; set; }

    bool enableDoubleV = false;



    public float mouseX { get; set; }
    public float mouseY { get; set; }



    public bool inputJump { get; set; }
    public bool inputAttack { get; set; }
    public bool inputInteract { get; set; }



    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        inputDownH = Input.GetButtonDown("Horizontal");
        inputH = Input.GetAxis("Horizontal");
        inputUpH = Input.GetButtonUp("Horizontal");
        DetectDoubleInputHorizontal();

        inputDownV = Input.GetButtonDown("Vertical");
        inputV = Input.GetAxis("Vertical");
        inputUpV = Input.GetButtonUp("Vertical");
        DetectDoubleInputVertical();

        inputJump = Input.GetButtonDown("Jump");
        inputAttack = Input.GetMouseButton(0);
        inputInteract = Input.GetMouseButton(1);

    }


    void DetectDoubleInputVertical()
    {
        if (inputDownV)
        {
            if (enableDoubleV)
            {
                // Run
                inputDoubleV = true;
            }
            else
            {
                // First input
                enableDoubleV = true;
                Invoke("CancelDoubleV", 0.3f);
            }
        }

        if (inputUpV)
        {
            inputDoubleV = false;
        }
    }

    void CancelDoubleV()
    {
        enableDoubleV = false;
    }


    void DetectDoubleInputHorizontal()
    {
        if (inputDownH)
        {
            if (enableDoubleH)
            {
                // Run
                inputDoubleH = true;
            }
            else
            {
                // First input
                enableDoubleH = true;
                Invoke("CancelDoubleH", 0.3f);
            }
        }

        if (inputUpH)
        {
            inputDoubleH = false;
        }
    }

    void CancelDoubleH()
    {
        enableDoubleH = false;
    }
}
