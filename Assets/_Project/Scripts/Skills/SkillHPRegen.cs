using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHPRegen : MonoBehaviour
{
    private GameObject player;
    public float regenerated = 0f;
    public float minToRegenDefault;
    public float minToRegenMastered;
    public float toRegen;
    public float maxToRegenDefault;
    public float maxToRegenMastered;
    public float regenPerTickDefault = 0.5f;
    public float regenPerTickMastered = 0.75f;
    private float loadingTime = 0f;
    public float sumOfHealthRegen;
    public float regenForLevelOne;

    private void UseSkill()
    {
        regenerated = 0f;
        player = GameObject.FindGameObjectWithTag("Player");

        if (GetComponent<SkillController>().mastering >= 1f)
        {
            if (toRegen < minToRegenMastered)
            {
                toRegen = minToRegenMastered;
            }
        }
        else
        {
            if (toRegen < minToRegenDefault)
            {
                toRegen = minToRegenDefault;
            }
        }

        if (sumOfHealthRegen <= regenForLevelOne)
        {
            sumOfHealthRegen += toRegen;
            GetComponent<SkillController>().mastering = sumOfHealthRegen / regenForLevelOne;

            if (sumOfHealthRegen >= regenForLevelOne)
            {
                sumOfHealthRegen = regenForLevelOne;
                GetComponent<SkillController>().mastering = sumOfHealthRegen / regenForLevelOne;
            }
        }

        InvokeRepeating(nameof(HealthRegen), 0f, 0.1f);
        loadingTime = 0f;
    }

    private void LoadSkill()
    {
        if (loadingTime >= 0.5f)
        {
            if (GetComponent<SkillController>().mastering >= 1f)
            {
                if (toRegen < maxToRegenMastered)
                {
                    toRegen += 0.15f;
                }

                if (toRegen >= maxToRegenMastered)
                {
                    toRegen = maxToRegenMastered;
                }
            }
            else
            {
                if (toRegen < maxToRegenDefault)
                {
                    toRegen += 0.1f;
                }

                if (toRegen >= maxToRegenDefault)
                {
                    toRegen = maxToRegenDefault;
                }
            }
        }
        else
        {
            loadingTime += 0.01f;
        }
    }

    private void ResetLoading()
    {
        if (GetComponent<SkillController>().mastering >= 1f)
        {
            toRegen = minToRegenMastered;
        }
        else
        {
            toRegen = minToRegenDefault;
        }

        loadingTime = 0f;
    }

    private void HealthRegen()
    {
        if (GetComponent<SkillController>().mastering >= 1f)
        {
            regenerated += regenPerTickMastered;
            player.GetComponent<StatsController>().RegenerateHealth(regenPerTickMastered);
        }
        else
        {
            regenerated += regenPerTickDefault;
            player.GetComponent<StatsController>().RegenerateHealth(regenPerTickDefault);
        }

        if (regenerated >= toRegen)
        {
            regenerated = 0f;
            CancelInvoke(nameof(HealthRegen));

            if (GetComponent<SkillController>().mastering >= 1f)
            {
                toRegen = minToRegenMastered;
            }
            else
            {
                toRegen = minToRegenDefault;
            }
        }
    }
}
