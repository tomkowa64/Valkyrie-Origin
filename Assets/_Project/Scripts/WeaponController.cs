using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private BoxCollider2D coll;
    [SerializeField] private LayerMask enemy;
    private PlayerController player;
    private GameObject collisionTarget;
    private StatsController targetStats;
    private StatsController playerStats;
    private float damage;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isAttacking)
        {
            if(AttackHit())
            {
                collisionTarget.TryGetComponent<StatsController>(out targetStats);
                player.TryGetComponent<StatsController>(out playerStats);

                if(targetStats.defence >= playerStats.attack / 2)
                {
                    damage = playerStats.attack / 2;
                }
                else
                {
                    damage = playerStats.attack - targetStats.defence;
                }

                targetStats.DealDamage(damage);
                player.isAttacking = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionTarget = collision.gameObject;
    }

    private bool AttackHit()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, enemy);
    }
}
