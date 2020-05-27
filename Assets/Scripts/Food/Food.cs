﻿
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class Food : MonoBehaviour {
    //public LayerMask animalMask;
    [HideInInspector] public Color foodColor;
    public new Collider collider;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public bool enabled;
    [HideInInspector] public ArrayList list;
    [HideInInspector] public int healthInSmallFood;
    public void Awake()
    {
        int size = Random.Range(1, 4);
        //transform.localScale *= size;
        healthInSmallFood *= size;
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
            animalHealth.AddHealth(healthInSmallFood);
            list.Remove(instance); //dlist.RemoveAt(0);
            Destroy(gameObject);
        }
    }
}