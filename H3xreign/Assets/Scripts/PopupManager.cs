using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public PopupText damage;
    public PopupText crit;
    public PopupText dodge;
    public PopupText miss;
    public PopupText hacked;
    public PopupText stunned;
    public PopupText resistHacked;
    public PopupText resistStunned;
    public PopupText dead;
    public PopupText energy;
    public PopupText fail;


    public void DamagePopup(int dmg, bool isCrit)
    {
        string value = dmg.ToString();
        if (dmg <= 0)
            value = "Blocked!";

        // "I'm quite proud of this line of code" -Prof. Vicente Ordonez
        if(!isCrit)
            (Instantiate(damage.gameObject, transform.position + Vector3.up * 2, transform.rotation)
                as GameObject).GetComponent<PopupText>().Say(value);
        else
            (Instantiate(crit.gameObject, transform.position + Vector3.up * 2, transform.rotation)
                as GameObject).GetComponent<PopupText>().Say("*" + value + "*");
    }

    public void DodgePopup()
    {
        Instantiate(dodge.gameObject, transform.position + Vector3.up * 2, transform.rotation);
    }

    public void MissPopup()
    {
        Instantiate(miss.gameObject, transform.position + Vector3.up * 2, transform.rotation);
    }

    public void HackedPopup(bool resisted = false)
    {
        if(resisted)
            Instantiate(resistHacked.gameObject, transform.position + Vector3.up * 2, transform.rotation);
        else
            Instantiate(hacked.gameObject, transform.position + Vector3.up * 2, transform.rotation);
    }

    public void StunnedPopup(bool resisted = false)
    {
        if(resisted)
            Instantiate(resistStunned.gameObject, transform.position + Vector3.up * 2, transform.rotation);
        else
            Instantiate(stunned.gameObject, transform.position + Vector3.up * 2, transform.rotation);
    }

    // They arn't really dead... 
    public void DeathPopup()
    {
        Instantiate(dead.gameObject, transform.position + Vector3.up * 2, transform.rotation);
    }

    public void EnergyPopup(int energyGain)
    {
        (Instantiate(energy.gameObject, transform.position + Vector3.up * 2, transform.rotation)
            as GameObject).GetComponent<PopupText>().Say("+" + energyGain.ToString() + " Energy");
    }

    public void FailPopup()
    {
        Instantiate(fail.gameObject, transform.position + Vector3.up * 3f, transform.rotation);
    }
}
