using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplayMusicStarter : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.musicGameplay);
        }
    }
}
