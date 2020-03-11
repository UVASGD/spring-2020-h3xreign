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
    public EnemyGroup enemies;
    public GameObject pointer;
    public GameObject targetIndicator;

    public StatsDisplay display;

    Queue<BasicUnit> turnOrder = new Queue<BasicUnit>();

    [HideInInspector]
    public BasicUnit activeUnit;

    [HideInInspector]
    public bool inCombat = false;

    bool waiting = false;
    float waitTill;
    

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
        pointer.SetActive(inCombat);
        targetIndicator.SetActive(inCombat);

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EngageWithParty(enemies);
        }
        if (Input.GetKeyDown(KeyCode.P))
            party.TPK();
        if (Input.GetKeyDown(KeyCode.O))
            party.ReviveParty();
        if (inCombat)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                Win();

            if (Input.GetKeyDown(KeyCode.N))
                NextTurn();

            if (activeUnit)
            {
                display.UpdateDisplay(activeUnit);
                pointer.transform.position = activeUnit.transform.position + Vector3.up * .1f;
                if (!waiting)
                {

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
                else
                    if (Time.time >= waitTill)
                        NextTurn();
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
            if (unit)
            {
                print(unit.unitName);
                turnOrder.Enqueue(unit);
                unit.Initiative();
            }
        }
        NextTurn();
        //print("Initiative set");
    }

    public void ClearInitiative()
    {
        turnOrder.Clear();
    }

    public void NextTurn()
    {
        if (!waiting)
        {
            waiting = true;
            waitTill = Time.time + 1f;
        }
        else
        {
            waiting = false;
            BasicUnit nextUp = turnOrder.Dequeue();
            turnOrder.Enqueue(nextUp);  // Put back into turn order at the end
            activeUnit = nextUp;
            nextUp.OnTurnStart();
            display.UpdateDisplay(nextUp);
            if (nextUp.alive && !nextUp.stunned)
                print("Next up is " + nextUp.unitName);
            else if (!nextUp.alive)
            {
                print("Next up is " + nextUp.unitName + " but they are dead!");
                nextUp.popupText.DeathPopup();
                NextTurn();
                waitTill = Time.time + .5f;
                return;
            }
            else if (nextUp.stunned)
            {
                print("Next up is " + nextUp.unitName + " but they are stunned! Skipping turn...");
                NextTurn();
                return;
            }
            nextUp.ResetPosition();
        }
    }

    // Initiates combat with passed in enemy group
    public void EngageWithParty(EnemyGroup enemyParty)
    {
        if (!enemyParty.Defeated())
        {
            enemies = enemyParty;
            transform.position = party.transform.position;
            party.EnterCombat();
            enemyParty.EnterCombat();
            SetInitiative();
            inCombat = true;
        }
        else
            print("Group already defeated!");
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
            if (unit)
                leftAlive = unit.alive || leftAlive;
        }

        bool rightAlive = false;
        foreach (BasicUnit unit in rightside)
        {
            if (unit)
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

    public void IndicateTarget(int pos)
    {
        targetIndicator.transform.position = positions[pos].position + Vector3.up * .1f;
        targetIndicator.GetComponent<Animator>().SetTrigger("Indicate Target");
    }

    // Automatically wins combat for the player
    public void Win()
    {
        enemies.TPK();
    }

    

}
