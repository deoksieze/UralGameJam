using System.Collections;
using UnityEngine;

public class DeathInheritanceSequence : MonoBehaviour
{
    public static DeathInheritanceSequence Instance;

    [Header("Refs")]
    public Camera sceneCamera;
    public Transform currentPlayer;
    public MonoBehaviour currentPlayerInput;
    public Animator currentPlayerAnimator;

    public Input input;

    [Header("Heir")]
    public GameObject heirPrefab;
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

    // public void PlaySequence(Transform deadPlayer, Animator deadAnimator, MonoBehaviour inputToDisable)
    // {
    //     if (sequenceRunning) return;

    //     currentPlayer = deadPlayer;
    //     currentPlayerAnimator = deadAnimator;
    //     currentPlayerInput = inputToDisable;

    //     StartCoroutine(SequenceRoutine());
    // }

    [ContextMenu("PlayerSequence")]
    public void PlaySequence()
    {
        if (sequenceRunning) return;

        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        sequenceRunning = true;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.SetEnemiesPaused(true);

        if (currentPlayerAnimator != null)
            currentPlayerAnimator.SetTrigger("Death");


        yield return new WaitForSeconds(deathAnimWait);


        Vector3 corpseFocusPoint = currentPlayer.position + cameraOffset;
        // yield return StartCoroutine(MoveCameraTo(corpseFocusPoint, deathZoomSize));


        yield return new WaitForSeconds(afterZoomWait);


        Vector3 spawnPos = currentPlayer.position + new Vector3(heirSpawnOffsetX, 0f, 0f);
        GameObject heir = Instantiate(heirPrefab, spawnPos, Quaternion.identity);
        heir.SetActive(true);


        Animator heirAnimator = heir.GetComponentInChildren<Animator>();
        MonoBehaviour heirInput = heir.GetComponent<MonoBehaviour>();


        if (heirInput != null)
            heirInput.enabled = false;

        Vector3 heirTarget = currentPlayer.position + new Vector3(heirStopOffsetX, 0f, 0f);

        if (heirAnimator != null)
            heirAnimator.SetFloat("Speed", 1);

        yield return StartCoroutine(MoveCharacterTo(heir.transform, heirTarget, heirWalkSpeed));

        if (heirAnimator != null)
        {
            heirAnimator.SetFloat("Speed", 0);
            // heirAnimator.SetTrigger("RaiseSword");
        }

        yield return new WaitForSeconds(afterHeirArrivesWait);

        // yield return StartCoroutine(MoveCameraTo(heir.transform.position + cameraOffset, gameplaySize));

        if (heirInput != null)
            heirInput.enabled = true;

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