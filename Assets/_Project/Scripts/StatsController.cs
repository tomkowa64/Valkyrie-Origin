using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    #region Variables
    #region Health
    [Header ("Health")]
    public float health;
    public float maxHealth;
    #endregion
    #region Mana
    [Header("Mana")]
    public float mana;
    public float maxMana;
    public float manaRegen;
    #endregion
    #region Stamina
    [Header("Stamina")]
    public float stamina;
    public float maxStamina;
    public float staminaRegen;
    #endregion
    #region Fighting
    [Header("Fighting")]
    public float attack;
    public float defence;
    public float attackingTime;
    public float attackCooldown;
    #endregion
    #region Movement
    [Header("Movement")]
    public float movementSpeed;
    public float acceleration;
    public float decceleration;
    public float velocityPower;
    public float jumpPower;
    public float jumpCutMultiplier;
    public float fallGravityMultiplier;
    public float dashPower;
    public float dodgingTime;
    public float dodgeCooldown;
    #endregion
    #region Bools
    private bool regeneratingMana = false;
    private bool regeneratingStamina = false;
    private bool staminaInUse = false;
    private bool manaInUse = false;
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(staminaInUse)
        {
            regeneratingStamina = false;
            CancelInvoke(nameof(RegenerateStamina));
        }

        if(manaInUse)
        {
            regeneratingMana = false;
            CancelInvoke(nameof(RegenerateMana));
        }

        if(mana < maxMana && !regeneratingMana && !manaInUse)
        {
            InvokeRepeating(nameof(RegenerateMana), 0f, 1f / manaRegen);
        }

        if(stamina < maxStamina && !regeneratingStamina && !staminaInUse)
        {
            InvokeRepeating(nameof(RegenerateStamina), 0f, 1f / staminaRegen);
        }
    }

    #region Mana Functions
    private void RegenerateMana()
    {
        regeneratingMana = true;
        mana++;
        if(mana >= maxMana)
        {
            mana = maxMana;
            regeneratingMana = false;
            CancelInvoke(nameof(RegenerateMana));
        }
    }

    public void UseMana(float amount)
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
    #endregion

    #region Stamina Functions
    private void RegenerateStamina()
    {
        regeneratingStamina = true;
        stamina++;
        if(stamina >= maxStamina)
        {
            stamina = maxStamina;
            regeneratingStamina = false;
            CancelInvoke(nameof(RegenerateStamina));
        }
    }

    public void UseStamina(float amount, bool isUsing)
    {
        staminaInUse = isUsing;

        if(stamina < amount)
        {
            stamina = 0f;
        }
        else
        {
            stamina -= amount;
        }
    }
    #endregion

    #region Health Functions
    public void RegenerateHealth(float amount)
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

    public void DealDamage(float amount)
    {
        if(health - amount < 0f)
        {
            health = 0f;
        }
        else
        {
            health -= amount;
        }
    }
    #endregion
}
