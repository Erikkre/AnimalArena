﻿
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class Food : MonoBehaviour {
    //public LayerMask damageableMask;
    [HideInInspector] public Color foodColor;
    //public new Collider collider;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public bool enabled;
    [HideInInspector] public ArrayList list;
    [HideInInspector] public float addedHealthInSmallFood;

    private void OnTriggerEnter(Collider other)
    {


        var animalHealth = (AnimalHealth) other.GetComponent(
            typeof(AnimalHealth));

        Debug.Log(animalHealth.playerColor+", "+foodColor);
        if (animalHealth!=null &&  animalHealth.playerColor.Equals(foodColor) && animalHealth.currentHealth<100)
        {
            //AnimalHealth animalHealth = other.GetComponentInParent<AnimalHealth>();
            //Debug.Log("foodHealth: "+Mathf.Pow(instance.transform.localScale.x+1f, 2f) );
            //Debug.Log("addhealth: "+Mathf.Pow(instance.transform.localScale.x+addedHealthInSmallFood, 2.5f) );
            
            animalHealth.AddHealth(Mathf.Pow(instance.transform.localScale.x+addedHealthInSmallFood, 2.5f), false);
            animalHealth.GetComponent<AnimalLvl>().PickupFoodForXP();
            list.Remove(instance); //dlist.RemoveAt(0);
            gameObject.SetActive(false);
        }
    }
}