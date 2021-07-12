using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    new public string name = "New Skill";
    public Sprite icon = null;
    public GameObject skillObj;
    public AnimationClip animationClip;
    public int cost = 0;
    public float delay = 0.1f;          // Time it takes to take effect when using.
    public float cooldown = 0.1f;       // Time it takes to use it again.

    

    public virtual void Activate()
    {
        // Active Effect
        Debug.Log("Using " + name + ".");
    }

    public virtual void Passive()
    {
        // Passive Effect
    }
}
