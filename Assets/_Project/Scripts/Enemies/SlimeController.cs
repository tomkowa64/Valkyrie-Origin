using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    #region Variables
    private EnemiesController enemy;
    private StatsController enemyStats;
    private Rigidbody2D rb;
    private GameObject player;
    private CapsuleCollider2D coll;
    private bool isJumping;
    private bool isAttacking;
    [SerializeField] private float justJumpedTimer;
    [SerializeField] private float movementCdTimer;
    [SerializeField] private float movementCd;
    #endregion

    void Start()
    {
        enemy = GetComponent<EnemiesController>();
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        coll = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (!PauseController.gameIsPaused)
        {
            if (movementCdTimer > 0f)
            {
                movementCdTimer -= Time.deltaTime;
            }
            else
            {
                movementCdTimer = 0f;
            }

            if (justJumpedTimer > 0f)
            {
                justJumpedTimer -= Time.deltaTime;
            }
            else
            {
                justJumpedTimer = 0f;
            }

            if (justJumpedTimer == 0f && isJumping && enemy.IsGrounded())
            {
                isJumping = false;
                movementCdTimer = movementCd;
            }

            if (isJumping && isAttacking)
            {
                HitPlayer();
            }

            if (isJumping && HasCeilingAbove() && rb.velocity.y > 0)
            {
                rb.AddForce((1 - enemyStats.jumpCutMultiplier) * rb.velocity.y * Vector2.down, ForceMode2D.Impulse);
            }

            TryMove(enemy.isAggroed);
        }
    }

    private bool HasCeilingAbove()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x, coll.bounds.center.y + 0.5f), new Vector2(0.5f, 1f), 0f, Vector2.up, .1f, enemy.ground);
    }

    private bool CollideWithPlayer()
    {
        return Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.down, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.up, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.right, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.left, .1f, enemy.player);
    }

    private void Move(float x)
    {
        float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed * x;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

        rb.AddForce(enemyStats.jumpPower * Vector2.up, ForceMode2D.Impulse);
        rb.AddForce(movement * Vector2.right);

        isJumping = true;
        isAttacking = true;
        justJumpedTimer = 0.2f;
    }

    private void TryMove(bool isAggroed)
    {
        if (enemy.canMove && enemy.IsGrounded() && movementCdTimer <= 0f && !isJumping)
        {
            if (isAggroed)
            {
                enemy.lastXDir = Mathf.Sign(player.transform.position.x - transform.position.x);
                transform.localScale = new Vector3(enemy.lastXDir * transform.localScale.x, transform.localScale.y, transform.localScale.z);

                if (Mathf.Abs(player.transform.position.x - transform.position.x) * 100 > 300f)
                {
                    Move(20f);
                }
                else
                {
                    Move(Mathf.Abs(player.transform.position.x - transform.position.x) * 100 / enemyStats.movementSpeed);
                }
            }
            else
            {
                if (enemy.IsFacingObject() || enemy.IsFacingAnotherEnemy() || enemy.WillFall())
                {
                    enemy.lastXDir *= -1;
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }

                Move(20f);
            }

        }
    }

    private void HitPlayer()
    {
        if (CollideWithPlayer())
        {
            isAttacking = false;

            player.GetComponent<StatsController>().DealDamage(enemyStats.attack);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.black;

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x, coll.bounds.center.y + 0.5f), new Vector2(0.5f, 1f));
        }
    }
}
