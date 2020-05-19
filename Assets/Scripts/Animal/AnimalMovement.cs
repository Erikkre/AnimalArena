using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalMovement : MonoBehaviour
{
    [HideInInspector]public int playerNumber = 1;              // Used to identify which animal belongs to which player.  This is set by this animal's manager.
    public float speed = 1200f;                 // How fast the animal moves forward and back.
    public float jSpeed = 20f;            //jump speed
    //public float TurnSpeed = 180f;            // How fast the animal turns in degrees per second.
    public AudioSource rollingAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip engineIdling;            // Audio to play when the animal isn't moving.
    public AudioClip engineDriving;           // Audio to play when the animal is moving.
    public float pitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    private Rigidbody Rigidbody;              // Reference used to move the animal.

    private string xAxisName;          // The name of the x input axis
    private string yAxisName;
    private string zAxisName;
    private float xInputValue;         // The current value of the Xmovement input.
    private float yInputValue;         // The current value of the Ymovement input.
    private float zInputValue;         // The current value of the Zmovement input.

    private float TurnInputValue;             // The current value of the turn input.
    private float OriginalPitch;              // The pitch of the audio source at the start of the scene.
    [HideInInspector]public Color playerColor;

    public Transform flatCanvas, raisedCanvas;
    private Quaternion originalFlatCanvas, originalRaisedCanvas,originalLocalFlatCanvas, originalLocalRaisedCanvas;

    private void Awake ()
    {
        originalFlatCanvas = flatCanvas.rotation;
        originalRaisedCanvas = raisedCanvas.rotation;

        //originalLocalFlatCanvas = flatCanvas.localRotation;
        //originalLocalRaisedCanvas = raisedCanvas.localRotation;
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
        xAxisName = "x" + playerNumber;
        yAxisName = "y" + playerNumber;
        zAxisName = "z" + playerNumber;

        // Store the original pitch of the audio source.
        OriginalPitch = rollingAudio.pitch;
    }


    private void Update ()
    {
        // Store the value of both input axes.
        xInputValue = Input.GetAxis (xAxisName);
        zInputValue = Input.GetAxis (zAxisName);
        yInputValue = Input.GetAxis (yAxisName);

        EngineAudio ();

    }

    private void LateUpdate()
    {
        flatCanvas.rotation = originalFlatCanvas;
        raisedCanvas.rotation = originalRaisedCanvas;
        //flatCanvas.transform.localRotation = originalLocalFlatCanvas;
        //raisedCanvas.transform.localRotation=originalLocalRaisedCanvas;
    }

    private void EngineAudio ()
    {
        // If there is no input (the animal is stationary)...
        if (Mathf.Abs (xInputValue) < 0.1f && Mathf.Abs (TurnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (rollingAudio.clip == engineDriving)
            {
                // ... change the clip to idling and play it.
                rollingAudio.clip = engineIdling;
                rollingAudio.pitch = Random.Range (OriginalPitch - pitchRange, OriginalPitch + pitchRange);
                rollingAudio.Play ();
            }
        }
        else
        {
            // Otherwise if the animal is moving and if the idling clip is currently playing...
            if (rollingAudio.clip == engineIdling)
            {
                // ... change the clip to driving and play.
                rollingAudio.clip = engineDriving;
                rollingAudio.pitch = Random.Range(OriginalPitch - pitchRange, OriginalPitch + pitchRange);
                rollingAudio.Play();
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
        float movementX = xInputValue * speed * Time.deltaTime;
        float movementZ = zInputValue * speed * Time.deltaTime;
        float movementY = yInputValue * speed * Time.deltaTime;


        // Apply this movement to the rigidbody's position.
        Rigidbody.AddForce(new Vector3(movementX,movementY,movementZ));

        Vector3 force = new Vector3(movementX, Rigidbody.velocity.y*3,
            movementZ);
        // Create a vector in the direction the animal is facing with a magnitude based on the input, speed and the time between frames.
        //Rigidbody.velocity = force/3 ;

        // Apply this movement to the rigidbody's position.

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