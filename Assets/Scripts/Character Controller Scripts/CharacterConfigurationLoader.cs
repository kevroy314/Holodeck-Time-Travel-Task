using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterConfigurationLoader : MonoBehaviour {
    public static string configFile = "simulation.config";
    public static bool getConfigFileNameFromPlayerPrefs = true;
    public static string configFilePlayerPrefsString = "configFile";
    // Use this for initialization
    void Start() {
        if (getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(configFilePlayerPrefsString);

        FirstPersonController controller = GetComponent<FirstPersonController>();
        TimeController tcontroller = GetComponent<TimeController>();
        AudioSource audio = GetComponent<AudioSource>();
        TemporalImagingEffect vig = GetComponentInChildren<TemporalImagingEffect>();
        ItemClickController clicker = GetComponent<ItemClickController>();
        InventoryManager inventory = GetComponent<InventoryManager>();

        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);

        float forwardTimeSpeed = (float)ini.ReadValue("Character", "TimeForwardSpeed", 1.0);
        float backwardTimeSpeed = (float)ini.ReadValue("Character", "TimeBackwardSpeed", -1.0);
        float timeTransitionDuration = (float)ini.ReadValue("Character", "TimeTransitionDuration", 0.25);

        string timeKeyString = ini.ReadValue("Character", "KeyboardTimeButton", "LeftControl");
        KeyCode timeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), timeKeyString);

        string controllerTimeButton = ini.ReadValue("Character", "ControllerTimeButton", "x");

        bool stepSoundEnabled = ini.ReadValue("Character", "StepSoundEnabled", 1) != 0;

        float vignetteStrength = (float)ini.ReadValue("Character", "VignetteStrength", 0.25);

        float walkSpeed = (float)ini.ReadValue("Character", "WalkSpeed", 5.0);
        float mouseLookSensitivity = (float)ini.ReadValue("Character", "MouseLookSensitivity", 2.0);

        string keyboardClickButtonString = ini.ReadValue("Character", "KeyboardClickButton", "Space");
        KeyCode keyboardClickButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardClickButtonString);
        string controllerClickButton = ini.ReadValue("Character", "ControllerClickButton", "a");

        float itemClickDistance = (float)ini.ReadValue("Character", "ItemClickDistance", 3.0);

        string keyboardNextItemButtonString = ini.ReadValue("Character", "KeyboardNextItemButton", "Q");
        KeyCode keyboardNextItemButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardNextItemButtonString);
        string controllerNextItemButton = ini.ReadValue("Character", "ControllerNextItemButton", "y");

        float itemPlaceDistance = (float)ini.ReadValue("Character", "ItemPlaceDistance", 3.0);

        if (clicker != null) {
            clicker.keyClickButton = keyboardClickButton;
            clicker.controllerClickButton = controllerClickButton;
            clicker.minClickDistance = itemClickDistance;
        }

        if (inventory != null)
        {
            inventory.placeKeyCode = keyboardClickButton;
            inventory.placeButtonString = controllerClickButton;
            inventory.nextKeyCode = keyboardNextItemButton;
            inventory.nextButtonString = controllerNextItemButton;
            inventory.placeDistance = itemPlaceDistance;
        }

        if (tcontroller != null)
        {
            tcontroller.upTimeValue = forwardTimeSpeed;
            tcontroller.downTimeValue = backwardTimeSpeed;
            tcontroller.transitionDuration = timeTransitionDuration;

            tcontroller.inputButton = controllerTimeButton;
            tcontroller.keyboardInputButton = timeKey;
        }
        if (vig != null)
        {
            vig.negativeVignett = vignetteStrength;
            vig.transitionDuration = timeTransitionDuration;
        }

        if (audio != null)
        {
            audio.enabled = stepSoundEnabled;
        }

        if (controller != null)
        {
            controller.setMovementSpeed(walkSpeed);
            controller.setMouseSensitivity(mouseLookSensitivity);
        }
        ini.Close();
    }
}
