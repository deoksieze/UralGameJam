using UnityEngine;

public class PlayerGather : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D c2d;
    public Transform tf; 

    public PlayerGround pg;

    public Animator animator;

    public GameObject FrontHitColider;
    
    public GameObject UpHitColider;

    public GameObject DownHitColider;

    public GameObject BlockColider;

    public BlockHitbox blockHitbox;

}
