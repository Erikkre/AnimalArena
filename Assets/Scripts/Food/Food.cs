﻿
using System;
using UnityEngine;

[Serializable]
public class Food : MonoBehaviour {
    //public LayerMask animalMask;
    [HideInInspector] public Color foodColor;
    public new Collider collider;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public bool enabled;
    private void Update()
    {
        //if (enabled) ;
        //interpolate
    }

    private void OnTriggerEnter(Collider other)
    {
        var animalHealth = (AnimalHealth) other.GetComponent(
            typeof(AnimalHealth));

        //AnimalHealth animalHealth = other.GetComponentInParent<AnimalHealth>();
        //Debug.Log("Player color: "+animalHealth.playerColor+", foodColor: "+foodColor);

        if (animalHealth != null && animalHealth.playerColor != foodColor)
        {
            Physics.IgnoreCollision(collider, other);
        }
        else if (animalHealth!=null)
        {
            animalHealth.AddHealth(100f/animalHealth.healthBarGroupings/2);
            Destroy(gameObject);
        }
    }
}