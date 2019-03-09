using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour {



    public Sound[] sounds; //all the sounds in the game

    public Sound[] WinnerAnnouncement;

    public AudioMixer audioMixer; //global audio settings

    public static AudioManager instance;

    public int announcerType;

    public bool muted = false;

	// Use this for initialization
	void Awake () {

        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        foreach (Sound s in sounds)  //initializes the array of Sounds
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;
        }

        foreach (Sound s in WinnerAnnouncement)  //initializes the array of Sounds for the win announcement
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;
        }

        announcerType = 2;//UnityEngine.Random.Range(1,3); //mickey mouse, old man, etc
	}

    private int randomSound;
  
    private int RandomSoundIndex(int max)
    {
        return UnityEngine.Random.Range(1, max + 1);
    }

    public void PlayRandomSound(string name) //for example use Death,3 will play either Death1, Death2, or Death3
    {
        switch(name)
        {
            case "Death":  instance.Play(name + RandomSoundIndex(4));  break;

            case "Ready": instance.Play(name + announcerType); break;
            default: Debug.LogError(name+" case not found"); break;
        } 
    }

    public void AnnounceWinner(int pNumber)
    {
        string name = "";
        switch(GameManager.instance.GetPlayerColor(pNumber))
        {
            case 0: name = "Red" + announcerType.ToString();  break; //change this to work with not just playernumber
            case 1: name = "Blue" + announcerType.ToString(); break;
            case 2: name = "Yellow" + announcerType.ToString(); break;
            case 3: name = "Yellow" + announcerType.ToString(); break;
            case 4: name = "Purple" + announcerType.ToString(); break;
            case 5: name = "Pink" + announcerType.ToString(); break;
            case 6: name = "Blue" + announcerType.ToString(); break;
            case 7: name = "Green" + announcerType.ToString(); break;
            default: break;
        }
         if(name != "")
            Array.Find(WinnerAnnouncement, sound => sound.name == name).source.Play();
    }
	
    public void Play(string name) //plays a sound if it is found
    {
            Sound s = Array.Find(sounds, sound => sound.name == name); //this is kinda cool 
            if (s == null) //error checking, only play a sound if you can find it
            {
                Debug.LogError("Sounds " + name + " could not be found");
                return;
            }
            s.source.Play();
            // Debug.Log("name of audio mixer group for sound " + s.name + ": " + s.source.outputAudioMixerGroup.name);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); //this is kinda cool 
        if (s == null) //error checking, only play a sound if you can find it
        {
            Debug.LogError("Sounds " + name + " could not be found");
            return;
        }
        s.source.Stop();
    }

    public void Mute()
    {
        float volume;
        if (!muted)
            volume = -80; //min volume
        else
            volume = 0; //max volume
        muted = !muted; //flip it

        audioMixer.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("SoundEffectsVolume", volume);

    }

    public void IsSoundPlaying(string name, out bool isOutputPlaying) {
        foreach (Sound s in sounds) {
            if (s.name.Equals(name)) {
                if (s.source.isPlaying)
                    isOutputPlaying = true;
                else 
                    isOutputPlaying = false;
                
                return;
            }
        }

        isOutputPlaying = false;
        Debug.Log("Sound " + name + " does not exist in audio manager!");
    }

    // uses same algorithm as Play(string) does to find sound
    public Sound GetSound(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name); //this is kinda cool 
        if (s == null) //error checking, only play a sound if you can find it
        {
            Debug.LogError("Sounds " + name + " could not be found");
            return null;
        }

        return s;
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        audioMixer.SetFloat("SoundEffectsVolume", volume);
    }

}



//place this anywhere in the game to play a noises 

//GameManager.instance.audioManager.Play("Bow");

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


    public AudioMixerGroup output;

}


//Source info 

//Bow sound effect from https://www.zapsplat.com/music/bow-and-arrow-bow-release-pluck-1/

/*

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
*/
