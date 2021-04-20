using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerController : MonoBehaviour {
    
    [SerializeField]
    public Sound[] sonidos;
    
    AudioManagerController instancia;

    void Awake() {
        if (instancia == null) { instancia = this; } else { Destroy(gameObject); return; }
        foreach (Sound sonido in sonidos) {
            sonido.source = gameObject.AddComponent<AudioSource>();
            sonido.source.clip = sonido.sonido;
            sonido.source.volume = sonido.volumen;
            sonido.source.pitch = sonido.tono;
            sonido.source.loop = sonido.repetir;
        }
    }

    void Start(){ 
        try { Play("menu-music"); } catch(Exception err) { Debug.Log(err); return; }
    }

    public void Play(string nombre) { 
        Sound sonido = Array.Find(sonidos, i => i.nombre == nombre);
        if (sonido == null) { return; } else { sonido.source.Play(); }
    }
 
    /*public void StopAllAudio() {
        foreach (Sound sonido in sonidos) {
            gameObject.AddComponent<AudioSource>().Stop();
        } //No me funcionó :(
    }*/
}
