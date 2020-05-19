using System;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AnimalManager
{
    // This class is to manage various settings on a animal.
    // It works with the GameManager class to control how the animals behave
    // and whether or not players have control of their animal in the
    // different phases of the game.

    
    [HideInInspector]public Color playerColor;                             // This is the color this animal will be tinted.
    public Transform spawnPoint;                          // The position and direction the animal will have when it spawns.
    [HideInInspector] public int playerNumber;            // This specifies which player this the manager for.
    [HideInInspector] public string coloredPlayerText;    // A string that represents the player with their number colored to match their animal.
    [HideInInspector] public GameObject instance;         // A reference to the instance of the animal when it is created.
    [HideInInspector] public int wins;                    // The number of wins this player has so far.

    [HideInInspector] public CoroutineManager coroutineManagerInstance;
    private AnimalMovement Movement;                        // Reference to animal's movement script, used to disable and enable control.
    private AnimalShooting Shooting;                        // Reference to animal's shooting script, used to disable and enable control.
    private AnimalHealth Health;
    private GameObject HealthCanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.


    public void Setup ()
    {
        // Get references to the components.
        Movement = instance.GetComponent<AnimalMovement> ();
        Shooting = instance.GetComponent<AnimalShooting> ();
        Health = instance.GetComponent<AnimalHealth> ();
        HealthCanvasGameObject = instance.GetComponentInChildren<Canvas> ().gameObject;

        // Set the player numbers to be consistent across the scripts.
        Movement.playerNumber = playerNumber;
        Shooting.playerNumber = playerNumber;
        Health.playerColor = playerColor;

        Shooting.coroutineManagerInstance = coroutineManagerInstance;
        // Create a string using the correct color that says 'PLAYER 1' etc based on the animal's color and the player's number.
        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER " + playerNumber + "</color>";

        // Get all of the renderers of the animal.
        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer> ();

        // Go through all the renderers...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... set their material color to the color specific to this animal.
            renderers[i].material.color = playerColor;
        }
    }


    // Used during the phases of the game where the player shouldn't be able to control their animal.
    public void DisableControl ()
    {
        Movement.enabled = false;
        Shooting.enabled = false;

        HealthCanvasGameObject.SetActive (false);
    }


    // Used during the phases of the game where the player should be able to control their animal.
    public void EnableControl ()
    {
        Movement.enabled = true;
        Shooting.enabled = true;

        HealthCanvasGameObject.SetActive (true);
    }


    // Used at the start of each round to put the animal into it's default state.
    public void Reset ()
    {
        instance.transform.position = 
            new Vector3(spawnPoint.position.x+Random.Range(-10,10),spawnPoint.position.y+Random.Range(10,30),spawnPoint.position.z+Random.Range(-10,10));
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive (false);
        instance.SetActive (true);

    }
}