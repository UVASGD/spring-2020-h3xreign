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
    public TextMeshProUGUI energyText;


    public void UpdateDisplay(BasicUnit unit)
    {
        unitName.text = unit.unitName;
        hpText.text = "HP: " + unit.hp + " / " + unit.maxHp;
        energyText.text = "Energy: " + unit.energy;
        statsText.text = "Speed: " + unit.speed + "\n" +
                            "Armor: " + unit.armor + "\n" +
                            "Dodge: " + unit.dodge + "\n\n" +
                            /*"Power: " + unit.power + "\n" +
                            "Finess: " + unit.finess + "\n" +
                            "Tenacity: " + unit.tenacity + "\n" +
                            "Intellect: " + unit.intellect + "\n" +
                            "Willpower: " + unit.willpower + "\n" +
                            "Charm: " + unit.charm;*/
                            "Accuracy: " + unit.accuracy + "\n" +
                            "Ingenuity: " + unit.ingenuity + "\n" +
                            "Resolve: " + unit.resolve;
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
