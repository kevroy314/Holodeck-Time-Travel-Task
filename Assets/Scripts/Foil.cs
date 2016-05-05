using UnityEngine;
using System.Collections;
using Chronos;

public class Foil : ClickableObject
{

    private MeshRenderer render;
    private BoxCollider collide;
    private Timeline time;
    private AudioSource audioSrc;

    // Use this for initialization
    new void Start () {
        base.Start();

        render = GetComponent<MeshRenderer>();
        collide = GetComponent<BoxCollider>();
        time = GetComponent<Timeline>();

        audioSrc = transform.parent.gameObject.GetComponent<AudioSource>();

        transform.localPosition = Vector3.zero;

        render.enabled = true;
        collide.enabled = true;

        clickStartTime = 0f;
        clickEndTime = float.MaxValue;
        localTime = time;
    }

    new void Update()
    {
        base.Update();
    }
}
