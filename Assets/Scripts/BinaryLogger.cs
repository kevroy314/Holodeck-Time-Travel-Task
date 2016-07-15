using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Chronos;
using System.Collections.Generic;

public class BinaryLogger : MonoBehaviour {

    public string dateTimeFormat = "";
    public string filenameFormat = "<sub>_<trial>_<phase>_<inv>_<datetime>.dat";
    private string header = "";
    private static int headerLength = 1024;

    public Timeline time;
    public Camera cam;

    public List<KeyCode> keys;
    public List<string> buttons;

    public ItemGenerator generator;
    private ClickableObject[] items;
    public BoundaryManager boundaries;

    private BinaryWriter writer;

    private bool firstUpdate = true;

    private int expectedNumItems;

	// Use this for initialization
	void Start () {
        string filename = filenameFormat;
        if (PlayerPrefs.HasKey("sub"))
            filename = filename.Replace("<sub>", PlayerPrefs.GetString("sub"));
        else
            filename = filename.Replace("<sub>", "unk");
        if(PlayerPrefs.HasKey("trial"))
            filename = filename.Replace("<trial>", "" + PlayerPrefs.GetInt("trial"));
        else
            filename = filename.Replace("<trial>", "u");
        if(PlayerPrefs.HasKey("phase"))
            filename = filename.Replace("<phase>", "" + PlayerPrefs.GetInt("phase"));
        else
            filename = filename.Replace("<phase>", "u");
        if (PlayerPrefs.HasKey("inv"))
            filename = filename.Replace("<inv>", "" + PlayerPrefs.GetInt("inv"));
        else
            filename = filename.Replace("<inv>", "u");
        DateTime time = DateTime.Now;
        string timeString = time.ToString(dateTimeFormat);
        filename = filename.Replace("<datetime>", timeString);
        Stream stream = new StreamWriter(filename).BaseStream;
        writer = new BinaryWriter(stream);
	}
	
	// Update is called once per frame
	void Update () {
        items = generator.getItems();
        if (firstUpdate)
        {
            expectedNumItems = generator.expectedNumItems;
            header = "time,f,timeScale,f,posXYZ,fff,rotXYZW,ffff,";
            for (int i = 0; i < keys.Count; i++)
                header += "key" + i + ",b,";
            for (int i = 0; i < buttons.Count; i++)
                header += "button" + i + ",b,";
            for (int i = 0; i < items.Length; i++)
                header += "itemXYZAC" + i + ",fffbb,";
            header += "boundaryNum,i,boundaryColor,fff";

            if (header.Length > headerLength)
            {
                Debug.LogError("Error: Header input length is longer than the 1k Maximum.");
                Application.Quit();
            }

            if (header.Length < headerLength)
                header = header.PadRight(headerLength);

            writer.Write(header);

            firstUpdate = false;
        }
        writer.Write(DateTime.Now.ToBinary());
        writer.Write(time.time);
        writer.Write(time.timeScale);
        writer.Write(cam.transform.position.x);
        writer.Write(cam.transform.position.y);
        writer.Write(cam.transform.position.z);
        writer.Write(cam.transform.rotation.x);
        writer.Write(cam.transform.rotation.y);
        writer.Write(cam.transform.rotation.z);
        writer.Write(cam.transform.rotation.w);
        for(int i = 0; i < keys.Count; i++)
            writer.Write(Input.GetKey(keys[i]));
        for (int i = 0; i < buttons.Count; i++)
        {
            bool state = false;
            try { state = Input.GetButton(buttons[i]); } catch (ArgumentException) { }
            writer.Write(state);
        }
        for (int i = 0; i < expectedNumItems; i++)
        {
            float _x = 0;
            float _y = 0;
            float _z = 0;
            bool _activeSelf = false;
            bool _hasBeenClicked = false;
            try
            {
                _x = items[i].transform.position.x;
                _y = items[i].transform.position.y;
                _z = items[i].transform.position.z;
                _activeSelf = items[i].gameObject.transform.parent.gameObject.activeSelf;
                _hasBeenClicked = items[i].HasBeenClicked();
            }
            catch (Exception) { };
            writer.Write(_x);
            writer.Write(_y);
            writer.Write(_z);
            writer.Write(_activeSelf);
            writer.Write(_hasBeenClicked);
        }
        writer.Write(boundaries.getCurrentState());
        Color c = boundaries.getCurrentColor();
        writer.Write(c.r);
        writer.Write(c.g);
        writer.Write(c.b);
    }

    public void SwapItem(int index, ClickableObject newItem)
    {
        items[index] = newItem;
    }

    void OnApplicationQuit()
    {
        writer.Close();
    }

    void OnDisable()
    {
        writer.Close();
    }
}
