using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCharge : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Collider2D coll;
    private float loadingTime = 0f;
    public float minChargePower;
    public float maxChargePower;
    private float chargePower;
    public float damage;
    private float destinationX;
    private float gravity;
    private GameObject lastTarget;
    public int sumOfEnemiesHit;
    public int hitsForLevelOne;

    private void MovePlayer()
    {
        rb.gravityScale = 0f;
        player.GetComponent<BoxCollider2D>().isTrigger = true;
        rb.AddForce(new Vector2(player.GetComponent<PlayerController>().lastXDir * Time.deltaTime * 50000f * chargePower, 0f));

        if (player.GetComponent<PlayerController>().triggerTarget.layer == LayerMask.NameToLayer("Enemy") && 
            player.GetComponent<PlayerController>().triggerTarget != lastTarget)
        {
            lastTarget = player.GetComponent<PlayerController>().triggerTarget;
            player.GetComponent<PlayerController>().triggerTarget.GetComponent<StatsController>().DealDamage(damage);

            if (sumOfEnemiesHit < hitsForLevelOne)
            {
                sumOfEnemiesHit++;
                GetComponent<SkillController>().mastering = sumOfEnemiesHit / hitsForLevelOne;
            }
        }
        
        if (player.GetComponent<PlayerController>().IsNextToWall())
        {
            StopMoving();
        }

        if (player.GetComponent<PlayerController>().lastXDir < 0)
        {
            if (player.transform.position.x <= destinationX)
            {
                StopMoving();
            }
        }
        else
        {
            if (player.transform.position.x >= destinationX)
            {
                StopMoving();
            }
        }
    }

    private void StopMoving()
    {
        CancelInvoke(nameof(MovePlayer));
        rb.velocity = new Vector2(0f, 0f);
        player.GetComponent<PlayerController>().canMove = true;
        player.GetComponent<BoxCollider2D>().isTrigger = false;
        rb.gravityScale = gravity;
    }

    private void UseSkill()
    {
        player.GetComponent<LineRenderer>().positionCount = 0;

        if (loadingTime > 0)
        {
            lastTarget = null;
            player = GameObject.FindGameObjectWithTag("Player");
            rb = player.GetComponent<Rigidbody2D>();
            coll = player.GetComponent<BoxCollider2D>();
            gravity = rb.gravityScale;

            if (chargePower < minChargePower)
            {
                chargePower = minChargePower;
            }

            destinationX = player.GetComponent<PlayerController>().lastXDir * chargePower + player.transform.position.x;

            player.GetComponent<PlayerController>().canMove = false;
            InvokeRepeating(nameof(MovePlayer), 0f, 0.01f);

            chargePower = minChargePower;
            loadingTime = 0f;
            GetComponent<SkillController>().loadingProgress = 0f;
        }
    }

    private void LoadSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        destinationX = player.GetComponent<PlayerController>().lastXDir * chargePower + player.transform.position.x;

        if (loadingTime >= 0.1f)
        {
            player.GetComponent<LineRenderer>().positionCount = 2;
            player.GetComponent<LineRenderer>().SetPosition(0, player.transform.position);
            player.GetComponent<LineRenderer>().SetPosition(1, new Vector3(destinationX + player.GetComponent<PlayerController>().lastXDir, player.transform.position.y, player.transform.position.z));

            if (GetComponent<SkillController>().mastering >= 1f)
            {
                if (chargePower < maxChargePower)
                {
                    chargePower += 0.05f;
                }

                if (chargePower >= maxChargePower)
                {
                    chargePower = maxChargePower;
                }
            }
            else
            {
                if (chargePower < maxChargePower)
                {
                    chargePower += 0.01f;
                }

                if (chargePower >= maxChargePower)
                {
                    chargePower = maxChargePower;
                }
            }

            GetComponent<SkillController>().loadingProgress = (chargePower - minChargePower) / (maxChargePower - minChargePower);
        }
        else
        {
            loadingTime += 0.01f;
        }
    }

    private void ResetLoading()
    {
        chargePower = minChargePower;
        loadingTime = 0f;
        GetComponent<SkillController>().loadingProgress = 0f;
    }
}
