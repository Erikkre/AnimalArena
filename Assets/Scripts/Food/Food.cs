﻿
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
[Serializable]
public class Food : MonoBehaviour {
    //public LayerMask animalMask;
    [HideInInspector] public Color foodColor;
    public new Collider collider;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public bool enabled;
    [HideInInspector] public ArrayList list;
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
        else if (animalHealth!=null && animalHealth.currentHealth<100)
        {
            animalHealth.AddHealth(100f/animalHealth.healthBarGroupings/2);
            list.Remove(instance); //dlist.RemoveAt(0);
            Destroy(gameObject);
        }
    }
}