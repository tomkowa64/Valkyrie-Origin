using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private StatsController playerStats;

    [SerializeField] private LayerMask jumpableGround;
    public Rigidbody2D rb;
    private BoxCollider2D coll;
    public GameObject triggerTarget;

    public float lastXDir = 1;

    private bool isDead = false;

    [Header ("Attack")]
    private bool canAttack = true;
    public bool isAttacking = false;
    public float attackStaminaCost = 15f;

    [Header("Skills")]
    public GameObject[] skills;
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
    private float rotationCounter = 0f;
    private bool canFlip = true;
    public bool canMove = true;
    public float gravity;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        foreach(GameObject skill in skills)
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

        if (canMove)
        {
            rb.velocity = new Vector2(dirX * playerStats.movementSpeed, rb.velocity.y);
        }

        if (Input.GetButtonUp("Jump") || IsGrounded() || !IsNextToWall() || playerStats.stamina == 0f)
        {
            CancelInvoke(nameof(Climb));
            playerStats.UseStamina(0f, false);
        }

        if (Input.GetButtonDown("Jump") && canMove)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, playerStats.jumpPower);
            } 
            else if (IsNextToWall() && playerStats.stamina >= climbingStaminaCost * 0.1f)
            {
                InvokeRepeating(nameof(Climb), 0f, 0.01f);
            }
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
                    GameObject.FindGameObjectWithTag("DoAFlip").GetComponent<Text>().color = Color.white;
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
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !skillIsLoading)
        {
            if (skills[1] != null)
            {
                chosenSkill = skills[1];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !skillIsLoading)
        {
            if (skills[2] != null)
            {
                chosenSkill = skills[2];
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
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    public bool IsNextToWall()
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
            GameObject.FindGameObjectWithTag("DoAFlip").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
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
                    if (script.GetType().ToString() != "SkillController")
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
            if (script.GetType().ToString() != "SkillController")
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
                        if (script.GetType().ToString() != "SkillController")
                        {
                            script.Invoke("UseSkill", 0f);
                        }
                        else
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
