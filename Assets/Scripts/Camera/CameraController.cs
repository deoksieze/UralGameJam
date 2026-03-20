using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float followSmooth = 10f;

    [Header("Zoom")]
    public Camera cam;
    public float defaultOrthoSize = 5f;

    [Header("Shake")]
    public float shakeMagnitude = 0.5f;
    public float shakeDuration = 0.2f;

    Vector3 followOffset;        // z-ось камеры
    Vector3 shakeOffset;
    bool isShaking;

    // пан/зум состояние
    bool isPanning;
    Vector3 panStartPos;
    Vector3 panTargetPos;
    float panT;
    float panDuration;
    float startOrtho;
    float targetOrtho;
    Vector3 bezierControl;       // контрольная точка Безье

    void Awake()
    {
        if (!cam) cam = GetComponent<Camera>();
        followOffset = transform.position - target.position;
        defaultOrthoSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        UpdateFollow();
        UpdatePanZoom();
        ApplyShake();
    }

    void UpdateFollow()
    {
        if (isPanning) return; // во время панорамы не следуем за игроком

        Vector3 desired = target.position + followOffset;
        transform.position = Vector3.Lerp(transform.position, desired, followSmooth * Time.deltaTime);
    }

    void UpdatePanZoom()
    {
        if (!isPanning) return;

        panT += Time.deltaTime / panDuration;
        float t = Mathf.Clamp01(panT);

        // квадратичная Безье: B(t) = Lerp( Lerp(P0,P1,t), Lerp(P1,P2,t), t )
        Vector3 p0 = panStartPos;
        Vector3 p1 = bezierControl;
        Vector3 p2 = panTargetPos;

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 bezierPos = Vector3.Lerp(a, b, t);

        transform.position = bezierPos;

        cam.orthographicSize = Mathf.Lerp(startOrtho, targetOrtho, t);

        if (t >= 1f)
            isPanning = false;
    }

    void ApplyShake()
    {
        transform.position += shakeOffset;
    }

    // ===== ПУБЛИЧНОЕ API =====

    // 1) Шейк от удара
    public void Shake(Vector2 direction, float magnitude, float duration)
    {
        StopAllCoroutines(); // можно сделать отдельный корутин только для shake
        StartCoroutine(ShakeRoutine(direction, magnitude, duration));
    }

    IEnumerator ShakeRoutine(Vector2 dir, float mag, float dur)
    {
        isShaking = true;
        shakeOffset = Vector3.zero;

        Vector3 d = new Vector3(dir.x, dir.y, 0f).normalized;
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float normalized = t / dur;
            float strength = mag * (1f - normalized); // затухание

            // линейная интерполяция туда-сюда вдоль направления
            float offset = Mathf.Sin(normalized * Mathf.PI * 2f) * strength;
            shakeOffset = d * offset;

            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }

    // 2) Пан+зум к точке интереса
    public void PanAndZoomTo(Transform pointOfInterest, float zoomMultiplier, float duration)
    {
        panStartPos = transform.position;
        panTargetPos = new Vector3(
            pointOfInterest.position.x + followOffset.x,
            pointOfInterest.position.y + followOffset.y,
            followOffset.z + pointOfInterest.position.z
        );

        startOrtho = cam.orthographicSize;
        targetOrtho = defaultOrthoSize * zoomMultiplier;

        panDuration = duration;
        panT = 0f;

        // простая контрольная точка Безье – середина между началом и концом, немного выше
        bezierControl = (panStartPos + panTargetPos) * 0.5f + Vector3.up * 1.5f;

        isPanning = true;
    }

    // 3) Возврат камеры к игроку после панорамы
    public void ReturnToPlayer(float duration)
    {
        // паним обратно к позиции фоллоу
        Vector3 desired = target.position + followOffset;
        panStartPos = transform.position;
        panTargetPos = desired;

        startOrtho = cam.orthographicSize;
        targetOrtho = defaultOrthoSize;

        panDuration = duration;
        panT = 0f;
        bezierControl = (panStartPos + panTargetPos) * 0.5f + Vector3.up * 1.5f;
        isPanning = true;
    }
}
