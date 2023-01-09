using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    #region Variables
    private GameObject player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject[] spawners;
    [SerializeField] private float[] cooldowns;
    [SerializeField] private float projectileDamage;
    [SerializeField] private bool passThroughTerrain;
    [SerializeField] private bool spawnerWorking;
    #endregion

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cooldowns = new float[spawners.Length];
        spawnerWorking = false;

        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowns.SetValue(RandomCooldownTime(), i);
        }
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (!PauseController.gameIsPaused)
        {
            if (spawnerWorking)
            {
                #region Timers
                for (int i = 0; i < cooldowns.Length; i++)
                {
                    if (cooldowns[i] > 0f)
                    {
                        cooldowns[i] -= Time.deltaTime;
                    }
                    else
                    {
                        cooldowns[i] = 0f;
                    }
                }
                #endregion

                for (int i = 0; i < spawners.Length; i++)
                {
                    if (cooldowns[i] <= 0f)
                    {
                        SpawnProjectile(i);
                    }
                }
            }
        }
    }

    private float RandomCooldownTime()
    {
        return Random.Range(0.5f, 4f);
    }

    private void SpawnProjectile(int spawnerId)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawners[spawnerId].transform.position, Quaternion.identity);
        projectile.GetComponent<ProjectileController>().target = player;
        projectile.GetComponent<ProjectileController>().damage = projectileDamage;
        projectile.GetComponent<ProjectileController>().passThroughTerrain = passThroughTerrain;
        cooldowns[spawnerId] = RandomCooldownTime();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spawnerWorking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spawnerWorking = false;
        }
    }
}
