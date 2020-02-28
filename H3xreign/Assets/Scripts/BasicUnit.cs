using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUnit : MonoBehaviour
{
    public enum Sides { left, right };
    //public enum Attributes { power, finess, tenacity, intellect, willpower, charm };
    public enum Attributes { accuracy, ingenuity, resolve };
    public enum Effects { none, stun, burn, armor, dodge, confused, precise, clumsy, hacked };
    
    [Header("Utility")]
    public string unitName;
    


    [Header("Game Stats")]
    public int maxHp;  // Hit points
    public int hp;

    public int speed;  // Directly contributes to turn order (higher speed goes first)
    public int armor;  // Directly reduces incomming damage (an armor of 3 reduces incomming damage by 3)
    public int dodge;  // Chance to dodge attack that would hit 
                       // (Dodge of 15 means that, when an attack hits, there is a 15% chance to avoid all damage from that attack)
    public int crit;  // Chance to crit and attack that hits

    // Stats are directly chance to succeed on task that requires that attribute
    // For example, a power of 70 means a 70% chance to succeed on a strength-based attack or check

    /* OLD STATS, CONSIDERING REPLACING WITH THE THREE BELOW
    public int power;  // Chance of success for strength attacks and checks (str)
    public int finess;  // Chance of success for dexterity attacks and checks (dex)
    public int tenacity;  // Chance of success for constitution checks and resisting physical effects (con)
    public int intellect;  // Chance of success for intellect abilities (int)
    public int willpower;  // Chance of success for willpower abilities & resisting mental effects (wis)
    public int charm;  // Chance of success for fucking (cha)
    */

    // NEW STATS
    public int accuracy;  // Chance of success with physical attacks (str + dex)
    public int ingenuity;  // Chance of success with abilities and special interactions (int + cha)
    public int resolve;  // Chance of success for resisting effects (con + wis)

    // Energy
    public int energy;  // Current energy to use for abilities
    public int energyGain = 1;  // Energy gain per turn (usually 1)
    public int energyBonusChance;  // Percent chance to get extra energy at the start of the turn
    
    // Effective game stats
    public Dictionary<Attributes, int> stats = new Dictionary<Attributes, int>();

    // All effects and the turns they are effecting the unit for. A value of 0 means not effected
    public Dictionary<Effects, int> activeEffects = new Dictionary<Effects, int>();

    [Header("Combat")]
    public Ability[] moveset;
    public Sides side;
    public bool alive = true;
    public bool stunned = false;
    public int baseDamage;
    public int baseDamageRange;
    CombatController combat;
    int modifiers = 0;

    protected BasicUnit[] allies;
    protected BasicUnit[] enemies;

    [Header("Out of Combat")]
    public float moveSpeed;
    bool movement;
    Vector3 targetPos;


    [Header("References")]
    public ParticleController particles;
    public PopupManager popupText;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize everything when the game starts
        UpdateStats();
        Ragdoll(false);
        foreach (Effects eff in System.Enum.GetValues(typeof(Effects)))
        {
            activeEffects[eff] = 0;
        }
        combat = CombatController.combatController;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive && hp <= 0)
        {
            Die();
            // Unconscious or dead
        }

        if(alive)
        {
            animator.SetBool("Moving", movement);
            if (movement)
            {
                Vector3 dir = (targetPos - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, targetPos) < .1)
                    movement = false;
            }
        }
    }

    // Run this at the start of the unit's turn
    public void OnTurnStart()
    {
        // Check if stunned
        stunned = false;
        if(activeEffects[Effects.stun] > 0)
        {
            print("Stunned! Turn skipped");
            stunned = true;
        }
        
        // Gain energy at the start of turn, unless we are stunned, dead, or hacked
        if (alive && !stunned && activeEffects[Effects.hacked] <= 0)
        {
            GetEnergy();
        }

        // Handle all effects, applying them to the unit and decrementing their count.
        HandleEffects();
    }

    // Take this much damage with a number of modifiers
    public void Hurt(int basedmg, bool isCrit = false)
    {
        int totalDmg = Mathf.Max(basedmg - armor, 0);  // Add dmg mods. Cant take negative damage
        if (totalDmg <= 0)
            print("Blocked!");
        hp -= totalDmg;
        hp = Mathf.Clamp(hp, 0, maxHp);
        popupText.DamagePopup(totalDmg, isCrit);
    }

    // Healing effects
    public void Heal(int healing)
    {
        int totalHeal = healing;  // Add dmg mods
        hp += totalHeal;
        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    // 95% chance to hit. Missed! -XCOM
    // Returns true if hit
    public bool Hit(int dmg, bool crit = false)
    {
        // Dodge (can't dodge a crit)
        if (!crit && Random.Range(0, 100) < dodge)
        {
            print("Dodged!");
            popupText.DodgePopup();
            particles.miss.Play();
            animator.SetTrigger("Dodge");
            return false;
        }
        // Hit
        else if(crit)
        {
            particles.hit.Play();
            Hurt(dmg, true);
            animator.SetTrigger("Hit");
            return true;
        }
        else
        {
            particles.hit.Play();
            Hurt(dmg);
            animator.SetTrigger("Hit");
            return true;
        }
    }

    // Apply status effect to unit
    // "Oh god it burns!"
    public void EffectUnit(Effects effect, int turns = 1)
    {
        if (Roll(Attributes.resolve))
        {
            print("Resisted");
            if (effect == Effects.stun)
                popupText.StunnedPopup(true);
            if (effect == Effects.hacked)
                popupText.HackedPopup(true);
            return;
        }
        else
        {
            print("Suffered Effect");
            if (effect == Effects.stun)
            {
                particles.stunned.Play();
                popupText.StunnedPopup();
            }
            if (effect == Effects.hacked)
            {
                particles.hacked.Play();
                popupText.HackedPopup();
            }
            activeEffects[effect] += turns;
        }
    }

    // Use Action/ability/move/turn
    public void Action(int index, int target)
    {
        if (moveset[index].UseAction(this, target)) ;
            //combat.NextTurn();
        else
            print("Invalid target!");
    }


    // How do you wanna do this?
    // Requires target, attribute to make the attack, and percent of this unit's base damage that the attack does (1 for full damage)
    public bool AttackTarget(BasicUnit target, Attributes attribute, float percentBaseDmg = 1f)
    {
        if (Roll(attribute))
        {
            int dmg = (int)((Random.Range(baseDamage - baseDamageRange, baseDamage + baseDamageRange + 1)) * percentBaseDmg);
            if(Random.Range(0, 100) < crit)
            {
                dmg = dmg * 2;
                print("Crit!! " + dmg + " damage");
                return target.Hit(dmg, true);
            }
            print("Hit! " + dmg + " damage");
            return target.Hit(dmg);
        }
        target.particles.miss.Play();
        target.popupText.MissPopup();
        target.animator.SetTrigger("Dodge");
        return false;
    }

    // Attack target at position index
    public bool AttackPosition(int index, Attributes attribute, float percentBaseDmg = 1f)
    {
        if(index < 0 || index > enemies.Length)
        {
            print("Invalid target");
            return false;
        }
        return AttackTarget(enemies[index], attribute, percentBaseDmg);
    }

    // Attack groups from start (inclusive) to end (exclusive)
    public void AttackPositions(short start, short end, Attributes attribute, float percentBaseDmg = 1f)
    {
        if (start < 0 || end > enemies.Length)
        {
            print("Invalid targets");
            return;
        }
        for (int i = start; i <= end; i++)
        {
            AttackPosition(i, attribute, percentBaseDmg);
        }
    }

    // Apply status effect to opposing units at positions
    public void AffectPositions(short start, short end, Effects eff, int turns = 1)
    {
        if (start < 0 || end > enemies.Length)
        {
            print("Invalid targets");
            return;
        }
        for (int i = start; i < end; i++)
        {
            enemies[i].EffectUnit(eff, turns);
        }
    }

    // Apply a status effect to a unit on the same side
    public void BuffAlly(BasicUnit ally, Effects effect)
    {

    }

    // I would like everyone to roll for initiative
    public int Initiative()
    {
        allies = combat.GetAllies(side);
        enemies = combat.GetEnemies(side);
        return speed;
    }

    // Makes the dictionary if it doesnt exist and updates the values
    public void UpdateStats()
    {
        /* OLD STATS
        stats[Attributes.power] = power;
        stats[Attributes.finess] = finess;
        stats[Attributes.tenacity] = tenacity;
        stats[Attributes.intellect] = intellect;
        stats[Attributes.willpower] = willpower;
        stats[Attributes.charm] = charm;
        */

        stats[Attributes.accuracy] = accuracy;
        stats[Attributes.ingenuity] = ingenuity;
        stats[Attributes.resolve] = resolve;
    }

    // Updates effects at the start of the turn
    public void HandleEffects()
    {
        modifiers = 0;
        UpdateStats();
        // Stunned, skip turn
        if (activeEffects[Effects.stun] > 0)
        {
            //stunned = true;
            //particles.stunned.Play();
            print("Stunned! Turn Skipped");
            popupText.StunnedPopup();
            activeEffects[Effects.stun]--;
            if (activeEffects[Effects.stun] <= 0)
                particles.stunned.Stop();
            //combat.NextTurn();
        }
        // Burning, take DoT
        if (activeEffects[Effects.burn] > 0)
        {
            Hurt(Random.Range(1, 10));
            activeEffects[Effects.burn]--;
        }
        // Buff to all accuracy
        if (activeEffects[Effects.precise] > 0)
        {
            modifiers += 10;
            activeEffects[Effects.precise]--;
        }
        // Debuff to all accuracy
        if (activeEffects[Effects.clumsy] > 0)
        {
            modifiers -= 10;
            activeEffects[Effects.clumsy]--;
        }
        // Large debuff to everything and cannot gain energy
        if (activeEffects[Effects.hacked] > 0)
        {
            //particles.hacked.Play();
            modifiers -= 15;
            popupText.HackedPopup();
            activeEffects[Effects.hacked]--;
            if (activeEffects[Effects.hacked] <= 0)
                particles.hacked.Stop();
        }


    }

    // Gets energy at the start of the turn using unit's energy stats
    public void GetEnergy()
    {
        int energyGained = energyGain;
        if (Random.Range(1, 100) <= energyBonusChance)
            energyGained++;
        energy += energyGained;
        popupText.EnergyPopup(energyGained);
    }

    // Makes a check for the given attribute and returns true/false.
    // Cannot have a better chance than 95%
    public bool Roll(Attributes attribute)
    {
        return Random.Range(0, 100) < Mathf.Clamp(stats[attribute] + modifiers, 0, 95);
    }

    public void EnterCombat(int pos)
    {
        MoveToPosition(combat.positions[pos].position);
        if(side == Sides.left)
        {
            combat.leftside[pos] = this;
        }
        else
        {
            combat.rightside[pos - 4] = this;
        }
    }

    public void MoveToPosition(Vector3 pos)
    {
        movement = true;
        targetPos = pos;
    }
    
    // Pretty self explainatory
    public void Die()
    {
        print(unitName + " is dead! rip");
        popupText.DeathPopup();
        alive = false;
        animator.enabled = false;
        Ragdoll(true);
        //GetComponent<SpriteRenderer>().color = new Color(.3f, 0f, 0f);
        //Destroy(gameObject);
    }

    // Makes unit crumple and fall when they die
    public void Ragdoll(bool ragdoll)
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = !ragdoll;
            if (ragdoll)
                rb.AddForce(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }
    }

    public void Dance(bool dancing = true)
    {
        animator.SetBool("Dance", dancing);
    }
}
