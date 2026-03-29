using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Tuning")]
    public float attackDuration = 1f;
    public float knockbackForce = 5f;
    public int damage = 1;
    public int maxHealth = 5;
    public float knockbackTime = 0.2f;

    [Header("Components")]
    public PlayerHealth health;
    public PlayerCombat combat;
    public PlayerMovement movement;
    public PlayerJump jump;
    
    public PlayerAnimatorView animatorView;

    [Header("Swaping Heroes")]
    public List<GameObject> Heroes;
    private int curHeroIndx = 0;


    void Awake()
    {
        if (!movement) movement = GetComponent<PlayerMovement>();
        if (!combat)   combat   = GetComponent<PlayerCombat>();
        if (!health)   health   = GetComponent<PlayerHealth>();
        if (!jump)     jump     = GetComponent<PlayerJump>();

        combat.attackDuration = attackDuration;
        combat.damage = damage;
        combat.knockbackForce = knockbackForce;

        health.SetHp(maxHealth);
        health.knockbackTime = knockbackTime;


    }

    [ContextMenu("SwapHeroes")]
    void SwapHeroes()
    {
        Heroes[curHeroIndx].SetActive(false);

        curHeroIndx = (curHeroIndx + 1) % Heroes.Capacity;   

        GameObject currentHero = Heroes[curHeroIndx];
        currentHero.SetActive(true);

        PlayerGather gathering = currentHero.GetComponent<PlayerGather>();
        Rigidbody2D rb = gathering.rb;
        Collider2D cd = gathering.c2d;
        PlayerGround pg = gathering.pg;
        Transform tf = gathering.tf;
        Animator an = gathering.animator;

        movement.body = rb;
        movement.ground = pg;
        movement.playerTransform = tf;

        jump.body = rb;
        jump.ground = pg;

        combat.FrontHitColider = gathering.FrontHitColider;
        combat.UpHitColider = gathering.UpHitColider;
        combat.DownHitColider = gathering.DownHitColider;
        combat.BlockColider = gathering.BlockColider;
        combat.charGround = pg;
        combat.blockZone = gathering.blockHitbox;


        animatorView.animator = an;
        Debug.Log(an);
        animatorView.rb = rb;
        animatorView.bodyCollider = cd;



    }
}
