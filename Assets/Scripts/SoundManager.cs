using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }


    private void Awake()
    {
        instance = this;
    }

    public AudioSource bgMusic,oneShotSound;

    [SerializeField] AudioClip uiClick,jump,slideDown,explosion,menuBgMusic,inGameBgMusic,coinSound,swipeSound;


    public void PlayInGameMusic()
    {
        bgMusic.clip = inGameBgMusic;
        bgMusic.Play();
    }
    public void PlayMenuMusic()
    {
        bgMusic.clip = menuBgMusic;
        bgMusic.Play();
    }

    public void CoinPickup()
    {
        oneShotSound.PlayOneShot(coinSound);
    }
    public void UIClickSound()
    {
        oneShotSound.PlayOneShot(uiClick);
    }
    public void jumpSound()
    {
        oneShotSound.PlayOneShot(jump);
    }
    public void swipeDownSound()
    {
        oneShotSound.PlayOneShot(slideDown);
    }
    public void swipeSideWaysSound()
    {
        oneShotSound.PlayOneShot(swipeSound);
    }
    public void ExplosionSound()
    {
        oneShotSound.PlayOneShot(explosion);
    }
}
