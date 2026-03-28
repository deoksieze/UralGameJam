using System.Collections;
using UnityEngine;

public class GroundShooterEnemy2D : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Shoot,
        Stunned,
        Dead
    }

    [Header("Links")]
    public Rigidbody2D rb;
    public Transform[] patrolPoints;
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Move")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float pointReachDistance = 0.3f;

    [Header("Detect")]
    public float detectRadius = 7f;
    public float loseRadius = 9f;
    public float shootRadius = 5f;
    public LayerMask playerLayer;

    [Header("Ground Checks")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform frontCheck;
    public float groundCheckDistance = 0.2f;
    public float frontGroundCheckDistance = 0.5f;
    public float wallCheckDistance = 0.2f;

    [Header("Shoot")]
    public float shootCooldown = 1.2f;
    public float shootWindup = 0.15f;
    public float projectileSpeed = 8f;

    [Header("Health")]
    public int health = 5;
    public float knockBackTime = 0.25f;

    private EnemyState state = EnemyState.Patrol;
    private int currentPointIndex = 0;
    private int moveDirection = 1;
    private float nextShootTime = 0f;

    private bool isKnockedBack = false;
    private bool isShooting = false;
    private bool isDead = false;

    private Coroutine shootRoutine;
    private Coroutine knockbackRoutine;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (!isKnockedBack && !isShooting)
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

        if (isKnockedBack || isShooting)
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

        if (distance <= shootRadius)
        {
            state = EnemyState.Shoot;
            TryShoot();
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
        if (player != null && state != EnemyState.Patrol)
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

    private void TryShoot()
    {
        if (isShooting || isKnockedBack || isDead)
            return;

        if (Time.time < nextShootTime)
            return;

        shootRoutine = StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        state = EnemyState.Shoot;
        nextShootTime = Time.time + shootCooldown;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        yield return new WaitForSeconds(shootWindup);

        if (player != null && projectilePrefab != null && firePoint != null)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;

            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            EnemyProjectile2D projectile = bullet.GetComponent<EnemyProjectile2D>();
            if (projectile != null)
            {
                projectile.Launch(dir, projectileSpeed);
            }
            else
            {
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                    bulletRb.linearVelocity = dir * projectileSpeed;
            }
        }

        isShooting = false;
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

        if (shootRoutine != null)
            StopCoroutine(shootRoutine);

        isShooting = false;
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

        if (shootRoutine != null)
            StopCoroutine(shootRoutine);

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
        Gizmos.DrawWireSphere(transform.position, shootRadius);

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