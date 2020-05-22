using EnergyBarToolkit;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AnimalHealth : MonoBehaviour
{
    [HideInInspector]public float startingAndMinHealthToShoot;               // The amount of health each animal starts with.

    //public Image FillImage;                           // The image component of the slider.
    [HideInInspector]public Color playerColor;

    public GameObject explosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the animal dies.
    public EnergyBar healthBar;
    public int healthBarGroupings;    //standard is 6, each grouping = 2 mass

    private AudioSource explosionAudio;               // The audio source to play when the animal explodes.
    private ParticleSystem explosionParticles;        // The particle system the will play when the animal is destroyed.
    [HideInInspector]public float currentHealth;                      // How much health the animal currently has.
    private bool dead;                                // Has the animal been reduced beyond zero health yet?
    public Transform animalSphere;
    public SphereCollider sphereCollider;
    public int healthMassNumber;
    public float sizeScalingWithHealthMultiplier =1;
    private void Awake ()
    {

        startingAndMinHealthToShoot = healthMassNumber*100f / healthBarGroupings / 2; //player starts with 1 mass worth of health, if there are 6 groupings then 12 masses can be stored
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
        currentHealth = startingAndMinHealthToShoot;
        healthBar.valueCurrent = (int) currentHealth;
        dead = false;
        UpdateScale();
        // Update the health slider's value and color.
    }

    public void TakeDamage (float amount)
    {
        // Reduce current health by the amount of damage done.
        currentHealth -= amount;
        healthBar.valueCurrent = (int) currentHealth;

        // Change the UI elements appropriately.

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (currentHealth <= 0f && !dead)
        {
            OnDeath ();
        }

        UpdateScale();
    }

    public void AddHealth (float amount)
    {
        // Reduce current health by the amount of damage done.
        currentHealth += amount;
        //Debug.Log(amount+" health increased");
        if (currentHealth > 100) currentHealth = 100;
        healthBar.valueCurrent = (int) currentHealth;
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
        Destroy(this);
    }

    private void UpdateScale()
    {
        float scale = sizeScalingWithHealthMultiplier*currentHealth / 20;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}