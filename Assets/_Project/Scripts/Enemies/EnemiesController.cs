using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    #region Variables
    private StatsController enemyStats;

    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask dontMoveIfFacing;
    public Rigidbody2D rb;
    private BoxCollider2D coll;
    public Animator animator;

    private bool isDead = false;
    public bool isAggroed = false;
    private readonly int[] directionsList = { -1, 1 };

    [Header("Attack")]
    private bool canAttack = true;
    public bool isAttacking = false;
    public float attackStaminaCost = 15f;

    [Header("Movement")]
    public bool canMove = true;
    public float gravity;
    public float lastXDir;
    public bool movesRandomly;
    public bool wandering;
    private float movement;
    private float movementTime;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        gravity = rb.gravityScale;

        if (movesRandomly && wandering)
        {
            Debug.LogError("Enemy cannot both move randomly and wander.");
        }
    }

    // Update is called once per frame
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

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravity * enemyStats.fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (!isAggroed)
        {
            if (IsFacingObject())
            {
                lastXDir *= -1;
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                if (canMove)
                {
                    if (movesRandomly)
                    {
                        StartCoroutine(RandomMovement());
                    }
                    else if (wandering)
                    {

                    }
                }
            }
        }

        Debug.Log(WillFall());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private IEnumerator RandomMovement()
    {
        canMove = false;

        if (!CantChangeDirection())
        {
            lastXDir = directionsList[Random.Range(0, 2)];
            transform.localScale = new Vector3(-1 * lastXDir * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        float targetSpeed = lastXDir * enemyStats.movementSpeed;
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
        if (IsFacingObject() || movementTime <= 0f)
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

    public bool IsFacingObject()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, new Vector2(lastXDir, 0f), .1f, dontMoveIfFacing);
    }

    public bool CantChangeDirection()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, new Vector2(lastXDir * -1, 0f), .1f, dontMoveIfFacing);
    }

    public bool WillFall()
    {
        return !Physics2D.BoxCast(new Vector2(coll.bounds.center.x + coll.bounds.size.x * lastXDir, coll.bounds.center.y), new Vector2(coll.bounds.size.x / 2, coll.bounds.size.y), 0f, Vector2.down, .1f, ground);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(coll.bounds.center.x + coll.bounds.size.x * lastXDir, coll.bounds.center.y, coll.bounds.center.z), new Vector3(coll.bounds.size.x / 2, coll.bounds.size.y, coll.bounds.size.z));
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
