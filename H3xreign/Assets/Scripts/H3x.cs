﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H3x : BasicUnit
{
    // Inherting all from BasicUnit

    [Header("H3x Unique Abilities")]
    public int dmgBase;  // Base damage for all attacks
    public int dmgRange;

    // H3x's moveset
    public enum H3xMoves { BufferOverflow, DenialOfService, Spectre, Encryption, Rootkit, Botnet }

    // Single target, full damage. On hit, damages all units in a higher position with 50% base damage
    public void BufferOverflow(short position)
    {
        // If we hit 
        if (AttackPosition(position, Attributes.intellect, dmgBase, dmgRange))
        {
            for (int pos = position + 1; pos < enemies.Length; pos++)
            {
                enemies[pos].Hit(dmgBase / 2);
            }
        }
    }
}
