using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private StatsController playerStats;

    [SerializeField] private LayerMask jumpableGround;
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    private float lastXDir = 1;

    private bool isDead = false;

    private bool canDodge = true;
    private bool isDodging;
    public float dodgeStaminaCost = 20f;

    public float climbingStaminaCost = 3f;

    private float rotationCounter = 0f;
    private bool canFlip = true;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDodging || isDead)
        {
            return;
        }

        if(playerStats.health <= 0)
        {
            Die();
        }

        float dirX = Input.GetAxisRaw("Horizontal");

        if(dirX != 0)
        {
            lastXDir = dirX;
        }

        rb.velocity = new Vector2(dirX * playerStats.movementSpeed, rb.velocity.y);

        if (Input.GetButtonUp("Jump") || IsGrounded() || !IsNextToWall() || playerStats.stamina == 0f)
        {
            CancelInvoke(nameof(Climb));
            playerStats.UseStamina(0f, false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if(IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, playerStats.jumpPower);
            } 
            else if(IsNextToWall() && playerStats.stamina >= climbingStaminaCost * 0.1f)
            {
                InvokeRepeating(nameof(Climb), 0f, 0.01f);
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(IsGrounded())
            {
                if(canDodge && playerStats.stamina >= dodgeStaminaCost)
                {
                    StartCoroutine(Dodge());
                }
            }
            else
            {
                if(canFlip)
                {
                    canFlip = false;
                    GameObject.FindGameObjectWithTag("DoAFlip").GetComponent<Text>().color = Color.white;
                    InvokeRepeating(nameof(DoAFlip), 0f, 0.01f);
                }
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private bool IsNextToWall()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpableGround) 
            || Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);
    }

    private void Climb()
    {
        playerStats.UseStamina(climbingStaminaCost * 0.1f, true);
        rb.velocity = new Vector2(rb.velocity.x, playerStats.climbingSpeed);
    }

    private IEnumerator Dodge()
    {
        canDodge = false;
        isDodging = true;
        rb.velocity = new Vector2(lastXDir * playerStats.movementSpeed * playerStats.dashPower, rb.velocity.y);
        playerStats.UseStamina(dodgeStaminaCost, true);
        yield return new WaitForSeconds(playerStats.dodgingTime);
        isDodging = false;
        playerStats.UseStamina(0f, false);
        yield return new WaitForSeconds(playerStats.dodgeCooldown);
        canDodge = true;
    }

    private void DoAFlip()
    {
        rotationCounter += 10f;
        GetComponent<Rigidbody2D>().rotation += 10f;
        if(rotationCounter >= 360f)
        {
            GetComponent<Rigidbody2D>().rotation = 0f;
            rotationCounter = 0f;
            GameObject.FindGameObjectWithTag("DoAFlip").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            canFlip = true;
            CancelInvoke(nameof(DoAFlip));
        }
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
