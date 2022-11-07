using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummyController : MonoBehaviour
{
    public StatsController dummyStats;

    private int timeFromLastDamage = 0;
    private int healAfterSeconds = 4;
    private bool isRegenerating = false;
    private float lastRecordedHealth;

    // Start is called before the first frame update
    void Start()
    {
        dummyStats = GetComponent<StatsController>();
        lastRecordedHealth = dummyStats.maxHealth;
        InvokeRepeating(nameof(checkIfGotDamaged), 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(dummyStats.health <= 1)
        {
            dummyStats.health = dummyStats.maxHealth;
        }

        if(timeFromLastDamage == healAfterSeconds && dummyStats.health < dummyStats.maxHealth)
        {
            isRegenerating = true;
        }

        if(isRegenerating)
        {
            dummyStats.RegenerateHealth(dummyStats.maxHealth);
            lastRecordedHealth = dummyStats.health;
            isRegenerating = false;
            timeFromLastDamage = 0;
        }
    }

    private void checkIfGotDamaged()
    {
        if(dummyStats.health == lastRecordedHealth)
        {
            if(timeFromLastDamage < healAfterSeconds)
            {
                timeFromLastDamage++;
            }
            else
            {
                timeFromLastDamage = 0;
            }
        }
        else
        {
            timeFromLastDamage = 0;
            lastRecordedHealth = dummyStats.health;
        }
    }
}
