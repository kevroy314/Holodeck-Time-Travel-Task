using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using Chronos;

public class TemporalImagingEffect : MonoBehaviour {

    public VignetteAndChromaticAberration effectScript;
    public float positiveVignett = 0f;
    public float negativeVignett = 0.25f;
    public float transitionDuration = 0.25f;
    public GlobalClock clock;
    private float previousIntensity = 0f;

	// Update is called once per frame
	void Update () {
        float intensity = Mathf.Lerp(negativeVignett, positiveVignett, (clock.localTimeScale + 1) / 2);
        if (previousIntensity != intensity) {
            Hashtable ht = new Hashtable();
            ht.Add("from", effectScript.intensity);
            ht.Add("to", intensity);
            ht.Add("time", transitionDuration);
            ht.Add("onupdate", "changeIntensity");
            iTween.ValueTo(this.gameObject, ht);
        }
        previousIntensity = intensity;
	}

    void changeIntensity(float intensity)
    {
        effectScript.intensity = intensity;
    }
}
