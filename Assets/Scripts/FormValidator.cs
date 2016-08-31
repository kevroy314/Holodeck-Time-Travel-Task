using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class FormValidator : MonoBehaviour {

    public InputField subIDText;
    public Dropdown trialDropDown;
    public Button practiceButton;
    public Button studyButton;
    public Button testButton;
    public Toggle inversionToggle;
    public Toggle twoDAndMouseOnly;

    void Start()
    {
        foreach (Camera c in Camera.allCameras)
            c.ResetAspect();
    }

	// Update is called once per frame
	void Update () {
	    if(subIDText.text.Length == 3 && trialDropDown.value != 0)
        {
            practiceButton.interactable = true;
            studyButton.interactable = true;
            testButton.interactable = true;
        }
        else
        {
            practiceButton.interactable = false;
            studyButton.interactable = false;
            testButton.interactable = false;
        }
	}

    private void SetPlayerPrefValues(int phase)
    {
        PlayerPrefs.SetString("sub", subIDText.text.Trim());
        PlayerPrefs.SetInt("trial", trialDropDown.value);
        PlayerPrefs.SetInt("phase", phase);
        PlayerPrefs.SetInt("inv", inversionToggle.isOn ? 1 : 0);
    }

    public void BeginPractice()
    {
        VRSettings.enabled = true;
        PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice" + (twoDAndMouseOnly.isOn ? "2d" : "") + ".config");
        SetPlayerPrefValues(twoDAndMouseOnly.isOn ? 3 : 0);
        if (twoDAndMouseOnly.isOn)
        {
            VRSettings.enabled = false;
            foreach (Camera c in Camera.allCameras)
                c.aspect = 1f;
        }
        SceneManager.LoadScene(twoDAndMouseOnly.isOn ? 3 : 1);
    }

    public void BeginStudy()
    {
        VRSettings.enabled = true;
        PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study" + (twoDAndMouseOnly.isOn ? "2d" : "") + ".config");
        SetPlayerPrefValues(twoDAndMouseOnly.isOn ? 4 : 1);
        if (twoDAndMouseOnly.isOn) VRSettings.enabled = false;
        SceneManager.LoadScene(twoDAndMouseOnly.isOn ? 3 : 1);
    }

    public void BeginTest()
    {
        VRSettings.enabled = true;
        PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test" + (twoDAndMouseOnly.isOn ? "2d" : "") + ".config");
        SetPlayerPrefValues(twoDAndMouseOnly.isOn ? 5 : 2);
        if (twoDAndMouseOnly.isOn) VRSettings.enabled = false;
        SceneManager.LoadScene(twoDAndMouseOnly.isOn ? 4 : 2);
    }
}
