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
            InvokeRepeating(nameof(regenerateMana), 0f, 1f / manaRegen);
        }

        if(stamina < maxStamina && !regeneratingStamina)
        {
            InvokeRepeating(nameof(regenerateStamina), 0f, 1f / staminaRegen);
        }
    }

    private void regenerateMana()
    {
        regeneratingMana = true;
        mana++;
        if(mana == maxMana)
        {
            regeneratingMana = false;
            CancelInvoke(nameof(regenerateMana));
        }
    }

    private void regenerateStamina()
    {
        regeneratingStamina = true;
        stamina++;
        if(stamina == maxStamina)
        {
            regeneratingStamina = false;
            CancelInvoke(nameof(regenerateStamina));
        }
    }
}
