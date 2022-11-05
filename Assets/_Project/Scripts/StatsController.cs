using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;
    public int manaRegen;
    public int stamina;
    public int maxStamina;
    public int staminaRegen;
    public int attack;
    public int deffense;
    public float movementSpeed;
    public float jumpPower;
    public float dashPower;
    public float climbingSpeed;
    public float dodgingTime;
    public float dodgeCooldown;

    private bool regeneratingMana = false;
    private bool regeneratingStamina = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mana < maxMana && !regeneratingMana)
        {
            InvokeRepeating(nameof(RegenerateMana), 0f, 1f / manaRegen);
        }

        if(stamina < maxStamina && !regeneratingStamina)
        {
            InvokeRepeating(nameof(RegenerateStamina), 0f, 1f / staminaRegen);
        }
    }

    private void RegenerateMana()
    {
        regeneratingMana = true;
        mana++;
        if(mana == maxMana)
        {
            regeneratingMana = false;
            CancelInvoke(nameof(RegenerateMana));
        }
    }

    public void UseMana(int amount)
    {
        if(mana < amount)
        {
            mana = 0;
        }
        else
        {
            mana -= amount;
        }
    }

    private void RegenerateStamina()
    {
        regeneratingStamina = true;
        stamina++;
        if(stamina == maxStamina)
        {
            regeneratingStamina = false;
            CancelInvoke(nameof(RegenerateStamina));
        }
    }

    public void UseStamina(int amount)
    {
        if(stamina < amount)
        {
            stamina = 0;
        }
        else
        {
            stamina -= amount;
        }
    }

    public void RegenerateHealth(int amount)
    {
        if(health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }
    }

    public void GetDamage(int amount)
    {
        if(health - amount < 0)
        {
            health = 0;
        }
        else
        {
            health -= amount;
        }
    }
}
