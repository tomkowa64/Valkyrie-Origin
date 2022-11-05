using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHPRegen : MonoBehaviour
{
    private GameObject player;
    public float regenerated = 0f;
    public float toRegen = 50f;
    public float regenPerTick = 0.5f;

    private void UseSkill()
    {
        regenerated = 0f;
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating(nameof(HealthRegen), 0f, 0.1f);
    }

    private void HealthRegen()
    {
        regenerated += regenPerTick;
        player.GetComponent<StatsController>().RegenerateHealth(regenPerTick);
        if(regenerated >= toRegen)
        {
            regenerated = 0f;
            CancelInvoke(nameof(HealthRegen));
        }
    }
}
