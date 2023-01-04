using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    #region Variables
    private EnemiesController enemy;
    private StatsController enemyStats;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector3 enemyScale;
    private CapsuleCollider2D coll;
    private bool isJumping;
    private bool isJumpingBack;
    private bool isAttacking;
    private bool jumpHitPlayer;
    [SerializeField] private float jumpCooldown;
    private float jumpCooldownTimer;
    private float justJumpedTimer;
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    [SerializeField] private float jumpRange;
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
        isJumping = false;
        isAttacking = false;
        isJumpingBack = false;
        jumpHitPlayer = false;
    }

    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (enemy.isAggroed)
            {
                if (enemy.canMove && jumpCooldownTimer == 0f)
                {
                    StayInRange(jumpRange);
                }
                else if (enemy.canMove)
                {
                    StayInRange(attackRange);
                }

                if (!isJumping && !isAttacking && !isJumpingBack)
                {
                    if (PlayerInAttackRange() && attackCooldownTimer == 0f)
                    {
                        StartCoroutine(UseBasicAttack());
                        StartCoroutine(JumpBack(false));
                    }
                    else if (!PlayerInAttackRange() && justJumpedTimer == 0f && jumpCooldownTimer == 0f)
                    {
                        StartCoroutine(JumpAttack());
                    }
                }
            }

            if (isJumping && CollideWithPlayer())
            {
                isJumping = false;
                jumpHitPlayer = true;
                player.GetComponent<StatsController>().DealDamage(enemyStats.attack);
            }

            if (jumpHitPlayer && enemy.IsGrounded())
            {
                StartCoroutine(JumpBack(false));
            }

            if (isJumpingBack && justJumpedTimer == 0f && enemy.IsGrounded())
            {
                enemy.canMove = true;
                enemy.canAttack = true;
                isJumpingBack = false;
            }

            if (isJumping && enemy.IsGrounded() && justJumpedTimer == 0f)
            {
                float wolfPosition = transform.position.x;
                float playerPosition = player.transform.position.x;

                isJumping = false;
                if (enemy.lastXDir > 0)
                {
                    if (playerPosition > wolfPosition)
                    {
                        if (PlayerInAttackRange())
                        {
                            StartCoroutine(UseBasicAttack());
                            StartCoroutine(JumpBack(false));
                        }
                        else
                        {
                            StartCoroutine(JumpBack(true));
                        }
                    }
                    else
                    {
                        StartCoroutine(JumpBack(false));
                    }
                }
                else
                {
                    if (playerPosition < wolfPosition)
                    {
                        if (PlayerInAttackRange())
                        {
                            StartCoroutine(UseBasicAttack());
                            StartCoroutine(JumpBack(false));
                        }
                        else
                        {
                            StartCoroutine(JumpBack(true));
                        }
                    }
                    else
                    {
                        StartCoroutine(JumpBack(false));
                    }
                }
            }

            if (jumpCooldownTimer > 0f)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
            else
            {
                jumpCooldownTimer = 0f;
            }

            if (justJumpedTimer > 0f)
            {
                justJumpedTimer -= Time.deltaTime;
            }
            else
            {
                justJumpedTimer = 0f;
            }

            if (attackCooldownTimer > 0f)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
            else
            {
                attackCooldownTimer = 0f;
            }
        }
    }

    private void StayInRange(float followRange)
    {
        Vector2 playerPosition = player.GetComponent<Rigidbody2D>().transform.position;
        Vector2 enemyPosition = rb.transform.position;

        if (enemy.canMove)
        {
            enemy.lastXDir = Mathf.Sign(playerPosition.x - enemyPosition.x);
            rb.transform.localScale = new Vector3(-1 * Mathf.Sign(playerPosition.x - enemyPosition.x) * Mathf.Abs(enemyScale.x), enemyScale.y, enemyScale.z);

            if (Mathf.Abs(playerPosition.x - enemyPosition.x) > followRange && !enemy.IsFacingAnotherEnemy() && !enemy.IsFacingObject())
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

    private bool CollideWithPlayer()
    {
        return Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.down, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.up, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.right, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.left, .1f, enemy.player);
    }

    private bool PlayerInAttackRange()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x + (attackRange / 2) * enemy.lastXDir, coll.bounds.center.y), new Vector2(attackRange, 1f), 0f, Vector2.left, .1f, enemy.player);
    }

    private IEnumerator JumpAttack()
    {
        enemy.canMove = false;
        enemy.canAttack = false;

        isJumping = true;
        justJumpedTimer = 0.4f;
        yield return new WaitForSeconds(0.25f);

        float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed * 50f;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

        rb.AddForce(enemyStats.jumpPower * Vector2.up, ForceMode2D.Impulse);
        rb.AddForce(movement * Vector2.right);

        jumpCooldownTimer = jumpCooldown;

        enemy.aggroCounter = enemy.aggroTime;
        enemy.isAggroed = true;
    }

    private IEnumerator JumpBack(bool changeDirection)
    {
        enemy.canMove = false;
        enemy.canAttack = false;
        jumpHitPlayer = false;
        isJumpingBack = true;

        justJumpedTimer = 0.4f;
        yield return new WaitForSeconds(0.25f);

        float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed * 40f;
        if (changeDirection) targetSpeed *= -1;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

        rb.AddForce(enemyStats.jumpPower * Vector2.up, ForceMode2D.Impulse);
        rb.AddForce(movement * Vector2.left);
    }

    private IEnumerator UseBasicAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        if (PlayerInAttackRange())
        {
            player.GetComponent<StatsController>().DealDamage(enemyStats.attack);
        }

        enemy.aggroCounter = enemy.aggroTime;
        enemy.isAggroed = true;
        isAttacking = false;
        attackCooldownTimer = attackCooldown;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + (jumpRange / 2) * enemy.lastXDir, coll.bounds.center.y), new Vector2(jumpRange, 0.5f));

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + (attackRange / 2) * enemy.lastXDir, coll.bounds.center.y), new Vector2(attackRange, 1f));
        }
    }
}
