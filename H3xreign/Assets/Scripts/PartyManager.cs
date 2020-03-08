using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager party;

    public Transform[] positions;
    public BasicUnit[] partyMembers;

    CombatController combat;

    private void Awake()
    {
        if (party != this)
        {
            party = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        combat = CombatController.combatController;
        Formation();
    }

    // Update is called once per frame
    void Update()
    {
        if(!combat.inCombat)
        {
            foreach (BasicUnit unit in partyMembers)
                if (unit)
                    unit.animator.SetBool("Party Movement", false);
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.forward * Time.deltaTime * 2.5f;
                foreach (BasicUnit unit in partyMembers)
                    if (unit)
                        unit.animator.SetBool("Party Movement", true);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += -1 * Vector3.forward * Time.deltaTime * 2.5f;
                foreach (BasicUnit unit in partyMembers)
                    if (unit)
                        unit.animator.SetBool("Party Movement", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
            DanceDanceBaby();
        else if (Input.GetKeyDown(KeyCode.F))
            DanceDanceBaby(false);
    }

    // Tells all party members to enter combat
    public void EnterCombat()
    {
        // Tells each unit in party to move to position 
        for (int i = 0; i < partyMembers.Length; i++)
        {
            partyMembers[i].EnterCombat(i);
        }
    }

    // Tells all party members to return to formation
    public void Formation()
    {
        for (int i = 0; i < partyMembers.Length; i++)
        {
            partyMembers[i].MoveToPosition(positions[i]);
        }
    }

    // Oh let's break it down!
    public void DanceDanceBaby(bool dance = true)
    {
        foreach (BasicUnit unit in partyMembers)
            if (unit)
                unit.Dance(dance);
    }

    // Brings unconscious units in the party back to life
    public void ReviveParty()
    {
        foreach (BasicUnit unit in partyMembers)
            if (unit)
                unit.Revive();
    }

    // "It happens. Deal with it bitches"
    public void TPK()
    {
        foreach (BasicUnit unit in partyMembers)
            if (unit)
                unit.Die();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // If we enter an enemy's trigger, engage with them
        if (other.tag == "EnemyGroup")
            combat.EngageWithParty(other.gameObject.GetComponent<EnemyGroup>());
    }
}
