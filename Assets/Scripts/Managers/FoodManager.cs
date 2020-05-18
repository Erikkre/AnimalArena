using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;
using static MathHelpers;

public class FoodManager : MonoBehaviour
{
    [Header("Spawning Food Colors")] public Color red, purple, green, yellow;

    [Header("Spawnpoints")] public Transform rSpawn, pSpawn, gSpawn, ySpawn;
    public GameObject FoodPrefab;
    public int maxFoodOnMap = 60, spawnDelay = 4;
    public float spawnInterval = 0.5f;
    private List<Food> rFoods = new List<Food>(),pFoods = new List<Food>(),gFoods = new List<Food>(),yFoods = new List<Food>();
    private bool rSpawning, pSpawning, gSpawning, ySpawning;
    private Random r = new Random();
    void Start()
    {
        rFoods = new List<Food>(maxFoodOnMap/4);pFoods = new List<Food>(maxFoodOnMap/4);
        gFoods = new List<Food>(maxFoodOnMap/4);yFoods = new List<Food>(maxFoodOnMap/4);
    }

    private void Update()
    {
        if (rSpawning&&rFoods.Count>=maxFoodOnMap/4) {CancelInvoke("rSpawnFood");rSpawning=false;}
        else if (!rSpawning&&rFoods.Count<maxFoodOnMap/4){InvokeRepeating("rSpawnFood", spawnDelay, spawnInterval);rSpawning=true;}
        
        if (pSpawning&&pFoods.Count>=maxFoodOnMap/4) {CancelInvoke("pSpawnFood");pSpawning=false;}
        else if (!pSpawning&&pFoods.Count<maxFoodOnMap/4){InvokeRepeating("pSpawnFood", spawnDelay, spawnInterval);pSpawning=true;}
        
        if (gSpawning&&gFoods.Count>=maxFoodOnMap/4) {CancelInvoke("gSpawnFood");gSpawning=false;}
        else if (!gSpawning&&gFoods.Count<maxFoodOnMap/4){InvokeRepeating("gSpawnFood", spawnDelay, spawnInterval);gSpawning=true;}
        
        if (ySpawning&&yFoods.Count>=maxFoodOnMap/4) {CancelInvoke("ySpawnFood");ySpawning=false;}
        else if (!ySpawning&&yFoods.Count<maxFoodOnMap/4){InvokeRepeating("ySpawnFood", spawnDelay, spawnInterval);ySpawning=true;}
    }

    void rSpawnFood()
    {
        Food rInstance = new Food();
         
         rInstance.Instance = Instantiate(FoodPrefab, 
             new Vector3(rSpawn.position.x+ (float) NextGaussianDouble(r)*6-3,rSpawn.position.y,rSpawn.position.z+(float) NextGaussianDouble(r)*6-3), 
             rSpawn.rotation);
         rInstance.Instance.GetComponentInChildren<MeshRenderer>().material.color=red; 
         
         rFoods.Add(rInstance);
    }

    void pSpawnFood()
    {
        Food pInstance = new Food();
        pInstance.Instance = Instantiate(FoodPrefab,
            new Vector3(pSpawn.position.x + (float) NextGaussianDouble(r)*6-3, pSpawn.position.y,
                pSpawn.position.z + (float) NextGaussianDouble(r)*6-3),
            pSpawn.rotation);
        pInstance.Instance.GetComponentInChildren<MeshRenderer>().material.color = purple;
        
        pFoods.Add(pInstance);
    }

    void gSpawnFood()
    {
        Food gInstance = new Food();
         
        gInstance.Instance = Instantiate(FoodPrefab, 
            new Vector3(gSpawn.position.x+(float) NextGaussianDouble(r)*6-3,gSpawn.position.y,gSpawn.position.z+(float) NextGaussianDouble(r)*6-3),
            gSpawn.rotation);
        gInstance.Instance.GetComponentInChildren<MeshRenderer>().material.color=green;
        
        gFoods.Add(gInstance);
    }

    void ySpawnFood(){
        Food yInstance = new Food();
        
         yInstance.Instance = Instantiate(FoodPrefab, 
             new Vector3(ySpawn.position.x+(float) NextGaussianDouble(r)*6-3,ySpawn.position.y,ySpawn.position.z+(float) NextGaussianDouble(r)*6-3), 
             ySpawn.rotation);
         yInstance.Instance.GetComponentInChildren<MeshRenderer>().material.color=yellow;
         
         yFoods.Add(yInstance);
    }
    
    // Used during the phases of the game where the player shouldn't be able to control their animal.
    public void DisableFoodMovement ()
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
    }


    public void Reset ()
    {
        
        yFoods.Clear();
        rFoods.Clear();
        gFoods.Clear();
        pFoods.Clear();
    }
}
