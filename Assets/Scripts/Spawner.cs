using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> spwaningObjects;
    public Transform upBorder;
    public Transform downBorder;
    
    public Transform parentTransform;

    float timer = 0;
    float spawnRate = 5f;

    void Start()
    {
        parentTransform = GetComponent<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            timer = 0;
            Spawn();
        }
    }

    [ContextMenu("Спавни объект")]
    void Spawn()
    {
        float yPosition = Random.Range(downBorder.position.y, upBorder.position.y);
        int ind = Random.Range(0, spwaningObjects.Count);
        
        Vector3 spawningPosition = parentTransform.position;
        spawningPosition.y = yPosition;

        Instantiate(spwaningObjects[ind], spawningPosition, parentTransform.rotation);
    }
}
