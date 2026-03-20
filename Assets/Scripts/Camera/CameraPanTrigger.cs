using System.Collections;
using UnityEngine;

public class CameraPanTrigger : MonoBehaviour
{
    public CameraController cam;
    public Transform pointOfInterest;

    public float zoomMultiplier = 1.5f;
    public float panDuration = 1f;
    public float returnDuration = 1f;

    bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(PanSequence());
    }

    IEnumerator PanSequence()
    {
        cam.PanAndZoomTo(pointOfInterest, zoomMultiplier, panDuration);
        yield return new WaitForSeconds(panDuration + 0.1f);

        cam.ReturnToPlayer(returnDuration);
    }
}
