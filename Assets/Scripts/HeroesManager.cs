using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class HeroesManager : MonoBehaviour
{
    public List<GameObject> Heroes;

    public GameObject activeHero;

    [ContextMenu("SwitchHero")]
    void SwitchHero()
    {
        Instantiate(Heroes[1]);
        Destroy(activeHero);
        activeHero = Heroes[1];
    }

    // void GenerateHero()
    // {
    //     chooseRace;
    //     chooseClase;
    //     chooseStats(Race, Class);
    //     activeHero = Generatedhero;

    // }
}
