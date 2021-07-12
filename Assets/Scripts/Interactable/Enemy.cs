using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class Enemy : Interactable
{
    CharacterStats myStats;

    private void Awake()
    {
        myStats = GetComponent<CharacterStats>();
    }

    protected override void StartInteraction()
    {
        base.StartInteraction();

        CharacterStats attackerStats = interactor.GetComponent<CharacterStats>();
        if (attackerStats != null)
        {
            //attackerStats.Attack(myStats);
        }
    }
}
