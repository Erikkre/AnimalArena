using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class AnimalShooting : MonoBehaviour
{
    
    [HideInInspector]public int playerNumber = 1;              // Used to identify the different players.
    public Mass mass;                      // Prefab of the mass.
    public Transform fireTransform;           // A child of the animal where the masss are spawned.
    public Transform animalPos;           // A child of the animal where the masss are spawned.
    public AnimalHealth animalHealth;
    [HideInInspector] public CoroutineManager coroutineManagerInstance;
    public Slider aimSlider;                  // A child of the animal that displays the current launch force.
    public AudioSource shootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip chargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip fireClip;                // Audio that plays when each shot is fired.
    public float minLaunchForce = 7f;        // The force given to the mass if the fire button is not held.
    public float maxLaunchForce = 80f;        // The force given to the mass if the fire button is held for the max charge time.
    public float maxChargeTime = 1f;       // How long the mass can charge for before it is fired at max force.
    public float shotToHealthUsageDivisor = 4f;
    public float massStartScale = 0.5f, massScaleMultiplier = 1.5f;

    private Transform targetTrans;
    

    private string FireButton,CancelButton;                // The input axis that is used for launching masss.
    private float currentLaunchForce;         // The force that will be given to the mass when the fire button is released.
    private float ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool Fired, charging;                       // Whether or not the mass has been launched with this button press.

    private bool cancelledFire;
    public Light explosionLight;
    private float originalExplosionLightIntensity;
    private void OnEnable() {
        // When the animal is turned on, reset the launch force and the UI
        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
    }


    private void Start ()
    {
        originalExplosionLightIntensity = explosionLight.intensity;
        GameManager.maxMassSize = massStartScale + massScaleMultiplier;
        // The fire axis is based on the player number.
        FireButton = "Fire" + playerNumber;
        CancelButton = "Cancel" + playerNumber;
        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        ChargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        //Mass.GetComponent<Mass>().shooterPlayerNumber = PlayerNumber;
        //Mass =Instantiate (Mass, FireTransform.position, FireTransform.rotation) as Mass;
        if (playerNumber==1) targetTrans = FindObjectOfType<GameManager>().transform.GetChild(0).transform.GetChild(0);
        aimSlider.transform.position = new Vector3(aimSlider.transform.position.x,aimSlider.transform.position.y,aimSlider.transform.position.z-1.2f);
    }


    private void LateUpdate ()
    {
        if (targetTrans && animalHealth.currentHealth>animalHealth.minHealthPercent)  //if player character and they have enough health to shoot
        {
            //Debug.Log("power shooting: "+currentLaunchForce+", % health: "+animalHealth.currentHealth+", aimslider val: "+aimSlider.value);
            
            if (Input.GetButtonDown(CancelButton))
            {
                cancelledFire = true;
                if (shootingAudio.isPlaying) shootingAudio.Stop();
                if (aimSlider.value>minLaunchForce) aimSlider.value = minLaunchForce;
                animalHealth.AddHealth(currentLaunchForce/1.13f/shotToHealthUsageDivisor);
                currentLaunchForce = minLaunchForce;
            }
            else if (Input.GetButtonUp(FireButton) && !Fired && !cancelledFire)            //fire Released
            {
                shootingAudio.clip = fireClip;
                shootingAudio.Play ();
                 
                if (currentLaunchForce > maxLaunchForce) currentLaunchForce = maxLaunchForce;
 
                Fire();
            }
            else if (Input.GetButtonDown(FireButton) && EnoughHealthToLaunch()) //fireButton just Pressed
            {
                if (cancelledFire) cancelledFire = false;
                Fired = false;
                charging = true;
                currentLaunchForce = minLaunchForce;
                
                shootingAudio.clip = chargingClip;
                shootingAudio.Play();
            }
            else if (Input.GetButton(FireButton) && !EnoughHealthToLaunch()&& !cancelledFire) //fireButton held/pressed and not enough charge
            {
                Aim();
                if (shootingAudio.isPlaying) shootingAudio.Stop();
            }
            else if (!Fired && Input.GetButton(FireButton) && EnoughHealthToLaunch() && !cancelledFire) //fireButton held
            {
                if (currentLaunchForce + ChargeSpeed * Time.deltaTime < maxLaunchForce)
                {
                    currentLaunchForce += ChargeSpeed * Time.deltaTime;
                    aimSlider.value = currentLaunchForce;
                    animalHealth.TakeDamage((ChargeSpeed * Time.deltaTime)/shotToHealthUsageDivisor, true); //take off each added power
                }
                Aim();
            }
        }
    }

    public void Aim()
    {
        int angleToAdd;
        if (targetTrans.position.x > animalPos.position.x) angleToAdd = -90;
        else angleToAdd = 90;
                
        aimSlider.transform.rotation = Quaternion.Euler(90,0, 
            -Mathf.Rad2Deg* ((float)  Math.Atan((targetTrans.position.z-animalPos.position.z)/-(targetTrans.position.x-animalPos.position.x))  ) + angleToAdd);
    }

    public bool EnoughHealthToLaunch()//health must stay above 1 mass with any prospective launch
    {
        return animalHealth.currentHealth - ((currentLaunchForce / maxLaunchForce)/shotToHealthUsageDivisor)*100 >=
               animalHealth.minHealthPercent;
        //animal health% - launch%/2 (e.g. 2% launch power used = 1% health taken) should stay above 1 mass
    }

    private void Fire ()
    {
        // Set the fired flag so only Fire is only called once.
        Fired = true;

        //print("Bullet Fired");
        // Create an instance of the mass and store a reference to it's rigidbody.
      
        //Rigidbody massInstance =
        //    Instantiate(mass.rBody, fireTransform.position, Quaternion.identity);//Quaternion.LookRotation(targetTrans.position-fireTransform.position)) as Rigidbody;

        Mass mass = new Mass();
        mass.instance = ObjectPoolerHelper.SharedInstance.GetPooledObject("Mass");
        mass.instance.transform.position = fireTransform.position;
        mass.instance.transform.rotation = Quaternion.identity;
        mass.instance.transform.localScale = new Vector3(massStartScale+(currentLaunchForce/maxLaunchForce)*massScaleMultiplier,massStartScale+(currentLaunchForce/maxLaunchForce)*massScaleMultiplier,massStartScale+(currentLaunchForce/maxLaunchForce)*massScaleMultiplier);
        mass.instance.GetComponent<Rigidbody>().velocity = currentLaunchForce * (targetTrans.position-fireTransform.position).normalized;
        
        
        Mass mInst = mass.instance.GetComponent<Mass>();
        mInst.shooter = animalHealth;
        mInst.coroutineManagerInstance = coroutineManagerInstance;
        mInst.playerShooterNum = playerNumber;
        // Set the mass's velocity to the launch force in the fire position's forward direction.
        
       if (mInst.originalExplosionLightIntensity == 0)
            mInst.originalExplosionLightIntensity = originalExplosionLightIntensity;

       float massDamageToSizeRatio = 4f;
        mInst.sizePercentRatio = (mInst.transform.localScale.x / GameManager.maxMassSize);
        mInst.explosionRadius *=  mInst.sizePercentRatio;
        mInst.explosionForce *=   mInst.sizePercentRatio;
        mInst.maxDamage *= massDamageToSizeRatio * (mInst.sizePercentRatio/massDamageToSizeRatio);
        mInst.explosionLight.intensity *= mInst.sizePercentRatio;
        mInst.explosionLight.range *= mInst.sizePercentRatio;
        
        //Debug.Log("massLight.intensity before math " + mInst.massLight.intensity);
        mInst.massLight.intensity *= mInst.sizePercentRatio;
        //Debug.Log("massLight.intensity after math " + mInst.massLight.intensity+", sizePercentRatio: "+mInst.sizePercentRatio);
        
        mInst.massLight.range *= mInst.sizePercentRatio;
        
        //Debug.Log("mass active");
        mass.instance.SetActive(true);
        
        // Reset the launch force.  This is a precaution in case of missing button events.
        currentLaunchForce = minLaunchForce;
        if (aimSlider.value>minLaunchForce) aimSlider.value = minLaunchForce;
    }
}