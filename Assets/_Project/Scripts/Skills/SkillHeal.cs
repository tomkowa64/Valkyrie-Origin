using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeal : MonoBehaviour
{
    private GameObject player;
    public float healAmount;

    private void UseSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<StatsController>().RegenerateHealth(healAmount);
    }
}
