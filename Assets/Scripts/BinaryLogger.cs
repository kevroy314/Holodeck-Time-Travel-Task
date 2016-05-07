using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Chronos;

public class BinaryLogger : MonoBehaviour {

    public string dateTimeFormat = "";
    public string filenameFormat = "<sub>_<trial>_<phase>_<datetime>.dat";
    public string header;
    public int headerLength = 1024;

    public Timeline time;
    public Camera cam;
    public KeyCode[] keys;
    public string[] buttons;
    public ItemGenerator generator;
    private ClickableObject[] items;
    public BoundaryManager boundaries;

    private BinaryWriter writer;

	// Use this for initialization
	void Start () {
        string filename = filenameFormat;
        if (PlayerPrefs.HasKey("sub"))
            filename.Replace("<sub>", PlayerPrefs.GetString("sub"));
        else
            filename.Replace("<sub>", "unk");
        if(PlayerPrefs.HasKey("trial"))
            filename.Replace("<trial>", "" + PlayerPrefs.GetInt("trial"));
        else
            filename.Replace("<trial>", "u");
        if(PlayerPrefs.HasKey("phase"))
            filename.Replace("<phase>", "" + PlayerPrefs.GetInt("phase"));
        else
            filename.Replace("<phase>", "u");
        DateTime time = DateTime.Now;
        string timeString = time.ToString(dateTimeFormat);
        filename.Replace("<datetime>", timeString);
        Stream stream = new StreamWriter(filename).BaseStream;
        writer = new BinaryWriter(stream);

        items = generator.getItems();

        if (header.Length > headerLength)
        {
            Debug.LogError("Error: Header input length is longer than the 1k Maximum.");
            Application.Quit();
        }

        if (header.Length < headerLength)
            header.PadRight(headerLength);
        
        writer.Write(header);
	}
	
	// Update is called once per frame
	void Update () {
        writer.Write(time.time);
        writer.Write(time.timeScale);
        writer.Write(cam.transform.position.x);
        writer.Write(cam.transform.position.y);
        writer.Write(cam.transform.position.z);
        writer.Write(cam.transform.rotation.x);
        writer.Write(cam.transform.rotation.y);
        writer.Write(cam.transform.rotation.z);
        writer.Write(cam.transform.rotation.w);
        for(int i = 0; i < keys.Length; i++)
            writer.Write(Input.GetKey(keys[i]));
        for (int i = 0; i < keys.Length; i++)
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
        writer.Write(boundaries.getTransitionProgress());
    }
}
