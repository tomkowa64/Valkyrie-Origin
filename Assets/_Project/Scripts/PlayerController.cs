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

    private bool canDodge = true;
    private bool isDodging;
    private float dodgingTime = 0.2f;
    private float dodgeCooldown = 0.5f;

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
        if(isDodging)
        {
            return;
        }

        float dirX = Input.GetAxisRaw("Horizontal");

        if(dirX != 0)
        {
            lastXDir = dirX;
        }

        rb.velocity = new Vector2(dirX * playerStats.movementSpeed, rb.velocity.y);

        if(Input.GetButtonDown("Jump"))
        {
            if(IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, playerStats.jumpPower);
            } 
            else if(IsNextToWall())
            {
                Debug.Log("Climbing");
                rb.velocity = new Vector2(rb.velocity.x, playerStats.jumpPower);
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(IsGrounded())
            {
                if(canDodge)
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

    private IEnumerator Dodge()
    {
        canDodge = false;
        isDodging = true;
        rb.velocity = new Vector2(lastXDir * playerStats.movementSpeed * 2.5f, rb.velocity.y);
        yield return new WaitForSeconds(dodgingTime);
        isDodging = false;
        yield return new WaitForSeconds(dodgeCooldown);
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
}
