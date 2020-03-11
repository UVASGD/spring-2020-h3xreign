using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelection : MonoBehaviour
{
    Ability selectedAbility;
    List<Ability> abilities;
    CombatController combat;

    // Start is called before the first frame update
    void Start()
    {
        combat = CombatController.combatController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetAbilities()
    {
        abilities.Clear();
        foreach (Ability ability in combat.activeUnit.moveset)
            abilities.Add(ability);
    }

    public void SelectAbility(int i)
    {
        GetAbilities();
        selectedAbility = abilities[i];
        UseSelectedAbility(0);
    }

    public void UseSelectedAbility(int target)
    {
        selectedAbility.UseAction(combat.activeUnit, target);
    }
}
