using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : PlayerBehaviour
{
    // Interact
    public Interactable currentFocus;
    public Transform pivot;
    public float range = 1f;

    void Update()
    {
        // Right click -> Focus on Object.
        if (Input.GetMouseButtonDown(1))
        {
            Ray rayMouse = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hitMouse;

            if (Physics.Raycast(rayMouse, out hitMouse))
            {
                // Interaction
                Interactable interactable = hitMouse.collider.transform.GetComponent<Interactable>();
                if (interactable != null && Vector3.Distance(pivot.position, hitMouse.point) <= range)
                {
                    SetFocus(interactable);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            RemoveFocus();
        }
    }

    private void OnTriggerStay(Collider itemTrigger)
    {
        // Interaction
        Interactable interactable = itemTrigger.transform.GetComponent<Interactable>();
        if (interactable != null)
        {
            SetFocus(interactable);
        }
    }
    
    void LookAtPoint(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != currentFocus)
        {
            // If there is already focus -> Disconnect.
            RemoveFocus();

            // Set new focus.
            currentFocus = newFocus;

        }
        newFocus.OnFocused(transform);
    }

    void RemoveFocus()
    {
        if (currentFocus != null)
        {
            currentFocus.OnDefocused();
        }
        currentFocus = null;
    }
}
