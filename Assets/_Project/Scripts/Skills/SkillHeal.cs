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
    public float sumOfHealingDone;
    public float healingForLevelOne;

    private void UseSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (healAmount < minHealAmount)
        {
            healAmount = minHealAmount;
        }

        if (sumOfHealingDone <= healingForLevelOne)
        {
            sumOfHealingDone += healAmount;
            GetComponent<SkillController>().mastering = sumOfHealingDone / healingForLevelOne;

            if (sumOfHealingDone >= healingForLevelOne)
            {
                sumOfHealingDone = healingForLevelOne;
                GetComponent<SkillController>().mastering = sumOfHealingDone / healingForLevelOne;
            }
        }

        player.GetComponent<StatsController>().RegenerateHealth(healAmount);
        healAmount = minHealAmount;
        loadingTime = 0f;
    }

    private void LoadSkill()
    {
        if (GetComponent<SkillController>().mastering >= 1f)
        {
            healAmount = maxHealAmount;
        }
        else
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
    }

    private void ResetLoading()
    {
        healAmount = minHealAmount;
        loadingTime = 0f;
    }
}
