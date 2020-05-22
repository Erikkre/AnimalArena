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
    public int maxFoodOnMap = 80, spawnDelay = 4;
    public float spawnInterval = 1.5f;
    private ArrayList rFoods = new ArrayList(),pFoods = new ArrayList(),gFoods = new ArrayList(),yFoods = new ArrayList();
    private bool rSpawning, pSpawning, gSpawning, ySpawning;
    private Random r = new Random();
    public int spawnSpread = 5;
    public float foodHeightOffset = -.25f;
    void Start()
    {
        rFoods = new ArrayList((maxFoodOnMap/4)+1);pFoods = new ArrayList((maxFoodOnMap/4)+1);
        gFoods = new ArrayList((maxFoodOnMap/4)+1);yFoods = new ArrayList((maxFoodOnMap/4)+1);
    }

    private void Update()
    {
        //Debug.Log("r:"+rFoods.Count+",r capacity:"+rFoods.Capacity+"y:"+yFoods.Count);
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
             new Vector3(rSpawn.position.x+ (float) NextGaussianDouble(r)*spawnSpread,rSpawn.position.y+foodHeightOffset,rSpawn.position.z+(float) NextGaussianDouble(r)*spawnSpread), 
             rSpawn.rotation);
         rInstance.instance.GetComponentInChildren<MeshRenderer>().material.color=red;
         rInstance.instance.GetComponent<Food>().foodColor = red;
         rInstance.instance.GetComponent<Food>().list = rFoods;
         rInstance.instance.GetComponent<Food>().instance = rInstance.instance;

         rFoods.Add(rInstance.instance);
    }

    void PSpawnFood()
    {
        Food pInstance = gameObject.AddComponent(typeof(Food)) as Food;

        pInstance.instance = Instantiate(foodPrefab,
            new Vector3(pSpawn.position.x + (float) NextGaussianDouble(r)*spawnSpread- (spawnSpread), pSpawn.position.y+foodHeightOffset,
                pSpawn.position.z + (float) NextGaussianDouble(r)*spawnSpread),
            pSpawn.rotation);
        pInstance.instance.GetComponentInChildren<MeshRenderer>().material.color = purple;
        pInstance.instance.GetComponent<Food>().foodColor = purple;
        pInstance.instance.GetComponent<Food>().list = pFoods;
        pInstance.instance.GetComponent<Food>().instance = pInstance.instance;
        pFoods.Add(pInstance);
    }

    void GSpawnFood()
    {
        Food gInstance = gameObject.AddComponent(typeof(Food)) as Food;
         
        gInstance.instance = Instantiate(foodPrefab, 
            new Vector3(gSpawn.position.x+(float) NextGaussianDouble(r)*spawnSpread- (spawnSpread),gSpawn.position.y+foodHeightOffset,gSpawn.position.z+(float) NextGaussianDouble(r)*spawnSpread- (spawnSpread/2f)),
            gSpawn.rotation);
        gInstance.instance.GetComponentInChildren<MeshRenderer>().material.color=green;
        gInstance.instance.GetComponent<Food>().foodColor = green;
        gInstance.instance.GetComponent<Food>().list = gFoods;
        gInstance.instance.GetComponent<Food>().instance = gInstance.instance;
        gFoods.Add(gInstance);
    }

    void YSpawnFood(){
        Food yInstance = gameObject.AddComponent(typeof(Food)) as Food;
        
         yInstance.instance = Instantiate(foodPrefab, 
             new Vector3(ySpawn.position.x+(float) NextGaussianDouble(r)*spawnSpread,ySpawn.position.y+foodHeightOffset,ySpawn.position.z+(float) NextGaussianDouble(r)*spawnSpread- (spawnSpread/2f)), 
             ySpawn.rotation);
         yInstance.instance.GetComponentInChildren<MeshRenderer>().material.color=yellow;
         yInstance.instance.GetComponent<Food>().foodColor = yellow;
         yInstance.instance.GetComponent<Food>().list = yFoods;
         yInstance.instance.GetComponent<Food>().instance = yInstance.instance;
         yFoods.Add(yInstance);
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
