using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    PlayerBehaviour[] playerBehaviours;

    // Start is called before the first frame update
    void Start()
    {
        playerBehaviours = GetComponents<PlayerBehaviour>();

        EquipmentManager.instance.onEquipmentModifiedChanged += OnEquipmentModifiedChanged;
    }

    // Update is called once per frame
    void OnEquipmentModifiedChanged(Equipment item, bool isAdding)
    {
        if (isAdding)
        {
            // Add Item
            defense.AddModifier(item.defenseModifier);
            attack.AddModifier(item.attackModifier);
            Debug.Log("Plus  " + item.name + " -> defense : " + item.defenseModifier + " / attack : " + item.attackModifier);
        }
        else
        {
            // Remove Item
            defense.RemoveModifier(item.defenseModifier);
            attack.RemoveModifier(item.attackModifier);
            Debug.Log("Minus " + item.name + " -> defense : " + item.defenseModifier + " / attack : " + item.attackModifier);
        }
    }
    
    // Player kill itself
    // PlayerStats.Die() -> ragdoll.ToggleRagdoll(true)
    // -> PlayerManager.Die() -> player.SetActive(false)

    // Player is revived by Game Manager
    // PlayerManager.Revive() -> player.SetActive(true)
    // -> Player's CharacterStats.Revive() -> ragdoll.ToggleRagdoll(false)

    public override void Die()
    {
        base.Die();

        PlayerManager.instance.Die();

        // All PlayerBehavior off
        foreach(PlayerBehaviour playerBehaviour in playerBehaviours)
        {
            playerBehaviour.enabled = false;
        }
    }

    public override void Revive()
    {
        base.Revive();

        // All PlayerBehavior on
        foreach (PlayerBehaviour playerBehaviour in playerBehaviours)
        {
            playerBehaviour.enabled = true;
        }
    }
}
