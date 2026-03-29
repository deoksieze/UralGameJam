using System.Collections;
using UnityEngine;

public class DeathInheritanceSequence : MonoBehaviour
{
    public static DeathInheritanceSequence Instance;

    [Header("Refs")]
    public Camera sceneCamera;
    public Transform currentPlayer;
    public MonoBehaviour playerInput;
    public Animator currentPlayerAnimator;

    public PlayerManager pm;

    [Header("Heir")]
    public float heirSpawnOffsetX = -1f;
    public float heirStopOffsetX = -1.0f;
    public float heirWalkSpeed = 2.5f;

    [Header("Camera")]
    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f);
    public float gameplaySize = 5f;
    public float deathZoomSize = 2.5f;
    public float cameraMoveSpeed = 6f;

    [Header("Timing")]
    public float deathAnimWait = 0.5f;
    public float afterZoomWait = 0.25f;
    public float afterHeirArrivesWait = 0.4f;

    private bool sequenceRunning = false;

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("PlayerSequence")]
    public void PlaySequence(GameObject heirPrefab)
    {
        if (sequenceRunning) return;

        StartCoroutine(SequenceRoutine(heirPrefab));
    }

    private IEnumerator SequenceRoutine(GameObject heirPrefab)
    {
        sequenceRunning = true;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.SetEnemiesPaused(true);

        if (currentPlayerAnimator != null)
            currentPlayerAnimator.SetTrigger("Death");


        yield return new WaitForSeconds(deathAnimWait);


        // Vector3 corpseFocusPoint = currentPlayer.position + cameraOffset;
        // yield return StartCoroutine(MoveCameraTo(corpseFocusPoint, deathZoomSize));


        yield return new WaitForSeconds(afterZoomWait);


        Vector3 spawnPos = currentPlayer.position + new Vector3(heirSpawnOffsetX, 0f, 0f);
        GameObject heir = Instantiate(heirPrefab, spawnPos, Quaternion.identity);
        heir.SetActive(true);
        pm.SetUpHero(heir);


        Animator heirAnimator = heir.GetComponentInChildren<Animator>();

        if (playerInput != null)
            playerInput.enabled = false;

        Vector3 heirTarget = currentPlayer.position + new Vector3(heirStopOffsetX, 0f, 0f);

        if (heirAnimator != null)
            heirAnimator.SetFloat("Speed", 1);

        yield return StartCoroutine(MoveCharacterTo(heir.transform, heirTarget, heirWalkSpeed));

        if (heirAnimator != null)
        {
            heirAnimator.SetFloat("Speed", 0);
        }

        yield return new WaitForSeconds(afterHeirArrivesWait);

        // yield return StartCoroutine(MoveCameraTo(heir.transform.position + cameraOffset, gameplaySize));

        if (playerInput != null)
            playerInput.enabled = true;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.SetEnemiesPaused(false);

        sequenceRunning = false;
    }

    private IEnumerator MoveCameraTo(Vector3 targetPos, float targetSize)
    {
         
        while (Vector3.Distance(sceneCamera.transform.position, targetPos) > 2f)
        {
            float dist = Vector3.Distance(sceneCamera.transform.position, targetPos);
            float abs =    sceneCamera.orthographicSize - targetSize;
            Debug.Log("In while statement: " + dist + " abs: " + abs);
            sceneCamera.transform.position = Vector3.MoveTowards(
                sceneCamera.transform.position,
                targetPos,
                cameraMoveSpeed * Time.deltaTime
            );

            sceneCamera.orthographicSize = Mathf.MoveTowards(
                sceneCamera.orthographicSize,
                targetSize,
                cameraMoveSpeed * Time.deltaTime
            );

            yield return null;
        }
        Debug.Log("Out of while statement");
        sceneCamera.transform.position = targetPos;
        sceneCamera.orthographicSize = targetSize;
    }

    private IEnumerator MoveCharacterTo(Transform actor, Vector3 targetPos, float speed)
    {
        while (Vector3.Distance(actor.position, targetPos) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(actor.position, targetPos, speed * Time.deltaTime);
            actor.position = newPos;

            if (targetPos.x > actor.position.x)
                actor.localScale = new Vector3(1, 1, 1);
            else
                actor.localScale = new Vector3(-1, 1, 1);

            yield return null;
        }

        actor.position = targetPos;
    }
}