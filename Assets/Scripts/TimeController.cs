using UnityEngine;
using System.Collections;
using Chronos;

public class TimeController : MonoBehaviour {

    public string inputButton = "a";
    public KeyCode keyboardInputButton = KeyCode.LeftControl;
    public GlobalClock clock;
    public float downTimeValue = -1f;
    public float upTimeValue = 1f;
    public float transitionDuration = 0.25f;
    private bool previousButtonState = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        bool currentButtonState = Input.GetButton(inputButton) || Input.GetKey(keyboardInputButton);

        if (!currentButtonState)
            clock.localTimeScale = upTimeValue;
        if (currentButtonState)
            clock.localTimeScale = downTimeValue;

        previousButtonState = currentButtonState;
	}
}
