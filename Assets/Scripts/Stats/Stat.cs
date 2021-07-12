using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public int baseValue;

    [SerializeField]
    private int sumModifier = 0;
    private List<int> modifiers = new List<int>();

    public int GetValue()
    {
        return baseValue + sumModifier;
    }

    public void SetModifier(int modifier)
    {
        sumModifier = modifier;
    }

    public void AddModifier(int modifier)
    {
        if (modifier != 0)
            modifiers.Add(modifier);
        
        sumModifier += modifier;
    }

    public void RemoveModifier(int modifier)
    {
        if (modifier != 0)
            modifiers.Remove(modifier);
        
        sumModifier -= modifier;
    }

    public int UpdateModifier()
    {
        int result = 0;
        modifiers.ForEach(x => result += x);
        return result;
    }
}
