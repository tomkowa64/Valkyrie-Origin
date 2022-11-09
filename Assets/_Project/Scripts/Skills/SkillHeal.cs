using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeal : MonoBehaviour
{
    private GameObject player;
    public float minHealAmount;
    private float healAmount;
    public float maxHealAmount;
    private float loadingTime = 0f;

    private void UseSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (healAmount < minHealAmount)
        {
            healAmount = minHealAmount;
        }

        player.GetComponent<StatsController>().RegenerateHealth(healAmount);
        healAmount = minHealAmount;
        loadingTime = 0f;
    }

    private void LoadSkill()
    {
        if (loadingTime >= 1.0f)
        {
            if (healAmount < maxHealAmount)
            {
                healAmount += 0.1f;
            }

            if (healAmount >= maxHealAmount)
            {
                healAmount = maxHealAmount;
            }
        }
        else
        {
            loadingTime += 0.01f;
        }
    }

    private void ResetLoading()
    {
        healAmount = minHealAmount;
        loadingTime = 0f;
    }
}
