﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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
    public float foodHeightOffset = -.25f;
    public int healthInSmallFood = 2; //2
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
        Debug.Log("r:"+rFoods.Count+" ,maxFoodOnMap / 4f:"+maxFoodOnMap / 4f+", rSpawning:"+rSpawning);
        if (ShouldFoodSpawn(rSpawning, rFoods,"R")) rSpawning = true; 
        if (ShouldFoodSpawn(gSpawning, gFoods,"G")) gSpawning = true;
        if (ShouldFoodSpawn(pSpawning, pFoods,"P")) pSpawning = true; 
        if (ShouldFoodSpawn(ySpawning, yFoods,"Y")) ySpawning = true; 
    }

    private void AssignFood(Transform spawnPoint,float xOffset,float zOffset,ArrayList l,Color c)
    {
        Vector3 position;
        do
        {
            position = new Vector3(spawnPoint.position.x+RandomGaussianSpreadAround0() + xOffset,
                spawnPoint.position.y + foodHeightOffset,spawnPoint.position.z+RandomGaussianSpreadAround0() + zOffset);
            
        } while (Physics.OverlapSphere(position, 2, levelMask).Length != 0);

        Food inst = gameObject.AddComponent(typeof(Food)) as Food;
        
        inst.instance = Instantiate(foodPrefab,
            new Vector3(position.x,position.y,position.z),
                spawnPoint.rotation);

        
        inst.instance.GetComponentInChildren<MeshRenderer>().material.color=c;
        inst.instance.GetComponent<Food>().list = l;
        inst.instance.GetComponent<Food>().healthInSmallFood = healthInSmallFood;
        inst.instance.GetComponent<Food>().foodColor = c;
        
        inst.instance.GetComponent<Food>().instance = inst.instance;
        l.Add(inst.instance);
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
        DestroyList(rFoods);
        DestroyList(gFoods);
        DestroyList(yFoods);
        DestroyList(pFoods);
    }

    void DestroyList(ArrayList ar)
    {
        for (int i = 0; i < ar.Count; i++)
        {
            Destroy(((GameObject)ar[i]));
        }
        ar.Clear();
    }
}
