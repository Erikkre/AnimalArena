﻿using System;
using System.Collections;
using System.Collections.Generic;
 using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;
using Random = System.Random;
using static MathHelpers;

public class FoodManager : MonoBehaviour
{
    [HideInInspector] public Color red, purple, green, yellow;

    [Header("Spawnpoints")] public Transform rSpawn, pSpawn, gSpawn, ySpawn;
    public GameObject foodPrefab;
    public int maxFoodOnMap = 80, spawnDelay = 4;
    public float spawnInterval = 1.5f;
    private ArrayList rFoods = new ArrayList(),pFoods = new ArrayList(),gFoods = new ArrayList(),yFoods = new ArrayList();
    private bool rSpawning, pSpawning, gSpawning, ySpawning;
    private Random r = new Random();
    public int spawnSpread = 5;
    public float addedHealthInSmallFood = 2f; //2f
    public LayerMask levelMask;
    void Start()
    {
        rFoods = new ArrayList((maxFoodOnMap/4)+1);pFoods = new ArrayList((maxFoodOnMap/4)+1);
        gFoods = new ArrayList((maxFoodOnMap/4)+1);yFoods = new ArrayList((maxFoodOnMap/4)+1);
    }

    private bool ShouldFoodSpawn(bool s, ArrayList l, String name)
    {
        if (!s && l.Count < maxFoodOnMap / 4f) {InvokeRepeating(name+"SpawnFood", spawnDelay, spawnInterval); return true;}
        return false;
    }

    private void Update()
    {
        //Debug.Log("r:"+rFoods.Count+" ,maxFoodOnMap / 4f:"+maxFoodOnMap / 4f+", rSpawning:"+rSpawning)
        if (ShouldFoodSpawn(rSpawning, rFoods,"R")) rSpawning = true; 
        if (ShouldFoodSpawn(gSpawning, gFoods,"G")) gSpawning = true;
        if (ShouldFoodSpawn(pSpawning, pFoods,"P")) pSpawning = true; 
        if (ShouldFoodSpawn(ySpawning, yFoods,"Y")) ySpawning = true; 
    }

    private void AssignFood(Transform spawnPoint,float xOffset,float zOffset,ArrayList l,Color c)
    {
        Vector3 position;
        do {
            position = new Vector3(spawnPoint.position.x+RandomGaussianSpreadAround0() + xOffset,
                spawnPoint.position.y,spawnPoint.position.z+RandomGaussianSpreadAround0() + zOffset);
            
        } while (Physics.OverlapSphere(position, 2, levelMask).Length != 0);
        
        Food inst = new Food();
        inst.instance = ObjectPoolerHelper.SharedInstance.GetPooledObject("Food"); 
        
        if (inst.instance != null)
        {
            float size = UnityEngine.Random.Range(0.5f, 2.5f);
            inst.instance.transform.localScale = size*0.7f*Vector3.one;
            
            inst.instance.transform.position = new Vector3(position.x, position.y- 0.65f - size/3.2f, position.z);
            inst.instance.transform.rotation = spawnPoint.rotation;

            inst.instance.GetComponentInChildren<MeshRenderer>().material.color=c;
            
            Food fInst = inst.instance.GetComponent<Food>();
            fInst.instance = inst.instance;
            fInst.list = l;
            fInst.addedHealthInSmallFood = addedHealthInSmallFood;
            fInst.foodColor = c;

            l.Add(inst.instance);
            Debug.Log("Food active");
            inst.instance.SetActive(true);
        }
    }

    float RandomGaussianSpreadAround0()
    {
        return (float) NextGaussianDouble(r) * spawnSpread;
    }
    void RSpawnFood()
    {
        if (rFoods.Count >= maxFoodOnMap / 4f) { CancelInvoke("RSpawnFood"); rSpawning = false; return ;}
        AssignFood(rSpawn,
            spawnSpread,
            spawnSpread,
            rFoods, red);
    }

    void PSpawnFood()
    {
        if (pFoods.Count >= maxFoodOnMap / 4f) { CancelInvoke("PSpawnFood"); pSpawning = false; return ;}
        AssignFood(pSpawn,
            -spawnSpread,
            spawnSpread,
            pFoods, purple);
    }

    void GSpawnFood()
    {
        if (gFoods.Count >= maxFoodOnMap / 4f) { CancelInvoke("GSpawnFood"); gSpawning = false; return ;}
        AssignFood(gSpawn,
            -spawnSpread,
            -spawnSpread,
            gFoods, green);
    }

    void YSpawnFood()
    {
        if (yFoods.Count >= maxFoodOnMap / 4f) { CancelInvoke("YSpawnFood"); ySpawning = false; return ;}
        AssignFood(ySpawn,
            spawnSpread,
            -spawnSpread,
            yFoods, yellow);
    }
    
    // Used during the phases of the game where the player shouldn't be able to control their animal.
    /*public void DisableFoodMovement ()
    {
        for (int i=0;i<yFoods.Count;i++)
        {
            yFoods[i].enabled = false;
        }
        for (int i=0;i<gFoods.Count;i++)
        {
            gFoods[i].enabled = false;
        }
        for (int i=0;i<rFoods.Count;i++)
        {
            rFoods[i].enabled = false;
        }
        for (int i=0;i<pFoods.Count;i++)
        {
            pFoods[i].enabled = false;
        }
    }


    // Used during the phases of the game where the player should be able to control their animal.
    public void EnableFoodMovement ()
    {
        for (int i=0;i<yFoods.Count;i++)
        {
            yFoods[i].enabled = true;
        }
        for (int i=0;i<gFoods.Count;i++)
        {
            gFoods[i].enabled = true;
        }
        for (int i=0;i<rFoods.Count;i++)
        {
            rFoods[i].enabled = true;
        }
        for (int i=0;i<pFoods.Count;i++)
        {
            pFoods[i].enabled = true;
        }
    }*/


    public void Reset ()
    {
        EmptyList(rFoods);
        EmptyList(gFoods);
        EmptyList(yFoods);
        EmptyList(pFoods);
    }

    void EmptyList(ArrayList ar)
    {
        for (int i = 0; i < ar.Count; i++)
        {
           ((GameObject)ar[i]).SetActive(false);
        }
        ar.Clear();
    }
}
