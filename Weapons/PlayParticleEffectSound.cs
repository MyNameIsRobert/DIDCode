using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleEffectSound : MonoBehaviour {

    public AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    void PlaySound(AudioClip clip)
    {
        Debug.Log(clip);
        aud.PlayOneShot(clip);
    }

}
