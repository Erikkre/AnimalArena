using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource source;
    public List<AudioClip> mainMenuSongs, battleSongs, shopSongs, creditsSongs;

    private List<AudioClip> currentlyPlayingList;
    // Start is called before the first frame update
    void Start()
    {
        switchToBattleMusic();
        //switchToMainMenuMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchToMainMenuMusic(){ PlayNextTrack(mainMenuSongs); }
    public void switchToBattleMusic(){ PlayNextTrack(battleSongs); }
    public void switchToShopMusic(){ PlayNextTrack(shopSongs); }
    public void switchToCreditsMusic(){ PlayNextTrack(creditsSongs); }

    public void PlayNextTrack(List<AudioClip> l)
    {
        if (l != currentlyPlayingList)
        {
            //Debug.Log("new list");
            currentlyPlayingList = l;
            if (source.isPlaying) source.Stop();

            source.clip = l[Random.Range(0, l.Count)];
            source.Play();

            if (l.Count == 1) source.loop = true;
            else
            {

                AudioClip nextTrack =
                    currentlyPlayingList[Random.Range(0, currentlyPlayingList.Count)];
                while (nextTrack == source.clip)
                {
                    nextTrack = currentlyPlayingList[Random.Range(0, currentlyPlayingList.Count)];
                }

                Invoke("PlayNextTrack", source.clip.length);
            }
        }
        else
        {
            AudioClip nextTrack =
                                currentlyPlayingList[Random.Range(0, currentlyPlayingList.Count)];
            while (nextTrack == source.clip)
            {
                nextTrack = currentlyPlayingList[Random.Range(0, currentlyPlayingList.Count)];
            }
        }
    }
}
