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
    private float size;
    private float healthByScale;
    public void Start()
    {
        size = Random.Range(0.5f, 2f);
        
        instance.transform.localScale = new Vector3(instance.transform.localScale.x*size,instance.transform.localScale.y*size,instance.transform.localScale.z*size);
        instance.transform.position = new Vector3(instance.transform.position.x,instance.transform.position.y - 0.10f - size/2.9f,instance.transform.position.z);
        
    }

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
            Debug.Log("foodHealth: "+Mathf.Pow(instance.transform.localScale.x+1f, 2f) );
            
            animalHealth.AddHealth(Mathf.Pow(instance.transform.localScale.x+addedHealthInSmallFood, 2.5f));
            list.Remove(instance); //dlist.RemoveAt(0);
            Destroy(gameObject);
        }
    }
}