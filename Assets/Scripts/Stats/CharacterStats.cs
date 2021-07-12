using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    int deathCount = 0;

    [Header("Debris")]
    public GameObject debris;
    Transform debrisParent;
    Vector3 debrisLocalPosition;
    Quaternion debrisLocalRotation;

    [Header("HP/MP")]
    public int maxHP = 100;
    public int maxMP = 100;
    public int currentHP { get; private set; }
    public int currentMP { get; private set; }

    public delegate void OnHPChanged(int maxHP, int currentHP);
    public OnHPChanged onHPChanged;
    //public event System.Action<int, int> OnHPChanged;

    [Header("Stat")]
    public Stat attack;
    public Stat defense;



    private void OnEnable()
    {
        if (deathCount == 0)
            Init();
        else
            Revive();
    }

    public void TakeDamage(int damage)
    {
        damage -= defense.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHP -= damage;
        Debug.Log(transform.name + " takes " + damage + "damage.");

        if (onHPChanged != null)
            onHPChanged.Invoke(maxHP, currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public virtual void Init()
    {
        Debug.Log(transform.name + " Init.");

        // Reset stats
        currentHP = maxHP;
        currentMP = maxMP;

        // Backup parent of debris
        if (debris != null)
        {
            debrisParent = debris.transform.parent;
            debrisLocalPosition = debris.transform.localPosition;
            debrisLocalRotation = debris.transform.localRotation;
            debris.SetActive(false);
        }
    }

    public virtual void Die()
    {
        Debug.Log(transform.name + " died.");

        deathCount++;

        // On Debris
        if (debris != null)
        {
            debris.transform.parent = null;
            debris.SetActive(true);
        }

        // Ragdoll
        Ragdoll ragdoll = GetComponent<Ragdoll>();
        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(true);
        }
        else
        {
            // If there is no ragdoll, it disappears.
            gameObject.SetActive(false);
        }
    }

    public virtual void Revive()
    {
        Debug.Log(transform.name + " revived.");

        // Reset stats
        currentHP = maxHP;
        currentMP = maxMP;

        // Off Debris
        if (debris != null)
        {
            debris.transform.parent = debrisParent;
            debris.transform.localPosition = debrisLocalPosition;
            debris.transform.localRotation = debrisLocalRotation;
            debris.SetActive(false);
        }

        // Ragdoll
        Ragdoll ragdoll = GetComponent<Ragdoll>();
        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(false);
        }
    }
}
