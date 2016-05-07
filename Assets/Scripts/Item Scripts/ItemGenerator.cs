using System.IO;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {

    public GameObject fallPrefabItem;
    public GameObject flyPrefabItem;
    public GameObject foilPrefabItem;
    public double itemHeight = 1.0;
    public double itemFallTime = 1.0;
    public double itemInactiveHeight = 10.0;
    public bool invertTimeline = false;

    enum ItemTypes
    {
        Fall=0, Fly=1, Foil=2
    }

    // Use this for initialization
    void Start () {
        string configFile;
        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);
        string imgRootPath = Application.dataPath + "/ItemImages/";
        int numItems = ini.ReadValue("Global", "NumItems", 0);
        invertTimeline = ini.ReadValue("Global", "InvertItemOrder", invertTimeline?1:0) != 0;
        float y = (float)ini.ReadValue("Global", "ItemHeight", itemHeight);
        float fallTime = (float)ini.ReadValue("Global", "ItemFallTime", itemFallTime);
        float inactiveHeight = (float)ini.ReadValue("Global", "ItemInactiveHeight", itemInactiveHeight);
        float endTime = (float)ini.ReadValue("Global", "EndTime", 10.0);
        string itemKey = "Item";

        Vector3[] locations = new Vector3[numItems];
        float[] delays = new float[numItems];
        ItemTypes[] types = new ItemTypes[numItems];
        Texture2D[] images = new Texture2D[numItems];
        for(int i = 0; i < numItems; i++)
        {
            float x = (float)ini.ReadValue("Items", itemKey + i + "X", 0.0);
            float z = (float)ini.ReadValue("Items", itemKey + i + "Z", 0.0);
            locations[i] = new Vector3(x, y, z);
            float delay = (float)ini.ReadValue("Items", itemKey + i + "Delay", 0.0);
            delays[i] = delay;
            string typeString = ini.ReadValue("Items", itemKey + i + "Type", "foil");
            switch (typeString)
            {
                case "foil":
                    types[i] = ItemTypes.Foil;
                    break;
                case "fly":
                    types[i] = ItemTypes.Fly;
                    break;
                case "fall":
                    types[i] = ItemTypes.Fall;
                    break;
                default:
                    types[i] = ItemTypes.Foil;
                    break;
            }
            string imageFilename = imgRootPath + ini.ReadValue("Items", itemKey + i + "Image", "error.png");
            byte[] fileData;
            if (File.Exists(imageFilename))
            {
                fileData = File.ReadAllBytes(imageFilename);
                Texture2D itemTexture = new Texture2D(400, 400);
                itemTexture.LoadImage(fileData);
                images[i] = itemTexture;
            }
            else
                Debug.Log("Could not load texture (" + imageFilename + "). File does not exist.");
        }

        ini.Close();

        if (invertTimeline)
            for (int i = 0; i < locations.Length;i++){
                if (types[i] == ItemTypes.Fall)
                    types[i] = ItemTypes.Fly;
                else if (types[i] == ItemTypes.Fly)
                    types[i] = ItemTypes.Fall;
                delays[i] = endTime - delays[i];
            }

        for (int i = 0; i < locations.Length; i++)
        {
            if (types[i] == ItemTypes.Fall)
            {
                GameObject tmp = Instantiate(fallPrefabItem);
                DisableAllCollidersInObject(tmp);
                tmp.transform.parent = transform;
                tmp.transform.localPosition = locations[i];
                if (images[i] != null)
                {
                    tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = images[i];
                    flipTexture(tmp);
                }
                FallFromSky script = tmp.GetComponentInChildren<FallFromSky>();
                script.transitionDelay = delays[i];
                script.transitionDuration = fallTime;
                script.startPos = new Vector3(script.startPos.x, inactiveHeight, script.startPos.z);
            }
            else if (types[i] == ItemTypes.Fly)
            {
                GameObject tmp = Instantiate(flyPrefabItem);
                DisableAllCollidersInObject(tmp);
                tmp.transform.parent = transform;
                tmp.transform.localPosition = locations[i];
                if (images[i] != null)
                {
                    tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = images[i];
                    flipTexture(tmp);
                }
                FlyToSky script = tmp.GetComponentInChildren<FlyToSky>();
                script.transitionDelay = delays[i];
                script.transitionDuration = fallTime;
                script.endPos = new Vector3(script.endPos.x, inactiveHeight, script.endPos.z);
            }
            else if (types[i] == ItemTypes.Foil)
            {
                GameObject tmp = Instantiate(foilPrefabItem);
                DisableAllCollidersInObject(tmp);
                if (images[i] != null)
                {
                    tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = images[i];
                    flipTexture(tmp);
                }
                tmp.transform.parent = transform;
                tmp.transform.localPosition = locations[i];
            }
        }
    }

    void DisableAllCollidersInObject(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        try { obj.GetComponent<BoxCollider>().enabled = false; } catch (System.Exception) { }
    }

    void flipTexture(GameObject obj)
    {
            //Get the mesh filter for this cube
            MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
            Mesh mesh = null;
            if (mf != null)
                mesh = mf.mesh;

            if (mesh == null || mesh.uv.Length != 24)
            {
                Debug.Log("Script needs to be attached to built-in cube");
                return;
            }

            //Get the current UVs (probably all 0,0;1,0;0,1;1,1)
            Vector2[] uvs = mesh.uv;

            // Back side UV flipped
            uvs[10] = new Vector2(0.0f, 0.0f);
            uvs[11] = new Vector2(-1f, 0.0f);
            uvs[6] = new Vector2(0.0f, -1f);
            uvs[7] = new Vector2(-1f, -1f);

            // Set the output UV once and it will be fixed for the rest of the object lifetime
            mesh.uv = uvs;
        }
}
