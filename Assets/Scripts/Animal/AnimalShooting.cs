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
    private Transform targetTrans;


    private string FireButton;                // The input axis that is used for launching masss.
    private float currentLaunchForce;         // The force that will be given to the mass when the fire button is released.
    private float ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool Fired;                       // Whether or not the mass has been launched with this button press.

    private int temp;
    private float temp2;

    private void OnEnable() {
        // When the animal is turned on, reset the launch force and the UI
        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
    }


    private void Start ()
    {

        // The fire axis is based on the player number.
        FireButton = "Fire" + playerNumber;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        ChargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        //Mass.GetComponent<Mass>().shooterPlayerNumber = PlayerNumber;
        //Mass =Instantiate (Mass, FireTransform.position, FireTransform.rotation) as Mass;
        if (playerNumber==1) targetTrans = FindObjectOfType<GameManager>().transform.GetChild(0).transform.GetChild(0);
        aimSlider.transform.position = new Vector3(aimSlider.transform.position.x,aimSlider.transform.position.y,aimSlider.transform.position.z-1.2f);
    }


    private void Update ()
    {

        if (targetTrans)  //if player character
        {
            Debug.Log("% power shooting/2: "+currentLaunchForce/maxLaunchForce/2+", % health: "+animalHealth.currentHealth/100);

            // The slider should have a default value of the minimum launch force.
            if (aimSlider.value!=minLaunchForce) aimSlider.value = minLaunchForce;

            // If the max force has been exceeded and the mass hasn't yet been launched...
            if (currentLaunchForce >= maxLaunchForce && !Fired)
            {
                // ... use the max force and launch the mass.
                currentLaunchForce = maxLaunchForce;
                Fire();
            }
            // Otherwise, if the fire button has just started being pressed & half of the percentage of minLaunchForce from the max is less than the min health percentage
            else if (Input.GetButtonDown(FireButton) && minLaunchForce/maxLaunchForce/2 < animalHealth.currentHealth/100 )
            {
                aimSlider.value = minLaunchForce;

                // ... reset the fired flag and reset the launch force.
                Fired = false;
                currentLaunchForce = minLaunchForce;

                // Change the clip to the charging clip and start it playing.
                shootingAudio.clip = chargingClip;
                shootingAudio.Play();
            }
            else if (Input.GetButtonDown(FireButton)) //else if button just pressed and not enough health, flash the health bar
            {

            }
            // Otherwise, if the fire button is being held and the mass hasn't been launched yet & half of the percentage of currentLaunchForce is less than the current health percentage
            else if (Input.GetButton(FireButton) && !Fired && currentLaunchForce/maxLaunchForce/2 < animalHealth.currentHealth/100 )
            {
                // Increment the launch force and update the slider.
                currentLaunchForce += ChargeSpeed * Time.deltaTime;


                aimSlider.value = currentLaunchForce;

                aimSlider.transform.rotation = Quaternion.Euler(90,0, 10);

                int angleToAdd;
                if (targetTrans.position.x > animalPos.position.x) angleToAdd = -90;
                else angleToAdd = 90;
                
                aimSlider.transform.rotation = Quaternion.Euler(90,0, 
                    -Mathf.Rad2Deg* ((float)  Math.Atan((targetTrans.position.z-animalPos.position.z)/-(targetTrans.position.x-animalPos.position.x))  ) + angleToAdd);

            }
            // Otherwise, if the fire button is released and the mass hasn't been launched yet...
            else if (Input.GetButtonUp(FireButton) && !Fired)
            {
                animalHealth.TakeDamage((currentLaunchForce/maxLaunchForce/2)*100); //take off half of the percentage of the shot strength
                // ... launch the mass.
                Fire();
                currentLaunchForce = 0;
            }
        }
    }


    private void Fire ()
    {

        // Set the fired flag so only Fire is only called once.
        Fired = true;
        mass.coroutineManagerInstance = coroutineManagerInstance;
        mass.playerShooterNum = playerNumber;
        //print(Quaternion.LookRotation(targetTrans.position-FireTransform.position));
        // Create an instance of the mass and store a reference to it's rigidbody.
        Rigidbody massInstance =
            Instantiate (mass.rBody, fireTransform.position, Quaternion.LookRotation(targetTrans.position-fireTransform.position)) as Rigidbody;


        // Set the mass's velocity to the launch force in the fire position's forward direction.
        massInstance.velocity = currentLaunchForce * (targetTrans.position-fireTransform.position).normalized; ;

        // Change the clip to the firing clip and play it.
        shootingAudio.clip = fireClip;
        shootingAudio.Play ();

        // Reset the launch force.  This is a precaution in case of missing button events.
        currentLaunchForce = minLaunchForce;
    }
}