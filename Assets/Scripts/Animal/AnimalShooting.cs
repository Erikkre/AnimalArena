using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class AnimalShooting : MonoBehaviour
{
    
    public int PlayerNumber = 1;              // Used to identify the different players.
    public Mass Mass;                      // Prefab of the mass.
    public Transform FireTransform;           // A child of the animal where the masss are spawned.
    public Transform AnimalPos;           // A child of the animal where the masss are spawned.
    [HideInInspector] public CoroutineManager coroutineManagerInstance;
    public Slider AimSlider;                  // A child of the animal that displays the current launch force.
    public AudioSource ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip FireClip;                // Audio that plays when each shot is fired.
    public float MinLaunchForce = 15f;        // The force given to the mass if the fire button is not held.
    public float MaxLaunchForce = 30f;        // The force given to the mass if the fire button is held for the max charge time.
    public float MaxChargeTime = 0.75f;       // How long the mass can charge for before it is fired at max force.
    private Transform targetTrans;


    private string FireButton;                // The input axis that is used for launching masss.
    private float CurrentLaunchForce;         // The force that will be given to the mass when the fire button is released.
    private float ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool Fired;                       // Whether or not the mass has been launched with this button press.

    private int temp;
    private float temp2;

    private void OnEnable() {
        // When the animal is turned on, reset the launch force and the UI
        CurrentLaunchForce = MinLaunchForce;
        AimSlider.value = MinLaunchForce;
    }


    private void Start ()
    {

        // The fire axis is based on the player number.
        FireButton = "Fire" + PlayerNumber;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
        //Mass.GetComponent<Mass>().shooterPlayerNumber = PlayerNumber;
        //Mass =Instantiate (Mass, FireTransform.position, FireTransform.rotation) as Mass;
        if (PlayerNumber==1) targetTrans = FindObjectOfType<GameManager>().transform.GetChild(0).transform.GetChild(0);
        AimSlider.transform.position = new Vector3(AimSlider.transform.position.x,AimSlider.transform.position.y,AimSlider.transform.position.z-1.2f);
    }


    private void Update ()
    {

        if (targetTrans)  //if player character
        {

            // The slider should have a default value of the minimum launch force.
            AimSlider.value = MinLaunchForce;

            // If the max force has been exceeded and the mass hasn't yet been launched...
            if (CurrentLaunchForce >= MaxLaunchForce && !Fired)
            {
                // ... use the max force and launch the mass.
                CurrentLaunchForce = MaxLaunchForce;
                Fire();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (Input.GetButtonDown(FireButton))
            {
                // ... reset the fired flag and reset the launch force.
                Fired = false;
                CurrentLaunchForce = MinLaunchForce;

                // Change the clip to the charging clip and start it playing.
                ShootingAudio.clip = ChargingClip;
                ShootingAudio.Play();
            }
            // Otherwise, if the fire button is being held and the mass hasn't been launched yet...
            else if (Input.GetButton(FireButton) && !Fired)
            {
                // Increment the launch force and update the slider.
                CurrentLaunchForce += ChargeSpeed * Time.deltaTime;


                AimSlider.value = CurrentLaunchForce;

                AimSlider.transform.rotation = Quaternion.Euler(90,0, 10);

                int angleToAdd;
                if (targetTrans.position.x > AnimalPos.position.x) angleToAdd = -90;
                else angleToAdd = 90;
                
                AimSlider.transform.rotation = Quaternion.Euler(90,0, 
                    -Mathf.Rad2Deg* ((float)  Math.Atan((targetTrans.position.z-AnimalPos.position.z)/-(targetTrans.position.x-AnimalPos.position.x))  ) + angleToAdd);

            }
            // Otherwise, if the fire button is released and the mass hasn't been launched yet...
            else if (Input.GetButtonUp(FireButton) && !Fired)
            {
                // ... launch the mass.
                Fire();
            }
        }
    }


    private void Fire ()
    {
        // Set the fired flag so only Fire is only called once.
        Fired = true;
        Mass.coroutineManagerInstance = coroutineManagerInstance;
        Mass.playerShooterNum = PlayerNumber;
        //print(Quaternion.LookRotation(targetTrans.position-FireTransform.position));
        // Create an instance of the mass and store a reference to it's rigidbody.
        Rigidbody massInstance =
            Instantiate (Mass.rBody, FireTransform.position, Quaternion.LookRotation(targetTrans.position-FireTransform.position)) as Rigidbody;


        // Set the mass's velocity to the launch force in the fire position's forward direction.
        massInstance.velocity = CurrentLaunchForce * (targetTrans.position-FireTransform.position).normalized; ;

        // Change the clip to the firing clip and play it.
        ShootingAudio.clip = FireClip;
        ShootingAudio.Play ();

        // Reset the launch force.  This is a precaution in case of missing button events.
        CurrentLaunchForce = MinLaunchForce;
    }
}