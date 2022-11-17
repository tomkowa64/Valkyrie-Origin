using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private StatsController playerStats;
    private GameManager gameManager;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask dontMoveIfFacing;
    public Rigidbody2D rb;
    private BoxCollider2D coll;
    private CircleCollider2D circleColl;
    public GameObject triggerTarget;
    public Animator animator;

    public float lastXDir = 1;

    private bool isDead = false;

    [Header ("Attack")]
    private bool canAttack = true;
    public bool isAttacking = false;
    public float attackStaminaCost = 15f;

    [Header("Skills")]
    public GameObject[] skills;
    public int chosenSkillSlot;
    [SerializeField] private GameObject chosenSkill;
    public bool skillCancelled = false;
    public bool skillIsLoading = false;
    private float skillLoadingTime = 0f;

    [Header("Dodge")]
    private bool canDodge = true;
    private bool isDodging;
    public float dodgeStaminaCost = 20f;

    [Header("Climb")]
    public float climbingStaminaCost = 3f;

    [Header("Rest")]
    public bool canMove = true;
    private float rotationCounter = 0f;
    private bool canFlip = true;
    public float gravity;
    public bool isJumping;
    public bool jumpInputReleased;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        circleColl = GetComponent<CircleCollider2D>();
        gravity = rb.gravityScale;
        GetComponent<LineRenderer>().positionCount = 0;
        chosenSkillSlot = gameManager.saveData.chosenSkillSlot;

        if (chosenSkillSlot > 0 && chosenSkillSlot <= 3)
        {
            chosenSkill = skills[chosenSkillSlot - 1];
        }

        foreach (GameObject skill in skills)
        {
            if (skill != null)
            {
                skill.GetComponent<SkillController>().cdTimer = 0f;
                skill.GetComponent<SkillController>().onCooldown = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDodging || isDead)
        {
            return;
        }

        if (playerStats.health <= 0)
        {
            Die();
        }

        float dirX = Input.GetAxisRaw("Horizontal");

        if (dirX != 0 && canMove)
        {
            lastXDir = dirX;
            transform.localScale = new Vector3(dirX, transform.localScale.y, transform.localScale.z);
        }

        if (canMove && !IsFacingObject())
        {
            float targetSpeed = dirX * playerStats.movementSpeed;
            float speedDiff = targetSpeed - rb.velocity.x;
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerStats.acceleration : playerStats.decceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, playerStats.velocityPower) * Mathf.Sign(speedDiff);

            rb.AddForce(movement * Vector2.right);
        }

        if (canMove)
        {
            animator.SetFloat("Speed", Mathf.Abs(dirX * playerStats.movementSpeed));
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = playerStats.coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && canMove)
        {
            jumpInputReleased = false;
            jumpBufferCounter = playerStats.jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rb.AddForce(Vector2.up * playerStats.jumpPower, ForceMode2D.Impulse);
            isJumping = true;
            jumpBufferCounter = 0f;
        }
        else if (Input.GetButtonDown("Jump") && canMove && IsNextToWall() && playerStats.stamina >= climbingStaminaCost * 0.1f)
        {
            InvokeRepeating(nameof(Climb), 0f, 0.01f);
        }

        if (Input.GetButtonUp("Jump"))
        {
            coyoteTimeCounter = 0f;
            jumpInputReleased = true;

            if (rb.velocity.y > 0 && isJumping)
            {
                rb.AddForce((1 - playerStats.jumpCutMultiplier) * rb.velocity.y * Vector2.down, ForceMode2D.Impulse);
            }
        }

        if (Input.GetButtonUp("Jump") || IsGrounded() || !IsNextToWall() || playerStats.stamina == 0f)
        {
            CancelInvoke(nameof(Climb));
            playerStats.UseStamina(0f, false);
        }

        if (IsGrounded() && jumpInputReleased)
        {
            isJumping = false;
        }

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravity * playerStats.fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canMove)
        {
            if (IsGrounded())
            {
                if (canDodge && playerStats.stamina >= dodgeStaminaCost && !IsNextToWall())
                {
                    StartCoroutine(Dodge());
                }
            }
            else
            {
                if (canFlip)
                {
                    canFlip = false;
                    InvokeRepeating(nameof(DoAFlip), 0f, 0.01f);
                }
            }
        }

        if (Input.GetButtonDown("Fire1") && canAttack && playerStats.stamina >= attackStaminaCost && !skillIsLoading)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetButtonDown("Fire1") && skillIsLoading)
        {
            skillCancelled = true;
            skillIsLoading = false;
            GetComponent<LineRenderer>().positionCount = 0;
        }

        if (Input.GetButtonDown("Fire2") && chosenSkill != null && !skillCancelled)
        {
            if (chosenSkill.GetComponent<SkillController>().playerCanMoveWhileLoading)
            {
                skillIsLoading = true;
                InvokeRepeating(nameof(LoadSkill), 0f, 0.01f);
            }
            else
            {
                if (IsGrounded())
                {
                    skillIsLoading = true;
                    InvokeRepeating(nameof(LoadSkill), 0f, 0.01f);
                }
            }
        }

        if (skillCancelled)
        {
            CancelInvoke(nameof(LoadSkill));
            CancelLoading();
        }

        if (Input.GetButtonUp("Fire2") && chosenSkill != null)
        {
            CancelInvoke(nameof(LoadSkill));
            skillIsLoading = false;

            if (skillCancelled)
            {
                skillCancelled = false;
            }
            else
            {
                if (chosenSkill.GetComponent<SkillController>().playerCanMoveWhileLoading)
                {
                    UseSkill();
                }
                else
                {
                    if (IsGrounded())
                    {
                        UseSkill();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !skillIsLoading)
        {
            if (skills[0] != null)
            {
                chosenSkill = skills[0];
                chosenSkillSlot = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !skillIsLoading)
        {
            if (skills[1] != null)
            {
                chosenSkill = skills[1];
                chosenSkillSlot = 2;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !skillIsLoading)
        {
            if (skills[2] != null)
            {
                chosenSkill = skills[2];
                chosenSkillSlot = 3;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerTarget = collision.gameObject;

        if (triggerTarget.layer == LayerMask.NameToLayer("Ground"))
        {
            coll.isTrigger = false;
        }
    }

    private bool IsGrounded()
    {
        //return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        return Physics2D.CircleCast(circleColl.bounds.center, circleColl.radius, Vector2.down, .01f, jumpableGround);
    }

    public bool IsNextToWall()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpableGround) 
            || Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);
    }

    public bool IsFacingObject()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, new Vector2(lastXDir, 0f), .1f, dontMoveIfFacing);
    }

    private void Climb()
    {
        playerStats.UseStamina(climbingStaminaCost * 0.1f, true);
        rb.velocity = new Vector2(rb.velocity.x, playerStats.climbingSpeed);
    }

    private IEnumerator Dodge()
    {
        gravity = rb.gravityScale;
        canDodge = false;
        isDodging = true;
        rb.gravityScale = 0f;
        coll.isTrigger = true;
        rb.velocity = new Vector2(lastXDir * playerStats.movementSpeed * playerStats.dashPower, rb.velocity.y);
        playerStats.UseStamina(dodgeStaminaCost, true);
        yield return new WaitForSeconds(playerStats.dodgingTime);
        isDodging = false;
        coll.isTrigger = false;
        rb.gravityScale = gravity;
        playerStats.UseStamina(0f, false);
        yield return new WaitForSeconds(playerStats.dodgeCooldown);
        canDodge = true;
    }

    private void DoAFlip()
    {
        rotationCounter += 10f;
        GetComponent<Rigidbody2D>().rotation += 10f;
        if (rotationCounter >= 360f)
        {
            GetComponent<Rigidbody2D>().rotation = 0f;
            rotationCounter = 0f;
            canFlip = true;
            CancelInvoke(nameof(DoAFlip));
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;
        playerStats.UseStamina(attackStaminaCost, true);
        yield return new WaitForSeconds(playerStats.attackingTime);
        isAttacking = false;
        playerStats.UseStamina(0f, false);
        yield return new WaitForSeconds(playerStats.attackCooldown);
        canAttack = true;
    }

    private void LoadSkill()
    {
        if (!chosenSkill.GetComponent<SkillController>().onCooldown)
        {
            if (chosenSkill.GetComponent<SkillController>().manaCost <= playerStats.mana)
            {
                skillLoadingTime += 0.01f;
                canMove = chosenSkill.GetComponent<SkillController>().playerCanMoveWhileLoading;

                foreach (MonoBehaviour script in chosenSkill.GetComponents<MonoBehaviour>())
                {
                    if (script.GetType().ToString() != "SkillController" && script.GetType().ToString() != "UnityEngine.UI.Image")
                    {
                        script.Invoke("LoadSkill", 0f);
                    }
                }
            }
            else
            {
                Debug.Log("Not enough mana");
            }
        }
        else
        {
            Debug.Log("Skill on cooldown");
        }
    }

    private void CancelLoading()
    {
        skillLoadingTime = 0f;
        canMove = true;

        foreach (MonoBehaviour script in chosenSkill.GetComponents<MonoBehaviour>())
        {
            if (script.GetType().ToString() != "SkillController" && script.GetType().ToString() != "UnityEngine.UI.Image")
            {
                script.Invoke("ResetLoading", 0f);
            }
        }
    }

    private void UseSkill()
    {
        if (skillLoadingTime > 0f)
        {
            skillLoadingTime = 0f;
            canMove = true;

            if (!chosenSkill.GetComponent<SkillController>().onCooldown)
            {
                if (chosenSkill.GetComponent<SkillController>().manaCost <= playerStats.mana)
                {
                    playerStats.mana -= chosenSkill.GetComponent<SkillController>().manaCost;

                    foreach (MonoBehaviour script in chosenSkill.GetComponents<MonoBehaviour>())
                    {
                        if (script.GetType().ToString() != "SkillController" && script.GetType().ToString() != "UnityEngine.UI.Image")
                        {
                            script.Invoke("UseSkill", 0f);
                        }
                        else if (script.GetType().ToString() != "UnityEngine.UI.Image")
                        {
                            script.Invoke("StartCooldown", 0f);
                        }
                    }
                }
                else
                {
                    Debug.Log("Not enough mana");
                }
            }
            else
            {
                Debug.Log("Skill on cooldown");
            }
        }
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().RespawnPlayer();
    }
}
