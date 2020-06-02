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
        lvlBar.SetValueNoBurn(lvl);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PickupFoodForXP()
    {
        lvl += .7f;
        lvlBar.SetValueNoBurn(lvl);
    }
    
    public void DamagePlayerForXP(float dmgPercentRatio)
    {
        lvl += dmgPercentRatio;
        lvlBar.SetValueNoBurn(lvl);
    }
}
