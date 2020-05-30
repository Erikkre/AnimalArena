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
    [HideInInspector] public float addedHealthInSmallFood;

    private void OnTriggerEnter(Collider other)
    {
        var animalHealth = (AnimalHealth) other.GetComponent(
            typeof(AnimalHealth));
        
        if (animalHealth != null && animalHealth.playerColor != foodColor)
        {
            Physics.IgnoreCollision(collider, other);
        }
        else if (animalHealth!=null && animalHealth.currentHealth<100)
        {
            //AnimalHealth animalHealth = other.GetComponentInParent<AnimalHealth>();
            //Debug.Log("foodHealth: "+Mathf.Pow(instance.transform.localScale.x+1f, 2f) );
            Debug.Log(instance.transform.position.y.ToString() );
            
            animalHealth.AddHealth(Mathf.Pow(instance.transform.localScale.x+addedHealthInSmallFood, 2.5f));
            animalHealth.GetComponent<AnimalLvl>().PickupFood();
            list.Remove(instance); //dlist.RemoveAt(0);
            gameObject.SetActive(false);
        }
    }
}