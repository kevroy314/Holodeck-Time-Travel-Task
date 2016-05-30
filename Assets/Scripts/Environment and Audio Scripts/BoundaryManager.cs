using UnityEngine;
using System.Collections;
using Chronos;

public class BoundaryManager : MonoBehaviour {
    private string configFile = CharacterConfigurationLoader.configFile;
    public Timeline time;
    public MeshRenderer[] renderers;
    private Color[] boundaryColors;
    private float[] delays;
    private float prevTime;
    public float transitionDuration = 2f;
    public int currentState = 0;

    // Use this for initialization
    void Start () {
        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);

        int numBoundaryColors = ini.ReadValue("Global", "NumBoundaryColors", 4);
        float duration = (float)ini.ReadValue("Global", "BoundaryColorTransitionDuration", transitionDuration);

        transitionDuration = duration;
        boundaryColors = new Color[numBoundaryColors];
        delays = new float[numBoundaryColors];
        string key = "Boundary";
        for (int i = 0; i < numBoundaryColors; i++)
        {
            string colorString = ini.ReadValue("BoundaryColors", key + i + "Color", "");
            float delay = (float)ini.ReadValue("BoundaryColors", key + i + "Delay", 0.0);
            string[] colorStringSplit = colorString.Split(new string[] { "(", ")", "," }, System.StringSplitOptions.RemoveEmptyEntries);
            Color color = renderers[0].material.color;
            try
            {
                if (colorString != "")
                    color = new Color(float.Parse(colorStringSplit[0]) / 255f, float.Parse(colorStringSplit[1]) / 255f, float.Parse(colorStringSplit[2]) / 255f);
            }
            catch (System.Exception) { }
            boundaryColors[i] = color;
            delays[i] = delay;
        }

        ini.Close();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < delays.Length; i++)
        {
            if (time.deltaTime > 0 && time.time >= (delays[i] - (transitionDuration / 2)) && prevTime < (delays[i] - (transitionDuration / 2)))
            {
                SetColor(boundaryColors[i]);
                currentState = i;
            }
            if (time.deltaTime < 0 && time.time < (delays[i] + (transitionDuration / 2)) && prevTime >= (delays[i] + (transitionDuration / 2)))
            {
                if (i - 1 >= 0)
                {
                    SetColor(boundaryColors[i - 1]);
                    currentState = i;
                }
            }
        }
        prevTime = time.time;
    }

    void SetColor(Color c)
    {
        for (int i = 0; i < renderers.Length; i++)
            iTween.ColorTo(renderers[i].gameObject, c, transitionDuration);
    }

    public int getCurrentState()
    {
        return currentState;
    }

    public Color getCurrentColor()
    {
        return renderers[0].material.color;
    }
}
