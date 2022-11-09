using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHPRegen : MonoBehaviour
{
    private GameObject player;
    public float regenerated = 0f;
    public float minToRegen;
    public float toRegen;
    public float maxToRegen;
    public float regenPerTick = 0.5f;
    private float loadingTime = 0f;

    private void UseSkill()
    {
        regenerated = 0f;
        player = GameObject.FindGameObjectWithTag("Player");

        if (toRegen < minToRegen)
        {
            toRegen = minToRegen;
        }

        InvokeRepeating(nameof(HealthRegen), 0f, 0.1f);
        loadingTime = 0f;
    }

    private void LoadSkill()
    {
        if (loadingTime >= 0.5f)
        {
            if (toRegen < maxToRegen)
            {
                toRegen += 0.1f;
            }

            if (toRegen >= maxToRegen)
            {
                toRegen = maxToRegen;
            }
        }
        else
        {
            loadingTime += 0.01f;
        }
    }

    private void ResetLoading()
    {
        toRegen = minToRegen;
        loadingTime = 0f;
    }

    private void HealthRegen()
    {
        regenerated += regenPerTick;
        player.GetComponent<StatsController>().RegenerateHealth(regenPerTick);
        if(regenerated >= toRegen)
        {
            regenerated = 0f;
            CancelInvoke(nameof(HealthRegen));
            toRegen = minToRegen;
        }
    }
}
