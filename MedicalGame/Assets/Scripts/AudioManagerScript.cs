﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Prefab("SoundContainer", true, "")]
public class AudioManagerScript : Singleton<AudioManagerScript>
{

    private AudioSource normalButtonSound;
    private AudioSource sliderButtonSound;
    public AudioSource wrongAnwserSound;
    public AudioSource achievementUnlockSound;
    public AudioSource TimeUpSound;
    public AudioSource lvlUpSound;

    public static bool pause;
    void Start() {
        AudioSource[] a_sources = GetComponents<AudioSource>();
        normalButtonSound = a_sources[0];
        sliderButtonSound = a_sources[1];
        wrongAnwserSound = a_sources[2];
		achievementUnlockSound = a_sources[3];
        TimeUpSound = a_sources[4];
        lvlUpSound = a_sources[5];
    }

    //Normal button function
    public void OnButtonclick()
    {
        normalButtonSound.Play();
    }
    //Slider button function
    public void OnSliderClick()
    {
        sliderButtonSound.Play();
    }

    public void pauseAudio()
  {
      Debug.Log(pause);
      if(AudioListener.pause)
      {
          AudioListener.pause = false;
      }
      else  AudioListener.pause = true;
  }

}
