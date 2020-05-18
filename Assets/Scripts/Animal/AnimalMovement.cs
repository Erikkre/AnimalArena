using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    public int PlayerNumber = 1;              // Used to identify which animal belongs to which player.  This is set by this animal's manager.
    public float Speed = 120f;                 // How fast the animal moves forward and back.
    public float j_Speed = 2f;            //jump speed
    //public float TurnSpeed = 180f;            // How fast the animal turns in degrees per second.
    public AudioSource RollingAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip EngineIdling;            // Audio to play when the animal isn't moving.
    public AudioClip EngineDriving;           // Audio to play when the animal is moving.
    public float PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    private Rigidbody Rigidbody;              // Reference used to move the animal.

    private string xAxisName;          // The name of the x input axis
    private string yAxisName;
    private string zAxisName;
    private float xInputValue;         // The current value of the Xmovement input.
    private float yInputValue;         // The current value of the Xmovement input.
    private float zInputValue;         // The current value of the Xmovement input.

    private float TurnInputValue;             // The current value of the turn input.
    private float OriginalPitch;              // The pitch of the audio source at the start of the scene.


    private void Awake ()
    {
        Rigidbody = GetComponent<Rigidbody> ();
    }


    private void OnEnable ()
    {
        // When the animal is turned on, make sure it's not kinematic.
        Rigidbody.isKinematic = false;

        // Also reset the input values.
        xInputValue = 0f;
        TurnInputValue = 0f;
    }

    private void OnDisable ()
    {
        // When the animal is turned off, set it to kinematic so it stops moving.
        Rigidbody.isKinematic = true;
    }

    private void Start ()
    {
        // The axes names are based on player number.
        xAxisName = "x" + PlayerNumber;
        yAxisName = "y" + PlayerNumber;
        zAxisName = "z" + PlayerNumber;

        // Store the original pitch of the audio source.
        OriginalPitch = RollingAudio.pitch;
    }


    private void Update ()
    {
        // Store the value of both input axes.
        xInputValue = Input.GetAxis (xAxisName);
        zInputValue = Input.GetAxis (zAxisName);
        yInputValue = Input.GetAxis (yAxisName);

        EngineAudio ();
    }

    private void EngineAudio ()
    {
        // If there is no input (the animal is stationary)...
        if (Mathf.Abs (xInputValue) < 0.1f && Mathf.Abs (TurnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (RollingAudio.clip == EngineDriving)
            {
                // ... change the clip to idling and play it.
                RollingAudio.clip = EngineIdling;
                RollingAudio.pitch = Random.Range (OriginalPitch - PitchRange, OriginalPitch + PitchRange);
                RollingAudio.Play ();
            }
        }
        else
        {
            // Otherwise if the animal is moving and if the idling clip is currently playing...
            if (RollingAudio.clip == EngineIdling)
            {
                // ... change the clip to driving and play.
                RollingAudio.clip = EngineDriving;
                RollingAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange);
                RollingAudio.Play();
            }
        }
    }


    private void FixedUpdate ()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move ();
        //Turn ();
    }


    private void Move ()
    {
        // Create a vector in the direction the animal is facing with a magnitude based on the input, speed and the time between frames.
        Rigidbody.velocity = new Vector3(Speed * Time.deltaTime*xInputValue, Rigidbody.velocity.y, Speed * Time.deltaTime*zInputValue) ;

        // Apply this movement to the rigidbody's position.
        //Rigidbody.AddForce(force);
    }


    /*private void Turn ()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = TurnInputValue * TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler (turn, turn, turn);

        // Apply this rotation to the rigidbody's rotation.
        Rigidbody.AddForce(); (Rigidbody.rotation * turnRotation);
    }*/
}