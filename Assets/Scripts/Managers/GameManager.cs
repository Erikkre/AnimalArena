using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public CoroutineManager coroutineManagerInstance;
    public int NumRoundsToWin = 2;            // The number of rounds a single player has to win to win the game.
    public float StartDelay = 0.1f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float EndDelay = 0.1f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    public CameraControl CameraControl;       // Reference to the CameraControl script for control during different phases.
    public Text MessageText;                  // Reference to the overlay Text to display winning text, etc.
    public GameObject AnimalPrefab;             // Reference to the prefab the players will control.
    public AnimalManager[] Animals;               // A collection of managers for enabling and disabling different aspects of the animals.
    
    public bool playerIsAlive = true;
    private int RoundNumber;                  // Which round the game is currently on.
    private WaitForSeconds StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds EndWait;           // Used to have a delay whilst the round or game ends.
    private AnimalManager RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private AnimalManager GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.


    private void Start()
    {
        // Create the delays so they only have to be made once.
        StartWait = new WaitForSeconds (StartDelay);
        EndWait = new WaitForSeconds (EndDelay);


        SpawnAllAnimals();
        SetCameraTargets();

        // Once the animals have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
    }


    private void SpawnAllAnimals()
    {
        // For all the animals...
        for (int i = 0; i < Animals.Length; i++)
        {

            // ... create them, set their player number and references needed for control.
            Animals[i].Instance =
                Instantiate(AnimalPrefab, Animals[i].SpawnPoint.position, Animals[i].SpawnPoint.rotation);
            Animals[i].PlayerNumber = i + 1;
            Animals[i].coroutineManagerInstance = coroutineManagerInstance;
            Animals[i].Setup();
            //if (i == 1) CameraControl.mainPlayer = Animals[i];
        }
    }


    private void SetCameraTargets()
    {
        /*// Create a collection of transforms the same size as the number of animals.
        Transform[] targets = new Transform[Animals.Length];

        // For each of these transforms...
        for (int i = 0; i < targets.Length; i++)
        {
            // ... set it to the appropriate animal transform.
            targets[i] = Animals[i].Instance.transform;
        }*/

        // These are the targets the camera should follow.

        CameraControl.Targets = Animals;
    }


    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop ()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundStarting ());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine (RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (GameWinner != null)
        {
            // If there is a game winner, restart the level.
            Application.LoadLevel (Application.loadedLevel);
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine (GameLoop ());
        }
    }


    private IEnumerator RoundStarting ()
    {
        // As soon as the round starts reset the animals and make sure they can't move.
        ResetAllAnimals ();
        DisableAnimalControl ();

        // Snap the camera's zoom and position to something appropriate for the reset animals.
        CameraControl.SetStartPositionAndSize ();

        // Increment the round number and display text showing the players what round it is.
        RoundNumber++;
        MessageText.text = "ROUND " + RoundNumber;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return StartWait;
    }


    private IEnumerator RoundPlaying ()
    {
        // As soon as the round begins playing let the players control the animals.
        EnableAnimalControl ();

        // Clear the text from the screen.
        MessageText.text = string.Empty;

        // While there is not one animal left...
        while (!OneAnimalLeft())
        {
            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding ()
    {
        // Stop animals from moving.
        DisableAnimalControl ();

        // Clear the winner from the previous round.
        RoundWinner = null;

        // See if there is a winner now the round is over.
        RoundWinner = GetRoundWinner ();

        // If there is a winner, increment their score.
        if (RoundWinner != null)
            RoundWinner.Wins++;

        // Now the winner's score has been incremented, see if someone has one the game.
        GameWinner = GetGameWinner ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage ();
        MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return EndWait;
    }


    // This is used to check if there is one or fewer animals remaining and thus the round should end.
    private bool OneAnimalLeft()
    {
        // Start the count of animals left at zero.
        int numAnimalsLeft = 0;

        // Go through all the animals...
        for (int i = 0; i < Animals.Length; i++)
        {
            // ... and if they are active, increment the counter.
            if (Animals[i].Instance.activeSelf)
                numAnimalsLeft++;
        }

        // If there are one or fewer animals remaining return true, otherwise return false.
        return numAnimalsLeft <= 1;
    }


    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer animals are currently active.
    private AnimalManager GetRoundWinner()
    {
        // Go through all the animals...
        for (int i = 0; i < Animals.Length; i++)
        {
            // ... and if one of them is active, it is the winner so return it.
            if (Animals[i].Instance.activeSelf)
                return Animals[i];
        }

        // If none of the animals are active it is a draw so return null.
        return null;
    }


    // This function is to find out if there is a winner of the game.
    private AnimalManager GetGameWinner()
    {
        // Go through all the animals...
        for (int i = 0; i < Animals.Length; i++)
        {
            // ... and if one of them has enough rounds to win the game, return it.
            if (Animals[i].Wins == NumRoundsToWin)
                return Animals[i];
        }

        // If no animals have enough rounds to win, return null.
        return null;
    }


    // Returns a string message to display at the end of each round.
    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";

        // If there is a winner then change the message to reflect that.
        if (RoundWinner != null)
            message = RoundWinner.ColoredPlayerText + " WINS THE ROUND!";

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Go through all the animals and add each of their scores to the message.
        for (int i = 0; i < Animals.Length; i++)
        {
            message += Animals[i].ColoredPlayerText + ": " + Animals[i].Wins + " WINS\n";
        }

        // If there is a game winner, change the entire message to reflect that.
        if (GameWinner != null)
            message = GameWinner.ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    // This function is used to turn all the animals back on and reset their positions and properties.
    private void ResetAllAnimals()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].Reset();
        }
    }


    private void EnableAnimalControl()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].EnableControl();
        }
    }


    private void DisableAnimalControl()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].DisableControl();
        }
    }
}