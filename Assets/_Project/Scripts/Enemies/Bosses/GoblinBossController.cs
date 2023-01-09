using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoblinBossController : MonoBehaviour
{
    #region Variables
    private EnemiesController enemy;
    private StatsController enemyStats;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector3 enemyScale;

    private bool healthBarSet;
    public bool bossFightStarted;
    [SerializeField] private GameObject bossFightStartPosition;
    [SerializeField] private GameObject[] enemiesPrefabs;
    [SerializeField] private GameObject[] teleportLocations = new GameObject[2];
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private List<GameObject> spawnedEnemies;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileSpawner;
    public GameObject bossFightWall;
    [SerializeField] private Slider bossHealthBar;

    private bool spawnPhase;
    private float spawnPhaseCooldown;
    private float spawnPhaseTimer;
    private int enemiesCount;

    [SerializeField] private float projectileSpawnMinCooldown;
    [SerializeField] private float projectileSpawnMaxCooldown;
    private float projectileSpawnTimer;
    #endregion

    void Start()
    {
        enemy = GetComponent<EnemiesController>();
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        enemyScale = rb.transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player");
        spawnPhaseTimer = 0f;
        projectileSpawnTimer = 0f;
        enemiesCount = 0;
        spawnPhase = false;
        bossFightStarted = false;
        healthBarSet = false;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (!PauseController.gameIsPaused)
        {
            if (enemy.canMove)
            {
                Rotate();
            }

            if (Mathf.Round(player.transform.position.x) == Mathf.Round(bossFightStartPosition.transform.position.x) && !bossFightStarted)
            {
                bossFightStarted = true;
            }

            if (bossFightStarted)
            {
                bossFightWall.SetActive(true);

                if (!healthBarSet)
                {
                    bossHealthBar.gameObject.SetActive(true);
                    bossHealthBar.GetComponent<Slider>().minValue = 0f;
                    bossHealthBar.GetComponent<Slider>().maxValue = enemyStats.maxHealth;
                    healthBarSet = true;
                }
                
                bossHealthBar.GetComponent<Slider>().value = enemyStats.health;

                if (!spawnPhase && spawnPhaseTimer <= 0f)
                {
                    rb.transform.position = teleportLocations[1].transform.position;

                    StartCoroutine(SpawnEnemies());

                    projectileSpawnTimer = Random.Range(projectileSpawnMinCooldown, projectileSpawnMaxCooldown);
                    spawnPhase = true;
                }

                if (spawnPhase && projectileSpawnTimer <= 0f)
                {
                    SpawnProjectile();
                }

                if (spawnedEnemies.Count > 0)
                {
                    for (int i = 0; i < spawnedEnemies.Count; i++)
                    {
                        spawnedEnemies[i].GetComponent<EnemiesController>().aggroCounter = spawnedEnemies[i].GetComponent<EnemiesController>().aggroTime;
                        spawnedEnemies[i].GetComponent<EnemiesController>().aggroTime = 0f;
                        spawnedEnemies[i].GetComponent<EnemiesController>().isAggroed = true;

                        if (spawnedEnemies[i].GetComponent<StatsController>().health <= 0)
                        {
                            spawnedEnemies.Remove(spawnedEnemies[i]);
                            enemiesCount -= 1;
                        }
                    }
                }
                else if (spawnPhase && enemiesCount <= 0)
                {
                    StartCoroutine(EndSpawnPhase());
                }

                #region Timers
                if (spawnPhaseTimer > 0f)
                {
                    spawnPhaseTimer -= Time.deltaTime;
                }
                else
                {
                    spawnPhaseTimer = 0f;
                    enemy.canMove = true;
                }

                if (projectileSpawnTimer > 0f)
                {
                    projectileSpawnTimer -= Time.deltaTime;
                }
                else
                {
                    projectileSpawnTimer = 0f;
                }
                #endregion
            }
        }
    }

    private void Rotate()
    {
        Vector2 playerPosition = player.GetComponent<Rigidbody2D>().transform.position;
        Vector2 enemyPosition = rb.transform.position;

        enemy.lastXDir = Mathf.Sign(playerPosition.x - enemyPosition.x);
        rb.transform.localScale = new Vector3(-1 * Mathf.Sign(playerPosition.x - enemyPosition.x) * Mathf.Abs(enemyScale.x), enemyScale.y, enemyScale.z);
    }

    private void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawner.transform.position, Quaternion.identity);
        projectile.GetComponent<ProjectileController>().target = player;
        projectile.GetComponent<ProjectileController>().damage = enemyStats.attack;
        projectile.GetComponent<ProjectileController>().passThroughTerrain = true;

        projectileSpawnTimer = Random.Range(projectileSpawnMinCooldown, projectileSpawnMaxCooldown);
    }

    private IEnumerator SpawnEnemies()
    {
        enemiesCount = Mathf.RoundToInt(Random.Range(1, 4));

        for (int i = 0; i < enemiesCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 4f));

            int enemy = Mathf.RoundToInt(Random.Range(0, enemiesPrefabs.Length));
            int position = Mathf.RoundToInt(Random.Range(0, spawnPoints.Length));
            spawnedEnemies.Add(Instantiate(enemiesPrefabs[enemy], spawnPoints[position].transform.position, Quaternion.identity));
        }
    }

    private IEnumerator EndSpawnPhase()
    {
        yield return new WaitForSeconds(1f);
        spawnPhaseTimer = spawnPhaseCooldown;
        spawnPhase = false;
        enemy.canMove = false;
        rb.transform.position = teleportLocations[0].transform.position;
    }
}
