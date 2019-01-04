using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {


    // Returns the current inputManager
    private static AudioManager _audioManager;

    public static AudioManager audioManager
    {
        get {
            if (_audioManager != null)
                return _audioManager;
            _audioManager = FindObjectOfType<AudioManager>();
            if (_audioManager == null)
            {
                Debug.LogWarning("Audio Manager not found, Created new Audio Manager.");
                _audioManager = new GameObject("Audio Manager").AddComponent<AudioManager>();
                GameObject g = GameObject.Find("Managers");
                _audioManager.transform.SetParent(g != null ? g.transform : new GameObject("Managers").transform);
            }
            return _audioManager;
        }
    }

    public Sound[] sounds; //all the sounds in the game

	// Use this for initialization
	void Awake () {
        
        if (audioManager != this)
            Destroy(this);
            

        foreach(Sound s in audioManager.sounds)  //initializes the array of Sounds
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
	}
	
    public void Play(string name) //plays a sound if it is found
    {
        //Debug.Log("Going to play : " + name);
        try
        {
            Sound s = Array.Find(sounds, sound => sound.name == name); //this is kinda cool 
            if (s == null) //error checking, only play a sound if you can find it
            {
                Debug.LogError("Sounds " + name + " could not be found");
                return;
            }
            s.source.Play();
        }
        catch
        {
            Debug.Log("Something was caught it saeems");
        }

       // Debug.Log(name + " wwas okated rught? On a volume of " + s.source.volume);

        
    }
}

//place this anywhere in the game to play a noises 

//AudioManager.audioManager.Play("Bow");

//for all sound effects + audio
[System.Serializable]
public class Sound
{

    public AudioClip clip;
    public string name;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;


}


//Source info 

//Bow sound effect from https://www.zapsplat.com/music/bow-and-arrow-bow-release-pluck-1/