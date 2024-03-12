using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] footstepClips;

    private float timer = 0f;
    private int count = 0;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        Jam.Events.PlayerSoundEvent.onFootStep += playFootstep;
    }
    public void playFootstep(float moveAmount, bool running)
    {
        if (moveAmount == 0) return;

        float timeToStep = running ? 0.5f : 0.7f;

        timer += Time.deltaTime;

        if(timer > timeToStep)
        {
            source.PlayOneShot(footstepClips[0]);
            timer = 0f;
            count++;
        }

        if (count > footstepClips.Length - 1) count = 0;
    }
}
