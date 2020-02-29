using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    // Almost an exact copy of PartyManager but not static; 
    // there will be multiple instances of this for each group of enemies
    public Transform[] positions;
    public BasicUnit[] groupMembers;

    CombatController combat;
    

    // Start is called before the first frame update
    void Start()
    {
        combat = CombatController.combatController;
        Formation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DanceDanceBaby();
        else if (Input.GetKeyDown(KeyCode.F))
            DanceDanceBaby(false);
    }

    // Tells all party members to enter combat
    public void EnterCombat()
    {
        // Tells each unit in party to move to position 
        for (int i = 0; i < groupMembers.Length; i++)
        {
            groupMembers[i].EnterCombat(i+4);
        }
    }

    // Tells all party members to return to formation
    public void Formation()
    {
        for (int i = 0; i < groupMembers.Length; i++)
        {
            groupMembers[i].MoveToPosition(positions[i].position);
        }
    }

    // Oh let's break it down!
    public void DanceDanceBaby(bool dance = true)
    {
        foreach (BasicUnit unit in groupMembers)
            if (unit != null)
                unit.Dance(dance);
    }

    // Brings unconscious units in the party back to life
    public void ReviveParty()
    {
        foreach (BasicUnit unit in groupMembers)
            unit.Revive();
    }

    // "It happens. Deal with it bitches"
    public void TPK()
    {
        foreach (BasicUnit unit in groupMembers)
            unit.Die();
    }

    // Returns true if everybody is dead
    public bool Defeated()
    {
        bool aliveUnit = false;
        foreach (BasicUnit unit in groupMembers)
            aliveUnit = aliveUnit || unit.alive;
        return !aliveUnit;
    }
}
