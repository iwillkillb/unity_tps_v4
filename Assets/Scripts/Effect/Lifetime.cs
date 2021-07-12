using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float lifetime = 5f;
    public enum AfterEffect {Deactivate, Delete}
    public AfterEffect afterEffect = AfterEffect.Deactivate;



    private void OnEnable()
    {
        CancelInvoke("AfterLifetime");
        Invoke("AfterLifetime", lifetime);
    }

    void AfterLifetime()
    {
        switch (afterEffect)
        {
            case AfterEffect.Deactivate:
                gameObject.SetActive(false);
                break;

            case AfterEffect.Delete:
                Destroy(gameObject);
                break;
        }
    }
}
