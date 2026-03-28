using System.Collections;
using UnityEngine;

public class FlyingEnemy2D : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Stunned,
        Dead
    }

    [Header("Links")]
    public Rigidbody2D rb;
    public Transform[] patrolPoints;
    public Transform player;
    public Collider2D attackCollider;

    [Header("Move")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float pointReachDistance = 0.2f;

    [Header("Detect")]
    public float detectRadius = 5f;
    public float loseRadius = 7f;
    public float attackRadius = 1.4f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public float attackCooldown = 1f;
    public float attackActiveTime = 0.25f;
    public int damage = 1;

    [Header("Health")]
    public int health = 5;
    public float knockBackTime = 0.25f;

    private EnemyState state = EnemyState.Patrol;
    private int currentPointIndex = 0;
    private float nextAttackTime = 0f;

    private bool isKnockedBack = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private Coroutine attackRoutine;
    private Coroutine knockbackRoutine;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;

        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void Update()
    {
        if (isDead)
            return;

        if (!isKnockedBack)
        {
            DetectPlayer();
            UpdateState();
        }

        UpdateFlip();
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (isKnockedBack || isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        MoveEnemy();
    }

    private void DetectPlayer()
    {
        if (player == null)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRadius, playerLayer);
            if (hit != null)
                player = hit.transform;
        }
    }

    private void UpdateState()
    {
        if (player == null)
        {
            state = EnemyState.Patrol;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRadius)
        {
            state = EnemyState.Attack;
            TryAttack();
        }
        else if (distance <= detectRadius)
        {
            state = EnemyState.Chase;
        }
        else if (distance > loseRadius)
        {
            player = null;
            state = EnemyState.Patrol;
        }
        else
        {
            state = EnemyState.Chase;
        }
    }

    private void MoveEnemy()
    {
        Vector2 targetPos = transform.position;
        float speed = 0f;

        if (state == EnemyState.Patrol)
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            targetPos = patrolPoints[currentPointIndex].position;
            speed = patrolSpeed;

            float dist = Vector2.Distance(transform.position, targetPos);
            if (dist <= pointReachDistance)
                NextPatrolPoint();
        }
        else if (state == EnemyState.Chase)
        {
            if (player == null)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            targetPos = player.position;
            speed = chaseSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (targetPos - rb.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void NextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        currentPointIndex++;
        if (currentPointIndex >= patrolPoints.Length)
            currentPointIndex = 0;
    }

    private void TryAttack()
    {
        if (isAttacking || isKnockedBack || isDead)
            return;

        if (Time.time < nextAttackTime)
            return;

        attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        state = EnemyState.Attack;
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = Vector2.zero;

        if (attackCollider != null)
            attackCollider.enabled = true;

        yield return new WaitForSeconds(attackActiveTime);

        if (attackCollider != null)
            attackCollider.enabled = false;

        isAttacking = false;
    }

    public void TakeDamage(int amount, Vector2 knockBack)
    {
        if (isDead)
            return;

        health -= amount;

        if (health <= 0)
        {
            Die();
            return;
        }

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(knockBack));
    }

    private IEnumerator KnockbackRoutine(Vector2 knockBack)
    {
        isKnockedBack = true;
        state = EnemyState.Stunned;

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        isAttacking = false;

        if (attackCollider != null)
            attackCollider.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockBack, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackTime);

        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void Die()
    {
        isDead = true;
        state = EnemyState.Dead;

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        if (attackCollider != null)
            attackCollider.enabled = false;

        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void UpdateFlip()
    {
        if (player != null && state != EnemyState.Patrol)
        {
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (patrolPoints[currentPointIndex].position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else if (patrolPoints[currentPointIndex].position.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRadius);
    }
}