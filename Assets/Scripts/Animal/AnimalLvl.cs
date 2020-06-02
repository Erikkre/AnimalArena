using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalLvl : MonoBehaviour
{
    public  MOBAEnergyBar lvlBar;

    public static float lvl = 0; //100 levels
    // Start is called before the first frame update
    void Start()
    {
        lvlBar.MaxValue = 100f;
        lvlBar.Value = 100;
        lvlBar.Value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PickupFoodForXP()
    {
        lvl += .7f;
        lvlBar.Value = lvl;
    }
    
    public void DamagePlayerForXP(float dmgPercentRatio)
    {
        lvl += dmgPercentRatio*10f;
        lvlBar.Value = lvl;
    }
}
