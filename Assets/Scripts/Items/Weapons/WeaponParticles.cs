using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParticles 
{
    //This script will handle the playing, pausing, stopping, of particle systems used for the Charge Bow
    //This will be an object that PlayerController will have an instance of so this won't be a MonoBehavior

    public GameObject Root; //the root GameObject that contains all of the particles 

    public ParticleSystem[] Particles;

    public ParticleSystem[] LoopParticles; //particles that loop after the charge is complete

    public ParticleSystem Circle; //making this first as a demo

    public void UpdateRoot(GameObject root, Color playerColor) //the root is the parent of all of the particle game objects;
    {
        Root = root;
        //Particles = new ParticleSystem[1];
        Particles = Root.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem part in Particles)
        {
            part.GetComponent<Renderer>().material.color = playerColor * 2.5f;
        }
        //Particles[0] = Root.transform.Find("Center").GetComponent<ParticleSystem>();
        //Particles[0].GetComponent<Renderer>().material.color = playerColor * 1.5f;
        //Particles[1] = Root.transform.Find("Lines").GetComponent<ParticleSystem>();
        //Particles[1].GetComponent<Renderer>().material.color = playerColor * 1.5f;
        //Particles[1] = Root.transform.Find("Circle").GetComponent<ParticleSystem>();
        //Particles[1].GetComponent<Renderer>().material.color = playerColor * 1.5f;
        //Circle = Root.transform.Find("Circle").GetComponent<ParticleSystem>(); //hardcoded for the hell of it hee hee
        //Circle.GetComponent<Renderer>().material.color = playerColor * 1.5f;


    }

    public void Play()
    {
        foreach (ParticleSystem part in Particles)
            part.Play();
    }

    public void Stop()
    {
        foreach (ParticleSystem part in Particles)
        {
            part.Stop();
            part.Clear();
        }
        //Circle.Stop();
        //Circle.Clear();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
