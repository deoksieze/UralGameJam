using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 2f;

    bool isKnockedBack = false;

    void Update()
    {
        if (isKnockedBack) return;
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    public void SetKnockBack(bool flag)
    {
        isKnockedBack = flag;
    }
}
