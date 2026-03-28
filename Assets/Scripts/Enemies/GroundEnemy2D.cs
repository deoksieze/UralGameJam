using System.Collections;
using UnityEngine;

public class GroundEnemy2D : MonoBehaviour
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
    public float pointReachDistance = 0.3f;

    [Header("Detect")]
    public float detectRadius = 5f;
    public float loseRadius = 7f;
    public float attackRadius = 1.2f;
    public LayerMask playerLayer;

    [Header("Ground Checks")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform frontCheck;
    public float groundCheckDistance = 0.2f;
    public float frontGroundCheckDistance = 0.5f;
    public float wallCheckDistance = 0.2f;

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
    private int moveDirection = 1;

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
            UpdateDirectionLogic();
        }

        UpdateFlip();
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (isKnockedBack || isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
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

    private void UpdateDirectionLogic()
    {
        if (state == EnemyState.Chase && player != null)
        {
            if (player.position.x > transform.position.x)
                moveDirection = 1;
            else if (player.position.x < transform.position.x)
                moveDirection = -1;
        }
        else if (state == EnemyState.Patrol && patrolPoints != null && patrolPoints.Length > 0)
        {
            float targetX = patrolPoints[currentPointIndex].position.x;

            if (targetX > transform.position.x)
                moveDirection = 1;
            else if (targetX < transform.position.x)
                moveDirection = -1;
        }
    }

    private void MoveEnemy()
    {
        float speed = 0f;

        if (state == EnemyState.Patrol)
            speed = patrolSpeed;
        else if (state == EnemyState.Chase)
            speed = chaseSpeed;
        else
            speed = 0f;

        bool grounded = IsGrounded();
        bool groundAhead = IsGroundAhead();
        bool wallAhead = IsWallAhead();

        float moveX = 0f;

        if (state == EnemyState.Patrol)
        {
            if (CanMoveForward(grounded, groundAhead, wallAhead))
            {
                moveX = moveDirection * speed;
            }
            else
            {
                NextPatrolPoint();
            }

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                float distToPoint = Mathf.Abs(transform.position.x - patrolPoints[currentPointIndex].position.x);
                if (distToPoint <= pointReachDistance)
                    NextPatrolPoint();
            }
        }
        else if (state == EnemyState.Chase)
        {
            if (CanMoveForward(grounded, groundAhead, wallAhead))
                moveX = moveDirection * speed;
        }

        rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);
    }

    private bool CanMoveForward(bool grounded, bool groundAhead, bool wallAhead)
    {
        return grounded && groundAhead && !wallAhead;
    }

    private bool IsGrounded()
    {
        Vector2 origin = groundCheck != null ? groundCheck.position : transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool IsGroundAhead()
    {
        Vector2 origin = frontCheck != null
            ? (Vector2)frontCheck.position
            : (Vector2)transform.position + new Vector2(moveDirection * 0.4f, 0f);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, frontGroundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool IsWallAhead()
    {
        Vector2 origin = groundCheck != null ? groundCheck.position : transform.position;
        Vector2 dir = moveDirection == 1 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void NextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        currentPointIndex++;
        if (currentPointIndex >= patrolPoints.Length)
            currentPointIndex = 0;

        float targetX = patrolPoints[currentPointIndex].position.x;
        moveDirection = targetX >= transform.position.x ? 1 : -1;
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
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

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

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
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
        if (moveDirection > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDirection < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRadius);

        Vector3 groundOrigin = groundCheck != null ? groundCheck.position : transform.position;
        Vector3 frontOrigin = frontCheck != null ? frontCheck.position : transform.position + new Vector3(moveDirection * 0.4f, 0f, 0f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundOrigin, groundOrigin + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(frontOrigin, frontOrigin + Vector3.down * frontGroundCheckDistance);

        Gizmos.color = Color.magenta;
        Vector3 wallDir = moveDirection == 1 ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(groundOrigin, groundOrigin + wallDir * wallCheckDistance);
    }
}