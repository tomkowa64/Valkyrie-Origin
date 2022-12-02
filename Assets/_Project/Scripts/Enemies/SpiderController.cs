using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    #region Variables
    private EnemiesController enemy;
    private StatsController enemyStats;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector3 enemyScale;

    [SerializeField] private float defaultAttackRange;
    #endregion

    void Start()
    {
        enemy = GetComponent<EnemiesController>();
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyScale = rb.transform.localScale;
    }

    void Update()
    {
        if (enemy.isAggroed)
        {
            StayInRange();
        }
    }

    void StayInRange()
    {
        Vector2 playerPosition = player.GetComponent<Rigidbody2D>().transform.position;
        Vector2 enemyPosition = rb.transform.position;

        Debug.Log(Mathf.Abs(playerPosition.x - enemyPosition.x));

        enemy.lastXDir = Mathf.Sign(playerPosition.x - enemyPosition.x);
        rb.transform.localScale = new Vector3(-1 * Mathf.Sign(playerPosition.x - enemyPosition.x) * Mathf.Abs(enemyScale.x), enemyScale.y, enemyScale.z);

        if (Mathf.Abs(playerPosition.x - enemyPosition.x) > defaultAttackRange)
        {
            float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed / 1.5f;
            float speedDiff = targetSpeed - rb.velocity.x;
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

            rb.AddForce(movement * Vector2.right);
        }
    }
}
