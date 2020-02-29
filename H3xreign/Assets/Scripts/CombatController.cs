using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour
{
    public static CombatController combatController;

    public Transform[] positions;

    public BasicUnit[] leftside;
    public BasicUnit[] rightside;

    PartyManager party;
    public GameObject pointer;

    public StatsDisplay display;

    Queue<BasicUnit> turnOrder = new Queue<BasicUnit>();

    BasicUnit activeUnit;

    [HideInInspector]
    public bool inCombat = false;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (combatController != this)
        {
            combatController = this;
        }
    }

    private void Start()
    {
        party = PartyManager.party;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("3dScene");
            return;
        }
        // Oh lets break it down!
        if(Input.GetKeyDown(KeyCode.D))
        {
            foreach (BasicUnit unit in leftside)
                if (unit != null)
                    unit.Dance();
            foreach (BasicUnit unit in rightside)
                if (unit != null)
                    unit.Dance();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetInitiative();
            party.EnterCombat();
            inCombat = true;
            //while (turnOrder.Count > 0)
            //{
            //    print(turnOrder.Dequeue().unitName);
            //}
            print("Initiative done!");
        }
        if (Input.GetKeyDown(KeyCode.P))
            party.TPK();
        if (Input.GetKeyDown(KeyCode.O))
            party.ReviveParty();
        if (inCombat)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
            if (activeUnit)
            {
                display.UpdateDisplay(activeUnit);
                pointer.transform.position = activeUnit.transform.position + Vector3.up * .1f;

                if (activeUnit.alive && !activeUnit.stunned)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                        activeUnit.GetEnergy();
                    if (activeUnit.unitName != "H3x")
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha0))
                            activeUnit.Action(0, 0);
                        else if (Input.GetKeyDown(KeyCode.Alpha1))
                            activeUnit.Action(0, 1);
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                            activeUnit.Action(0, 2);
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                            activeUnit.Action(0, 3);
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha0))
                            activeUnit.Action(0, 0);
                        else if (Input.GetKeyDown(KeyCode.Alpha1))
                            activeUnit.Action(0, 1);
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                            activeUnit.Action(0, 2);
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                            activeUnit.Action(0, 3);
                        else if (Input.GetKeyDown(KeyCode.R))
                            activeUnit.AffectPositions(0, 4, BasicUnit.Effects.stun);
                        else if (Input.GetKeyDown(KeyCode.H))
                            activeUnit.AffectPositions(0, 4, BasicUnit.Effects.hacked, 4);
                        else if (Input.GetKeyDown(KeyCode.X))
                            activeUnit.Action(1, 0);
                    }
                }
                if (activeUnit.stunned)
                {
                    NextTurn();
                }
            }
            inCombat = !CheckVictory();
        }
    }

    public void SetInitiative()
    {
        ClearInitiative();
        List<BasicUnit> positions = new List<BasicUnit>(leftside.Length + rightside.Length);
        
        positions.AddRange(leftside);
        positions.AddRange(rightside);

        positions.Sort((x, y) => y.speed.CompareTo(x.speed));
        
        foreach (BasicUnit unit in positions)
        {
            print(unit.unitName);
            turnOrder.Enqueue(unit);
            unit.Initiative();
        }
        //print("Initiative set");
    }

    public void ClearInitiative()
    {
        turnOrder.Clear();
    }

    public void NextTurn()
    {
        BasicUnit nextUp = turnOrder.Dequeue();
        turnOrder.Enqueue(nextUp);  // Put back into turn order at the end
        nextUp.OnTurnStart();
        display.UpdateDisplay(nextUp);
        if (nextUp.alive && !nextUp.stunned)
            print("Next up is " + nextUp.unitName);
        else if (!nextUp.alive)
        {
            print("Next up is " + nextUp.unitName + " but they are dead!");
            nextUp.popupText.DeathPopup();
            NextTurn();
            return;
        }
        else if (nextUp.stunned)
        {
            print("Next up is " + nextUp.unitName + " but they are stunned! Skipping turn...");
            NextTurn();
            return;
        }
        activeUnit = nextUp;
    }

    // Returns list of units on same side as current unit
    public BasicUnit[] GetAllies(BasicUnit.Sides side)
    {
        if (side == BasicUnit.Sides.left)
            return leftside;
        else
            return rightside;
    }

    // Returns list of units on opposite side as current unit
    public BasicUnit[] GetEnemies(BasicUnit.Sides side)
    {
        if (side == BasicUnit.Sides.left)
            return rightside;
        else
            return leftside;
    }

    // Checks to see if a side is defeated
    public bool CheckVictory()
    {
        bool leftAlive = false;
        foreach (BasicUnit unit in leftside)
        {
            leftAlive = unit.alive || leftAlive;
        }

        bool rightAlive = false;
        foreach (BasicUnit unit in rightside)
        {
            rightAlive = unit.alive || rightAlive;
        }

        if (leftAlive && !rightAlive)
            print("Left side wins!");
        else if (rightAlive && !leftAlive)
            print("Right side wins!");

        // If this is true, combat is over
        if (!leftAlive || !rightAlive)
        {
            party.Formation();
            party.ReviveParty();
            return true;
        }
        return false;
    }

    

}
