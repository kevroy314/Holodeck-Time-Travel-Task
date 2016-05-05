using UnityEngine;
using System.Collections;
using Chronos;

public class ClickableObject : MonoBehaviour
{

    public float clickStartTime = 0f;
    public float clickEndTime = 1f;
    public bool clickable = false;
    public GameObject targetObject;
    private MeshRenderer targetRender;
    private Component halo;
    private bool previousClickable = false;
    private Hashtable pulseHash;
    public Timeline localTime;

    // Use this for initialization
    public void Start()
    {
        targetObject.transform.localPosition = new Vector3(0f, -0.9f, 0f);
        targetRender = targetObject.GetComponent<MeshRenderer>();
        halo = targetObject.GetComponent("Halo");
        pulseHash = new Hashtable();
        pulseHash.Add("amount", new Vector3(0.02f, 0.02f, 0.02f));
        pulseHash.Add("time", 1f);
        pulseHash.Add("eastype", iTween.EaseType.easeInSine);
        pulseHash.Add("looptype", iTween.LoopType.loop);
    }

    // Update is called once per frame
    public void Update()
    {
        if (localTime.time > clickStartTime && localTime.time < clickEndTime)
            clickable = true;
        else
            clickable = false;
        if (clickable && !previousClickable)
        {
            iTween.PunchScale(targetObject, pulseHash);
            targetRender.enabled = true;
            halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
        }
        else if (!clickable)
        {
            targetRender.enabled = false;
            halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
            iTween.Stop(targetObject);
        }
        previousClickable = clickable;
    }
}
