using System;
using System.Collections;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterCombat : MonoBehaviour
{
    public GameObject FrontHitColider;
    public GameObject UpHitColider;
    public GameObject DownHitColider;
    public GameObject BlockColider;
    public BlockHitbox blockZone;

    public characterGround charGround;
    public characterHealth health;

    public float attackDuration = 0.2f;

    [Header("Calculations")]
    private bool isAttacking;
    private bool isBlocking;

    public void OnAttack(InputAction.CallbackContext context)
    {
        TryStartAttack(context, FrontHitColider);
    }

    public void OnAttackUp(InputAction.CallbackContext context)
    {
        TryStartAttack(context, UpHitColider);
    }

    public void OnAttackDown(InputAction.CallbackContext context)
    {
        if (charGround.GetOnGround())
        {
            Debug.Log("You are on the ground, you can't hit");
            return;
        }

        TryStartAttack(context, DownHitColider);
    } 

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (isAttacking) return;

        if (context.performed)
        {
            StartBlock();
        }

        if (context.canceled)
        {
            EndBlock();
        }
    }

    private void TryStartAttack(InputAction.CallbackContext context, GameObject hitCollider)
    {
        if (!context.performed || isAttacking || isBlocking) return;

        isAttacking = true;
        StartCoroutine(DoAttack(hitCollider));
    }

    private IEnumerator DoAttack(GameObject hitCollider)
    {
        hitCollider.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        hitCollider.SetActive(false);
        isAttacking = false;
    }

    void StartBlock()
    {
        if (isAttacking) return;

        isBlocking = true;
        BlockColider.SetActive(true);
        health.SetBlocking(true);

        Debug.Log("Block started");
        return;
    }

    void EndBlock()
    {

        isBlocking = false;
        BlockColider.SetActive(false);
        health.SetBlocking(false);
        Debug.Log("Block finished");
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    public bool IsThisDealerBlocked(DamageDealer dealer)
    {
        return isBlocking && blockZone.Contains(dealer);
    }


}
