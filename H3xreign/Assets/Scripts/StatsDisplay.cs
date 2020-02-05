using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI effectsText;


    public void UpdateDisplay(BasicUnit unit)
    {
        unitName.text = unit.unitName;
        hpText.text = "HP: " + unit.hp + " / " + unit.maxHp;
        statsText.text = "Speed: " + unit.speed + "\n" +
                            "Armor: " + unit.armor + "\n" +
                            "Dodge: " + unit.dodge + "\n" +
                            "Power: " + unit.power + "\n" +
                            "Finess: " + unit.finess + "\n" +
                            "Tenacity: " + unit.tenacity + "\n" +
                            "Intellect: " + unit.intellect + "\n" +
                            "Willpower: " + unit.willpower + "\n" +
                            "Charm: " + unit.charm;
        string currentEffects = "";
        foreach (BasicUnit.Effects effect in unit.activeEffects.Keys)
        {
            if (unit.activeEffects[effect] > 0)
            {
                currentEffects += effect.ToString() + " (" + unit.activeEffects[effect] +")\n";
            }
        }
        effectsText.text = currentEffects;
    }
}
