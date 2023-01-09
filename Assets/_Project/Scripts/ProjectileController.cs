using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    #region Variables
    private Rigidbody2D rb;
    private Vector3 scale;
    public float maxRange;
    public float damage;
    public GameObject target;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    public float speed;
    private bool targetSet;
    public bool passThroughTerrain;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scale = rb.transform.localScale;
        targetSet = false;
        startPosition = transform.position;
    }

    void Update()
    {
        if (!targetSet && target != null)
        {
            targetPosition = target.transform.position;
            targetSet = true;

            if (targetPosition.x > transform.position.x)
            {
                rb.transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetPosition.y - transform.position.y, targetPosition.x - transform.position.x) * Mathf.Rad2Deg, Vector3.forward);
            }
            else
            {
                rb.transform.localScale = new Vector3(scale.x * -1, scale.y * -1, scale.z);
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetPosition.y - transform.position.y, targetPosition.x - transform.position.x) * Mathf.Rad2Deg, Vector3.forward);
            }
        }

        if (targetSet)
        {
            float angle = Mathf.Atan2(targetPosition.y - transform.position.y, targetPosition.x - transform.position.x);
            transform.Translate(Quaternion.Euler(0, 0, angle) * new Vector3(speed, 0, 0) * Time.deltaTime);
        }

        if (Vector2.Distance(startPosition, transform.position) >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<StatsController>().DealDamage(damage);
            Destroy(gameObject);
        }

        if (!passThroughTerrain)
        {
            if (collision.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
