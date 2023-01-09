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
    [SerializeField] private LayerMask wall;
    [SerializeField] private LayerMask dontMoveIfFacing;
    [SerializeField] private LayerMask enemy;
    public Rigidbody2D rb;
    private BoxCollider2D coll;
    private CircleCollider2D circleColl;
    public GameObject triggerTarget;
    public Animator animator;

    public float lastXDir = 1;

    private bool isDead = false;

    [Header ("Attack")]
    public bool isAttacking = false;
    private bool canAttack = true;
    public float attackStaminaCost = 15f;
    private GameObject attackTarget;
    List<GameObject> targetsHit;

    [Header("Skills")]
    public GameObject[] skills = new GameObject[3];
    public int chosenSkillSlot;
    [SerializeField] public GameObject chosenSkill;
    public bool skillCancelled = false;
    public bool skillIsLoading = false;
    private float skillLoadingTime = 0f;

    [Header("Dodge")]
    private bool canDodge = true;
    private bool isDodging;
    public float dodgeStaminaCost = 20f;

    [Header("Rest")]
    public bool canMove = true;
    private float rotationCounter = 0f;
    private bool canFlip = true;
    public float gravity;
    public bool isJumping;
    public bool jumpInputReleased;
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
        targetsHit = new List<GameObject>();

        #region Load Skills
        for (int i = 0; i < gameManager.saveData.playerSkills.Length; i++)
        {
            if (gameManager.saveData.playerSkills[i] != -1)
            {
                skills[i] = gameManager.skills[gameManager.saveData.playerSkills[i]];
            }
        }

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
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (isDodging || isDead)
            {
                return;
            }

            if (playerStats.health <= 0)
            {
                Die();
            }

            #region Movement
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
            #endregion

            #region Animations
            if (canMove && !IsFacingObject())
            {
                animator.SetFloat("Direction", Mathf.Abs(dirX));
            }
            /*else if (!canMove && !IsFacingObject())
            {
                rb.velocity = new Vector2(0f, 0f);
                animator.SetFloat("Direction", Mathf.Abs(0f));
            }*/
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                animator.SetFloat("Direction", Mathf.Abs(0f));
            }

            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsAttacking", isAttacking);
            #endregion

            #region Jump
            if (Input.GetButtonDown("Jump") && canMove && IsGrounded())
            {
                rb.AddForce(Vector2.up * playerStats.jumpPower, ForceMode2D.Impulse);
                isJumping = true;
                jumpInputReleased = false;
            }

            if (Input.GetButtonUp("Jump"))
            {
                jumpInputReleased = true;

                if (rb.velocity.y > 0 && isJumping)
                {
                    rb.AddForce((1 - playerStats.jumpCutMultiplier) * rb.velocity.y * Vector2.down, ForceMode2D.Impulse);
                }
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
            #endregion

            #region Dodge
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
            else if (Input.GetKeyDown(KeyCode.LeftShift) && isAttacking)
            {
                if (IsGrounded() && canDodge && playerStats.stamina >= dodgeStaminaCost && !IsNextToWall())
                {
                    StopAttack();
                    EndAttackCooldown();
                    StartCoroutine(Dodge());
                }
            }
            #endregion

            #region Attack
            if (Input.GetButtonDown("Fire1") && canAttack && playerStats.stamina >= attackStaminaCost && !skillIsLoading)
            {
                Attack();
            }
            #endregion

            #region Skills
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
            #endregion
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
        return Physics2D.CircleCast(circleColl.bounds.center, circleColl.radius, Vector2.down, .1f, jumpableGround);
    }

    public bool IsNextToWall()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, wall) 
            || Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, wall);
    }

    public bool IsFacingObject()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, new Vector2(lastXDir, 0f), .1f, dontMoveIfFacing);
    }

    private IEnumerator Dodge()
    {
        playerStats.isInvincible = true;
        canDodge = false;
        isDodging = true;
        rb.gravityScale = 0f;
        coll.isTrigger = true;
        circleColl.isTrigger = true;
        rb.velocity = new Vector2(lastXDir * playerStats.movementSpeed * playerStats.dashPower, rb.velocity.y);
        playerStats.UseStamina(dodgeStaminaCost, true);
        yield return new WaitForSeconds(playerStats.dodgingTime);
        isDodging = false;
        coll.isTrigger = false;
        circleColl.isTrigger = false;
        rb.gravityScale = gravity;
        playerStats.UseStamina(0f, false);
        playerStats.isInvincible = false;
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

    private void Attack()
    {
        canAttack = false;
        isAttacking = true;
        canMove = false;
    }

    private bool AttackHit()
    {
        RaycastHit2D attackArea;

        if (lastXDir < 0)
        {
            attackArea = Physics2D.BoxCast(new Vector2(coll.bounds.center.x + (coll.bounds.size.x * lastXDir) - .1f, coll.bounds.center.y - .1f), new Vector2(coll.bounds.size.x * 2f + .2f, coll.bounds.size.y + .2f), 0f, Vector2.right * lastXDir, .1f, enemy);
        }
        else
        {
            attackArea = Physics2D.BoxCast(new Vector2(coll.bounds.center.x + (coll.bounds.size.x * lastXDir) + .1f, coll.bounds.center.y - .1f), new Vector2(coll.bounds.size.x * 2f + .2f, coll.bounds.size.y + .2f), 0f, Vector2.right * lastXDir, .1f, enemy);
        }

        if (attackArea)
        {
            attackTarget = attackArea.collider.gameObject;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartAttack()
    {
        targetsHit.Clear();
        playerStats.UseStamina(attackStaminaCost, true);
        InvokeRepeating(nameof(TryAttack), 0f, 0.01f);
    }

    private void StopAttack()
    {
        CancelInvoke(nameof(TryAttack));
        targetsHit.Clear();
    }

    private void EndAttackCooldown()
    {
        isAttacking = false;
        canMove = true;
        playerStats.UseStamina(0f, false);
        canAttack = true;
    }

    private void TryAttack()
    {
        if (AttackHit() && !targetsHit.Contains(attackTarget))
        {
            float damage;

            targetsHit.Add(attackTarget);
            attackTarget.TryGetComponent<StatsController>(out StatsController targetStats);

            if (targetStats.defence >= playerStats.attack / 2)
            {
                damage = playerStats.attack / 2;
            }
            else
            {
                damage = playerStats.attack - targetStats.defence;
            }

            targetStats.DealDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            #region Attack HitBox
            if (lastXDir < 0)
            {
                Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + (coll.bounds.size.x * lastXDir) - .1f, coll.bounds.center.y - .1f), new Vector2(coll.bounds.size.x * 2f + .2f, coll.bounds.size.y + .2f));
            }
            else
            {
                Gizmos.DrawWireCube(new Vector2(coll.bounds.center.x + (coll.bounds.size.x * lastXDir) + .1f, coll.bounds.center.y - .1f), new Vector2(coll.bounds.size.x * 2f + .2f, coll.bounds.size.y + .2f));
            }
            
            #endregion
        }
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
        gameManager.LoadGame();

        /*Destroy(gameObject);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().RespawnPlayer();*/
    }
}
