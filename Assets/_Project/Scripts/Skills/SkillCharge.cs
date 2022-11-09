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

    private void MovePlayer()
    {
        rb.gravityScale = 0f;
        player.GetComponent<BoxCollider2D>().isTrigger = true;
        rb.AddForce(new Vector2(player.GetComponent<PlayerController>().lastXDir * Time.deltaTime * 10000f * chargePower, 0f));

        if (player.GetComponent<PlayerController>().triggerTarget.layer == LayerMask.NameToLayer("Enemy") && 
            player.GetComponent<PlayerController>().triggerTarget != lastTarget)
        {
            lastTarget = player.GetComponent<PlayerController>().triggerTarget;
            player.GetComponent<PlayerController>().triggerTarget.GetComponent<StatsController>().DealDamage(damage);
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
        }
    }

    private void LoadSkill()
    {
        if (loadingTime >= 0.1f)
        {
            if (chargePower < maxChargePower)
            {
                chargePower += 0.1f;
            }

            if (chargePower >= maxChargePower)
            {
                chargePower = maxChargePower;
            }
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
    }
}
