using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    #region Variables
    private EnemiesController enemy;
    private StatsController enemyStats;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector3 enemyScale;
    private CapsuleCollider2D coll;
    [SerializeField] private float attackRange;
    #endregion

    void Start()
    {
        enemy = GetComponent<EnemiesController>();
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        enemyScale = rb.transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player");
        coll = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (enemy.isAggroed)
            {
                StayInRange();

                if (enemy.canAttack)
                {
                    if (PlayerInAttackRange())
                    {
                        StartCoroutine(UseBasicAttack());
                    }
                }
            }
        }
    }

    private IEnumerator UseBasicAttack()
    {
        enemy.canMove = false;
        enemy.canAttack = false;
        yield return new WaitForSeconds(0.5f);

        if (PlayerInAttackRange())
        {
            float attackLoadingTime = Random.Range(0.5f, 5f);

            yield return new WaitForSeconds(attackLoadingTime);
            if (PlayerInAttackRange())
            {
                player.GetComponent<StatsController>().DealDamage(enemyStats.attack);
            }
            yield return new WaitForSeconds(0.5f);
        }

        enemy.canMove = true;
        enemy.canAttack = true;
        enemy.aggroCounter = enemy.aggroTime;
        enemy.isAggroed = true;
    }

    private void StayInRange()
    {
        Vector2 playerPosition = player.GetComponent<Rigidbody2D>().transform.position;
        Vector2 enemyPosition = rb.transform.position;

        if (enemy.canMove)
        {
            enemy.lastXDir = Mathf.Sign(playerPosition.x - enemyPosition.x);
            rb.transform.localScale = new Vector3(-1 * Mathf.Sign(playerPosition.x - enemyPosition.x) * Mathf.Abs(enemyScale.x), enemyScale.y, enemyScale.z);

            if (Mathf.Abs(playerPosition.x - enemyPosition.x) > attackRange && !enemy.IsFacingAnotherEnemy() && !enemy.IsFacingObject())
            {
                float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed / 2f;
                float speedDiff = targetSpeed - rb.velocity.x;
                float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
                float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

                rb.AddForce(movement * Vector2.right);
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y);
            }
        }
    }

    private bool PlayerInAttackRange()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x - 0.2f + (attackRange * 0.5f) * enemy.lastXDir, coll.bounds.center.y + 0.2f), new Vector2(attackRange, 2f), 0f, Vector2.left, .1f, enemy.player);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x - 0.2f + (attackRange * 0.5f) * enemy.lastXDir, coll.bounds.center.y + 0.2f), new Vector2(attackRange, 2f));
        }
    }
}
