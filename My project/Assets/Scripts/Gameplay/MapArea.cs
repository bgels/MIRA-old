using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<partymember> wildMobs;

    public partymember getRandomWildMob()
    {
        var wildMob =  wildMobs[Random.Range(0, wildMobs.Count)];
        wildMob.Init();
        return wildMob;
    }
}
