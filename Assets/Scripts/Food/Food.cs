
using System;
using UnityEngine;

[Serializable]
public class Food {
    public Color PlayerColor;                             // This is the color this animal will be tinted.
    public Transform SpawnPoint;
    [HideInInspector] public GameObject Instance;
    [HideInInspector] public bool enabled;
    private void Update()
    {
        if (enabled) ;
        //interpolate
    }
}