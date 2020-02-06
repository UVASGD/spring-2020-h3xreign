﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour
{
    public BasicUnit[] leftside;
    public BasicUnit[] rightside;

    public StatsDisplay display;

    Queue<BasicUnit> turnOrder = new Queue<BasicUnit>();

    BasicUnit activeUnit;

    bool inCombat;

    // Start is called before the first frame update
    void Start()
    {
        
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetInitiative();
            inCombat = true;
            //while (turnOrder.Count > 0)
            //{
            //    print(turnOrder.Dequeue().unitName);
            //}
            print("Initiative done!");
        }
        if (inCombat)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
            if (activeUnit)
            {
                transform.position = activeUnit.transform.position;

                if (activeUnit.alive && !activeUnit.stunned)
                {
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

        return !leftAlive || !rightAlive;
    }

}
