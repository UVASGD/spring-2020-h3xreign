using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnergySiphonAbility", order = 2)]
public class EnergyDrainAbility : Ability
{
    public override bool UseAction(BasicUnit user, int position)
    {
        bool outcome = true;
        if (user.energy >= energyCost && ValidTarget(position))
        {
            user.energy -= energyCost;

            if (user.Roll(attribute))
            {
                int energySiphoned = 0;
                if (!targetAll)
                {
                    BasicUnit target = user.GetEnemies()[position];

                    energySiphoned = (int)Mathf.Min((float)target.energy, (float)user.baseDamage * attackMod);

                    target.AddEnergy(-1 * energySiphoned);

                }
                else
                {
                    foreach (BasicUnit enemy in user.GetEnemies())
                    {
                        int siphon = (int)Mathf.Min((float)enemy.energy, (float)(user.baseDamage * attackMod) / 4f);
                        enemy.AddEnergy(-1 * siphon);
                        energySiphoned += siphon;
                    }
                }
                foreach (BasicUnit ally in user.GetAllies())
                    ally.AddEnergy(energySiphoned / 4);
                user.AddEnergy(energySiphoned % 4);
            }
            else
                user.popupText.FailPopup();
        }
        else
            return false;
        return outcome;
    }
}
