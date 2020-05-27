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

    void Start()
    {
        rFoods = new ArrayList((maxFoodOnMap/4)+1);pFoods = new ArrayList((maxFoodOnMap/4)+1);
        gFoods = new ArrayList((maxFoodOnMap/4)+1);yFoods = new ArrayList((maxFoodOnMap/4)+1);
    }


    private void ChooseFoodSpawn(Transform s)
    {
        //while (Physics.OverlapSphere()|| )
    }

    private bool FoodSpawning(bool s, ArrayList l, String name)
    {
        if (s && l.Count >= maxFoodOnMap / 4) { CancelInvoke(name+"SpawnFood"); return true;}
        if (!s && l.Count<maxFoodOnMap/4) InvokeRepeating(name+"SpawnFood", spawnDelay, spawnInterval); return false;
    }

    private void Update()
    {
        if (FoodSpawning(rSpawning, rFoods,"R")) rSpawning = false; else rSpawning = true;
        if (FoodSpawning(gSpawning, gFoods,"G")) gSpawning = false; else gSpawning = true;
        if (FoodSpawning(pSpawning, pFoods,"P")) pSpawning = false; else pSpawning = true;
        if (FoodSpawning(ySpawning, yFoods,"Y")) ySpawning = false; else ySpawning = true;
    }

    private Food AssignFood(Food inst,Transform spawnPoint,float xOffset,float zOffset,ArrayList l,Color c)
    {
        inst.instance = Instantiate(foodPrefab,
            new Vector3(spawnPoint.position.x+xOffset,spawnPoint.position.y+foodHeightOffset,spawnPoint.position.z+zOffset),
                spawnPoint.rotation);

        Food f = inst.instance.GetComponent<Food>();
        f.GetComponentInChildren<MeshRenderer>().material.color=c;
        f.list = l;
        f.healthInSmallFood = healthInSmallFood;
        f.foodColor = c;
        f.instance = inst.instance;
        return inst;
    }

    void RSpawnFood()
    {
        rFoods.Add(
             AssignFood(gameObject.AddComponent(typeof(Food)) as Food,
                 rSpawn,
                 (float) NextGaussianDouble(r) * spawnSpread + spawnSpread,
                 (float) NextGaussianDouble(r) * spawnSpread + spawnSpread,
                 rFoods, red)
         );
    }

    void PSpawnFood()
    {
        pFoods.Add(
            AssignFood(gameObject.AddComponent(typeof(Food)) as Food,
                pSpawn,
                (float) NextGaussianDouble(r) * spawnSpread - spawnSpread,
                (float) NextGaussianDouble(r) * spawnSpread + spawnSpread,
                pFoods, purple)
        );
    }

    void GSpawnFood()
    {
        gFoods.Add(
            AssignFood(gameObject.AddComponent(typeof(Food)) as Food,
                gSpawn,
                (float) NextGaussianDouble(r) * spawnSpread - spawnSpread,
                (float) NextGaussianDouble(r) * spawnSpread - spawnSpread,
                gFoods, green)
        );
    }

    void YSpawnFood(){
        yFoods.Add(
            AssignFood(gameObject.AddComponent(typeof(Food)) as Food,
                ySpawn,
                (float) NextGaussianDouble(r) * spawnSpread + spawnSpread,
                (float) NextGaussianDouble(r) * spawnSpread - spawnSpread,
                yFoods, yellow)
        );
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
        yFoods.Clear();
        rFoods.Clear();
        gFoods.Clear();
        pFoods.Clear();
    }
}
