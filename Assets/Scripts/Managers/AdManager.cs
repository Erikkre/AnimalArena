using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Debug.Log("test");
        if (Advertisement.IsReady("sportsAd"))
        {
            Advertisement.Show("sportsAd");
            Debug.Log("test2");
        }
        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
            Debug.Log("test3");
        }
    }
}
