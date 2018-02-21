using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class FormValidator : MonoBehaviour {

    public InputField subIDText;
    public Dropdown trialDropDown;
    public Button practiceButton;
    public Button practiceTestButton;
    public Button studyButton;
    public Button testButton;
    public Toggle inversionToggle;
    public Dropdown mode;

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
            practiceTestButton.interactable = true;
            studyButton.interactable = true;
            testButton.interactable = true;
        }
        else
        {
            practiceButton.interactable = false;
            practiceTestButton.interactable = false;
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
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 0;
                UnityEngine.XR.XRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 1:
                phase = 3;
                UnityEngine.XR.XRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practiceVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 2:
                phase = 6;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice2d.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(3);
                break;
            case 3:
                phase = 12; //Skipping some numbers to keep consistent with previous version
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practicetimeonly.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(6);
                break;
        }
    }

    public void BeginPracticeTest()
    {
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 9;
                UnityEngine.XR.XRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practicetest.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 1:
                phase = 10;
                UnityEngine.XR.XRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practicetestVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 2:
                phase = 11;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practicetest2d.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(4);
                break;
            case 3:
                phase = 13;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practicetesttimeonly.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(7);
                break;
        }
    }

    public void BeginStudy()
    {
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 1;
                UnityEngine.XR.XRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 1:
                phase = 4;
                UnityEngine.XR.XRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "studyVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 2:
                phase = 7;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study2d.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(3);
                break;
            case 3:
                phase = 14;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "studytimeonly.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(6);
                break;
        }
    }

    public void BeginTest()
    {
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 2;
                UnityEngine.XR.XRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 1:
                phase = 5;
                UnityEngine.XR.XRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "testVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 2:
                phase = 8;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test2d.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(4);
                break;
            case 3:
                phase = 15;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "testtimeonly.config");
                UnityEngine.XR.XRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(7);
                break;
        }
    }
}
