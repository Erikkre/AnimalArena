using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource source;
    public List<AudioClip> mainMenuSongs, battleSongs, shopSongs, creditsSongs;

    private List<AudioClip> currentlyPlayingList;

    private AudioClip nextTrack;
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
    public void PlayNextTrackInCurrentList(){PlayNextTrack(currentlyPlayingList);}
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

                do
                {
                    nextTrack =
                        currentlyPlayingList[Random.Range(0, currentlyPlayingList.Count)];
                } while (nextTrack == source.clip);

                Invoke("PlayNextTrackInCurrentList", source.clip.length);
            }
        }
        else
        {
            source.clip = nextTrack;
            source.Play();
            do
            {
                nextTrack =
                    currentlyPlayingList[Random.Range(0, currentlyPlayingList.Count)];
            } while (nextTrack == source.clip);
            
        }
    }
}
