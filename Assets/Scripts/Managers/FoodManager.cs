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
    [HideInInspector] public Color red, purple, green, yellow;

    [Header("Spawnpoints")] public Transform rSpawn, pSpawn, gSpawn, ySpawn;
    public GameObject foodPrefab;
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
        if (rSpawning&&rFoods.Count>=maxFoodOnMap/4) {CancelInvoke("RSpawnFood");rSpawning=false;}
        else if (!rSpawning&&rFoods.Count<maxFoodOnMap/4){InvokeRepeating("RSpawnFood", spawnDelay, spawnInterval);rSpawning=true;}
        
        if (pSpawning&&pFoods.Count>=maxFoodOnMap/4) {CancelInvoke("PSpawnFood");pSpawning=false;}
        else if (!pSpawning&&pFoods.Count<maxFoodOnMap/4){InvokeRepeating("PSpawnFood", spawnDelay, spawnInterval);pSpawning=true;}
        
        if (gSpawning&&gFoods.Count>=maxFoodOnMap/4) {CancelInvoke("GSpawnFood");gSpawning=false;}
        else if (!gSpawning&&gFoods.Count<maxFoodOnMap/4){InvokeRepeating("GSpawnFood", spawnDelay, spawnInterval);gSpawning=true;}
        
        if (ySpawning&&yFoods.Count>=maxFoodOnMap/4) {CancelInvoke("YSpawnFood");ySpawning=false;}
        else if (!ySpawning&&yFoods.Count<maxFoodOnMap/4){InvokeRepeating("YSpawnFood", spawnDelay, spawnInterval);ySpawning=true;}
    }

    void RSpawnFood()
    {
        Food rInstance = gameObject.AddComponent(typeof(Food)) as Food;
         
         rInstance.instance = Instantiate(foodPrefab, 
             new Vector3(rSpawn.position.x+ (float) NextGaussianDouble(r)*6-3,rSpawn.position.y,rSpawn.position.z+(float) NextGaussianDouble(r)*6-3), 
             rSpawn.rotation);
         rInstance.instance.GetComponentInChildren<MeshRenderer>().material.color=red;
         rInstance.instance.GetComponent<Food>().foodColor = red;
         rFoods.Add(rInstance);
    }

    void PSpawnFood()
    {
        Food pInstance = gameObject.AddComponent(typeof(Food)) as Food;

        pInstance.instance = Instantiate(foodPrefab,
            new Vector3(pSpawn.position.x + (float) NextGaussianDouble(r)*6-3, pSpawn.position.y,
                pSpawn.position.z + (float) NextGaussianDouble(r)*6-3),
            pSpawn.rotation);
        pInstance.instance.GetComponentInChildren<MeshRenderer>().material.color = purple;
        pInstance.instance.GetComponent<Food>().foodColor = purple;

        pFoods.Add(pInstance);
    }

    void GSpawnFood()
    {
        Food gInstance = gameObject.AddComponent(typeof(Food)) as Food;
         
        gInstance.instance = Instantiate(foodPrefab, 
            new Vector3(gSpawn.position.x+(float) NextGaussianDouble(r)*6-3,gSpawn.position.y,gSpawn.position.z+(float) NextGaussianDouble(r)*6-3),
            gSpawn.rotation);
        gInstance.instance.GetComponentInChildren<MeshRenderer>().material.color=green;
        gInstance.instance.GetComponent<Food>().foodColor = green;
        gFoods.Add(gInstance);
    }

    void YSpawnFood(){
        Food yInstance = gameObject.AddComponent(typeof(Food)) as Food;
        
         yInstance.instance = Instantiate(foodPrefab, 
             new Vector3(ySpawn.position.x+(float) NextGaussianDouble(r)*6-3,ySpawn.position.y,ySpawn.position.z+(float) NextGaussianDouble(r)*6-3), 
             ySpawn.rotation);
         yInstance.instance.GetComponentInChildren<MeshRenderer>().material.color=yellow;
         yInstance.instance.GetComponent<Food>().foodColor = yellow;
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
