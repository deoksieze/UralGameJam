using UnityEngine;

public class PlayerAnimatorView : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public Collider2D bodyCollider;
    public LayerMask groundLayer;

    void Update()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);
        bool isGrounded = bodyCollider.IsTouchingLayers(groundLayer);
        bool isJumping = !isGrounded && rb.linearVelocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.linearVelocity.y < -0.1f;



        animator.SetFloat("Speed", speed);
        animator.SetBool("Up", isJumping);
        animator.SetBool("Down", isFalling);

    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void SetDead()
    {
        animator.SetTrigger("Death");
    }
}