using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanardPitch : MonoBehaviour
{
    public float MinSoundDelay = 1;
    public float RandomDelay = 5;
    private float NextSoundDelay = 0;

    public float MaxDeltaPitch = 0.1f;

    private AudioSource aSource;


    private void computeNextSoundDelay()
    {
        NextSoundDelay = MinSoundDelay + Random.Range(0, RandomDelay);
    }

    void Start()
    {
        aSource = GetComponent<AudioSource>();
        computeNextSoundDelay();
    }

    void Update()
    {
        NextSoundDelay -= Time.deltaTime;
        if (NextSoundDelay <= 0)
        {
            aSource.Play();
            //Variation du pitch entre +MaxDeltaPitch et -MaxDeltaPitch
            aSource.pitch = 1 + Random.Range(0, MaxDeltaPitch) * 2 - MaxDeltaPitch;
            computeNextSoundDelay();
        }
    }
}