using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound {
    [SerializeField]
    public string nombre;
    
    [SerializeField]
    public AudioClip sonido;
    
    [SerializeField]
    [Range(0F, 1F)]
    public float volumen;
    
    [SerializeField]
    [Range(0.01F, 3F)]
    public float tono;
    
    [SerializeField]
    public bool repetir;

    [HideInInspector]
    public AudioSource source;
}
