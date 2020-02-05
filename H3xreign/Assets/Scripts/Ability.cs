using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;

    [HideInInspector]
    public BasicUnit origin;

    public int[] targetablePositions;
    public bool attack;
    public bool debuff;
    public bool buff;
    public bool heal;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UseAction(BasicUnit user, int position, int power, BasicUnit.Effects effect = BasicUnit.Effects.none)
    {
        if(ValidTarget())
        if (attack)
            user.AttackPosition(position);
    }

    bool ValidTarget()
    {

    }
}
