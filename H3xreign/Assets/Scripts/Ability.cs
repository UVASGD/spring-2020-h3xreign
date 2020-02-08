using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability", order = 1)]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;

    [HideInInspector]
    public BasicUnit origin;  // Unit using ability

    public int[] targetablePositions;  // Valid positions this ability can target (on the enemy side)
    public BasicUnit.Attributes attribute;  // What attribute do we use for this ability?

    public bool attack;  // Is this an attack ability?
    public float attackMod = 1;  // % of base damage the ability does

    public bool debuff;  // Is this a debuff ability?
    public BasicUnit.Effects[] harmEffects;

    public bool buff;  // Is this a buff ability?
    public BasicUnit.Effects[] goodEffects;

    public bool heal;  // Is this a heal ability
    public float healMod;  // Percent of base damage to heal for



    public bool UseAction(BasicUnit user, int position)
    {
        bool outcome = true;
        if (ValidTarget(position))
        {
            if (attack)
                user.AttackPosition(position, attribute, attackMod);
        }
        else
            return false;
        return outcome;
    }

    bool ValidTarget(int position)
    {
        foreach (int i in targetablePositions)
            if (position == i)
                return true;
        return false;
    }
}
