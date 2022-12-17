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
    private CapsuleCollider2D coll;
    private float timeWithNoAggro;
    private float timeToHang;
    private bool isHanging;
    [SerializeField] private float followRange;

    #region Aggro ranges
    private float defaultAggroRangeTop;
    private float defaultAggroRangeBottom;
    private float defaultAggroRangeRight;
    private float defaultAggroRangeLeft;
    [SerializeField] private float hangingAggroRangeTop;
    [SerializeField] private float hangingAggroRangeBottom;
    [SerializeField] private float hangingAggroRangeRight;
    [SerializeField] private float hangingAggroRangeLeft;
    #endregion
    #endregion

    void Start()
    {
        enemy = GetComponent<EnemiesController>();
        enemyStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyScale = rb.transform.localScale;
        coll = GetComponent<CapsuleCollider2D>();
        timeWithNoAggro = 0f;
        timeToHang = Random.Range(5f, 10f);
        isHanging = false;
        defaultAggroRangeBottom = enemy.aggroRangeBottom;
        defaultAggroRangeTop = enemy.aggroRangeTop;
        defaultAggroRangeLeft = enemy.aggroRangeLeft;
        defaultAggroRangeRight = enemy.aggroRangeRight;
    }

    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (enemy.isAggroed)
            {
                timeWithNoAggro = 0f;

                if (isHanging)
                {
                    StopHanging();
                }
                else
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
            else
            {
                if (timeWithNoAggro < timeToHang)
                {
                    timeWithNoAggro += Time.deltaTime;
                }
                else
                {
                    Hang();
                }
            }
        }
    }

    private void StayInRange()
    {
        Vector2 playerPosition = player.GetComponent<Rigidbody2D>().transform.position;
        Vector2 enemyPosition = rb.transform.position;

        if (enemy.canMove)
        {
            enemy.lastXDir = Mathf.Sign(playerPosition.x - enemyPosition.x);
            rb.transform.localScale = new Vector3(-1 * Mathf.Sign(playerPosition.x - enemyPosition.x) * Mathf.Abs(enemyScale.x), enemyScale.y, enemyScale.z);

            if (Mathf.Abs(playerPosition.x - enemyPosition.x) > followRange && !enemy.IsFacingAnotherEnemy() && !enemy.IsFacingObject())
            {
                float targetSpeed = enemy.lastXDir * enemyStats.movementSpeed / 1.5f;
                float speedDiff = targetSpeed - rb.velocity.x;
                float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyStats.acceleration : enemyStats.decceleration;
                float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, enemyStats.velocityPower) * Mathf.Sign(speedDiff);

                rb.AddForce(movement * Vector2.right);
            }
        }
    }

    private void Hang()
    {
        if (IsCeilingAbove())
        {
            isHanging = true;
            enemy.canMove = false;
            enemy.canAttack = false;
            enemy.gravityWorking = false;
            rb.gravityScale = 0f;
            rb.AddForce(5f * Vector2.up);
            rb.transform.localScale = new Vector3(enemyScale.x, enemyScale.y * -1, enemyScale.z);
            enemy.aggroRangeTop = hangingAggroRangeTop;
            enemy.aggroRangeBottom = hangingAggroRangeBottom;
            enemy.aggroRangeLeft = hangingAggroRangeLeft;
            enemy.aggroRangeRight = hangingAggroRangeRight;
        }
        else
        {
            timeWithNoAggro = 0f;
        }
    }

    private void StopHanging()
    {
        isHanging = false;
        enemy.canMove = true;
        enemy.canAttack = true;
        enemy.gravityWorking = true;
        rb.transform.localScale = new Vector3(enemyScale.x, enemyScale.y, enemyScale.z);
        enemy.aggroRangeTop = defaultAggroRangeTop;
        enemy.aggroRangeBottom = defaultAggroRangeBottom;
        enemy.aggroRangeLeft = defaultAggroRangeLeft;
        enemy.aggroRangeRight = defaultAggroRangeRight;
        timeWithNoAggro = 0f;
        InvokeRepeating(nameof(HitPlayerOnLanding), 0.5f, 0.01f);
    }

    private void HitPlayerOnLanding()
    {
        if (CollideWithPlayer())
        {
            player.GetComponent<StatsController>().DealDamage(enemyStats.attack);
            CancelInvoke(nameof(HitPlayerOnLanding));
        }

        if (enemy.IsGrounded())
        {
            CancelInvoke(nameof(HitPlayerOnLanding));
        }
    }

    private IEnumerator UseBasicAttack()
    {
        enemy.canMove = false;
        enemy.canAttack = false;
        yield return new WaitForSeconds(0.5f);

        if (PlayerInAttackRange())
        {
            yield return new WaitForSeconds(0.5f);
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

    private bool PlayerInAttackRange()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x + 0.5f * enemy.lastXDir, coll.bounds.center.y + 0.25f), new Vector2(1f, 1f), 0f, Vector2.left, .1f, enemy.player);
    }

    private bool CollideWithPlayer()
    {
        return Physics2D.CapsuleCast(coll.bounds.center, coll.size, coll.direction, 0f, Vector2.down, .1f, enemy.player);
    }

    private bool IsCeilingAbove()
    {
        return Physics2D.BoxCast(new Vector2(coll.bounds.center.x, coll.bounds.center.y + coll.bounds.size.y + 3f), new Vector2(coll.bounds.size.x, coll.bounds.size.y + 6f), 0f, Vector2.up, .1f, enemy.ground);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + 0.5f * enemy.lastXDir, coll.bounds.center.y + 0.25f), new Vector2(1f, 1f));

            Gizmos.color = Color.black;

            Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x, coll.bounds.center.y + coll.bounds.size.y + 3f), new Vector2(coll.bounds.size.x, coll.bounds.size.y + 6f));
        }
    }
}
