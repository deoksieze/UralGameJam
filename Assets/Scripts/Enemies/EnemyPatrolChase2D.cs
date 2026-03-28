using UnityEngine;

public class EnemyPatrolChase2D : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("Links")]
    public Rigidbody2D rb;
    public Transform[] patrolPoints;
    public Transform player;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float pointReachDistance = 0.15f;

    [Header("Detection")]
    public float detectRadius = 5f;
    public float loseRadius = 7f;
    public float attackRadius = 1.2f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public float attackCooldown = 1f;
    public int damage = 1;

    private EnemyState state = EnemyState.Patrol;
    private int currentPointIndex = 0;
    private float nextAttackTime = 0f;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        DetectPlayer();
        UpdateState();
        FlipByTarget();
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Patrol)
            PatrolMove();

        if (state == EnemyState.Chase)
            ChaseMove();
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            state = EnemyState.Attack;
            TryAttack();
        }
        else if (distanceToPlayer <= detectRadius)
        {
            state = EnemyState.Chase;
        }
        else if (distanceToPlayer > loseRadius)
        {
            player = null;
            state = EnemyState.Patrol;
        }
        else
        {
            state = EnemyState.Chase;
        }
    }

    private void PatrolMove()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = patrolPoints[currentPointIndex].position;

        Vector2 nextPos = Vector2.MoveTowards(currentPos, targetPos, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPos);

        if (Vector2.Distance(currentPos, targetPos) <= pointReachDistance)
        {
            currentPointIndex++;
            if (currentPointIndex >= patrolPoints.Length)
                currentPointIndex = 0;
        }
    }

    private void ChaseMove()
    {
        if (player == null)
            return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = player.position;

        Vector2 nextPos = Vector2.MoveTowards(currentPos, targetPos, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPos);
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        // Здесь вызывай урон игроку
        // Например:
        // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);

        Debug.Log(name + " attacks player for " + damage + " damage");
    }

    private void FlipByTarget()
    {
        Vector3 target = transform.position;

        if (state == EnemyState.Patrol && patrolPoints != null && patrolPoints.Length > 0)
            target = patrolPoints[currentPointIndex].position;

        if ((state == EnemyState.Chase || state == EnemyState.Attack) && player != null)
            target = player.position;

        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else if (target.x < transform.position.x)
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
    }
}
