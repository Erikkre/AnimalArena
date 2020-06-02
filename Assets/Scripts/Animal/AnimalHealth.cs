using System;
using System.Collections.Generic;
using EnergyBarToolkit;
using Managers;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AnimalHealth : MonoBehaviour
{
    public float minHealthPercent = 5;               // The amount of health each animal starts with.

    //public Image FillImage;                           // The image component of the slider.
    [HideInInspector]public Color playerColor;
    [HideInInspector]public int playerNumber;
    public GameObject explosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the animal dies.
    public MOBAHealthBarPanel hpAndLvlBar;
    private MOBAEnergyBar hpBar;

    private AudioSource explosionAudio;               // The audio source to play when the animal explodes.
    private ParticleSystem explosionParticles;        // The particle system the will play when the animal is destroyed.
    [HideInInspector]public float currentHealth;                      // How much health the animal currently has.
    private bool dead;                                // Has the animal been reduced beyond zero health yet?
    public Transform animalSphere;
    public SphereCollider sphereCollider;

    public float sizeScalingWithHealthMultiplier = 1;
    public ParticleSystem dustTrail;
    private ParticleSystem.MinMaxCurve dustSize;
    
    private float hpAndLvlBarStartingY;
    [HideInInspector] public CoroutineManager coroutineManagerInstance;
    private float scaleTarget, tempSphereScaleHolder;
    public Rigidbody rBody;
    public float startingMass = 0.7f, massScalingDivisor = 5f;
    private Vector3 velBeforePhysicsUpdate;

    void FixedUpdate()
    {
        //1/Time.deltaTime
        velBeforePhysicsUpdate = rBody.velocity;
    }

    private float RammingDamagedSizePortionEquation(float scaleTarget)
    {
        return Mathf.Log(scaleTarget + 0.04f) / 5f + 0.5f;
    }
    private void OnCollisionEnter(Collision other)
    {//Debug.Log("Animal"+playerNumber+" going "+velBeforePhysicsUpdate.magnitude * rBody.mass);
        if ( velBeforePhysicsUpdate.magnitude * rBody.mass > 10f ){


            var otherAnimalHealth = (AnimalHealth) other.transform.GetComponent(
                typeof(AnimalHealth));

            //other.relativeVelocity
            if (otherAnimalHealth != null && otherAnimalHealth.playerNumber != playerNumber)
            {
                float
                    thisDamagePotential; //= otherAnimalHealth.velBeforePhysicsUpdate.magnitude + RammingDamagedSizePortionEquation(otherAnimalHealth.scaleTarget);

                thisDamagePotential =
                    Vector3.Dot(velBeforePhysicsUpdate,other.contacts[0].normal*-1) * rBody.mass;

                //Debug.Log("animal"+otherAnimalHealth.playerNumber+": "+Vector3.Dot(otherAnimalHealth.velBeforePhysicsUpdate,other.contacts[0].normal) * otherAnimalHealth.rBody.mass+", my animal"+playerNumber+": "+thisDamagePotential);

                if (thisDamagePotential > 5)
                {
                    otherAnimalHealth.TakeDamage(
                        thisDamagePotential,
                        false);

                    GetComponent<AnimalLvl>().DamagePlayerForXP(thisDamagePotential);
                    //Debug.Log("Animal " + otherAnimalHealth.playerNumber + " damaged for " + thisDamagePotential + " damage.");
                }
            }
        }
    }

    private void Awake ()
    {
        dustSize = dustTrail.main.startSize;
        hpBar = hpAndLvlBar.HealthBar; 
        hpAndLvlBarStartingY = hpAndLvlBar.transform.localPosition.y;
        hpBar.MaxValue = 100f;
        


        //player starts with 1 mass worth of health, if there are 6 groupings then 12 masses can be stored
        //healthBar.GetComponent<RepeatedRendererUGUI>().spriteIcon.color = playerColor;

        // Instantiate the explosion prefab and get a reference to the particle system on it.
        explosionParticles = Instantiate (explosionPrefab).GetComponent<ParticleSystem> ();

        // Get a reference to the audio source on the instantiated prefab.
        explosionAudio = explosionParticles.GetComponent<AudioSource> ();

        // Disable the prefab so it can be activated when it's required.
        explosionParticles.gameObject.SetActive (false);
    }


    private void OnEnable()
    {
        // When the animal is enabled, reset the animal's health and whether or not it's dead.
        currentHealth = 100;//minHealthPercent;
        hpBar.Value = 100;
        hpBar.Value = currentHealth;
        dead = false;
        UpdateScale();
        // Update the health slider's value and color.
    }

    public void TakeDamage (float amount, bool dmgFromFiring)
    {
        // Reduce current health by the amount of damage done.
        currentHealth -= amount;
        if (dmgFromFiring)
        {
            
        }
        hpBar.Value = currentHealth;

        // Change the UI elements appropriately.

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (currentHealth <= 0f && !dead)
        {
            OnDeath ();
        }

        UpdateScale();
    }

    public void AddHealth (float health)
    {
        // Reduce current health by the amount of damage done.
        currentHealth += health;
        //Debug.Log(amount+" health increased");
        if (currentHealth > 100) currentHealth = 100;
        hpBar.Value = (int) currentHealth;
        UpdateScale();
    }

    private void OnDeath ()
    {
        // Set the flag so that this function is only called once.
        dead = true;

        // Move the instantiated explosion prefab to the animal's position and turn it on.
        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive (true);

        // Play the particle system of the animal exploding.
        explosionParticles.Play ();

        // Play the animal explosion sound effect.
        explosionAudio.Play();
        
        // Turn the animal off.
        gameObject.SetActive (false);
        Destroy(transform.parent);
    }

    private void Update()
    {
        //if (playerColor.r==1) Debug.Log("ScaleHolder: " + tempSphereScaleHolder + ", scaleTarget: " + scaleTarget);
        
        if (Math.Abs(tempSphereScaleHolder-scaleTarget)>0.05)
        {
            sphereCollider.radius = Mathf.Lerp(sphereCollider.radius, scaleTarget / 2f, 0.15f);

            tempSphereScaleHolder = Mathf.Lerp(tempSphereScaleHolder, scaleTarget, 0.15f);
            animalSphere.transform.localScale = Vector3.one * tempSphereScaleHolder;
        }
    }

    private void UpdateScale()
    {
        scaleTarget = 0.3f + sizeScalingWithHealthMultiplier*currentHealth / 50;
        tempSphereScaleHolder = animalSphere.transform.localScale.x;
        
        
        rBody.mass = startingMass + scaleTarget/massScalingDivisor;
        
        
        dustTrail.transform.localScale = new Vector3(scaleTarget, scaleTarget/2, scaleTarget/2);
        dustTrail.transform.localPosition = new Vector3(dustTrail.transform.localPosition.x, -sphereCollider.radius/4,sphereCollider.radius/1.3f);
            //dustSize.constantMin = scale*10000;
            //dustSize.constantMax = scale*100000;
        
        hpAndLvlBar.transform.localPosition =
            new Vector3(hpAndLvlBar.transform.localPosition.x,hpAndLvlBarStartingY/3 + scaleTarget*50, hpAndLvlBar.transform.localPosition.z);//reposition health and lvl along with scale of animal
    }
}