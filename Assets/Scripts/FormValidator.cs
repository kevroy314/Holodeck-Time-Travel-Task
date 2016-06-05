using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FormValidator : MonoBehaviour {

    public InputField subIDText;
    public Dropdown trialDropDown;
    public Button practiceButton;
    public Button studyButton;
    public Button testButton;
    public Toggle inversionToggle;
	
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
        PlayerPrefs.SetString("sub", subIDText.text);
        PlayerPrefs.SetInt("trial", trialDropDown.value);
        PlayerPrefs.SetInt("phase", phase);
        PlayerPrefs.SetInt("inv", inversionToggle.isOn ? 1 : 0);
    }

    public void BeginPractice()
    {
        PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice.config");
        SetPlayerPrefValues(0);
        SceneManager.LoadScene(1);
    }

    public void BeginStudy()
    {
        PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study.config");
        SetPlayerPrefValues(1);
        SceneManager.LoadScene(1);
    }

    public void BeginTest()
    {
        PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test.config");
        SetPlayerPrefValues(2);
        SceneManager.LoadScene(2);
    }
}
