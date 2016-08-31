﻿using UnityEngine;
using System.Collections;
using Chronos;

public class FlyToSky : ClickableObject
{

    public float transitionDelay = 10f;
    public float transitionDuration = 1f;

    public Vector3 startPos = new Vector3(0f, 0f, 0f);
    public Vector3 endPos = new Vector3(0f, 10f, 0f);

    private MeshRenderer render;
    public Timeline time;
    private AudioSource audioSrc;
    private bool playedForward = false;
    private bool playedBackward = true;

    private Hashtable goToStartHash;
    private Hashtable goToEndHash;

    // Use this for initialization
    public void Start()
    {
        StartI();

        render = GetComponent<MeshRenderer>();
        //time = GetComponent<Timeline>();

        audioSrc = transform.parent.gameObject.GetComponent<AudioSource>();

        transform.localPosition = startPos;

        goToStartHash = new Hashtable();
        goToStartHash.Add("position", startPos);
        goToStartHash.Add("islocal", true);
        goToStartHash.Add("time", transitionDuration);
        goToStartHash.Add("easetype", iTween.EaseType.easeOutBounce);
        goToEndHash = new Hashtable();
        goToEndHash.Add("position", endPos);
        goToEndHash.Add("islocal", true);
        goToEndHash.Add("time", transitionDuration);
        goToEndHash.Add("easetype", iTween.EaseType.easeInBounce);

        clickStartTime = transitionDelay;
        clickEndTime = transitionDelay + transitionDuration;
        localTime = time;

        render.enabled = true;
    }

    public void Update()
    {
        UpdateI();

        if (time.deltaTime > 0 && time.time >= transitionDelay && !playedForward)
        {
            iTween.MoveTo(gameObject, goToEndHash);
            audioSrc.timeSamples = 0;
            audioSrc.pitch = 1;
            audioSrc.Play();
            playedForward = true;
            playedBackward = false;
        }
        if (time.deltaTime > 0 && time.time > transitionDelay + transitionDuration && render.enabled)
        {
            render.enabled = false;
        }
        if (time.deltaTime < 0 && time.time < transitionDelay + transitionDuration && !render.enabled)
        {
            render.enabled = true;
        }
        if (time.deltaTime < 0 && time.time < transitionDelay + transitionDuration && !playedBackward)
        {
            iTween.MoveTo(gameObject, goToStartHash);
            audioSrc.Stop();
            audioSrc.timeSamples = audioSrc.clip.samples - 1;
            audioSrc.pitch = -1;
            audioSrc.Play();
            playedBackward = true;
            playedForward = false;
        }
    }
}
