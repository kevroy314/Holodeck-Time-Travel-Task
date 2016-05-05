using UnityEngine;
using System.Collections;
using Chronos;

public class TimeController : MonoBehaviour {

    public string configFile = "simulation.config";

    public string inputButton = "a";
    public KeyCode keyboardInputButton = KeyCode.LeftControl;
    public GlobalClock clock;
    public float downTimeValue = -1f;
    public float upTimeValue = 1f;
    public float transitionDuration = 0.25f;
    private bool previousButtonState = false;
    public float simulationEndTimeLimit = 10f;

	// Use this for initialization
	void Start () {
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);
        float endTime = (float)ini.ReadValue("Global", "EndTime", simulationEndTimeLimit);
        simulationEndTimeLimit = endTime;
        ini.Close();
    }
	
	// Update is called once per frame
	void Update () {
        bool currentButtonState = Input.GetButton(inputButton) || Input.GetKey(keyboardInputButton);

        if (clock.time > simulationEndTimeLimit)
            clock.localTimeScale = 0f;

        if (!currentButtonState && clock.time < simulationEndTimeLimit)
            clock.localTimeScale = upTimeValue;
        else if(currentButtonState && clock.time > 0.5)
            clock.localTimeScale = downTimeValue;
        else
            clock.localTimeScale = 0;

        previousButtonState = currentButtonState;
    }
}
