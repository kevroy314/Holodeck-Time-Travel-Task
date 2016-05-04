using UnityEngine;
using System.Collections;
using Chronos;

public class FallFromSky : MonoBehaviour {

    public float modifiedStartDelay = 10f;
    public float reverseClipPreDelay = 0.75f;

    private MeshRenderer render;
    private BoxCollider collide;
    private Animator anim;
    public AnimationClip clip;
    private Timeline time;
    private AudioSource audioSrc;
    private bool playedForward = false;
    private bool playedBackward = false;

    // Use this for initialization
    void Start () {
        render = GetComponent<MeshRenderer>();
        collide = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        time = GetComponent<Timeline>();

        audioSrc = transform.parent.gameObject.GetComponent<AudioSource>();

        anim.enabled = false;
        render.enabled = false;
        collide.enabled = false;
    }

    void Update()
    {
        if (time.deltaTime > 0 && time.time >= modifiedStartDelay && anim.enabled == false)
        {
            anim.SetTime(0);
            anim.enabled = true;
            render.enabled = true;
            collide.enabled = true;
        }
        else if (time.deltaTime < 0 && time.time < modifiedStartDelay)
        {
            anim.enabled = false;
            render.enabled = false;
            collide.enabled = false;
        }

        if (time.deltaTime > 0 && time.time >= modifiedStartDelay && !playedForward)
        {
            audioSrc.timeSamples = 0;
            audioSrc.pitch = 1;
            audioSrc.Play();
            playedForward = true;
            playedBackward = false;
        }
        else if (time.deltaTime < 0 && time.time < modifiedStartDelay + reverseClipPreDelay && !playedBackward)
        {
            audioSrc.Stop();
            audioSrc.timeSamples = audioSrc.clip.samples - 1;
            audioSrc.pitch = -1;
            audioSrc.Play();
            playedBackward = true;
            playedForward = false;
        }
    }
}
