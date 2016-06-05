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
        if (firstUpdate)
        {
            items = generator.getItems();
            header = "time,f,timeScale,f,posXYZ,fff,rotXYZW,ffff,";
            for (int i = 0; i < keys.Count; i++)
                header += "key" + i + ",b,";
            for (int i = 0; i < keys.Count; i++)
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
        for (int i = 0; i < keys.Count; i++)
            writer.Write(Input.GetButton(buttons[i]));
        for(int i = 0; i < items.Length; i++)
        {
            writer.Write(items[i].transform.position.x);
            writer.Write(items[i].transform.position.y);
            writer.Write(items[i].transform.position.z);
            writer.Write(items[i].gameObject.transform.parent.gameObject.activeSelf);
            writer.Write(items[i].HasBeenClicked());
        }
        writer.Write(boundaries.getCurrentState());
        Color c = boundaries.getCurrentColor();
        writer.Write(c.r);
        writer.Write(c.g);
        writer.Write(c.b);
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
