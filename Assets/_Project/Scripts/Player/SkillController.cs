using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public string skillName;
    public float manaCost;
    public float cooldownTime;
    public float cdTimer = 0f;
    public bool onCooldown = false;
    public bool playerCanMoveWhileLoading = true;

    private void StartCooldown()
    {
        onCooldown = true;
        InvokeRepeating(nameof(CooldownTimer), 0f, 0.01f);
    }

    private void CooldownTimer()
    {
        if(cdTimer < cooldownTime)
        {
            cdTimer += 0.01f;
        }
        else
        {
            cdTimer = 0f;
            onCooldown = false;
            CancelInvoke(nameof(CooldownTimer));
        }
    }
}
