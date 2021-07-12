using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component is for all objects that player can interact with such as enemies, item etc.
// It is mean to be used as a base class.
public class Interactable : MonoBehaviour
{
    protected Transform interactor;

    public bool isFocus = false;
    //bool hasInteracted = false;


    /*
    void Update()
    {
        
        if (isFocus && !hasInteracted && interactor != null)
        {
            // Character moves to item -> Contact -> Interact()
            float distance = Vector3.Distance(interactor.position, transform.position);
            if (distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }*/

    protected virtual void StartInteraction ()
    {
        // Debug.Log(interactor.name + " Interacted " + transform.name + ".");
    }

    public void OnFocused (Transform interactor)
    {
        this.interactor = interactor;

        isFocus = true;
        //hasInteracted = false;

        StartInteraction();
    }
    protected virtual void EndInteraction()
    {
        // Debug.Log(interactor.name + " Interacted " + transform.name + ".");
    }

    public void OnDefocused()
    {
        interactor = null;

        isFocus = false;
        //hasInteracted = false;

        EndInteraction();
    }
    /*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }*/
}
