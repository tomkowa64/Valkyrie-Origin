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
    [SerializeField] private bool isJumping;
    private bool isAttacking;
    [SerializeField] private bool jumpHitPlayer;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float jumpCooldownTimer;
    [SerializeField] private float justJumpedTimer;
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
    }

    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (enemy.isAggroed)
            {
                StayInRange(jumpRange);

                if (justJumpedTimer == 0f && jumpCooldownTimer == 0f && !isJumping && !isAttacking)
                {
                    StartCoroutine(JumpAttack());
                }
            }

            if (isJumping && CollideWithPlayer())
            {
                Debug.Log("player hit");
                isJumping = false;
                jumpHitPlayer = true;
                player.GetComponent<StatsController>().DealDamage(enemyStats.attack);
            }

            if (jumpHitPlayer && enemy.IsGrounded())
            {
                StartCoroutine(JumpBack());
            }

            if (justJumpedTimer == 0f && enemy.IsGrounded())
            {
                isJumping = false;
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
        }
    }

    private bool CollideWithPlayer()
    {
        return Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.down, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.up, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.right, .1f, enemy.player)
            || Physics2D.CapsuleCast(coll.bounds.center, coll.bounds.size, coll.direction, 0f, Vector2.left, .1f, enemy.player);
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

        enemy.canMove = true;
        enemy.canAttack = true;
        enemy.aggroCounter = enemy.aggroTime;
        enemy.isAggroed = true;
    }

    private IEnumerator JumpBack()
    {
        enemy.canMove = false;
        enemy.canAttack = false;
        jumpHitPlayer = false;

        yield return new WaitForSeconds(0.25f);

        float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed * 35f;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

        rb.AddForce(enemyStats.jumpPower * Vector2.up, ForceMode2D.Impulse);
        rb.AddForce(movement * Vector2.left);

        enemy.canMove = true;
        enemy.canAttack = true;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + (jumpRange / 2) * enemy.lastXDir, coll.bounds.center.y), new Vector2(jumpRange, 0.5f));
        }
    }
}
