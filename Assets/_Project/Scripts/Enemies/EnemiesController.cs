using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    #region Variables
    private StatsController enemyStats;

    public LayerMask ground;
    public LayerMask player;
    [SerializeField] private LayerMask dontMoveIfFacing;
    [SerializeField] private LayerMask enemies;
    public Rigidbody2D rb;
    private CapsuleCollider2D coll;
    public Animator animator;

    private bool isDead = false;
    private readonly int[] directionsList = { -1, 1 };

    [Header("Aggro")]
    public bool isAggroed = false;
    public float aggroRangeTop;
    public float aggroRangeBottom;
    public float aggroRangeRight;
    public float aggroRangeLeft;
    public float aggroCounter;
    public float aggroTime;

    [Header("Attack")]
    public bool canAttack = true;
    public bool isAttacking = false;
    public float attackStaminaCost = 15f;

    [Header("Movement")]
    public bool canMove = true;
    public float gravity;
    public bool gravityWorking = true;
    public float lastXDir;
    public bool movesRandomly;
    public bool wandering;
    private float movement;
    private float movementTime;
    #endregion

    void Start()
    {
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        gravity = rb.gravityScale;

        if (movesRandomly && wandering)
        {
            Debug.LogError("Enemy cannot both move randomly and wander.");
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (enemyStats.health <= 0)
        {
            Die();
        }

        if (gravityWorking)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * enemyStats.fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = gravity;
            }
        }

        if (!isAggroed)
        {
            if (canMove)
            {
                if (movesRandomly)
                {
                    StartCoroutine(RandomMovement());
                }
                else if (wandering)
                {
                    Wander();
                }
            }
        }

        if (SeePlayer())
        {
            if (aggroCounter < aggroTime)
            {
                aggroCounter += Time.deltaTime;
            }
            else
            {
                aggroCounter = aggroTime;
                isAggroed = true;
            }
        }
        else
        {
            if (aggroCounter > 0)
            {
                aggroCounter -= Time.deltaTime;
            }
            else
            {
                aggroCounter = 0f;
                isAggroed = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);

            if (!isAggroed)
            {
                isAggroed = true;
                aggroCounter = aggroTime;
            }
        }
    }

    private IEnumerator RandomMovement()
    {
        canMove = false;

        if(IsFacingObject() || IsFacingAnotherEnemy())
        {
            lastXDir *= -1;
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if (!CantChangeDirection())
        {
            lastXDir = directionsList[Random.Range(0, 2)];
            transform.localScale = new Vector3(-1 * lastXDir * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        float targetSpeed = lastXDir * enemyStats.movementSpeed / 2f;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
        movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

        movementTime = Random.Range(.5f, 1f);

        InvokeRepeating(nameof(TryToMove), 0f, 0.01f);

        float cooldown = Random.Range(1f, 5f);
        yield return new WaitForSeconds(cooldown);
        canMove = true;
    }

    private void TryToMove()
    {
        if (IsFacingObject() || IsFacingAnotherEnemy() || movementTime <= 0f)
        {
            movementTime = 0f;
            CancelInvoke(nameof(TryToMove));
        }
        else if (WillFall())
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            movementTime = 0f;
            CancelInvoke(nameof(TryToMove));
        }
        else
        {
            movementTime -= Time.deltaTime;
            rb.AddForce(movement * Vector2.right);
        }
    }

    private void Wander()
    {
        if (IsFacingObject() || IsFacingAnotherEnemy() || WillFall())
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            lastXDir *= -1;
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            float targetSpeed = lastXDir * enemyStats.movementSpeed / 2f / 1.5f;
            float speedDiff = targetSpeed - rb.velocity.x;
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
            movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

            rb.AddForce(movement * Vector2.right);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.CapsuleCast(coll.bounds.center, coll.size, coll.direction, 0f, Vector2.down, .1f, ground);
    }

    public bool IsFacingObject()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x, coll.bounds.center.y), new Vector2(coll.bounds.size.x, coll.bounds.size.y - 0.1f), 0f, new Vector2(lastXDir, 0f), .1f, dontMoveIfFacing);
    }

    public bool IsFacingAnotherEnemy()
    {
        if (Physics2D.BoxCast(new Vector2(coll.bounds.center.x, coll.bounds.center.y), new Vector2(coll.bounds.size.x, coll.bounds.size.y - 0.1f), 0f, new Vector2(lastXDir, 0f), .1f, enemies).collider.gameObject != gameObject)
        {
            return true;
        }

        return false;
    }

    private bool CantChangeDirection()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x, coll.bounds.center.y), new Vector2(coll.bounds.size.x, coll.bounds.size.y - 0.1f), 0f, new Vector2(lastXDir * -1, 0f), .1f, dontMoveIfFacing);
    }

    private bool WillFall()
    {
        return !Physics2D.BoxCast(new Vector2(coll.bounds.center.x + coll.bounds.size.x * lastXDir, coll.bounds.center.y), new Vector2(coll.bounds.size.x / 2, coll.bounds.size.y), 0f, Vector2.down, .1f, ground);
    }

    private bool SeePlayer()
    {
        if (transform.localScale.x > 0)
        {
            return Physics2D.BoxCast(new Vector2(coll.bounds.center.x - (aggroRangeLeft / 2) + (aggroRangeRight / 2), coll.bounds.center.y + aggroRangeTop / 2 - aggroRangeBottom / 2), new Vector2(aggroRangeLeft + aggroRangeRight, aggroRangeBottom + aggroRangeTop), 0f, Vector2.left, .1f, player);
        }
        else
        {
            return Physics2D.BoxCast(new Vector2(coll.bounds.center.x + (aggroRangeLeft / 2) - (aggroRangeRight / 2), coll.bounds.center.y + aggroRangeTop / 2 - aggroRangeBottom / 2), new Vector2(aggroRangeLeft + aggroRangeRight, aggroRangeBottom + aggroRangeTop), 0f, Vector2.right, .1f, player);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (transform.localScale.x > 0)
        {
            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x - (aggroRangeLeft / 2) + (aggroRangeRight / 2), coll.bounds.center.y + aggroRangeTop / 2 - aggroRangeBottom / 2), new Vector2(aggroRangeLeft + aggroRangeRight, aggroRangeBottom + aggroRangeTop));
        }
        else
        {
            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + (aggroRangeLeft / 2) - (aggroRangeRight / 2), coll.bounds.center.y + aggroRangeTop / 2 - aggroRangeBottom / 2), new Vector2(aggroRangeLeft + aggroRangeRight, aggroRangeBottom + aggroRangeTop));
        }
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
