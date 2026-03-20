using System.Collections.Generic;
using UnityEngine;

public class BlockHitbox : MonoBehaviour
{   
    private readonly HashSet<DamageDealer> inside = new HashSet<DamageDealer>();
    public bool Contains(DamageDealer dealer) => inside.Contains(dealer);


    void OnTriggerEnter2D(Collider2D other)
    {
        var dealer = other.GetComponent<DamageDealer>();
        if (dealer != null)
            inside.Add(dealer);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var dealer = other.GetComponent<DamageDealer>();
        if (dealer != null)
            inside.Remove(dealer);
    }

}
